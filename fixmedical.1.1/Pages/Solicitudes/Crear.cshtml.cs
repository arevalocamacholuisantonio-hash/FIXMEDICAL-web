using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;          
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.Solicitudes   
{
    [Authorize] // 1) Solo usuarios logueados
    public class CrearModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CrearModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string NombreUsuario { get; set; } = string.Empty;

        // 2) Bindeo automático: todo lo que llegue del formulario cae aquí
        [BindProperty]
        public SolicitudMaterial Solicitud { get; set; } = new();

        // 3) GUARD DE ROLES: Admin + EQC + Comercial.
        //    Si mañana Comercial no debe entrar, borras UNA línea.
        private bool TieneAcceso()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            return rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase)
                || rol.Equals("EQC", StringComparison.OrdinalIgnoreCase)
                || rol.Equals("Comercial", StringComparison.OrdinalIgnoreCase);
        }

        public IActionResult OnGet()
        {
            if (!TieneAcceso()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";

            // Valores por defecto para que el form no llegue vacío
            Solicitud.FechaSolicitud = DateTime.Today;
            Solicitud.HoraSolicitud = DateTime.Now;
            Solicitud.Estado = "Pendiente";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!TieneAcceso()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";

            // 4) Si falta algún [Required], regresa al form mostrando errores
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 5) El número de solicitud lo genera el sistema, no el usuario
            Solicitud.NumeroSolicitud = GenerarNumeroSolicitud();
            Solicitud.Estado = "Pendiente"; // forzado, ignorando lo que venga del form

            _context.SolicitudesMaterial.Add(Solicitud);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Solicitud {Solicitud.NumeroSolicitud} enviada a Programaciones.";
            return RedirectToPage("/Dashboard");
        }

        // 6) Correlativo tipo SOL-20260901-0001
        private string GenerarNumeroSolicitud()
        {
            var siguiente = _context.SolicitudesMaterial.Count() + 1;
            return $"SOL-{DateTime.Now:yyyyMMdd}-{siguiente:D4}";
        }
    }
}