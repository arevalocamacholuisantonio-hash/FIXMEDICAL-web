    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class HistorialCambio
    {
        [Key]
        public int ID_Log { get; set; }

        [Required]
        public int ID_Usuario { get; set; }

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Accion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Tabla_Afectada { get; set; } = string.Empty;

        [Required]
        public int ID_Registro_Afectado { get; set; }

        [Required]
        public DateTime Fecha_Hora { get; set; } = DateTime.Now;

        [Required]
        public string Descripcion_Cambio { get; set; } = string.Empty;
    }
}