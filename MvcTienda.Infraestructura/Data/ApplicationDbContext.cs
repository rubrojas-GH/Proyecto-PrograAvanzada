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
            // 1. Llave Primaria Compuesta para DetalleOrden
            modelBuilder.Entity<DetalleOrden>()
                .HasKey(d => new { d.idOrden, d.idProducto })
                .Ignore(d => d.idDetalleOrden);

            // 2. Mapeo de nombres de tablas (Para seguir el estándar que iniciaste)
            modelBuilder.Entity<Rol>().ToTable("ROLES");
            modelBuilder.Entity<Usuario>().ToTable("USUARIOS");
            modelBuilder.Entity<Producto>().ToTable("PRODUCTOS");
            modelBuilder.Entity<Categoria>().ToTable("CATEGORIAS");
            modelBuilder.Entity<Resena>().ToTable("RESENAS");
            modelBuilder.Entity<ImagenProducto>().ToTable("IMAGENES_PRODUCTO");
            modelBuilder.Entity<Orden>().ToTable("ORDENES");
            modelBuilder.Entity<DetalleOrden>().ToTable("DETALLES_ORDEN");

            base.OnModelCreating(modelBuilder);
        }
    }
}