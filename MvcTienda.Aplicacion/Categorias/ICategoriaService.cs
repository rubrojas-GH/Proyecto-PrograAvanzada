using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Categorias
{
    public interface ICategoriaService
    {
        IEnumerable<CategoriaDto> GetAllCategorias();
        CategoriaDto GetCategoriaById(int id);
        bool CreateCategoria(CategoriaDto dto);
        bool UpdateCategoria(CategoriaDto dto);
        void DeleteCategoria(int id);
    }
}