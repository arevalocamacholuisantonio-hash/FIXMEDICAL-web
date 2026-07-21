using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class HojaGastoDetalle
    {
        [Key]
        public int ID_Detalle { get; set; }

        [Required]
        public int ID_HojaGasto { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Referencia { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(50)]
        public string Lote { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string RegInvima { get; set; } = string.Empty;

        // Relación con la cabecera
        [ForeignKey("ID_HojaGasto")]
        public virtual HojaGasto? HojaGasto { get; set; }
    }
}