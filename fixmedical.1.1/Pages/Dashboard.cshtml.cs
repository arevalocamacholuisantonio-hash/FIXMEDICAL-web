using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public bool EsAdminView { get; set; }
        public bool EsEQCView { get; set; }
        public bool EsComercialView { get; set; }

        // Programaciones pendientes que se muestran en la tabla
        public List<ProgramacionCirugia> Programaciones { get; set; } = new();

        // Solicitudes del comercial (solo para Comercial)
        public List<SolicitudMaterial> MisSolicitudes { get; set; } = new();

        public void OnGet()
        {
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            RolUsuario = string.IsNullOrEmpty(rol) ? "Sin Rol" : rol;

            EsAdminView = rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
            EsEQCView = rol.Equals("EQC", StringComparison.OrdinalIgnoreCase);
            EsComercialView = rol.Equals("Comercial", StringComparison.OrdinalIgnoreCase);

            // Filtro por rol: Admin ve todas, EQC ve solo las suyas, Comercial no ve programaciones
            var query = _context.ProgramacionesCirugia
                .Where(p => p.Estado != "Realizada");

            if (EsEQCView)
            {
                int uid = ObtenerUserId();
                query = query.Where(p => p.ID_EQC_Asignado == uid);
            }

            Programaciones = query
                .OrderBy(p => p.FechaProgramada)
                .ToList();

            // Si es Comercial, carga sus propias solicitudes (historial rápido)
            if (EsComercialView)
            {
                string nombre = User.Identity?.Name ?? "";
                MisSolicitudes = _context.SolicitudesMaterial
                    .Where(s => s.EjecutivoComercial != null &&
                                s.EjecutivoComercial.Trim().ToUpper().Contains(nombre.Trim().ToUpper()))
                    .OrderByDescending(s => s.FechaSolicitud)
                    .Take(5)
                    .ToList();
            }
        }

        private int ObtenerUserId()
        {
            var name = User.Identity?.Name ?? "";
            return _context.Usuarios
                .Where(u => u.Email == name || u.Nombre_Usuario == name)
                .Select(u => u.ID_Usuario)
                .FirstOrDefault();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}