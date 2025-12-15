using MvcTienda.Domain.Entities;
using System.Collections.Generic;

namespace MvcTienda.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        // CRUD BÁSICO
        Categoria GetById(int id);
        IEnumerable<Categoria> GetAll();
        void Add(Categoria categoria);
        void Update(Categoria categoria);
        void Delete(int id);

        // CONSULTA ESPECÍFICA
        bool Exists(string nombre);
    }
}