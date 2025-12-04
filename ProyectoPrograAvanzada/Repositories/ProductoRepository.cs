using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Web;
using System.Data.Entity;

namespace ProyectoPrograAvanzada.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _db;

        // Constructor con inicialización manual del DbContext
        public ProductoRepository()
        {
            _db = new ApplicationDbContext();
        }

        // --- Métodos CRUD de Producto (RF2) ---

        public Producto GetProductoById(int id)
        {
            // Usamos .Include() para cargar las entidades relacionadas (Imágenes y Reseñas)
            // Esto es necesario para mostrar la información completa en la vista de detalles.
            return _db.Productos
                       .Include(p => p.Imagenes)
                       .Include(p => p.Reseñas)
                       .FirstOrDefault(p => p.idProducto == id);
        }

        public IEnumerable<Producto> GetAllProductos()
        {
            // Cargamos las imágenes (al menos una) para el catálogo principal
            return _db.Productos.Include(p => p.Imagenes).ToList();
        }

        public void AddProducto(Producto producto)
        {
            _db.Productos.Add(producto);
            _db.SaveChanges();
        }

        public void UpdateProducto(Producto producto)
        {
            // Adjunta la entidad y marca su estado como modificado
            _db.Entry(producto).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void DeleteProducto(int id)
        {
            var producto = _db.Productos.Find(id);
            if (producto != null)
            {
                // Es buena práctica manejar la eliminación de elementos dependientes 
                // (Imágenes y Reseñas) si la base de datos no tiene eliminación en cascada configurada.

                // Eliminar imágenes
                var imagenes = _db.ImagenesProducto.Where(i => i.idProducto == id).ToList();
                _db.ImagenesProducto.RemoveRange(imagenes);

                // Eliminar reseñas
                var reseñas = _db.Reseñas.Where(r => r.idProducto == id).ToList();
                _db.Reseñas.RemoveRange(reseñas);

                _db.Productos.Remove(producto);
                _db.SaveChanges();
            }
        }

        // --- Métodos de Gestión de ImagenProducto ---

        public void AddImagen(ImagenProducto imagen)
        {
            _db.ImagenesProducto.Add(imagen);
            _db.SaveChanges();
        }

        public void DeleteImagen(int idImagen)
        {
            var imagen = _db.ImagenesProducto.Find(idImagen);
            if (imagen != null)
            {
                _db.ImagenesProducto.Remove(imagen);
                _db.SaveChanges();
            }
        }
    }
}