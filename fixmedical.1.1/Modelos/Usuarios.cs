    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMedicalWeb.Models
{
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre_Usuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Contrasena_Hash { get; set; } = string.Empty;

        [Required]
        public int ID_Rol { get; set; }

        [ForeignKey("ID_Rol")]
        public virtual Role Rol { get; set; } = null!;

        public bool Activo { get; set; } = true;

        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

        public DateTime? Ultimo_Acceso { get; set; }
    }
}