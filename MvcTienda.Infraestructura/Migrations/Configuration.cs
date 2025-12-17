namespace MvcTienda.Infraestructura.Migrations
{
    using MvcTienda.Domain.Entities;
    using MvcTienda.Domain.Security;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

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
            // Se utiliza el email como identificador único para la verificación
            var adminExistente = context.Usuarios.SingleOrDefault(u => u.email == "admin@tienda.com");

            if (adminExistente == null)
            {
                var adminRole = context.Roles.SingleOrDefault(r => r.nombreRol == "Administrador");
                if (adminRole != null)
                {
                    context.Usuarios.Add(new Usuario
                    {
                        email = "admin@tienda.com",
                        contrasena = PasswordHasher.HashPassword("Admin123*"),
                        nombre = "Admin Sistema",
                        estado = true,
                        idRol = adminRole.idRol,
                        ultimaConexion = DateTime.Now
                    });
                }
            }
            else
            {
                // Forzamos el hash correcto por si el anterior falló
                adminExistente.contrasena = PasswordHasher.HashPassword("Admin123*");
                adminExistente.estado = true;
            }
            context.SaveChanges();

            // 3. POBLAR CATEGORÍAS (Contexto: Bazar y Librería)
            context.Categorias.AddOrUpdate(
                            c => c.nombreCategoria,
                            new Categoria { nombreCategoria = "Libros y Literatura" },
                            new Categoria { nombreCategoria = "Papelería y Oficina" },
                            new Categoria { nombreCategoria = "Artículos de Bazar" }
                        );
            context.SaveChanges();

            // 4. OBTENER IDs DE LAS CATEGORÍAS CREADAS
            // Buscamos los IDs generados para relacionarlos con los productos
            var librosId = context.Categorias.First(c => c.nombreCategoria == "Libros y Literatura").idCategoria;
            var papeleriaId = context.Categorias.First(c => c.nombreCategoria == "Papelería y Oficina").idCategoria;
            var bazarId = context.Categorias.First(c => c.nombreCategoria == "Artículos de Bazar").idCategoria;

            // 5. POBLAR PRODUCTOS (Usando la FK idCategoria y contexto de Librería)
            context.Productos.AddOrUpdate(
                            p => p.nombreProducto,
                            new Producto
                            {
                                nombreProducto = "Don Quijote de la Mancha",
                                precioProducto = 12500.00m,
                                stock = 15,
                                descripcion = "Edición de lujo con tapa dura y comentarios.",
                                idCategoria = librosId,
                                estadoProducto = true,
                                // Agregamos la imagen directamente aquí:
                                Imagenes = new List<ImagenProducto> {
                        new ImagenProducto { urlImagen = "https://m.media-amazon.com/images/S/compressed.photo.goodreads.com/books/1678144051i/122858226.jpg" }
                                }
                            },
                            new Producto
                            {
                                nombreProducto = "Set de Plumas Fuente",
                                precioProducto = 25000.00m,
                                stock = 8,
                                descripcion = "Estuche elegante con 3 plumas para caligrafía.",
                                idCategoria = papeleriaId,
                                estadoProducto = true,
                                Imagenes = new List<ImagenProducto> {
                        new ImagenProducto { urlImagen = "https://ss213.liverpool.com.mx/xl/1117145537.jpg" }
                                }
                            },
                            new Producto
                            {
                                nombreProducto = "Organizador de Escritorio Madera",
                                precioProducto = 18500.00m,
                                stock = 12,
                                descripcion = "Hecho a mano, ideal para oficina u hogar.",
                                idCategoria = bazarId,
                                estadoProducto = true,
                                Imagenes = new List<ImagenProducto> {
                        new ImagenProducto { urlImagen = "https://m.media-amazon.com/images/I/71I4vaAhLfS._AC_UF894,1000_QL80_.jpg" }
                                }
                            },
                            new Producto
                            {
                                nombreProducto = "Cuaderno Profesional Rayado",
                                precioProducto = 3500.00m,
                                stock = 100,
                                descripcion = "100 hojas de papel premium, pasta resistente.",
                                idCategoria = papeleriaId,
                                estadoProducto = true,
                                Imagenes = new List<ImagenProducto> {
                        new ImagenProducto { urlImagen = "https://proveedoradeoficinas.com/ARTICULOS/17088/1.jpg" }
                                }
                            }
                        );
            context.SaveChanges();
        }
    }
}