using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;  

using FixMedicalWeb.Models;
using FixMedicalWeb.Services;

namespace fixmedical._1._1.Pages.Admin
{
    [Authorize]
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterModel(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public List<Role> RolesDisponibles { get; set; } = new();

        public class RegisterInput
        {
            public string NombreUsuario { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
            public int ID_Rol { get; set; }
        }

        private bool EsAdmin()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            return rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
        }

        // 🔐 Reglas de complejidad. Devuelve la lista de requisitos NO cumplidos.
        private static List<string> ValidarComplejidad(string pwd)
        {
            var errores = new List<string>();
            if (string.IsNullOrEmpty(pwd)) { errores.Add("La contraseña es obligatoria."); return errores; }

            if (pwd.Length < 8)
                errores.Add("Mínimo 8 caracteres.");
            if (!Regex.IsMatch(pwd, "[A-Z]"))
                errores.Add("Al menos 1 letra MAYÚSCULA.");
            if (!Regex.IsMatch(pwd, "[a-z]"))
                errores.Add("Al menos 1 letra minúscula.");
            if (Regex.Matches(pwd, @"\d").Count < 5)
                errores.Add("Al menos 5 NÚMEROS.");
            if (!Regex.IsMatch(pwd, @"[^A-Za-z0-9]"))
                errores.Add("Al menos 1 carácter especial (ej: ! @ # $ % & *).");

            return errores;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            RolesDisponibles = await _context.Roles.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            RolesDisponibles = await _context.Roles.ToListAsync();

            if (!ModelState.IsValid)
                return Page();

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return Page();
            }

            // 🔐 Aplico las reglas de complejidad (servidor = la verdad)
            foreach (var e in ValidarComplejidad(Input.Password))
                ModelState.AddModelError(string.Empty, e);

            if (!ModelState.IsValid)
                return Page();

            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == Input.Email);
            if (existeEmail)
            {
                ModelState.AddModelError(string.Empty, "El correo electrónico ya se encuentra registrado.");
                return Page();
            }

            var nuevoUsuario = new Usuario
            {
                Nombre_Usuario = Input.NombreUsuario,
                Email = Input.Email,
                Contrasena_Hash = _passwordHasher.HashPassword(Input.Password),
                ID_Rol = Input.ID_Rol,
                Activo = true
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Usuario registrado correctamente.";
            return RedirectToPage("/Admin/Register");
        }
    }
}