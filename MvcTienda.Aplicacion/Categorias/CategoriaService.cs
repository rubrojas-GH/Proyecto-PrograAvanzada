using MvcTienda.Aplicacion.Categorias;
using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Aplicacion.Categorias
{
    public class CategoriaService : ICategoriaService
    {
        // Inyectamos el Repositorio de Categorías para acceder a la base de datos
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public IEnumerable<CategoriaDto> GetAllCategorias()
        {
            // Obtenemos las categorías del repositorio
            var categorias = _categoriaRepository.GetAll();

            return categorias
                .OrderBy(c => c.idCategoria)
                .Select(c => new CategoriaDto
                {
                    IdCategoria = c.idCategoria,
                    NombreCategoria = c.nombreCategoria,
                    DescripcionCategoria = c.descripcionCategoria
                })
                .ToList();
        }

        public CategoriaDto GetCategoriaById(int id)
        {
            var categoria = _categoriaRepository.GetById(id);

            if (categoria == null)
            {
                return null;
            }

            return new CategoriaDto
            {
                IdCategoria = categoria.idCategoria,
                NombreCategoria = categoria.nombreCategoria,
                DescripcionCategoria = categoria.descripcionCategoria
            };
        }

        public bool CreateCategoria(CategoriaDto dto)
        {
            // 1. Lógica de negocio: No permitir nombres duplicados
            if (_categoriaRepository.Exists(dto.NombreCategoria))
            {
                return false; // Indicamos que la creación falló por duplicidad
            }

            // 2. Mapeo de DTO a Entidad
            var nuevaCategoria = new Categoria
            {
                nombreCategoria = dto.NombreCategoria,
                descripcionCategoria = dto.DescripcionCategoria
            };

            // 3. Llamada al repositorio
            _categoriaRepository.Add(nuevaCategoria);
            return true;
        }

        public bool UpdateCategoria(CategoriaDto dto)
        {
            var categoriaExistente = _categoriaRepository.GetById(dto.IdCategoria);

            if (categoriaExistente == null)
            {
                return false;
            }

            // Lógica de negocio: Validar duplicidad, asegurando que no se compare consigo mismo
            if (_categoriaRepository.GetAll().Any(c =>
                c.nombreCategoria.Equals(dto.NombreCategoria, System.StringComparison.OrdinalIgnoreCase) &&
                c.idCategoria != dto.IdCategoria))
            {
                return false;
            }

            // Mapeo (actualización)
            categoriaExistente.nombreCategoria = dto.NombreCategoria;
            categoriaExistente.descripcionCategoria = dto.DescripcionCategoria;

            _categoriaRepository.Update(categoriaExistente);
            return true;
        }

        public void DeleteCategoria(int id)
        {
            // Nota de Negocio: En un sistema real, antes de borrar una categoría, 
            // se debe verificar si hay productos asociados. 
            // Si los hay, se debería impedir el borrado o asignar esos productos a una categoría por defecto.
            _categoriaRepository.Delete(id);
        }
    }
}