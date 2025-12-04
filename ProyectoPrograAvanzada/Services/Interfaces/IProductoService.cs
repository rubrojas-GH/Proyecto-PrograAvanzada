using System;
using System.Collections.Generic;
using ProyectoPrograAvanzada.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Services.Interfaces
{
    public interface IProductoService
    {
        // Métodos de Catálogo (para Asociados)
        IEnumerable<Producto> GetCatalogoProductosActivos();
        Producto GetProductoConDetalles(int id); // Incluye imágenes y reseñas

        // Métodos CRUD (para Administradores - RF2)
        void CreateProducto(Producto producto, IEnumerable<string> urlsImagenes);
        void UpdateProducto(Producto producto);
        void DeleteProducto(int id);
    }
}
