using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.Admin
{
    [Authorize]
    public class ProgramacionesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProgramacionesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== PROPIEDADES PARA LA VISTA =====
        public string NombreUsuario { get; set; } = string.Empty;
        public string RolDisplay { get; set; } = string.Empty;
        public bool EsAdminView { get; set; }
        public List<ProgramacionCirugia> Programaciones { get; set; } = new();
        public List<SolicitudMaterial> SolicitudesPendientes { get; set; } = new();
        public List<Usuario> ListaEQCs { get; set; } = new();

        // ===== HELPERS =====
        private bool EsAdmin() =>
            (User.FindFirst(ClaimTypes.Role)?.Value ?? "")
            .Equals("Administrador", StringComparison.OrdinalIgnoreCase);

        private bool EsEQC() =>
            (User.FindFirst(ClaimTypes.Role)?.Value ?? "")
            .Equals("EQC", StringComparison.OrdinalIgnoreCase);

        // Busca el ID del usuario logueado por email o nombre.
        // No depende de qué claim use tu login.
        private int ObtenerUserId()
        {
            var name = User.Identity?.Name ?? "";
            return _context.Usuarios
                .Where(u => u.Email == name || u.Nombre_Usuario == name)
                .Select(u => u.ID_Usuario)
                .FirstOrDefault();
        }

        private void CargarPanelAdmin()
        {
            SolicitudesPendientes = _context.SolicitudesMaterial
                .Where(s => s.Estado == "Pendiente")
                .OrderBy(s => s.FechaCirugia)
                .ToList();

            ListaEQCs = _context.Usuarios
                .Where(u => u.ID_Rol == 2 && u.Activo)
                .OrderBy(u => u.Nombre_Usuario)
                .ToList();
        }

        // ===== ONGET =====
        public IActionResult OnGet()
        {
            // Guard: solo Admin o EQC entran. Comercial/Auditor rebota al Dashboard.
            if (!EsAdmin() && !EsEQC()) return RedirectToPage("/Dashboard");

            NombreUsuario = User.Identity?.Name ?? "Usuario";
            EsAdminView = EsAdmin();
            RolDisplay = EsAdmin() ? "Administrador" : "EQC";

            if (EsAdmin())
            {
                // Admin ve TODO
                CargarPanelAdmin();
                Programaciones = _context.ProgramacionesCirugia
                    .Include(p => p.EQCAsignado)
                    .OrderByDescending(p => p.FechaProgramada)
                    .ToList();
            }
            else // EQC
            {
                // EQC ve SOLO lo que le asignaron
                int uid = ObtenerUserId();
                Programaciones = _context.ProgramacionesCirugia
                    .Include(p => p.EQCAsignado)
                    .Where(p => p.ID_EQC_Asignado == uid)
                    .OrderByDescending(p => p.FechaProgramada)
                    .ToList();
            }

            return Page();
        }

        // ===== CREAR (manual, ya existía) =====
        public async Task<IActionResult> OnPostCrearAsync(string cirujano, string cirugia, string fecha)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            EsAdminView = true;
            RolDisplay = "Administrador";

            if (string.IsNullOrWhiteSpace(cirujano)) ModelState.AddModelError("", "El cirujano es obligatorio.");
            if (string.IsNullOrWhiteSpace(cirugia)) ModelState.AddModelError("", "La cirugía es obligatoria.");
            if (!ParseFecha(fecha, out DateTime fechaParsed)) ModelState.AddModelError("", "La fecha/hora no es válida.");

            if (!ModelState.IsValid) { CargarPanelAdmin(); CargarProgramacionesAdmin(); return Page(); }

            _context.ProgramacionesCirugia.Add(new ProgramacionCirugia
            {
                Cirujano = cirujano.Trim().ToUpper(),
                CirugiaProgramada = cirugia.Trim().ToUpper(),
                FechaProgramada = fechaParsed,
                Estado = "Pendiente"
            });
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // ===== ACTUALIZAR (desde modal, ya existía) =====
        public async Task<IActionResult> OnPostActualizarAsync(int id, string cirujano, string cirugia, string fecha)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            EsAdminView = true;
            RolDisplay = "Administrador";

            var p = await _context.ProgramacionesCirugia.FindAsync(id);
            if (p == null) { CargarPanelAdmin(); CargarProgramacionesAdmin(); return Page(); }

            if (string.IsNullOrWhiteSpace(cirujano)) ModelState.AddModelError("", "El cirujano es obligatorio.");
            if (string.IsNullOrWhiteSpace(cirugia)) ModelState.AddModelError("", "La cirugía es obligatoria.");
            if (!ParseFecha(fecha, out DateTime fechaParsed)) ModelState.AddModelError("", "La fecha/hora no es válida.");

            if (!ModelState.IsValid) { CargarPanelAdmin(); CargarProgramacionesAdmin(); return Page(); }

            p.Cirujano = cirujano.Trim().ToUpper();
            p.CirugiaProgramada = cirugia.Trim().ToUpper();
            p.FechaProgramada = fechaParsed;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // ===== ELIMINAR (ya existía) =====
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            var p = await _context.ProgramacionesCirugia.FindAsync(id);
            if (p != null)
            {
                _context.ProgramacionesCirugia.Remove(p);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // ===== APROBAR SOLICITUD (Admin) =====
        public async Task<IActionResult> OnPostAprobarAsync(int idSolicitud, int idEqc)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");

            var sol = await _context.SolicitudesMaterial.FindAsync(idSolicitud);
            if (sol == null || sol.Estado != "Pendiente") return RedirectToPage();

            // Fusiona fecha y hora en un solo DateTime
            DateTime fechaCirugia = sol.FechaCirugia.Date.Add(sol.HoraCirugia);

            var nueva = new ProgramacionCirugia
            {
                Cirujano = sol.Especialista.Trim().ToUpper(),
                CirugiaProgramada = sol.Especialidad.Trim().ToUpper(),
                FechaProgramada = fechaCirugia,
                Estado = "Pendiente",
                ID_SolicitudMaterial = sol.ID_SolicitudMaterial,
                ID_EQC_Asignado = idEqc
            };
            _context.ProgramacionesCirugia.Add(nueva);

            sol.Estado = "Aprobada";
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // ===== RECHAZAR SOLICITUD (Admin) =====
        public async Task<IActionResult> OnPostRechazarAsync(int idSolicitud)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");

            var sol = await _context.SolicitudesMaterial.FindAsync(idSolicitud);
            if (sol == null) return RedirectToPage();

            if (sol.Estado == "Pendiente")
            {
                sol.Estado = "Rechazada";
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // Helper para recargar la lista del Admin (usada al volver con errores)
        private void CargarProgramacionesAdmin()
        {
            Programaciones = _context.ProgramacionesCirugia
                .Include(p => p.EQCAsignado)
                .OrderByDescending(p => p.FechaProgramada)
                .ToList();
        }

        // ===== Helper: parsea datetime-local sin depender de cultura =====
        private static bool ParseFecha(string s, out DateTime resultado)
        {
            resultado = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;
            return DateTime.TryParseExact(s,
                new[] { "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out resultado);
        }
    }
}