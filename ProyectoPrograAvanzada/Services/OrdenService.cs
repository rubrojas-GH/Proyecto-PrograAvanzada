using System.Web;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using ProyectoPrograAvanzada.Repositories;

namespace ProyectoPrograAvanzada.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;

        // Inyección de dos dependencias: Repositorio de Órdenes y Repositorio de Productos
        public OrdenService(IOrdenRepository ordenRepository, IProductoRepository productoRepository)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
        }

        public IEnumerable<Orden> GetAllOrdenes()
        {
            // Delega la llamada al Repositorio para obtener todas las órdenes.
            return _ordenRepository.GetAllOrdenes();
        }

        // --- Método clave del Proceso de Compra (RF3) ---

        public Orden ProcesarCompra(int idUsuario, Dictionary<int, int> itemsCarrito)
        {
            var detalles = new List<DetalleOrden>();
            decimal totalCompra = 0;

            // 1. Verificación de Stock y Construcción de Detalles
            foreach (var item in itemsCarrito)
            {
                int idProducto = item.Key;
                int cantidadRequerida = item.Value;

                // Obtener el producto para verificar stock y precio
                var producto = _productoRepository.GetProductoById(idProducto);

                if (producto == null || producto.estadoProducto == false)
                {
                    throw new InvalidOperationException($"Producto con ID {idProducto} no encontrado o inactivo.");
                }

                // Lógica de Negocio: Verificar disponibilidad (RF3)
                if (producto.stock < cantidadRequerida)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto {producto.nombreProducto}. Disponibles: {producto.stock}.");
                }

                // Si hay stock, se crea el detalle de la orden
                var detalle = new DetalleOrden
                {
                    idProducto = idProducto,
                    cantidad = cantidadRequerida,
                    precioUnitario = producto.precioProducto,
                    calcularSubtotal = producto.precioProducto * cantidadRequerida
                };

                detalles.Add(detalle);
                totalCompra += detalle.calcularSubtotal;
            }

            // 2. Crear la Cabecera de la Orden
            var nuevaOrden = new Orden
            {
                idUsuario = idUsuario,
                fecha = DateTime.Now,
                total = totalCompra
            };

            // 3. Registrar la Orden y sus Detalles (Transacción)
            // El Repositorio se encarga de guardar la Orden y los Detalles.
            _ordenRepository.SaveNewOrder(nuevaOrden, detalles);

            // 4. Actualizar el Inventario (Stock)
            foreach (var detalle in detalles)
            {
                var productoAActualizar = _productoRepository.GetProductoById(detalle.idProducto);
                if (productoAActualizar != null)
                {
                    productoAActualizar.stock -= detalle.cantidad;
                    _productoRepository.UpdateProducto(productoAActualizar);
                }
            }

            return nuevaOrden;
        }

        // --- Métodos de Consulta ---

        public IEnumerable<Orden> GetHistorialUsuario(int idUsuario)
        {
            return _ordenRepository.GetHistorialByUsuario(idUsuario);
        }

        public Orden GetOrdenById(int idOrden)
        {
            return _ordenRepository.GetOrdenById(idOrden);
        }
    }
}