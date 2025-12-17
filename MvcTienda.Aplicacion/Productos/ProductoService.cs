using MvcTienda.Aplicacion.Resenas;
using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Aplicacion.Productos
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IResenaService _resenaService;

        public ProductoService(IProductoRepository productoRepository, IResenaService resenaService)
        {
            _productoRepository = productoRepository;
            _resenaService = resenaService;
        }

        // =========================
        // CATÁLOGO PÚBLICO
        // =========================

        public IEnumerable<ProductoDto> GetCatalogoProductosActivos(int? categoriaId = null)
        {
            var query = _productoRepository.GetAll()
                .Include(p => p.Imagenes)
                .Include(p => p.Categoria)
                .Where(p => p.estadoProducto == true); // 🟢 'estadoProducto' en minúscula

            if (categoriaId.HasValue)
            {
                // 🟢 'idCategoria' en minúscula (como en tu entidad)
                query = query.Where(p => p.idCategoria == categoriaId.Value);
            }

            return query.Select(p => new ProductoDto
            {
                // Lado Izquierdo = DTO (Mayúscula) | Lado Derecho = Entidad (Minúscula/Nombre largo)
                Id = p.idProducto,
                Nombre = p.nombreProducto,
                Descripcion = p.descripcion,
                Precio = p.precioProducto,
                Stock = p.stock,
                EstadoProducto = p.estadoProducto,
                IdCategoria = p.idCategoria,
                NombreCategoria = p.Categoria.nombreCategoria,

                Imagenes = p.Imagenes.Select(i => new ImagenProductoDto
                {
                    Id = i.idImagen, // Revisa si en ImagenProducto.cs es idImagen o Id
                    UrlImagen = i.urlImagen
                }).ToList()
            }).ToList();
        }

        public ProductoDto GetProductoConDetalles(int id)
        {
            var producto = _productoRepository.GetProductoById(id);

            if (producto == null)
            {
                return null;
            }

            // 1. Obtener las reseñas usando el Servicio de Reseñas
            var resenasAprobadas = _resenaService.GetResenasAprobadasByProducto(id);

            // 2. Mapear el Producto DTO
            return new ProductoDto
            {
                Id = producto.idProducto,
                Nombre = producto.nombreProducto,
                Descripcion = producto.descripcion,
                Precio = Convert.ToDecimal(producto.precioProducto),
                Stock = producto.stock,
                EstadoProducto = producto.estadoProducto,

                // Mapear ID y Nombre de Categoría para Detalles
                IdCategoria = producto.idCategoria,
                NombreCategoria = producto.Categoria?.nombreCategoria,

                // Mapeo de colecciones (Imágenes)
                Imagenes = producto.Imagenes
                                .Where(i => i != null)
                                .Select(i => new ImagenProductoDto { Id = i.idImagen, UrlImagen = i.urlImagen })
                                .ToList(),

                Resenas = resenasAprobadas.ToList()
            };
        }

        // =========================
        // CRUD ADMINISTRADOR (RF2)
        // =========================

        public IEnumerable<ProductoDto> GetAllProductosAdmin()
        {
            return _productoRepository.GetAllProductos()
               .Select(p => new ProductoDto
               {
                   Id = p.idProducto,
                   Nombre = p.nombreProducto,
                   Precio = p.precioProducto,
                   Stock = p.stock,
                   EstadoProducto = p.estadoProducto,
                   NombreCategoria = p.Categoria?.nombreCategoria // Mostrar nombre
               })
               .ToList();
        }

        public void CreateProducto(ProductoDto productoDto, IEnumerable<string> urlsImagenes)
        {
            if (productoDto.Stock < 0)
                throw new ArgumentException("El stock inicial no puede ser negativo.");

            var producto = new Producto
            {
                nombreProducto = productoDto.Nombre,
                precioProducto = productoDto.Precio,
                stock = productoDto.Stock,
                estadoProducto = true,
                idCategoria = productoDto.IdCategoria // 🟢 Usar la FK del DTO
            };

            _productoRepository.AddProducto(producto);

            if (urlsImagenes != null)
            {
                foreach (var url in urlsImagenes)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        _productoRepository.AddImagen(new ImagenProducto
                        {
                            urlImagen = url,
                            idProducto = producto.idProducto
                        });
                    }
                }
            }
        }

        public void UpdateProducto(ProductoDto productoDto)
        {
            var producto = _productoRepository.GetProductoById(productoDto.Id);

            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado.");

            producto.nombreProducto = productoDto.Nombre;
            producto.precioProducto = productoDto.Precio;
            producto.stock = productoDto.Stock;
            producto.idCategoria = productoDto.IdCategoria; // Actualizar la FK

            // Lógica de negocio: si no hay stock, se desactiva
            producto.estadoProducto = producto.stock > 0;

            _productoRepository.UpdateProducto(producto);
        }

        public void DeleteProducto(int id)
        {
            _productoRepository.DeleteProducto(id);
        }
    }
}