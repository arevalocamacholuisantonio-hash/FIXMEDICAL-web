using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixMedicalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace fixmedical._1._1.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;
        public bool EsAdministrador { get; set; }

        // Programaciones pendientes que se muestran en la tabla
        public List<ProgramacionCirugia> Programaciones { get; set; } = new();

        public void OnGet()
        {
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            RolUsuario = User.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";
            EsAdministrador = RolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase);

            Programaciones = _context.ProgramacionesCirugia
                .Where(p => p.Estado != "Realizada")
                .OrderBy(p => p.FechaProgramada)
                .ToList();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}