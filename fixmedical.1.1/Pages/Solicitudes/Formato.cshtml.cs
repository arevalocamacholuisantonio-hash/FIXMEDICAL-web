using fixmedical._1._1.Modelos;
using FixMedicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixmedical._1._1.Pages.Solicitudes
{
    public class FormatoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FormatoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SolicitudMaterial Solicitud { get; set; } = new SolicitudMaterial();

        // TRUE si la página se abrió con ?pdf=true (auto-descarga)
        public bool ModoPdf { get; set; }

        public string FormatoHora(TimeSpan hora)
        {
            return DateTime.Today.Add(hora)
                .ToString("h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
        }

        public string FormatoFecha(DateTime fecha)
        {
            return fecha == DateTime.MinValue ? "" : fecha.ToString("dd/MM/yyyy");
        }

        public string FormatoHoraOpcional(TimeSpan hora)
        {
            return hora == TimeSpan.Zero ? "" : FormatoHora(hora);
        }

        public async Task<IActionResult> OnGetAsync(int? id, bool pdf = false)
        {
            ModoPdf = pdf;

            if (id == null) return NotFound();

            var sol = await _context.SolicitudesMaterial.FindAsync(id.Value);
            if (sol == null) return NotFound();

            Solicitud = sol;
            return Page();
        }
    }
}

