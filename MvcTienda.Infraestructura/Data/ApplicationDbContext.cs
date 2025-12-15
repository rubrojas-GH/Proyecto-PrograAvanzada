using MvcTienda.Domain.Entities;
using System.Data.Entity;

namespace MvcTienda.Infraestructura.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        // =========================
        // DbSets
        // =========================

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Resena> Resenas { get; set; }
        public DbSet<ImagenProducto> ImagenesProducto { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<DetalleOrden> DetallesOrden { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // --- Configuración de Llaves Compuestas (Diagrama Relacional) ---

            // Configurar DetalleOrden para tener una Llave Primaria Compuesta (idOrden, idProducto)
            modelBuilder.Entity<DetalleOrden>()
                .HasKey(d => new { d.idOrden, d.idProducto })
                // Ignorar la propiedad idDetalle ya que la PK es compuesta
                .Ignore(d => d.idDetalleOrden);

            // Opcional: Configurar nombres de tablas para coincidir con el diagrama relacional
            modelBuilder.Entity<Orden>().ToTable("ORDENES");
            modelBuilder.Entity<DetalleOrden>().ToTable("DETALLES_ORDEN");

            base.OnModelCreating(modelBuilder);
        }
    }
}