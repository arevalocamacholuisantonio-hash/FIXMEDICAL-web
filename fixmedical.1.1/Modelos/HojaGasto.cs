using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Agregar este using

namespace FixMedicalWeb.Models
{
    public class HojaGasto
    {
        [Key]
        public int ID_HojaGasto { get; set; }

        // NUEVA PROPIEDAD PARA EL CONSECUTIVO
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

        [StringLength(50)]
        public string Linea { get; set; } = string.Empty;

        [StringLength(100)]
        public string Fabricante { get; set; } = string.Empty;

        [StringLength(10)]
        public string DomicilioPais { get; set; } = string.Empty;

        public virtual ICollection<HojaGastoDetalle> Detalles { get; set; } = new List<HojaGastoDetalle>();
    }
}