using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Services.Interfaces
{
    public interface IReseñaService
    {
        // Métodos para Asociados (Escribir y Consultar)
        void CreateReseña(Reseña reseña);
        IEnumerable<Reseña> GetReseñasAprobadasByProducto(int idProducto);

        // Métodos para Administradores (Aprobación - RF4)
        IEnumerable<Reseña> GetReseñasPendientes();
        void AprobarReseña(int idReseña);
        void RechazarReseña(int idReseña);
    }
}
