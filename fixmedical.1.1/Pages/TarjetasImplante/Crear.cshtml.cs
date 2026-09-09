using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace fixmedical._1._1.Pages.TarjetasImplante
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
        public TarjetaImplante NuevaTarjeta { get; set; } = new();

        public string ConsecutivoGenerado { get; set; } = string.Empty;
        public string ProgIdActual { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            NuevaTarjeta.FechaCirugia = DateTime.Now;
            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
            ProgIdActual = Request.Query["progId"].ToString();
        }

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
                .Select(p => new { codigo = p.Codigo_Fix, referencia = p.Referencia, descripcion = p.Descripcion, regInvima = p.Registro_Invima })
                .ToList();
            return new JsonResult(lista);
        }

        public JsonResult OnGetBuscarProducto(string referencia)
        {
            var p = _context.Productos.FirstOrDefault(x => x.Referencia == referencia);
            return p != null
                ? new JsonResult(new { codigo = p.Codigo_Fix, descripcion = p.Descripcion, regInvima = p.Registro_Invima })
                : new JsonResult(null);
        }

        public async Task<IActionResult> OnPostAsync(
            string FirmaEspBase64, string FirmaEqcBase64, string FirmaIqxBase64,
            string PdfBase64,
            string LatInicioStr, string LngInicioStr, string FhInicioStr,
            string LatFinStr, string LngFinStr, string FhFinStr)
        {
            // 1) Removemos del ModelState los campos que el servidor llena
            //    (evita el Required implícito de <Nullable>enable sobre strings sin ?)
            ModelState.Remove("NuevaTarjeta.NumeroConsecutivo");
            ModelState.Remove("NuevaTarjeta.FechaHoraInicio");
            ModelState.Remove("NuevaTarjeta.FechaHoraFin");

            // 2) Ubicación como texto → número con punto fijo
            NuevaTarjeta.LatitudInicio = ParseD(LatInicioStr);
            NuevaTarjeta.LongitudInicio = ParseD(LngInicioStr);
            NuevaTarjeta.FechaHoraInicio = string.IsNullOrWhiteSpace(FhInicioStr) ? "" : FhInicioStr;
            NuevaTarjeta.LatitudFin = ParseD(LatFinStr);
            NuevaTarjeta.LongitudFin = ParseD(LngFinStr);
            NuevaTarjeta.FechaHoraFin = string.IsNullOrWhiteSpace(FhFinStr) ? "" : FhFinStr;

            // 3) ¿Viene de una cirugía programada? + snapshots para el Historial
            var progIdStr = Request.Form["progIdHidden"].ToString();
            if (string.IsNullOrEmpty(progIdStr)) progIdStr = Request.Query["progId"].ToString();
            ProgramacionCirugia? prog = null;
            if (int.TryParse(progIdStr, out int pid))
                prog = await _context.ProgramacionesCirugia.FindAsync(pid);

            if (prog != null)
            {
                NuevaTarjeta.ID_Programacion = prog.ID_Programacion;
                NuevaTarjeta.CirujanoSnapshot = prog.Cirujano ?? "";
                NuevaTarjeta.CirugiaSnapshot = prog.CirugiaProgramada ?? "";
            }

            // 4) Validación
            if (!ModelState.IsValid)
            {
                ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                ModelState.AddModelError("", "Faltan campos obligatorios. Revise el formulario.");
                return Page();
            }

            // 5) Firmas + PDF (usando los nombres REALES del modelo)
            NuevaTarjeta.FirmaEspecialista = GuardarFirma(FirmaEspBase64, "esp");
            NuevaTarjeta.firmaEQC = GuardarFirma(FirmaEqcBase64, "eqc");         // ← minúscula inicial
            NuevaTarjeta.FirmaIqxInstitucion = GuardarFirma(FirmaIqxBase64, "iqx");
            NuevaTarjeta.RutaPdf = GuardarPdf(PdfBase64);

            // 6) Guardado con reintento anti-duplicado
            const int MAX_INTENTOS = 5;
            for (int intento = 0; intento < MAX_INTENTOS; intento++)
            {
                NuevaTarjeta.NumeroConsecutivo = await GenerarSiguienteConsecutivoAsync();

                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.TarjetasImplante.Add(NuevaTarjeta);
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
                    _context.Entry(NuevaTarjeta).State = EntityState.Detached;
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
            return msg.Contains("IX_TarjetasImplante_NumeroConsecutivo", StringComparison.OrdinalIgnoreCase)
                || (msg.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
                    && msg.Contains("NumeroConsecutivo", StringComparison.OrdinalIgnoreCase));
        }

        private static double? ParseD(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? (double?)d : null;
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
            string pref = "TI" + DateTime.Now.ToString("ddMMyyyy") + "_";
            var ult = await _context.TarjetasImplante
                .Where(t => t.NumeroConsecutivo != null && t.NumeroConsecutivo.StartsWith(pref))
                .OrderByDescending(t => t.NumeroConsecutivo)
                .FirstOrDefaultAsync();
            int n = 1;
            if (ult != null && int.TryParse(ult.NumeroConsecutivo.Substring(pref.Length), out int num))
                n = num + 1;
            return $"{pref}{n:D3}";
        }
    }
}