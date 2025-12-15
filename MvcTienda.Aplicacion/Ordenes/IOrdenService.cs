using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Ordenes
{
    public interface IOrdenService
    {
        /// <summary>
        /// Procesa la compra completa: verifica stock, calcula total, guarda la orden y actualiza inventario.
        /// </summary>
        /// <param name="idUsuario">ID del usuario que realiza la compra.</param>
        /// <param name="itemsCarrito">Lista de productos y cantidades a comprar.</param>
        /// <returns>La Orden creada si la transacción es exitosa.</returns>
        OrdenDto ProcesarCompra(int idUsuario, Dictionary<int, int> itemsCarrito);

        /// Obtiene el historial de órdenes de un usuario específico.
        IEnumerable<OrdenDto> GetHistorialUsuario(int idUsuario);

        /// Obtiene todas las órdenes (uso administrativo).
        IEnumerable<OrdenDto> GetAllOrdenes();

        /// <summary>
        /// Obtiene una orden específica por su ID.
        /// </summary>
        OrdenDto GetOrdenById(int idOrden);
    }
}
