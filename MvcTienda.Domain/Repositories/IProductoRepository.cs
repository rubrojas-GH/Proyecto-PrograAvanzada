using MvcTienda.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Domain.Repositories
{
    public interface IProductoRepository
    {
        Producto GetProductoById(int id);
        IEnumerable<Producto> GetAllProductos();

        IQueryable<Producto> GetAll();

        void AddProducto(Producto producto);
        void UpdateProducto(Producto producto);
        void DeleteProducto(int id);

        // Gestión de imágenes
        void AddImagen(ImagenProducto imagen);
        void DeleteImagen(int idImagen);
    }
}
