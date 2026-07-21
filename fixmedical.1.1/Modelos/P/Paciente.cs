using System.ComponentModel.DataAnnotations;

namespace fixmedical._1._1.Modelos.P
{
    public class Paciente
    {
        [Key]
        public int ID_Paciente { get; set; }

        [Required]
        [StringLength(50)]
        public string Documento_Identidad { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Nombre_Completo { get; set; } = string.Empty;

        public int? Edad_Registro { get; set; }

        [StringLength(100)]
        public string? Ciudad { get; set; }

        public virtual ICollection<InformacionPaciente> Cirugias { get; set; } = new List<InformacionPaciente>();
    }
}