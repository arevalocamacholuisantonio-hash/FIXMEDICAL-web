using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FixMedicalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fixmedical._1._1.Pages.Admin.Database
{
    public class ReferenciasModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ReferenciasModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // ===== Contrato con el frontend =====
        [BindProperty]
        public Producto Nuevo { get; set; } = new Producto();

        // PK original: si viene vacía = crear; si trae valor = editar
        [BindProperty]
        public string CodigoOriginal { get; set; }

        // Filtro de búsqueda:
        //  - En GET llega por parámetro (?busqueda=...) y lo asignamos a mano.
        //  - En POST llega por el input oculto del formulario (BindProperty normal).
        [BindProperty]
        public string Busqueda { get; set; } = "";

        public List<Producto> Lista { get; set; } = new List<Producto>();

        public bool Editando => !string.IsNullOrEmpty(CodigoOriginal);

        // ===== GET: lista + (opcional) cargar una referencia para editar =====
        public async Task OnGetAsync(string? editar = null, string? busqueda = null)
        {
            Busqueda = busqueda ?? "";
            await CargarListaAsync();

            if (!string.IsNullOrEmpty(editar))
            {
                var p = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(x => x.Codigo_Fix == editar);
                if (p != null)
                {
                    Nuevo = p;
                    CodigoOriginal = p.Codigo_Fix;
                }
            }
        }

        // ===== POST: crear o actualizar =====
        public async Task<IActionResult> OnPostAsync()
        {
            Normalizar();

            if (!ModelState.IsValid)
            {
                await CargarListaAsync();
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Nuevo.Codigo_Fix))
            {
                ModelState.AddModelError(nameof(Nuevo) + "." + nameof(Nuevo.Codigo_Fix), "El Código Fix es obligatorio.");
                await CargarListaAsync();
                return Page();
            }

            if (string.IsNullOrEmpty(CodigoOriginal))
            {
                // ----- CREAR -----
                var existe = await _db.Productos.AnyAsync(p => p.Codigo_Fix == Nuevo.Codigo_Fix);
                if (existe)
                {
                    ModelState.AddModelError(nameof(Nuevo) + "." + nameof(Nuevo.Codigo_Fix), $"Ya existe una referencia con el Código Fix '{Nuevo.Codigo_Fix}'.");
                    await CargarListaAsync();
                    return Page();
                }
                _db.Productos.Add(Nuevo);
                TempData["OkMessage"] = $"Referencia '{Nuevo.Codigo_Fix}' creada correctamente.";
            }
            else
            {
                // ----- ACTUALIZAR -----
                var original = await _db.Productos.FindAsync(CodigoOriginal);
                if (original == null)
                {
                    TempData["ErrorMessage"] = $"La referencia '{CodigoOriginal}' ya no existe.";
                    return RedirectToPage(new { busqueda = Busqueda });
                }

                // Si cambió el código, que el nuevo esté libre
                if (original.Codigo_Fix != Nuevo.Codigo_Fix && await _db.Productos.AnyAsync(p => p.Codigo_Fix == Nuevo.Codigo_Fix))
                {
                    ModelState.AddModelError(nameof(Nuevo) + "." + nameof(Nuevo.Codigo_Fix), $"Ya existe otra referencia con el Código Fix '{Nuevo.Codigo_Fix}'.");
                    await CargarListaAsync();
                    return Page();
                }

                original.Codigo_Fix = Nuevo.Codigo_Fix;
                original.Referencia = Nuevo.Referencia;
                original.Descripcion = Nuevo.Descripcion;
                original.Nombre_Generico = Nuevo.Nombre_Generico;
                original.Registro_Invima = Nuevo.Registro_Invima;
                original.Fecha_Vencimiento_Invima = Nuevo.Fecha_Vencimiento_Invima;
                original.Fabricante = Nuevo.Fabricante;
                original.Pais_Origen = Nuevo.Pais_Origen;
                original.Clasificacion_Riesgo = Nuevo.Clasificacion_Riesgo;
                original.Tipo_Producto = Nuevo.Tipo_Producto;
                TempData["OkMessage"] = $"Referencia '{Nuevo.Codigo_Fix}' actualizada correctamente.";
            }

            await _db.SaveChangesAsync();
            return RedirectToPage(new { busqueda = Busqueda });
        }

        // ===== POST: eliminar =====
        public async Task<IActionResult> OnPostEliminarAsync(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return RedirectToPage(new { busqueda = Busqueda });

            var p = await _db.Productos.FindAsync(codigo);
            if (p != null)
            {
                _db.Productos.Remove(p);
                await _db.SaveChangesAsync();
                TempData["OkMessage"] = $"Referencia '{codigo}' eliminada.";
            }
            return RedirectToPage(new { busqueda = Busqueda });
        }

        // ===== Helpers =====
        private async Task CargarListaAsync()
        {
            var q = _db.Productos.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                var b = Busqueda.Trim().ToUpper();
                q = q.Where(p => p.Referencia.ToUpper().Contains(b) || p.Codigo_Fix.ToUpper().Contains(b));
            }
            Lista = await q.OrderBy(p => p.Codigo_Fix).Take(200).ToListAsync();
        }

        private void Normalizar()
        {
            if (Nuevo == null) return;
            Nuevo.Codigo_Fix = Mayuscula(Nuevo.Codigo_Fix);
            Nuevo.Referencia = Mayuscula(Nuevo.Referencia);
            Nuevo.Descripcion = Mayuscula(Nuevo.Descripcion);
            Nuevo.Nombre_Generico = Mayuscula(Nuevo.Nombre_Generico);
            Nuevo.Registro_Invima = Mayuscula(Nuevo.Registro_Invima);
            Nuevo.Fabricante = Mayuscula(Nuevo.Fabricante);
            Nuevo.Pais_Origen = Mayuscula(Nuevo.Pais_Origen);
            Nuevo.Clasificacion_Riesgo = Mayuscula(Nuevo.Clasificacion_Riesgo);
            Nuevo.Tipo_Producto = Mayuscula(Nuevo.Tipo_Producto);
        }

        private static string Mayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return valor.Trim().ToUpperInvariant();
        }
    }
}