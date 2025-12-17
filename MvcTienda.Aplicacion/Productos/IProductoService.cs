using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Productos
{
    public interface IProductoService
    {
        ProductoDto GetProductoConDetalles(int id);

        void CreateProducto(ProductoDto productoDto, IEnumerable<string> urlsImagenes);
        void UpdateProducto(ProductoDto productoDto);
        void DeleteProducto(int id);

        IEnumerable<ProductoDto> GetAllProductosAdmin();

        IEnumerable<ProductoDto> GetCatalogoProductosActivos(int? categoriaId = null);
    }
}