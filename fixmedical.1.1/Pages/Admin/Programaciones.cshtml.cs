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

        public string NombreUsuario { get; set; } = string.Empty;
        public List<ProgramacionCirugia> Programaciones { get; set; } = new();

        private bool EsAdmin()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            return rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase);
        }

        private void CargarLista()
        {
            Programaciones = _context.ProgramacionesCirugia
                .OrderByDescending(p => p.FechaProgramada)
                .ToList();
        }

        public IActionResult OnGet()
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            CargarLista();
            return Page();
        }

        // ===== CREAR =====
        public async Task<IActionResult> OnPostCrearAsync(string cirujano, string cirugia, string fecha)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";

            if (string.IsNullOrWhiteSpace(cirujano)) ModelState.AddModelError("", "El cirujano es obligatorio.");
            if (string.IsNullOrWhiteSpace(cirugia)) ModelState.AddModelError("", "La cirugía es obligatoria.");
            if (!ParseFecha(fecha, out DateTime fechaParsed)) ModelState.AddModelError("", "La fecha/hora no es válida.");

            if (!ModelState.IsValid) { CargarLista(); return Page(); }

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

        // ===== ACTUALIZAR (desde el modal) =====
        public async Task<IActionResult> OnPostActualizarAsync(int id, string cirujano, string cirugia, string fecha)
        {
            if (!EsAdmin()) return RedirectToPage("/Dashboard");
            NombreUsuario = User.Identity?.Name ?? "Usuario";

            var p = await _context.ProgramacionesCirugia.FindAsync(id);
            if (p == null) { CargarLista(); return Page(); }

            if (string.IsNullOrWhiteSpace(cirujano)) ModelState.AddModelError("", "El cirujano es obligatorio.");
            if (string.IsNullOrWhiteSpace(cirugia)) ModelState.AddModelError("", "La cirugía es obligatoria.");
            if (!ParseFecha(fecha, out DateTime fechaParsed)) ModelState.AddModelError("", "La fecha/hora no es válida.");

            if (!ModelState.IsValid) { CargarLista(); return Page(); }

            p.Cirujano = cirujano.Trim().ToUpper();
            p.CirugiaProgramada = cirugia.Trim().ToUpper();
            p.FechaProgramada = fechaParsed;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        // ===== ELIMINAR =====
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

        // ===== Helper: parsea el datetime-local sin depender de la cultura =====
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