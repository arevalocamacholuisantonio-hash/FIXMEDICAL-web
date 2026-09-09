using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class HojaGasto
    {
        [Key]
        public int ID_HojaGasto { get; set; }

        [StringLength(15)]
        public string NumeroConsecutivo { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Institucion { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Remision { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Especialista { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Paciente { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PacienteID { get; set; } = string.Empty;

        [StringLength(100)]
        public string Linea { get; set; } = string.Empty;

        [StringLength(100)]
        public string Fabricante { get; set; } = string.Empty;

        [StringLength(50)]
        public string DomicilioPais { get; set; } = string.Empty;

        [StringLength(255)]
        public string FirmaEspecialista { get; set; } = string.Empty;

        [StringLength(255)]
        public string FirmaInstitucion { get; set; } = string.Empty;

        [StringLength(255)]
        public string FirmaFixMedical { get; set; } = string.Empty;

      
        public double? LatitudInicio { get; set; }
        public double? LongitudInicio { get; set; }
        [StringLength(30)]
        public string FechaHoraInicio { get; set; } = string.Empty;

        public double? LatitudFin { get; set; }
        public double? LongitudFin { get; set; }
        [StringLength(30)]
        public string FechaHoraFin { get; set; } = string.Empty;

  
        public int? DuracionSegundos { get; set; }

     
        public int? ID_Programacion { get; set; }

  
        [StringLength(150)]
        public string CirujanoSnapshot { get; set; } = string.Empty;

        [StringLength(200)]
        public string CirugiaSnapshot { get; set; } = string.Empty;

        [StringLength(255)]
        public string RutaPdf { get; set; } = string.Empty;

        public virtual ICollection<HojaGastoDetalle> Detalles { get; set; } = new List<HojaGastoDetalle>();
    }
}