using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace fixmedical._1._1.Pages.TarjetasImplante
{
    [RequestSizeLimit(524288000)]
    [RequestFormLimits(ValueLengthLimit = 524288000, MultipartBodyLengthLimit = 524288000)]
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

        // ▼▼▼ Canal servidor→toasts: JSON con las etiquetas de los campos que fallaron ▼▼▼
        public string ErroresJson { get; set; } = "[]";

        // Mapa campo→etiqueta en español (para no mostrar "The Ciudad field is required")
        private static readonly Dictionary<string, string> Etiquetas = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Institucion"] = "Institución",
            ["Ciudad"] = "Ciudad",
            ["Remision"] = "Remisión",
            ["FechaCirugia"] = "Fecha de cirugía",
            ["NombrePaciente"] = "Nombre del paciente",
            ["NumeroIdentificacion"] = "Número de identificación",
            ["NombreEspecialista"] = "Nombre del especialista",
            ["NombreEQC"] = "Nombre del EQC",
            ["NombreIqxInstitucion"] = "Nombre IQX institución",
            ["Fabricante"] = "Fabricante",
            ["Pais"] = "País",
            ["FirmaEspecialista"] = "Firma del especialista",
            ["firmaEQC"] = "Firma del EQC",
            ["FirmaIqxInstitucion"] = "Firma IQX institución",
        };

        // Traduce las claves de ModelState a etiquetas en español y las serializa a JSON
        private string ErroresAMensajeJson()
        {
            var msgs = new List<string>();
            foreach (var kv in ModelState)
            {
                if (string.IsNullOrEmpty(kv.Key)) continue; // el global no lo mostramos (lo cubre el cliente)
                var campo = kv.Key.StartsWith("NuevaTarjeta.")
                    ? kv.Key.Substring("NuevaTarjeta.".Length)
                    : kv.Key;
                var etiqueta = Etiquetas.TryGetValue(campo, out var e) ? e : campo;
                if (!msgs.Contains(etiqueta)) msgs.Add(etiqueta);
            }
            return JsonSerializer.Serialize(msgs);
        }

        public async Task OnGetAsync()
        {
            NuevaTarjeta.FechaCirugia = DateTime.Now;
            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
            ProgIdActual = Request.Query["progId"].ToString();
            ErroresJson = "[]";
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
            string? FirmaEspBase64, string? FirmaEqcBase64, string? FirmaIqxBase64,
            string? PdfBase64,
            string? LatInicioStr, string? LngInicioStr, string? FhInicioStr,
            string? LatFinStr, string? LngFinStr, string? FhFinStr,
            string[]? Codigos,
            string[]? Referencias,
            string[]? Descripciones,
            string[]? Cantidades,
            string[]? Lotes,
            string[]? Invimas,
            string? AdjuntosBase64Json)
        {
            ModelState.Remove("NuevaTarjeta.NumeroConsecutivo");
            ModelState.Remove("NuevaTarjeta.FechaHoraInicio");
            ModelState.Remove("NuevaTarjeta.FechaHoraFin");

            NuevaTarjeta.LatitudInicio = ParseD(LatInicioStr);
            NuevaTarjeta.LongitudInicio = ParseD(LngInicioStr);
            NuevaTarjeta.FechaHoraInicio = string.IsNullOrWhiteSpace(FhInicioStr) ? "" : FhInicioStr;
            NuevaTarjeta.LatitudFin = ParseD(LatFinStr);
            NuevaTarjeta.LongitudFin = ParseD(LngFinStr);
            NuevaTarjeta.FechaHoraFin = string.IsNullOrWhiteSpace(FhFinStr) ? "" : FhFinStr;

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

            if (!ModelState.IsValid)
            {
                ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                ErroresJson = ErroresAMensajeJson();   // ← viaja al hidden → toasts (hoja intacta)
                return Page();
            }

            NuevaTarjeta.FirmaEspecialista = GuardarFirma(FirmaEspBase64, "esp");
            NuevaTarjeta.firmaEQC = GuardarFirma(FirmaEqcBase64, "eqc");
            NuevaTarjeta.FirmaIqxInstitucion = GuardarFirma(FirmaIqxBase64, "iqx");
            NuevaTarjeta.RutaPdf = GuardarPdf(PdfBase64);

            Codigos = Codigos ?? Array.Empty<string>();
            Referencias = Referencias ?? Array.Empty<string>();
            Descripciones = Descripciones ?? Array.Empty<string>();
            Cantidades = Cantidades ?? Array.Empty<string>();
            Lotes = Lotes ?? Array.Empty<string>();
            Invimas = Invimas ?? Array.Empty<string>();

            int filas = Math.Max(Codigos.Length,
                        Math.Max(Referencias.Length,
                        Math.Max(Descripciones.Length,
                        Math.Max(Cantidades.Length,
                        Math.Max(Lotes.Length, Invimas.Length)))));

            for (int i = 0; i < filas; i++)
            {
                string referencia = i < Referencias.Length ? (Referencias[i] ?? "").Trim() : "";
                string codigo = i < Codigos.Length ? (Codigos[i] ?? "").Trim() : "";
                string descripcion = i < Descripciones.Length ? (Descripciones[i] ?? "").Trim() : "";
                string cantidadStr = i < Cantidades.Length ? (Cantidades[i] ?? "").Trim() : "";
                string lote = i < Lotes.Length ? (Lotes[i] ?? "").Trim() : "";
                string invima = i < Invimas.Length ? (Invimas[i] ?? "").Trim() : "";

                if (string.IsNullOrEmpty(referencia)) continue;

                int.TryParse(cantidadStr, out int cantidad);
                if (cantidad <= 0) cantidad = 1;

                NuevaTarjeta.Detalles.Add(new TarjetaImplanteDetalle
                {
                    Codigo = codigo,
                    Referencias = referencia,
                    Descripcion = descripcion,
                    Cantidad = cantidad,
                    Lote = lote,
                    RegInvima = invima
                });
            }

            const int MAX_INTENTOS = 5;
            for (int intento = 0; intento < MAX_INTENTOS; intento++)
            {
                NuevaTarjeta.NumeroConsecutivo = await GenerarSiguienteConsecutivoAsync();

                using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    var detalles = NuevaTarjeta.Detalles.ToList();
                    NuevaTarjeta.Detalles.Clear();

                    _context.TarjetasImplante.Add(NuevaTarjeta);
                    await _context.SaveChangesAsync();

                    foreach (var d in detalles)
                    {
                        d.ID_TarjetaImplante = NuevaTarjeta.ID_TarjetaImplante;
                        _context.TarjetasImplanteDetalle.Add(d);
                    }
                    await _context.SaveChangesAsync();

                    await tx.CommitAsync();

                    await GuardarAdjuntosAsync(AdjuntosBase64Json);

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
                    ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                    ErroresJson = JsonSerializer.Serialize(new[] { "Error al guardar: " + msg });
                    return Page();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
                    ErroresJson = JsonSerializer.Serialize(new[] { "Error general: " + ex.Message });
                    return Page();
                }
            }

            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
            ErroresJson = JsonSerializer.Serialize(new[] { "No se pudo generar un consecutivo único. Intente de nuevo." });
            return Page();
        }

        private class AdjuntoDto
        {
            public string? nombre { get; set; }
            public string? tipo { get; set; }
            public string? base64 { get; set; }
        }

        private async Task GuardarAdjuntosAsync(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;

            List<AdjuntoDto>? lista;
            try { lista = JsonSerializer.Deserialize<List<AdjuntoDto>>(json); }
            catch { return; }
            if (lista == null || lista.Count == 0) return;

            var extImg = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extVid = new[] { ".mp4", ".mov", ".webm", ".mkv" };

            var carpetaBase = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "adjuntos", "tarjetas", NuevaTarjeta.ID_TarjetaImplante.ToString());
            Directory.CreateDirectory(carpetaBase);

            foreach (var a in lista)
            {
                if (a == null || string.IsNullOrWhiteSpace(a.base64)) continue;

                var ext = Path.GetExtension(a.nombre ?? "").ToLowerInvariant();
                bool esImg = extImg.Contains(ext);
                bool esVid = extVid.Contains(ext);
                if (!esImg && !esVid) continue;

                byte[] bytes;
                try { bytes = Convert.FromBase64String(a.base64); }
                catch { continue; }

                var nombre = $"{Guid.NewGuid():N}{ext}";
                await System.IO.File.WriteAllBytesAsync(Path.Combine(carpetaBase, nombre), bytes);

                _context.TarjetasImplanteAdjuntos.Add(new TarjetasImplanteAdjuntos
                {
                    ID_TarjetaImplante = NuevaTarjeta.ID_TarjetaImplante,
                    Tipo = esImg ? "imagen" : "video",
                    Ruta = $"/adjuntos/tarjetas/{NuevaTarjeta.ID_TarjetaImplante}/{nombre}",
                    NombreOriginal = a.nombre ?? "",
                    TamanoBytes = bytes.Length,
                    FechaSubida = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
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