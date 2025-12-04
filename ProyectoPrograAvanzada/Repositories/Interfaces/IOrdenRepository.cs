using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Repositories
{
    public interface IOrdenRepository
    {
        // CRUD de la Orden (Header)
        Orden GetOrdenById(int id);
        IEnumerable<Orden> GetAllOrdenes(); // Para el Panel de Administración

        // Historial de Compras (RF3)
        IEnumerable<Orden> GetHistorialByUsuario(int idUsuario);

        // Método clave para el proceso de compra
        void SaveNewOrder(Orden orden, List<DetalleOrden> detalles);

        // Métodos de gestión de detalles (aunque se usarán internamente con SaveNewOrder)
        // DetalleOrden GetDetalleOrden(int idOrden, int idProducto);
    }
}
