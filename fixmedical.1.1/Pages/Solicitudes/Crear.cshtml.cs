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
    [Authorize]
    public class CrearModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CrearModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string NombreUsuario { get; set; } = string.Empty;

        [BindProperty]
        public SolicitudMaterial Solicitud { get; set; } = new();

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

            // Pre-genera el número para que el campo readonly no llegue vacío al POST
            Solicitud.NumeroSolicitud = GenerarNumeroSolicitud();
            Solicitud.FechaSolicitud = DateTime.Today;
            Solicitud.HoraSolicitud = DateTime.Now.TimeOfDay;
            Solicitud.Estado = "Pendiente";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!TieneAcceso()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario"; Solicitud.HoraSolicitud = DateTime.Now.TimeOfDay;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Regenera por seguridad (el readonly solo viajó para pasar validación)
            Solicitud.NumeroSolicitud = GenerarNumeroSolicitud();
            Solicitud.Estado = "Pendiente";

            _context.SolicitudesMaterial.Add(Solicitud);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"Solicitud {Solicitud.NumeroSolicitud} enviada a Programaciones.";
            return RedirectToPage("/Dashboard");
        }

        // Max(id)+1 con chequeo: no repite números aunque borres filas
        private string GenerarNumeroSolicitud()
        {
            var maxId = _context.SolicitudesMaterial.Max(s => (int?)s.ID_SolicitudMaterial) ?? 0;
            string numero;
            do
            {
                maxId++;
                numero = $"SOL-{DateTime.Now:yyyyMMdd}-{maxId:D4}";
            }
            while (_context.SolicitudesMaterial.Any(s => s.NumeroSolicitud == numero));
            return numero;
        }
    }
}