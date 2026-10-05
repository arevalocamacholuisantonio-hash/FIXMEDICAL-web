using FixMedicalWeb.Models;
using fixmedical._1._1.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.Admin.Database
{
    // ============================================================
    // DTO de VISTA: NO es entidad → sin DbSet, sin migración.
    // El "Excel" se arma leyendo tablas que YA existen.
    // ============================================================
    public class RegistroConsumoVm
    {
        public string Consecutivo { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string PacienteId { get; set; } = "";
        public string CodigoFix { get; set; } = "";
        public string Referencia { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Cantidad { get; set; }
        public string Lote { get; set; } = "";
        public string RegistroSanitario { get; set; } = "";
        public string Fabricante { get; set; } = "";
        public string Domicilio { get; set; } = "";
        public string Eqc { get; set; } = "";
    }

    public class RegistrosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistrosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== Contrato exacto con el .cshtml =====
        public string Tipo { get; set; } = "hojas";
        public List<RegistroConsumoVm> Registros { get; set; } = new();
        public List<RegistroConsumoVm> RegistrosTarjetas { get; set; } = new();
        public string Buscar { get; set; } = "";
        public string FechaDesde { get; set; } = "";
        public string FechaHasta { get; set; } = "";

        public async Task OnGetAsync(string? tipo, string? buscar, string? fechaDesde, string? fechaHasta)
        {
            Tipo = string.Equals(tipo, "tarjetas", StringComparison.OrdinalIgnoreCase) ? "tarjetas" : "hojas";
            Buscar = buscar ?? "";
            FechaDesde = fechaDesde ?? "";
            FechaHasta = fechaHasta ?? "";

            if (Tipo == "tarjetas")
                RegistrosTarjetas = await ConsultarTarjetas();
            else
                Registros = await ConsultarHojas();
        }

        // ============================================================
        // LIBRO 1: HOJAS DE GASTO (cabecera + detalle)
        // ============================================================
        private async Task<List<RegistroConsumoVm>> ConsultarHojas()
        {
            var q = from d in _context.HojasGastoDetalle
                    join h in _context.HojasGasto on d.ID_HojaGasto equals h.ID_HojaGasto
                    select new { d, h };

            // ----- filtros (sobre entidades, antes de proyectar) -----
            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                var b = Buscar.Trim();
                q = q.Where(x =>
                    x.d.Referencia.Contains(b) ||
                    x.d.Descripcion.Contains(b) ||
                    x.d.Codigo.Contains(b) ||
                    x.h.NumeroConsecutivo.Contains(b) ||
                    x.h.PacienteID.Contains(b));
            }
            if (DateTime.TryParse(FechaDesde, out var d1))
                q = q.Where(x => x.h.Fecha >= d1);
            if (DateTime.TryParse(FechaHasta, out var d2))
                q = q.Where(x => x.h.Fecha <= d2.AddDays(1));

            // ----- orden + proyección al DTO -----
            return await q
                .OrderByDescending(x => x.h.Fecha)
                .ThenBy(x => x.h.NumeroConsecutivo)
                .ThenBy(x => x.d.Referencia)
                .Select(x => new RegistroConsumoVm
                {
                    Consecutivo = x.h.NumeroConsecutivo ?? "",
                    Fecha = x.h.Fecha,
                    PacienteId = x.h.PacienteID ?? "",
                    CodigoFix = x.d.Codigo ?? "",
                    Referencia = x.d.Referencia ?? "",
                    Descripcion = x.d.Descripcion ?? "",
                    Cantidad = x.d.Cantidad,
                    Lote = x.d.Lote ?? "",
                    RegistroSanitario = x.d.RegInvima ?? "",
                    Fabricante = x.h.Fabricante ?? "",
                    Domicilio = x.h.DomicilioPais ?? "",
                    Eqc = "" // TODO: cuando la Hoja guarde el nombre del EQC
                })
                .ToListAsync();
        }

        // ============================================================
        // LIBRO 2: TARJETAS DE IMPLANTE (cabecera + detalle)
        // ⚠ Si tu DbSet/clase del detalle se llama distinto
        //   (typo conocido: TarjetaImplanteDetalle), ajusta la línea del join.
        // ============================================================
        private async Task<List<RegistroConsumoVm>> ConsultarTarjetas()
        {
            var q = from d in _context.TarjetasImplanteDetalle
                    join t in _context.TarjetasImplante on d.ID_TarjetaImplante equals t.ID_TarjetaImplante
                    select new { d, t };

            // ----- filtros -----
            if (!string.IsNullOrWhiteSpace(Buscar))
            {
                var b = Buscar.Trim();
                q = q.Where(x =>
                    x.d.Referencias.Contains(b) ||
                    x.d.Descripcion.Contains(b) ||
                    x.d.Codigo.Contains(b) ||
                    x.t.NumeroConsecutivo.Contains(b) ||
                    x.t.NumeroIdentificacion.Contains(b) ||
                    x.t.NombrePaciente.Contains(b));
            }
            if (DateTime.TryParse(FechaDesde, out var d1))
                q = q.Where(x => x.t.FechaCirugia >= d1);
            if (DateTime.TryParse(FechaHasta, out var d2))
                q = q.Where(x => x.t.FechaCirugia <= d2.AddDays(1));

            // ----- orden + proyección -----
            return await q
                .OrderByDescending(x => x.t.FechaCirugia)
                .ThenBy(x => x.t.NumeroConsecutivo)
                .ThenBy(x => x.d.Referencias)
                .Select(x => new RegistroConsumoVm
                {
                    Consecutivo = x.t.NumeroConsecutivo ?? "",
                    Fecha = x.t.FechaCirugia,
                    PacienteId = x.t.NumeroIdentificacion ?? "",
                    CodigoFix = x.d.Codigo ?? "",
                    Referencia = x.d.Referencias ?? "",
                    Descripcion = x.d.Descripcion ?? "",
                    Cantidad = x.d.Cantidad,
                    Lote = x.d.Lote ?? "",
                    RegistroSanitario = x.d.RegInvima ?? "",
                    Fabricante = x.t.Fabricante ?? "",
                    Domicilio = x.t.Pais ?? "",
                    Eqc = x.t.NombreEQC ?? ""
                })
                .ToListAsync();
        }
    }
}