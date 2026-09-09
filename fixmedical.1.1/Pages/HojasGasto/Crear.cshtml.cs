using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.HojasGasto
{
    [RequestFormLimits(ValueLengthLimit = 52428800, MultipartBodyLengthLimit = 52428800)]
    public class CrearModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CrearModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public HojaGasto NuevaHoja { get; set; } = new();

        public string ConsecutivoGenerado { get; set; } = string.Empty;
        public string ProgIdActual { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            NuevaHoja.Fecha = DateTime.Now;
            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
            ProgIdActual = Request.Query["progId"].ToString();
        }

        // ====== ENDPOINT 1: Enter / blur tras escribir referencia ======
        public JsonResult OnGetBuscarProducto(string referencia)
        {
            if (string.IsNullOrWhiteSpace(referencia))
                return new JsonResult(null);

            var producto = _context.Productos.FirstOrDefault(p => p.Referencia == referencia);
            if (producto != null)
                return new JsonResult(new
                {
                    codigo = producto.Codigo_Fix,
                    descripcion = producto.Descripcion,
                    regInvima = producto.Registro_Invima   
                });
            return new JsonResult(null);
        }

        // ====== ENDPOINT 2: sugerencias mientras escribes ======
        public JsonResult OnGetBuscarReferencias(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new JsonResult(new object[0]);

            texto = texto.Trim();
            var lista = _context.Productos
                .Where(p => p.Referencia.Contains(texto))
                .Take(50).ToList()
                .OrderBy(p => p.Referencia.StartsWith(texto, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(p => p.Referencia)
                .Take(10)
                .Select(p => new
                {
                    codigo = p.Codigo_Fix,
                    referencia = p.Referencia,
                    descripcion = p.Descripcion,
                    regInvima = p.Registro_Invima   
                })
                .ToList();
            return new JsonResult(lista);
        }

        public async Task<IActionResult> OnPostAsync(
            string[] Codigos,
            string[] Referencias,
            string[] Descripciones,
            string[] Cantidades,
            string[] Lotes,
            string[] Invimas,
            string FirmaEspBase64,
            string FirmaInstBase64,
            string FirmaFixBase64,
            string PdfBase64,
            string LatInicioStr, string LngInicioStr, string FhInicioStr,
            string LatFinStr, string LngFinStr, string FhFinStr)
        {
            // 1) LÍNEA y PAÍS
            NuevaHoja.Linea = LeerMarcados("Linea_", new[] { "FIXCMF", "BONEFIX", "FIXPIEZO", "FIXPECTUS", "FIXTORAX", "FIXANCLAS", "FIXHAND" });
            NuevaHoja.DomicilioPais = LeerMarcados("Pais_", new[] { "CH", "IT", "AL", "USA", "BRA" });

            // 1b) UBICACIÓN como texto → número con PUNTO fijo
            NuevaHoja.LatitudInicio = ParseD(LatInicioStr);
            NuevaHoja.LongitudInicio = ParseD(LngInicioStr);
            NuevaHoja.FechaHoraInicio = FhInicioStr ?? string.Empty;
            NuevaHoja.LatitudFin = ParseD(LatFinStr);
            NuevaHoja.LongitudFin = ParseD(LngFinStr);
            NuevaHoja.FechaHoraFin = FhFinStr ?? string.Empty;

            // 2) ¿Viene de una cirugía programada? + snapshot
            var progIdStr = Request.Form["progIdHidden"].ToString();
            if (string.IsNullOrEmpty(progIdStr)) progIdStr = Request.Query["progId"].ToString();
            ProgramacionCirugia? prog = null;
            if (int.TryParse(progIdStr, out int pid))
                prog = await _context.ProgramacionesCirugia.FindAsync(pid);

            if (prog != null)
            {
                NuevaHoja.ID_Programacion = prog.ID_Programacion;
                NuevaHoja.CirujanoSnapshot = prog.Cirujano;
                NuevaHoja.CirugiaSnapshot = prog.CirugiaProgramada;
            }

            // 3) Validación
            if (!ModelState.IsValid)
            {
                ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                ModelState.AddModelError("", "Faltan campos obligatorios (Institución, Fecha, Remisión, Especialista, Paciente, ID). Revise el formulario.");
                return Page();
            }

            // 4) Firmas + PDF como archivos (una sola vez)
            NuevaHoja.FirmaEspecialista = GuardarFirma(FirmaEspBase64, "esp");
            NuevaHoja.FirmaInstitucion = GuardarFirma(FirmaInstBase64, "inst");
            NuevaHoja.FirmaFixMedical = GuardarFirma(FirmaFixBase64, "fix");
            NuevaHoja.RutaPdf = GuardarPdf(PdfBase64);

            if (NuevaHoja.Fecha == default) NuevaHoja.Fecha = DateTime.Now;

            // 5) GUARDADO CON REINTENTO anti-duplicado
            const int MAX_INTENTOS = 5;
            for (int intento = 0; intento < MAX_INTENTOS; intento++)
            {
                NuevaHoja.NumeroConsecutivo = await GenerarSiguienteConsecutivoAsync();

                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.HojasGasto.Add(NuevaHoja);
                    await _context.SaveChangesAsync();

                    int total = Referencias?.Length ?? 0;
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(Referencias[i]))
                        {
                            _context.HojasGastoDetalle.Add(new HojaGastoDetalle
                            {
                                ID_HojaGasto = NuevaHoja.ID_HojaGasto,
                                Codigo = Codigos[i] ?? "",
                                Referencia = Referencias[i] ?? "",
                                Descripcion = Descripciones[i] ?? "",
                                Cantidad = int.TryParse(Cantidades[i], out int c) ? c : 0,
                                Lote = Lotes[i] ?? "",
                                RegInvima = Invimas[i] ?? ""
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    await tx.CommitAsync();

                    if (prog != null)
                    {
                        prog.Estado = "Realizada";
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToPage("/Dashboard");
                }
                catch (DbUpdateException ex) when (EsDuplicadoConsecutivo(ex) && intento < MAX_INTENTOS - 1)
                {
                    await tx.RollbackAsync();
                    _context.Entry(NuevaHoja).State = EntityState.Detached;
                    await Task.Delay(50);
                    continue;
                }
                catch (DbUpdateException dbEx)
                {
                    await tx.RollbackAsync();
                    string msg = dbEx.InnerException?.Message ?? dbEx.Message;
                    ModelState.AddModelError("", "ERROR AL GUARDAR (SQL): " + msg);
                    ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                    return Page();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    ModelState.AddModelError("", "ERROR GENERAL: " + ex.Message);
                    ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                    return Page();
                }
            }

            ModelState.AddModelError("", "No se pudo generar un consecutivo único tras varios intentos. Intente de nuevo.");
            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
            return Page();
        }

        private static bool EsDuplicadoConsecutivo(DbUpdateException ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return msg.Contains("IX_HojasGasto_NumeroConsecutivo", StringComparison.OrdinalIgnoreCase)
                || (msg.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
                    && msg.Contains("NumeroConsecutivo", StringComparison.OrdinalIgnoreCase));
        }

        private static double? ParseD(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : (double?)null;
        }

        private string LeerMarcados(string prefijo, string[] claves)
        {
            var marcados = new List<string>();
            foreach (var k in claves)
            {
                var v = Request.Form[prefijo + k].ToString().Trim();
                if (!string.IsNullOrEmpty(v)) marcados.Add(k);
            }
            return string.Join(",", marcados);
        }

        private string GuardarFirma(string base64, string sufijo)
        {
            if (string.IsNullOrWhiteSpace(base64) || !base64.Contains(","))
                return string.Empty;
            try
            {
                var carpeta = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "firmas");
                Directory.CreateDirectory(carpeta);
                var datos = base64.Split(',');
                var bytes = Convert.FromBase64String(datos[1]);
                var nombre = $"{Guid.NewGuid():N}_{sufijo}.png";
                System.IO.File.WriteAllBytes(Path.Combine(carpeta, nombre), bytes);
                return "/firmas/" + nombre;
            }
            catch { return string.Empty; }
        }

        private string GuardarPdf(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
            try
            {
                if (base64.Contains(",")) base64 = base64.Substring(base64.IndexOf(',') + 1);
                var bytes = Convert.FromBase64String(base64);
                var carpeta = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "pdfs");
                Directory.CreateDirectory(carpeta);
                var nombre = $"{Guid.NewGuid():N}.pdf";
                System.IO.File.WriteAllBytes(Path.Combine(carpeta, nombre), bytes);
                return "/pdfs/" + nombre;
            }
            catch { return string.Empty; }
        }

        private async Task<string> GenerarSiguienteConsecutivoAsync()
        {
            string prefijo = DateTime.Now.ToString("ddMMyyyy") + "_";
            var ultimo = await _context.HojasGasto
                .Where(h => h.NumeroConsecutivo != null && h.NumeroConsecutivo.StartsWith(prefijo))
                .OrderByDescending(h => h.NumeroConsecutivo)
                .FirstOrDefaultAsync();

            int n = 1;
            if (ultimo != null)
            {
                var parte = ultimo.NumeroConsecutivo.Substring(prefijo.Length);
                if (int.TryParse(parte, out int num)) n = num + 1;
            }
            return $"{prefijo}{n:D3}";
        }
    }
}