using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixMedicalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace fixmedical._1._1.Pages.Admin.Database
{
    [Authorize]
    public class HistorialModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HistorialModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public string NombreUsuario { get; set; } = string.Empty;
        public List<HojaGasto> Registros { get; set; } = new();
        public List<TarjetaImplante> RegistrosTarjetas { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string Buscar { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string FechaDesde { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string FechaHasta { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string Tipo { get; set; } = "hojas";

        private bool EsAdmin() =>
            (User.FindFirst(ClaimTypes.Role)?.Value ?? "")
            .Equals("Administrador", StringComparison.OrdinalIgnoreCase);

        public IActionResult OnGet()
        {
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            if (!EsAdmin()) return RedirectToPage("/Dashboard");

            if (!"tarjetas".Equals(Tipo, StringComparison.OrdinalIgnoreCase))
                Tipo = "hojas";

            if (Tipo == "tarjetas")
            {
                var qt = _context.TarjetasImplante.Where(t => t.ID_Programacion != null).AsQueryable();

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    var b = Buscar.Trim();
                    qt = qt.Where(t =>
                        (t.NumeroConsecutivo != null && t.NumeroConsecutivo.Contains(b)) ||
                        (t.CirujanoSnapshot != null && t.CirujanoSnapshot.Contains(b)) ||
                        (t.CirugiaSnapshot != null && t.CirugiaSnapshot.Contains(b)) ||
                        (t.NombrePaciente != null && t.NombrePaciente.Contains(b)));
                }
                if (DateTime.TryParse(FechaDesde, out var d1)) qt = qt.Where(t => t.FechaCirugia >= d1);
                if (DateTime.TryParse(FechaHasta, out var d2)) qt = qt.Where(t => t.FechaCirugia <= d2.AddDays(1));

                RegistrosTarjetas = qt.OrderByDescending(t => t.FechaCirugia).ToList();
            }
            else
            {
                var q = _context.HojasGasto.Where(h => h.ID_Programacion != null).AsQueryable();

                if (!string.IsNullOrWhiteSpace(Buscar))
                {
                    var b = Buscar.Trim();
                    q = q.Where(h =>
                        (h.NumeroConsecutivo != null && h.NumeroConsecutivo.Contains(b)) ||
                        (h.CirujanoSnapshot != null && h.CirujanoSnapshot.Contains(b)) ||
                        (h.CirugiaSnapshot != null && h.CirugiaSnapshot.Contains(b)) ||
                        (h.Paciente != null && h.Paciente.Contains(b)));
                }
                if (DateTime.TryParse(FechaDesde, out var d1)) q = q.Where(h => h.Fecha >= d1);
                if (DateTime.TryParse(FechaHasta, out var d2)) q = q.Where(h => h.Fecha <= d2.AddDays(1));

                Registros = q.OrderByDescending(h => h.Fecha).ToList();
            }

            return Page();
        }

        // Handler para devolver los adjuntos de una tarjeta en JSON
        public async Task<IActionResult> OnGetAdjuntosAsync(int id)
        {
            if (!EsAdmin()) return Unauthorized();

            var lista = await _context.TarjetasImplanteAdjuntos
                .Where(a => a.ID_TarjetaImplante == id)
                .Select(a => new
                {
                    ruta = a.Ruta,
                    tipo = a.Tipo,
                    nombre = a.NombreOriginal,
                    tamanoMB = Math.Round(a.TamanoBytes / (1024.0 * 1024.0), 2)
                })
                .OrderBy(a => a.nombre)
                .ToListAsync();

            return new JsonResult(lista);
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id, string tipoBorrar)
        {
            NombreUsuario = User.Identity?.Name ?? "Usuario";
            if (!EsAdmin()) return RedirectToPage("/Dashboard");

            if ("tarjetas".Equals(tipoBorrar, StringComparison.OrdinalIgnoreCase))
            {
                var t = await _context.TarjetasImplante.FindAsync(id);
                if (t == null || t.ID_Programacion == null)
                    return RedirectToPageConFiltros();

                // Borrar primero los HIJOS (detalles y adjuntos) para evitar conflicto de FK
                var detalles = await _context.TarjetasImplanteDetalle
                    .Where(d => d.ID_TarjetaImplante == id)
                    .ToListAsync();

                var adjuntos = await _context.TarjetasImplanteAdjuntos
                    .Where(a => a.ID_TarjetaImplante == id)
                    .ToListAsync();

                var rutas = new[] { t.RutaPdf, t.FirmaEspecialista, t.firmaEQC, t.FirmaIqxInstitucion };

                _context.TarjetasImplanteDetalle.RemoveRange(detalles);
                _context.TarjetasImplanteAdjuntos.RemoveRange(adjuntos);
                _context.TarjetasImplante.Remove(t);
                await _context.SaveChangesAsync();

                foreach (var r in rutas) BorrarArchivo(r);
                foreach (var a in adjuntos) BorrarArchivo(a.Ruta);

                TempData["OkMessage"] = "Tarjeta de implante eliminada del historial.";
                Tipo = "tarjetas";
                return RedirectToPageConFiltros();
            }

            var h = await _context.HojasGasto.FindAsync(id);
            if (h == null || h.ID_Programacion == null)
                return RedirectToPageConFiltros();

            var rutasH = new[] { h.RutaPdf, h.FirmaEspecialista, h.FirmaInstitucion, h.FirmaFixMedical };
            var detallesH = _context.HojasGastoDetalle.Where(d => d.ID_HojaGasto == id);
            _context.HojasGastoDetalle.RemoveRange(detallesH);
            _context.HojasGasto.Remove(h);
            await _context.SaveChangesAsync();
            foreach (var r in rutasH) BorrarArchivo(r);

            TempData["OkMessage"] = "Registro del historial eliminado correctamente.";
            Tipo = "hojas";
            return RedirectToPageConFiltros();
        }

        private IActionResult RedirectToPageConFiltros()
        {
            var qs = new List<string> { "tipo=" + (Tipo == "tarjetas" ? "tarjetas" : "hojas") };
            if (!string.IsNullOrWhiteSpace(Buscar)) qs.Add("Buscar=" + Uri.EscapeDataString(Buscar));
            if (!string.IsNullOrWhiteSpace(FechaDesde)) qs.Add("FechaDesde=" + Uri.EscapeDataString(FechaDesde));
            if (!string.IsNullOrWhiteSpace(FechaHasta)) qs.Add("FechaHasta=" + Uri.EscapeDataString(FechaHasta));
            var url = "/Admin/Database/Historial?" + string.Join("&", qs);
            return Redirect(url);
        }

        private void BorrarArchivo(string rutaRel)
        {
            if (string.IsNullOrWhiteSpace(rutaRel)) return;
            try
            {
                var fisica = System.IO.Path.Combine(
                    _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot"),
                    rutaRel.TrimStart('/'));
                if (System.IO.File.Exists(fisica)) System.IO.File.Delete(fisica);
            }
            catch { }
        }
    }
}