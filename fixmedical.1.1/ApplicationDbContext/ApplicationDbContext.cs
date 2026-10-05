using fixmedical._1._1.Modelos;
using fixmedical._1._1.Modelos.P;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FixMedicalWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Institucion> Instituciones { get; set; }
        public DbSet<Especialista> Especialistas { get; set; }
        public DbSet<Linea> Lineas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<InformacionPaciente> InformacionesPacientes { get; set; }
        public DbSet<RegistroConsumo> RegistrosConsumos { get; set; }
        public DbSet<HistorialCambio> HistorialesCambios { get; set; }
        public DbSet<ProgramacionCirugia> ProgramacionesCirugia { get; set; }
        public DbSet<HojaGasto> HojasGasto { get; set; }
        public DbSet<HojaGastoDetalle> HojasGastoDetalle { get; set; }
        public DbSet<TarjetaImplante> TarjetasImplante { get; set; }
        public DbSet<TarjetaImplanteDetalle> TarjetasImplanteDetalle { get; set; }
        public DbSet<SolicitudMaterial> SolicitudesMaterial { get; set; }
        public DbSet<TarjetasImplanteAdjuntos> TarjetasImplanteAdjuntos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HojaGasto>()
                .HasIndex(h => h.NumeroConsecutivo)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<InformacionPaciente>()
                .HasIndex(i => i.Consecutivo_Gasto)
                .IsUnique();

            // ▼▼▼ AGREGADO: relación Tarjeta ↔ Detalles con FK en ID_TarjetaImplante ▼▼▼
            modelBuilder.Entity<TarjetaImplanteDetalle>()
                .HasOne(d => d.Tarjeta)
                .WithMany(t => t.Detalles)
                .HasForeignKey(d => d.ID_TarjetaImplante);
            // ▲▲▲ FIN AGREGADO ▲▲▲

            // ▼▼▼ NUEVO: relación Tarjeta ↔ Adjuntos (imágenes/videos) ▼▼▼
            modelBuilder.Entity<TarjetasImplanteAdjuntos>()
                .HasOne(a => a.Tarjeta)
                .WithMany(t => t.Adjuntos)
                .HasForeignKey(a => a.ID_TarjetaImplante);
            // ▲▲▲ FIN NUEVO ▲▲▲

            modelBuilder.Entity<Role>().HasData(
                new Role { ID_Rol = 1, Nombre_Rol = "Administrador", Descripcion = "Acceso total. Administra usuarios, catálogo y auditoría." },
                new Role { ID_Rol = 2, Nombre_Rol = "EQC", Descripcion = "Especialista Quirúrgico. Registra pacientes, cirugías y consumos." },
                new Role { ID_Rol = 3, Nombre_Rol = "Auditor", Descripcion = "Solo lectura. Genera reportes y consulta consumos." },
                new Role { ID_Rol = 4, Nombre_Rol = "Comercial", Descripcion = "Genera solicitudes de material de cirugia ." }
            );
        }
    }
}