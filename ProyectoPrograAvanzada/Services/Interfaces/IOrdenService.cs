using ProyectoPrograAvanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Services.Interfaces
{
    public interface IOrdenService
    {
        /// <summary>
        /// Procesa la compra completa: verifica stock, calcula total, guarda la orden y actualiza inventario.
        /// </summary>
        /// <param name="idUsuario">ID del usuario que realiza la compra.</param>
        /// <param name="itemsCarrito">Lista de productos y cantidades a comprar.</param>
        /// <returns>La Orden creada si la transacción es exitosa.</returns>
        Orden ProcesarCompra(int idUsuario, Dictionary<int, int> itemsCarrito);

        /// Obtiene el historial de órdenes de un usuario específico.
        IEnumerable<Orden> GetHistorialUsuario(int idUsuario);

        IEnumerable<Orden> GetAllOrdenes();

        /// <summary>
        /// Obtiene una orden específica por su ID.
        /// </summary>
        Orden GetOrdenById(int idOrden);
    }
}
