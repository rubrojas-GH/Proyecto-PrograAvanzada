using System.Collections.Generic;
using MvcTienda.Domain.Entities;

namespace MvcTienda.Domain.Repositories
{
    public interface IResenaRepository
    {
        // Métodos de consulta
        Resena GetResenaById(int id);
        IEnumerable<Resena> GetResenasPendientes(); // Para el Administrador
        IEnumerable<Resena> GetResenasAprobadasByProducto(int idProducto); // Para el público

        // Métodos de escritura
        void AddResena(Resena resena);
        void UpdateResena(Resena resena); // Usado para APROBAR/RECHAZAR

        // Otros
        void DeleteResena(int id);
    }
}
