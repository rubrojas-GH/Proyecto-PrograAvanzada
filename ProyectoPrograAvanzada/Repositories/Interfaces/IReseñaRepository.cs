using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Repositories.Interfaces
{
    public interface IReseñaRepository
    {
        // Métodos de consulta
        Reseña GetReseñaById(int id);
        IEnumerable<Reseña> GetReseñasPendientes(); // Para el Administrador
        IEnumerable<Reseña> GetReseñasAprobadasByProducto(int idProducto); // Para el público

        // Métodos de escritura
        void AddReseña(Reseña reseña);
        void UpdateReseña(Reseña reseña); // Usado para APROBAR/RECHAZAR

        // Otros
        void DeleteReseña(int id);
    }
}
