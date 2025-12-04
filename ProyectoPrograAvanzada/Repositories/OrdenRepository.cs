using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Data.Entity;

namespace ProyectoPrograAvanzada.Repositories
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly ApplicationDbContext _db;

        public OrdenRepository()
        {
            _db = new ApplicationDbContext();
        }

        // --- Métodos de Lectura (Consultas) ---

        public Orden GetOrdenById(int id)
        {
            // Cargamos la orden con sus detalles y el producto para cada detalle.
            return _db.Ordenes
                       .Include(o => o.DetallesOrden.Select(d => d.Producto))
                       .FirstOrDefault(o => o.idOrden == id);
        }

        public IEnumerable<Orden> GetAllOrdenes()
        {
            // Para el panel de administración, cargamos el usuario asociado.
            return _db.Ordenes.Include(o => o.Usuario).ToList();
        }

        public IEnumerable<Orden> GetHistorialByUsuario(int idUsuario)
        {
            // Obtenemos las órdenes de un usuario específico, con sus detalles principales.
            return _db.Ordenes
                       .Include(o => o.DetallesOrden)
                       .Where(o => o.idUsuario == idUsuario)
                       .OrderByDescending(o => o.fecha)
                       .ToList();
        }

        // --- Método de Escritura (Transacción) ---

        // Este método encapsula la lógica para guardar la orden y sus detalles
        public void SaveNewOrder(Orden orden, List<DetalleOrden> detalles)
        {
            // EF6 maneja automáticamente las transacciones con SaveChanges si no hay fallos.

            // 1. Agregar el header de la Orden
            _db.Ordenes.Add(orden);
            _db.SaveChanges(); // Guarda la Orden y obtiene el idOrden generado

            // 2. Asignar el idOrden generado a todos los detalles
            foreach (var detalle in detalles)
            {
                detalle.idOrden = orden.idOrden;
                _db.DetallesOrden.Add(detalle);
            }

            // 3. Guardar todos los detalles en la base de datos
            _db.SaveChanges();
        }

        // NOTA: No se implementa aquí Update o Delete de órdenes, ya que las órdenes
        // son generalmente registros inmutables una vez creados.
    }
}