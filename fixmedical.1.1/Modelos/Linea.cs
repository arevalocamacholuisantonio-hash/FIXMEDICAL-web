using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class Linea
    {
        [Key]
        public int ID_Linea { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre_Linea { get; set; } = string.Empty; // Ej: FIXCMF, BONEFIX

        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}