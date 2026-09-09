using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;

namespace fixmedical._1._1.Pages.Admin
{
    [Authorize]
    public class DatabaseModel : PageModel
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            RolUsuario = User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";

            // 🔒 SOLO administradores
            if (!RolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                return RedirectToPage("/Dashboard");

            return Page();
        }
    }
}