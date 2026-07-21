using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// Importaciones de tu proyecto
using FixMedicalWeb.Models;
using FixMedicalWeb.Services;

namespace fixmedical._1._1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public IndexModel(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public class LoginInput
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            // ❌ Se eliminó la propiedad Recordarme para mayor seguridad
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Dashboard");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == Input.Email);

            if (usuario == null || !usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas o cuenta inactiva.");
                return Page();
            }

            bool claveCorrecta = _passwordHasher.VerifyPassword(Input.Password, usuario.Contrasena_Hash);

            if (!claveCorrecta)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.ID_Usuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre_Usuario),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre_Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 🔒 Iniciar sesión con una cookie de sesión normal (no persistente)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = false // Al cerrar el navegador, se cerrará la sesión
                });

            usuario.Ultimo_Acceso = DateTime.Now;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard");
        }
    }
}