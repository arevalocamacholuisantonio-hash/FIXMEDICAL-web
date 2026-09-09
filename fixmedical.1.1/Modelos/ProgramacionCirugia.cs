using fixmedical._1._1.Modelos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class ProgramacionCirugia
    {
        [Key]
        public int ID_Programacion { get; set; }

        [Required]
        [StringLength(150)]
        public string Cirujano { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string CirugiaProgramada { get; set; } = string.Empty;

        [Required]
        public DateTime FechaProgramada { get; set; }

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public int? ID_SolicitudMaterial { get; set; }

        public int? ID_EQC_Asignado { get; set; }

        [ForeignKey("ID_SolicitudMaterial")]
        public virtual SolicitudMaterial? SolicitudMaterial { get; set; }

        [ForeignKey("ID_EQC_Asignado")]
        public virtual Usuario? EQCAsignado { get; set; }

    }
}