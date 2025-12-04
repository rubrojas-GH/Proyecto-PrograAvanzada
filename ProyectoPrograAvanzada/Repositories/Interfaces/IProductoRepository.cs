using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Producto GetProductoById(int id);
        IEnumerable<Producto> GetAllProductos();
        void AddProducto(Producto producto);
        void UpdateProducto(Producto producto);
        void DeleteProducto(int id);

        // Métodos para la gestión de imágenes
        void AddImagen(ImagenProducto imagen);
        void DeleteImagen(int idImagen);
    }
}