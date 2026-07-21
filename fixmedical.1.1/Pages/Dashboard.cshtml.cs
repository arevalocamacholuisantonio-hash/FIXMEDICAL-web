using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace fixmedical._1._1.Pages
{
    [Authorize] // Asegura que solo usuarios logueados puedan entrar
    public class DashboardModel : PageModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;
        public bool EsAdministrador { get; set; }

        public void OnGet()
        {
            // Obtener datos del usuario desde la Cookie de autenticación
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            RolUsuario = User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";

            // Verificar si el rol es Administrador para habilitar opciones exclusivas
            EsAdministrador = RolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Método para cerrar sesión de forma segura
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}