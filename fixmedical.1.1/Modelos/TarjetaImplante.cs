     using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class  TarjetaImplante
    {
        [Key]
        public int ID_TarjetaImplante { get; set; }

        [StringLength(15)]
        public string NumeroConsecutivo { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Institucion { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Remision { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCirugia { get; set; } = DateTime.Now;

        [Required]
        [StringLength(150)]
        public string NombrePaciente { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NombreEspecialista { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NombreEQC { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NombreIqxInstitucion { get; set; } = string.Empty;

        //-------------------------------------//----------------------------------//

        [StringLength(250)]
        public string FirmaEspecialista { get; set; } = string.Empty;

        [StringLength(250)]
        public string firmaEQC { get; set; } = string.Empty;

        [StringLength(250)]
        public string FirmaIqxInstitucion { get; set; } = string.Empty;


        //-------------------------------------//----------------------------------//

        [StringLength(150)]
        public string Fabricante { get; set; } = string.Empty;

        [StringLength(150)]
        public string Pais { get; set; } = string.Empty;

        [StringLength(250)]
        public string RutaStiker { get; set; } = string.Empty;

        [StringLength(250)]

        public string RutaPdf { get; set; } = string.Empty;

        //-------------------------------------//----------------------------------//

        public double? LatitudInicio { get; set; }
        public double? LongitudInicio { get; set; }

        [StringLength(30)]
        public string FechaHoraInicio { get; set; } = string.Empty;

        public double? LatitudFin { get; set; }
        public double? LongitudFin { get; set; }
        [StringLength(30)]
         public string FechaHoraFin { get; set; } = string.Empty;

        public int DuracionSegundos { get; set; }


        public int? ID_Programacion { get; set; }
        [StringLength(150)]
        public string CirujanoSnapshot { get; set; } = string.Empty;
        [StringLength(200)]
        public string CirugiaSnapshot { get; set; } = string.Empty;
        
        //-------------------------------------//----------------------------------//
         
        public virtual ICollection<TarjetaImplanteDetalle> Detalles { get; set; } = new List<TarjetaImplanteDetalle>();
        public virtual ICollection<TarjetasImplanteAdjuntos> Adjuntos { get; set; } = new List<TarjetasImplanteAdjuntos>();

        //-------------------------------------//----------------------------------//

        public string? FirmaEspPath { get; set; }
        public string? FirmaEQCPath { get; set; }
        public string? FirmaIQXPath { get; set; }   
        public string? PdfPath { get; set; }






    }
}







