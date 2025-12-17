using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Aplicacion.Ordenes
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        // Inyección de dos dependencias: Repositorio de Órdenes y Repositorio de Productos
        public OrdenService(IOrdenRepository ordenRepository, IProductoRepository productoRepository, IUsuarioRepository usuarioRepository)
        {
            _ordenRepository = ordenRepository;
            _productoRepository = productoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public IEnumerable<OrdenDto> GetAllOrdenes()
        {
            return _ordenRepository
                .GetAllOrdenes()
                .Select(MapToDto)
                .ToList();
        }

        // --- Método clave del Proceso de Compra (RF3) ---

        public OrdenDto ProcesarCompra(int idUsuario, Dictionary<int, int> itemsCarrito)
        {
            var detalles = new List<DetalleOrden>();
            decimal totalCompra = 0;

            // 1. Verificación de Stock y Construcción de Detalles
            foreach (var item in itemsCarrito)
            {
                int idProducto = item.Key;
                int cantidadRequerida = item.Value;

                // Obtener el producto para verificar stock y precio
                // El repositorio debe devolver el producto con los datos necesarios (incluyendo nombre para el DTO).
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
                    subtotal = producto.precioProducto * cantidadRequerida,
                    // Temporalmente asociamos la entidad Producto para facilitar el mapeo si es necesario.
                    // Esto depende de cómo se maneje en la capa de persistencia.
                    Producto = producto
                };

                detalles.Add(detalle);
                totalCompra += detalle.subtotal;
            }

            // 2. Crear la Cabecera de la Orden
            var nuevaOrden = new Orden
            {
                idUsuario = idUsuario,
                fecha = DateTime.Now,
                total = totalCompra
            };

            // 3. Registrar la Orden y sus Detalles (Transacción)
            // Se asume que SaveNewOrder (en el Repositorio) maneja la transacción y asigna el ID a nuevaOrden.
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

            // Mapear la orden completa, incluyendo los detalles y el nombre del producto, para el retorno.
            // Para el DTO de retorno, aprovechamos la lista 'detalles' que contiene el 'Producto' para obtener el nombre.
            var ordenDto = MapToDto(nuevaOrden);

            ordenDto.Items = detalles.Select(d => new OrdenItemDto
            {
                IdOrden = ordenDto.Id,
                IdProducto = d.idProducto,
                Cantidad = d.cantidad,
                PrecioUnitario = d.precioUnitario,
                NombreProducto = d.Producto.nombreProducto, // Usamos la entidad Producto asociada temporalmente.
                // Subtotal ya está implementado en el DTO
            }).ToList();

            return ordenDto;
        }

        // --- Métodos de Consulta ---

        public IEnumerable<OrdenDto> GetHistorialUsuario(int idUsuario)
        {
            return _ordenRepository
                .GetHistorialByUsuario(idUsuario)
                .Select(MapToDto)
                .ToList();
        }

        public OrdenDto GetOrdenById(int idOrden)
        {
            var orden = _ordenRepository.GetOrdenById(idOrden);
            return orden == null ? null : MapToDto(orden);
        }

        // --- Mapper Privado (Ajustado para incluir ítems) ---

        private OrdenDto MapToDto(Orden orden)
        {
            string nombreUsuario = _usuarioRepository.GetUsuarioById(orden.idUsuario)?.nombre ?? "Usuario Desconocido";
            // Asumimos que la Entidad Orden incluye la colección 'detallesOrden' cargada por el repositorio.
            // Asumimos que cada DetalleOrden incluye la navegación 'Producto' cargada.
            return new OrdenDto
            {
                Id = orden.idOrden,
                IdUsuario = orden.idUsuario,
                NombreUsuario = nombreUsuario,
                Fecha = orden.fecha,
                Total = orden.total,
                // Mapeo de la colección de detalles de la orden
                Items = orden.DetallesOrden?
                    .Select(d => new OrdenItemDto
                    {
                        IdOrden = orden.idOrden,
                        IdProducto = d.idProducto,
                        Cantidad = d.cantidad,
                        PrecioUnitario = d.precioUnitario,
                        NombreProducto = d.Producto?.nombreProducto // Asumimos navegación a Producto
                    })
                    .ToList() ?? new List<OrdenItemDto>()
                // Nota: Si el usuario es necesario, el repositorio debería cargarlo y aquí se mapearía:
                // NombreUsuario = orden.Usuario?.nombre
            };
        }
    }
}