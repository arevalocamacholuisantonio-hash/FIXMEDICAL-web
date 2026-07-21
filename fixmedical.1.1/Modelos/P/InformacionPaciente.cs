using FixMedicalWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fixmedical._1._1.Modelos.P
{
    public class InformacionPaciente
    {
        [Key]
        public int ID_Registro { get; set; }

        [Required]
        public int ID_Paciente { get; set; }

        [ForeignKey("ID_Paciente")]
        public virtual Paciente Paciente { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Consecutivo_Gasto { get; set; } = string.Empty; // El número rojo (Ej: 15233)

        [StringLength(50)]
        public string? Remision { get; set; }

        [Required]
        public DateTime Fecha_Cirugia { get; set; }

        [Required]
        public int ID_Institucion { get; set; }

        [ForeignKey("ID_Institucion")]
        public virtual Institucion Institucion { get; set; } = null!;

        [Required]
        public int ID_Especialista { get; set; }

        [ForeignKey("ID_Especialista")]
        public virtual Especialista Especialista { get; set; } = null!;

        [Required]
        public int ID_Usuario_Registra { get; set; }

        [ForeignKey("ID_Usuario_Registra")]
        public virtual Usuario UsuarioRegistra { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Listo, Facturado

        public string? Observaciones { get; set; }

        public virtual ICollection<RegistroConsumo> Consumos { get; set; } = new List<RegistroConsumo>();
    }
}