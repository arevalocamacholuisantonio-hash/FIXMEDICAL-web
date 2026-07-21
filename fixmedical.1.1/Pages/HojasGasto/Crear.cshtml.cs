using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.HojasGasto
{
    public class CrearModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CrearModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HojaGasto NuevaHoja { get; set; } = new();

        public string ConsecutivoGenerado { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            NuevaHoja.Fecha = DateTime.Now;

            ConsecutivoGenerado = await GenerarSiguienteConsecutivoAsync();
        }

        public JsonResult OnGetBuscarProducto(string referencia)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.Referencia == referencia);

            if (producto != null)
            {
                return new JsonResult(new
                {
                    codigo = producto.Codigo_Fix,
                    descripcion = producto.Descripcion
                });
            }
            return new JsonResult(null);
        }

        public async Task<IActionResult> OnPostAsync(
            string[] Codigos,
            string[] Referencias,
            string[] Descripciones,
            string[] Cantidades,
            string[] Lotes,
            string[] Invimas)
        {
            NuevaHoja.NumeroConsecutivo = await GenerarSiguienteConsecutivoAsync();

            _context.HojasGasto.Add(NuevaHoja);
            await _context.SaveChangesAsync();

            for (int i = 0; i < Referencias.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(Referencias[i]))
                {
                    var detalle = new HojaGastoDetalle
                    {
                        ID_HojaGasto = NuevaHoja.ID_HojaGasto,
                        Codigo = Codigos[i],
                        Referencia = Referencias[i],
                        Descripcion = Descripciones[i],
                        Cantidad = int.TryParse(Cantidades[i], out int c) ? c : 0,
                        Lote = Lotes[i],
                        RegInvima = Invimas[i]
                    };

                    _context.HojasGastoDetalle.Add(detalle);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard");
        }

        private async Task<string> GenerarSiguienteConsecutivoAsync()
        {
            string fechaStr = DateTime.Now.ToString("ddMMyyyy");
            string prefijo = fechaStr + "_";

            var ultimoRegistro = await _context.HojasGasto
                .Where(h => h.NumeroConsecutivo.StartsWith(prefijo))
                .OrderByDescending(h => h.NumeroConsecutivo)
                .FirstOrDefaultAsync();

            int nuevoNumero = 1; 

            if (ultimoRegistro != null)
            {
                string parteNumerica = ultimoRegistro.NumeroConsecutivo.Substring(prefijo.Length);

                if (int.TryParse(parteNumerica, out int num))
                {
                    nuevoNumero = num + 1;
                }
            }

            
            return $"{prefijo}{nuevoNumero:D3}";
        }
    }
}