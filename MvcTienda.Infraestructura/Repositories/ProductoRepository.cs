using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;

namespace MvcTienda.Infraestructura.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _db;

        // ELIMINAR el constructor sin parámetros.
        // Solo mantenemos la inyección de dependencias para el DbContext.
        public ProductoRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        // =========================
        // CRUD PRODUCTOS
        // =========================

        public Producto GetProductoById(int id)
        {
            return _db.Productos
                      .Include(p => p.Imagenes)
                      .Include(p => p.Resenas)
                      .Include(p => p.Categoria)
                      .FirstOrDefault(p => p.idProducto == id);
        }

        public IEnumerable<Producto> GetAllProductos()
        {
            return _db.Productos
                      .Include(p => p.Imagenes)
                      .Include(p => p.Categoria)
                      .ToList();
        }

        public void AddProducto(Producto producto)
        {
            _db.Productos.Add(producto);
            _db.SaveChanges();
        }

        public void UpdateProducto(Producto producto)
        {
            _db.Entry(producto).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void DeleteProducto(int id)
        {
            var producto = _db.Productos.Find(id);

            if (producto != null)
            {
                // Eliminar imágenes asociadas
                var imagenes = _db.ImagenesProducto
                                  .Where(i => i.idProducto == id)
                                  .ToList();
                _db.ImagenesProducto.RemoveRange(imagenes);

                // Eliminar resenas asociadas
                var resenas = _db.Resenas
                                 .Where(r => r.idProducto == id)
                                 .ToList();
                _db.Resenas.RemoveRange(resenas);

                _db.Productos.Remove(producto);
                _db.SaveChanges();
            }
        }

        // =========================
        // IMÁGENES
        // =========================

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
