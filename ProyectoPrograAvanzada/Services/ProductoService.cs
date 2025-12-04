using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Web;

namespace ProyectoPrograAvanzada.Services
{
    public class ProductoService : IProductoService
    {
        // Dependencia del Repositorio (Inyección de Dependencias)
        private readonly IProductoRepository _productoRepository;

        // Constructor para inyectar la dependencia
        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // --- Métodos de Catálogo Público ---

        // Implementación: Obtiene solo los productos que están activos (estado = true)
        public IEnumerable<Producto> GetCatalogoProductosActivos()
        {
            // Lógica de negocio: Filtrar solo los activos
            return _productoRepository.GetAllProductos()
                                      .Where(p => p.estadoProducto == true && p.stock > 0)
                                      .ToList();
        }

        // Implementación: Obtiene un producto con todas sus entidades relacionadas (Imágenes y Reseñas)
        public Producto GetProductoConDetalles(int id)
        {
            // El Repositorio se encarga del .Include(). Aquí solo se retorna el objeto.
            return _productoRepository.GetProductoById(id);
        }

        // --- Métodos CRUD para Administradores (RF2) ---

        public void CreateProducto(Producto producto, IEnumerable<string> urlsImagenes)
        {
            // Lógica de Negocio: Validar stock, precio, etc., antes de guardar.

            if (producto.stock < 0)
            {
                throw new ArgumentException("El stock inicial no puede ser negativo.");
            }

            // 1. Guardar el Producto principal
            _productoRepository.AddProducto(producto); // Asumimos que AddProducto actualiza el idProducto del modelo

            // 2. Guardar las imágenes asociadas
            if (urlsImagenes != null)
            {
                foreach (var url in urlsImagenes)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        var imagen = new ImagenProducto
                        {
                            urlImagen = url,
                            idProducto = producto.idProducto // Usamos el ID generado por la DB
                        };
                        _productoRepository.AddImagen(imagen);
                    }
                }
            }
        }

        public void UpdateProducto(Producto producto)
        {
            // Lógica de Negocio: Asegurar que el estado del producto es coherente (ej. si stock es 0, quizás cambiar el estado a inactivo)

            // Si el stock llega a cero, la lógica puede forzar el estado a falso (inactivo)
            if (producto.stock == 0)
            {
                producto.estadoProducto = false;
            }

            _productoRepository.UpdateProducto(producto);
        }

        public void DeleteProducto(int id)
        {
            // El Repositorio se encarga de eliminar las dependencias (Imágenes, Reseñas).
            _productoRepository.DeleteProducto(id);
        }
    }
}