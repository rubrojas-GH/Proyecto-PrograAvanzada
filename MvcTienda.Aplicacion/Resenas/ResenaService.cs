using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Aplicacion.Resenas
{
    public class ResenaService : IResenaService
    {
        // Dependencia del Repositorio
        private readonly IResenaRepository _resenaRepository;

        // El servicio de resenas puede necesitar el de productos para validar la existencia del producto
        private readonly IProductoRepository _productoRepository;

        // Constructor para inyectar las dependencias
        public ResenaService(IResenaRepository resenaRepository, IProductoRepository productoRepository)
        {
            _resenaRepository = resenaRepository;
            _productoRepository = productoRepository;
        }

        // --- Métodos para Asociados ---

        public void CreateResena(ResenaDto dto, int idUsuario)
        {
            // 1. Validar que el producto existe
            var producto = _productoRepository.GetProductoById(dto.IdProducto);

            // Si el producto no existe, lanzar una excepción
            if (producto == null)
                throw new InvalidOperationException("No se puede añadir una reseña a un producto inexistente.");

            // 2. Mapear el DTO a la entidad de dominio
            var resena = new Resena
            {
                idProducto = dto.IdProducto,
                idUsuario = idUsuario, // Asociar la reseña al usuario que la crea
                contenido = dto.Comentario,
                calificacion = dto.Calificacion,

                // --- AJUSTES PARA LOGICA DE NEGOCIO ---
                fecha = DateTime.Now,           // Seteamos la fecha actual
                estado = "Pendiente"            // Forzamos el estado inicial para moderación
            };

            // 3. Persistir la nueva reseña
            _resenaRepository.AddResena(resena);
        }


        public IEnumerable<ResenaDto> GetResenasAprobadasByProducto(int idProducto)
        {
            return _resenaRepository
                .GetResenasAprobadasByProducto(idProducto)
                .Select(r => new ResenaDto
                {
                    Id = r.idResena,
                    IdProducto = r.idProducto,
                    Comentario = r.contenido,
                    Calificacion = r.calificacion,
                    Estado = r.estado,
                    // Mapeo de campos de visualización
                    NombreUsuario = r.Usuario != null ? r.Usuario.nombre : "Anónimo",
                    Fecha = r.fecha
                })
                .ToList(); // .ToList() para forzar la ejecución y el mapeo
        }

        // --- Métodos para Administradores (Aprobación - RF4) ---

        public IEnumerable<ResenaDto> GetResenasPendientes()
        {
            // El Repositorio se encarga de filtrar las reseñas 'Pendientes'
            return _resenaRepository
        .GetResenasPendientes()
        .Select(r => new ResenaDto
        {
            Id = r.idResena,
            IdProducto = r.idProducto,
            Comentario = r.contenido,
            Calificacion = r.calificacion,
            Estado = r.estado,
            // Mapeo de campos de visualización
            NombreUsuario = r.Usuario != null ? r.Usuario.nombre : "Anónimo",
            // Mapear el nombre del Producto
            NombreProducto = r.Producto != null ? r.Producto.nombreProducto : "Eliminado",
            Fecha = r.fecha
        })
        .ToList(); // .ToList() para forzar la ejecución y el mapeo
        }

        public void AprobarResena(int idResena)
        {
            var resena = _resenaRepository.GetResenaById(idResena);

            if (resena == null)
            {
                throw new InvalidOperationException("La reseña no existe.");
            }

            // Lógica RF4: Cambiar el estado a "Aprobada"
            resena.estado = "Aprobada";
            _resenaRepository.UpdateResena(resena);
        }

        public void RechazarResena(int idResena)
        {
            var resena = _resenaRepository.GetResenaById(idResena);

            if (resena == null)
            {
                throw new InvalidOperationException("La reseña no existe.");
            }

            // Lógica RF4: Se elimina la reseña si es rechazada
            _resenaRepository.DeleteResena(idResena);
        }
    }
}
