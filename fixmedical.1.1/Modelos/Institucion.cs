using fixmedical._1._1.Modelos.P;
using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class Institucion
    {
        [Key]
        public int ID_Institucion { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre_Institucion { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;

        public virtual ICollection<InformacionPaciente> Cirugias { get; set; } = new List<InformacionPaciente>();
    }
}