using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTienda.Aplicacion.Roles
{
    public class RolService : IRolService
    {
        // El repositorio es el punto de acceso a los datos del dominio.
        // La capa de Aplicación NO debe depender directamente de Entity Framework
        // ni del ApplicationDbContext.
        private readonly IRolRepository _rolRepository;

        // Inyección de dependencias del repositorio
        public RolService(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        // --- MÉTODOS DE CONSULTA ---

        /// <summary>
        /// Obtiene todos los roles disponibles en la base de datos.
        /// </summary>
        public IEnumerable<RolDto> GetAllRoles()
        {
            // Se obtienen las entidades desde el repositorio
            // y se transforman a DTOs para exponerlos a la capa Web.
            return _rolRepository
                .GetAllRoles()
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Obtiene un rol por su ID.
        /// </summary>
        public RolDto GetRolById(int idRol)
        {
            // Retorna null si el rol no existe
            var rol = _rolRepository.GetRolById(idRol);
            return rol == null ? null : MapToDto(rol);
        }

        /// <summary>
        /// Obtiene un rol por su nombre.
        /// </summary>
        public RolDto GetRolByName(string nombreRol)
        {
            // Busca el primer rol que coincida con el nombre, o retorna null
            var rol = _rolRepository.GetRolByName(nombreRol);
            return rol == null ? null : MapToDto(rol);
        }

        // --- MÉTODOS DE ADMINISTRACIÓN (CRUD) ---

        /// <summary>
        /// Crea un nuevo rol en el sistema.
        /// </summary>
        public void CreateRol(RolDto dto)
        {
            // 1. Verificar si el nombre del rol ya existe para evitar duplicados
            if (_rolRepository.ExistsRolName(dto.Nombre))
            {
                throw new InvalidOperationException("Ya existe un rol con ese nombre.");
            }

            // 2. Mapear el DTO a la entidad de dominio
            var rol = new Rol
            {
                nombreRol = dto.Nombre
            };

            // 3. Persistir el nuevo rol
            _rolRepository.AddRol(rol);
        }

        /// <summary>
        /// Actualiza un rol existente.
        /// </summary>
        public void UpdateRol(RolDto dto)
        {
            // 1. Verificar si el rol existe
            var rol = _rolRepository.GetRolById(dto.Id);
            if (rol == null)
            {
                throw new InvalidOperationException("El rol no existe.");
            }

            // 2. Verificar que el nuevo nombre no se duplique con otro rol
            if (_rolRepository.ExistsRolName(dto.Nombre, dto.Id))
            {
                throw new InvalidOperationException("Ya existe otro rol con ese nombre.");
            }

            // 3. Actualizar propiedades del dominio
            rol.nombreRol = dto.Nombre;

            // 4. Persistir cambios
            _rolRepository.UpdateRol(rol);
        }

        /// <summary>
        /// Elimina un rol existente.
        /// </summary>
        public void DeleteRol(int idRol)
        {
            // LÓGICA DE NEGOCIO CRÍTICA (RF1):
            // No se permite eliminar un rol si tiene usuarios asociados.
            if (_rolRepository.HasUsuarios(idRol))
            {
                throw new InvalidOperationException(
                    "No se puede eliminar este rol porque tiene usuarios asociados."
                );
            }

            // Eliminación definitiva del rol
            _rolRepository.DeleteRol(idRol);
        }

        // --- MAPPER PRIVADO ---

        /// <summary>
        /// Mapea una entidad Rol del dominio a un RolDto.
        /// </summary>
        private RolDto MapToDto(Rol rol)
        {
            return new RolDto
            {
                Id = rol.idRol,
                Nombre = rol.nombreRol
            };
        }
    }
}
