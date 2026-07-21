using fixmedical._1._1.Modelos.P;
using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class Especialista
    {
        [Key]
        public int ID_Especialista { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre_Especialista { get; set; } = string.Empty;

        public virtual ICollection<InformacionPaciente> Cirugias { get; set; } = new List<InformacionPaciente>();
    }
}