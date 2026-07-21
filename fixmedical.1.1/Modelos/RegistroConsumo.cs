using fixmedical._1._1.Modelos.P;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class RegistroConsumo
    {
        [Key]
        public int ID_Consumo { get; set; }

        [Required]
        public int ID_Registro { get; set; }

        [ForeignKey("ID_Registro")]
        public virtual InformacionPaciente InformacionPaciente { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Codigo_Fix { get; set; } = string.Empty;

        [ForeignKey("Codigo_Fix")]
        public virtual Producto Producto { get; set; } = null!;

        [Required]
        public int Cantidad { get; set; } = 1;

        [Required]
        [StringLength(50)]
        public string Lote { get; set; } = string.Empty; // El lote físico (Ej: 26000053)
    }
}