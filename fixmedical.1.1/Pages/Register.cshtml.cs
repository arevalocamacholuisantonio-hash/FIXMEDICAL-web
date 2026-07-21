using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

// 1. IMPORTACIONES CORRECTAS DE TU PROYECTO
using FixMedicalWeb.Models;      // 👈 Esto encuentra 'Rol', 'Usuario' y 'ApplicationDbContext'
using FixMedicalWeb.Services;    // Para tu interfaz IPasswordHasher

namespace fixmedical._1._1.Pages
{
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

        // Lista de roles para llenar el selector visual
        public List<Role> RolesDisponibles { get; set; } = new();

        public class RegisterInput
        {
            public string NombreUsuario { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string ConfirmPassword { get; set; } = string.Empty;
            public int ID_Rol { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Cargar los roles de la base de datos para el desplegable
            RolesDisponibles = await _context.Roles.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Volver a cargar los roles por si falla la validación
            RolesDisponibles = await _context.Roles.ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Validar que las contraseñas coincidan
            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return Page();
            }

            // 2. Verificar si el correo ya está registrado
            var existeEmail = await _context.Usuarios.AnyAsync(u => u.Email == Input.Email);
            if (existeEmail)
            {
                ModelState.AddModelError(string.Empty, "El correo electrónico ya se encuentra registrado.");
                return Page();
            }

            // 3. Crear el nuevo usuario con la contraseña hasheada
            var nuevoUsuario = new Usuario
            {
                Nombre_Usuario = Input.NombreUsuario,
                Email = Input.Email,
                Contrasena_Hash = _passwordHasher.HashPassword(Input.Password), // Encriptada con BCrypt
                ID_Rol = Input.ID_Rol,
                Activo = true // Registrado y activo por defecto
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Redirigir al login (Index) con un mensaje de éxito
            TempData["SuccessMessage"] = "Registro exitoso. ¡Ya puedes iniciar sesión!";
            return RedirectToPage("/Index");
        }
    }
}