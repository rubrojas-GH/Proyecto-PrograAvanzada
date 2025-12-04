using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Web;

namespace ProyectoPrograAvanzada.Services
{
    public class ReseñaService : IReseñaService
    {
        // Dependencia del Repositorio
        private readonly IReseñaRepository _reseñaRepository;

        // El servicio de reseñas puede necesitar el de productos para validar la existencia del producto
        private readonly IProductoRepository _productoRepository;

        // Constructor para inyectar las dependencias
        public ReseñaService(IReseñaRepository reseñaRepository, IProductoRepository productoRepository)
        {
            _reseñaRepository = reseñaRepository;
            _productoRepository = productoRepository;
        }

        // --- Métodos para Asociados ---

        public void CreateReseña(Reseña reseña)
        {
            // Lógica de Negocio: Validar que el producto exista antes de guardar.
            var producto = _productoRepository.GetProductoById(reseña.idProducto);

            if (producto == null)
            {
                throw new InvalidOperationException("No se puede añadir una reseña a un producto inexistente.");
            }

            // El Repositorio debe asignar la fecha y el estado 'Pendiente' (RF4).
            _reseñaRepository.AddReseña(reseña);
        }

        public IEnumerable<Reseña> GetReseñasAprobadasByProducto(int idProducto)
        {
            // El Repositorio se encarga de filtrar solo las reseñas 'Aprobadas' (RF4).
            return _reseñaRepository.GetReseñasAprobadasByProducto(idProducto);
        }

        // --- Métodos para Administradores (Aprobación - RF4) ---

        public IEnumerable<Reseña> GetReseñasPendientes()
        {
            // El Repositorio se encarga de filtrar las reseñas 'Pendientes'.
            return _reseñaRepository.GetReseñasPendientes();
        }

        public void AprobarReseña(int idReseña)
        {
            var reseña = _reseñaRepository.GetReseñaById(idReseña);

            if (reseña == null)
            {
                throw new InvalidOperationException("La reseña no existe.");
            }

            // Lógica RF4: Cambiar el estado a "Aprobada"
            reseña.estado = "Aprobada";
            _reseñaRepository.UpdateReseña(reseña);
        }

        public void RechazarReseña(int idReseña)
        {
            var reseña = _reseñaRepository.GetReseñaById(idReseña);

            if (reseña == null)
            {
                throw new InvalidOperationException("La reseña no existe.");
            }

            // Lógica RF4: Se puede eliminar (como en este caso) o cambiar el estado a "Rechazada".
            // Optar por eliminarla si la rechazan.
            _reseñaRepository.DeleteReseña(idReseña);
        }
    }
}