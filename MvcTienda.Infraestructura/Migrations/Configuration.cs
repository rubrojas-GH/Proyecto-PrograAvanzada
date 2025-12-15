namespace MvcTienda.Infraestructura.Migrations
{
    using MvcTienda.Domain.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity; // Necesario para PasswordHasher

    internal sealed class Configuration : DbMigrationsConfiguration<MvcTienda.Infraestructura.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MvcTienda.Infraestructura.Data.ApplicationDbContext context)
        {
            // 1. POBLAR ROLES
            context.Roles.AddOrUpdate(
                r => r.nombreRol,
                new Rol { nombreRol = "Administrador" },
                new Rol { nombreRol = "Asociado" }
            );
            context.SaveChanges();

            // 2. CREAR USUARIO ADMINISTRADOR POR DEFECTO
            // Usar 'email' en el SingleOrDefault
            if (context.Usuarios.SingleOrDefault(u => u.email == "admin@tienda.com") == null)
            {
                var passwordHasher = new PasswordHasher();
                string hashedPassword = passwordHasher.HashPassword("Admin123*");

                var adminRole = context.Roles.SingleOrDefault(r => r.nombreRol == "Administrador");

                // Creamos la instancia del Administrador con los nombres de propiedad correctos
                var adminUser = new Usuario
                {
                    email = "admin@tienda.com",
                    contrasena = hashedPassword,                 
                    nombre = "Admin",                                                             
                    estado = true,
                    idRol = adminRole.idRol
                };

                context.Usuarios.Add(adminUser);
            }
            context.SaveChanges();

            // 3. POBLAR CATEGORÍAS
            context.Categorias.AddOrUpdate(
                c => c.nombreCategoria,
                new Categoria { nombreCategoria = "Electrónica" },
                new Categoria { nombreCategoria = "Hogar" },
                new Categoria { nombreCategoria = "Oficina" }
            );
            context.SaveChanges();

            // 4. OBTENER IDs DE LAS CATEGORÍAS CREADAS
            var electronicaId = context.Categorias.Single(c => c.nombreCategoria == "Electrónica").idCategoria;
            var hogarId = context.Categorias.Single(c => c.nombreCategoria == "Hogar").idCategoria;
            var oficinaId = context.Categorias.Single(c => c.nombreCategoria == "Oficina").idCategoria;

            // 5. POBLAR PRODUCTOS (Usando la FK idCategoria)
            context.Productos.AddOrUpdate(
                p => p.nombreProducto, // 
                new Producto { nombreProducto = "Laptop Gamer X", precioProducto = 1200.00m, stock = 10, descripcion = "La última generación de laptops para gaming.", idCategoria = electronicaId, estadoProducto = true },
                new Producto { nombreProducto = "Taza de Café", precioProducto = 15.50m, stock = 50, descripcion = "Taza de cerámica con logo.", idCategoria = hogarId, estadoProducto = true },
                new Producto { nombreProducto = "Silla Ergonómica Pro", precioProducto = 350.00m, stock = 20, descripcion = "Máximo soporte lumbar.", idCategoria = oficinaId, estadoProducto = true }
            );
            context.SaveChanges();
        }
    }
}