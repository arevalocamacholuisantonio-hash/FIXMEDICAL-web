using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Security.Claims;

using FixMedicalWeb.Models;
using FixMedicalWeb.Models; // (si tu namespace de modelos es otro, ajusta)
using FixMedicalWeb.Services;

namespace fixmedical._1._1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMemoryCache _cache;

        public IndexModel(ApplicationDbContext context, IPasswordHasher passwordHasher, IMemoryCache cache)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _cache = cache;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public class LoginInput
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            // (se eliminó el honeypot: Chrome lo autocompletaba y bloqueaba a todos)
        }

        // ====== Rate-limit en memoria (sin tocar la BD) ======
        private const int MAX_FALLOS_EMAIL = 8;     // subido para no molestar en pruebas
        private const int MAX_FALLOS_IP = 50;
        private static readonly TimeSpan VENTANA = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan BLOQUEO = TimeSpan.FromMinutes(15);

        private string IpActual() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        private string KeyEmail(string e) => "login_fail_email:" + (e ?? "").Trim().ToLowerInvariant();
        private string KeyIp() => "login_fail_ip:" + IpActual();
        private string KeyBloqEmail(string e) => "login_block_email:" + (e ?? "").Trim().ToLowerInvariant();
        private string KeyBloqIp() => "login_block_ip:" + IpActual();

        private (bool bloqueado, int minutos) EstaBloqueado(string email)
        {
            try
            {
                if (_cache.TryGetValue(KeyBloqEmail(email), out DateTimeOffset hastaE))
                    return (true, Math.Max(1, (int)Math.Ceiling((hastaE - DateTimeOffset.UtcNow).TotalMinutes)));
                if (_cache.TryGetValue(KeyBloqIp(), out DateTimeOffset hastaIp))
                    return (true, Math.Max(1, (int)Math.Ceiling((hastaIp - DateTimeOffset.UtcNow).TotalMinutes)));
            }
            catch { }
            return (false, 0);
        }

        private void RegistrarFallo(string email)
        {
            try
            {
                _cache.TryGetValue(KeyEmail(email), out int ne); ne++;
                _cache.Set(KeyEmail(email), ne, VENTANA);
                if (ne >= MAX_FALLOS_EMAIL)
                    _cache.Set(KeyBloqEmail(email), DateTimeOffset.UtcNow.Add(BLOQUEO), BLOQUEO);

                _cache.TryGetValue(KeyIp(), out int ni); ni++;
                _cache.Set(KeyIp(), ni, VENTANA);
                if (ni >= MAX_FALLOS_IP)
                    _cache.Set(KeyBloqIp(), DateTimeOffset.UtcNow.Add(BLOQUEO), BLOQUEO);
            }
            catch { }
        }

        private void LimpiarContadores(string email)
        {
            try
            {
                _cache.Remove(KeyEmail(email));
                _cache.Remove(KeyBloqEmail(email));
                _cache.Remove(KeyIp());
                _cache.Remove(KeyBloqIp());
            }
            catch { }
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("/Dashboard");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Formato (required/email). NO cuenta como intento.
            if (!ModelState.IsValid)
                return Page();

            // 2) Rate-limit
            var (bloqueado, minutos) = EstaBloqueado(Input.Email);
            if (bloqueado)
            {
                ModelState.AddModelError(string.Empty, $"Demasiados intentos. Intente de nuevo en {minutos} min.");
                return Page();
            }

            // 3) Buscar usuario (mensaje SIEMPRE genérico: no revela si el correo existe)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == Input.Email);

            bool credencialOk = usuario != null
                                && usuario.Activo
                                && _passwordHasher.VerifyPassword(Input.Password, usuario.Contrasena_Hash);

            if (!credencialOk)
            {
                RegistrarFallo(Input.Email);
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas o cuenta inactiva.");
                return Page();
            }

            // 4) Éxito
            LimpiarContadores(Input.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.ID_Usuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre_Usuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre_Rol)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = false });

            usuario.Ultimo_Acceso = DateTime.Now;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard");
        }
    }
}