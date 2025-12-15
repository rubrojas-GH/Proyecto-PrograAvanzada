using System.Collections.Generic;
using MvcTienda.Domain.Entities;

namespace MvcTienda.Domain.Repositories
{
    public interface IProductoRepository
    {
        Producto GetProductoById(int id);
        IEnumerable<Producto> GetAllProductos();

        void AddProducto(Producto producto);
        void UpdateProducto(Producto producto);
        void DeleteProducto(int id);

        // Gestión de imágenes
        void AddImagen(ImagenProducto imagen);
        void DeleteImagen(int idImagen);
    }
}
