using System;
using System.ComponentModel.DataAnnotations;

namespace FixMedicalWeb.Models
{
    public class TarjetasImplanteAdjuntos
    {
        [Key]
        public int ID_TarjetasImplanteAdjuntos { get; set; }

        // FK hacia la tarjeta dueña del adjunto
        public int ID_TarjetaImplante { get; set; }

        // "imagen" o "video"
        [StringLength(20)]
        public string Tipo { get; set; } = "imagen";

        // Ruta web del archivo, ej: /adjuntos/tarjetas/7/abc123.jpg
        [StringLength(300)]
        public string Ruta { get; set; } = string.Empty;

        // Nombre con el que el usuario lo subió (para mostrarlo bonito)
        [StringLength(250)]
        public string NombreOriginal { get; set; } = string.Empty;

        public long TamanoBytes { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;

        // Navegación hacia la tarjeta (la que el DbContext necesita)
        public virtual TarjetaImplante Tarjeta { get; set; } = null!;
    }
}