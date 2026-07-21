using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class Producto
    {
        [Key]
        [StringLength(50)]
        public string Codigo_Fix { get; set; } = string.Empty; // El código único físico (PK)

        [Required]
        [StringLength(50)]
        public string Referencia { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Nombre_Generico { get; set; }

        [StringLength(100)]
        public string? Registro_Invima { get; set; }

        public DateTime? Fecha_Vencimiento_Invima { get; set; }

        [StringLength(150)]
        public string? Fabricante { get; set; }

        [StringLength(50)]
        public string? Pais_Origen { get; set; }

        [StringLength(10)]
        public string? Clasificacion_Riesgo { get; set; }

        [StringLength(50)]
        public string? Tipo_Producto { get; set; }

        public int? ID_Linea { get; set; }

        [ForeignKey("ID_Linea")]
        public virtual Linea? Linea { get; set; }
    }
}