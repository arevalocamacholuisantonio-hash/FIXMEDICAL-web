using  System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{

    public class TarjetaImplateDetalle
    {
        [Key]
        public int ID_TarjetaImplanteDetalle { get; set; }

        public int ID_TarjetaImplante { get; set; }

        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [StringLength(50)]
        public string Referencias { get; set; } = string.Empty;

        [StringLength(300)]
        public string Descripcion { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        [StringLength(100)]
        public string Lote { get; set; } = string.Empty;

        [StringLength(100)]
        public string RegInvima { get; set; } = string.Empty;

        public virtual TarjetaImplante Tarjeta { get; set; }= null!;

    }

}