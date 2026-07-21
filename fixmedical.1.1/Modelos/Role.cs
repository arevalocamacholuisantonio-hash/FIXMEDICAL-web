using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class Role
    {
        [Key]
        public int ID_Rol { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre_Rol { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        // Propiedad de navegación (Un rol tiene muchos usuarios)
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}