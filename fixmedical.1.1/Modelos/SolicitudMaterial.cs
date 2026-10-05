using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
namespace fixmedical._1._1.Modelos
{
    public class SolicitudMaterial
    {
        [Key]
        public int ID_SolicitudMaterial { get; set; }

        [Required]
        [StringLength(100)]
        public string NumeroSolicitud { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TipoSolicitud { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")] 
        public DateTime FechaSolicitud { get; set; }

        [Required]
        
        public DateTime HoraSolicitud { get; set; }

        [Required]
        [StringLength(100)]
        public string Sucursal { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Estado { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EjecutivoComercial { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Institucion { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Facturar_A { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Especialista { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")] 
        public DateTime FechaCirugia { get; set; }

        public DateTime HoraCirugia { get; set; }

        [Required]
        [StringLength(200)]
        public string NombrePaciente { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Indicacion { get; set; } = string.Empty;

        [Required]
        [StringLength(400)]
        public string MaterialSolicitado { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Motor { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string ObservacionesEjecutivos { get; set; } = string.Empty;
    }
}