using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Productos
{
    public interface IProductoService
    {
        // Métodos ajustados para usar ProductoDto
        IEnumerable<ProductoDto> GetCatalogoProductosActivos();
        ProductoDto GetProductoConDetalles(int id);

        // Los métodos de CRUD aceptan el nuevo ProductoDto
        void CreateProducto(ProductoDto productoDto, IEnumerable<string> urlsImagenes);
        void UpdateProducto(ProductoDto productoDto);
        void DeleteProducto(int id);

        IEnumerable<ProductoDto> GetAllProductosAdmin();
    }
}