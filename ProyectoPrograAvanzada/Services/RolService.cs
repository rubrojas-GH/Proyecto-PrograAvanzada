using Microsoft.Ajax.Utilities;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoPrograAvanzada.Services
{
    public class RolService : IRolService
    {
        // El ApplicationDbContext es tu conexión a la base de datos
        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS CRUD ---

        /// <summary>
        /// Obtiene todos los roles disponibles en la base de datos.
        /// </summary>
        public IEnumerable<Rol> GetAllRoles()
        {
            // Retorna una lista de todos los roles como IEnumerable<Rol> (que es más abstracto y mejor).
            return _context.Roles.OrderBy(r => r.idRol).ToList();
        }

        /// <summary>-
        /// Obtiene un rol por su ID.
        /// </summary>
        public Rol GetRolById(int id)
        {
            // Usa FirstOrDefault para retornar null si el rol no existe.
            return _context.Roles.FirstOrDefault(r => r.idRol == id);
        }

        /// <summary>
        /// Crea un nuevo rol en la base de datos.
        /// </summary>
        public bool CreateRol(Rol rol)
        {
            // 1. Verificar si el nombre del rol ya existe para evitar duplicados
            if (_context.Roles.Any(r => r.nombreRol == rol.nombreRol))
            {
                return false; // Indicamos que la creación falló por duplicado
            }

            // 2. Agregar a la colección y guardar cambios
            _context.Roles.Add(rol);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Actualiza un rol existente.
        /// </summary>
        public bool UpdateRol(Rol rol)
        {
            // 1. Verificar si el rol con ese ID existe
            var existingRol = GetRolById(rol.idRol);
            if (existingRol == null)
            {
                return false;
            }

            // 2. Verificar que el nuevo nombre no se duplique con otro rol
            if (_context.Roles.Any(r => r.nombreRol == rol.nombreRol && r.idRol != rol.idRol))
            {
                return false; // Nombre duplicado
            }

            // 3. Actualizar propiedades y guardar
            existingRol.nombreRol = rol.nombreRol;
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Elimina un rol existente.
        /// </summary>
        public bool DeleteRol(int id)
        {
            var rolToDelete = GetRolById(id);
            if (rolToDelete == null)
            {
                return false;
            }

            // LÓGICA DE NEGOCIO CRÍTICA (RF1): Verificar si hay usuarios asociados.
            // Asumiendo que tu modelo Usuario tiene una propiedad idRol:
            if (_context.Usuarios.Any(u => u.idRol == id)) // Asume que tienes DbSet<Usuario> Usuarios
            {
                // Un servicio debería idealmente lanzar una excepción en lugar de solo retornar false
                // para indicar la razón del fallo, pero mantenemos el 'bool' por consistencia.
                throw new InvalidOperationException("No se puede eliminar este rol porque tiene usuarios asociados.");
                // return false; 
            }
            // NOTA: Antes de eliminar, deberías verificar si hay usuarios asociados.
            // Si hay usuarios asociados, la eliminación fallará o deberías reasignar su rol.

            _context.Roles.Remove(rolToDelete);
            _context.SaveChanges();
            return true;
        }

        public Rol GetRolByName(string nombreRol)
        {
            // Busca el primer rol que coincida con el nombre, o retorna null si no existe.
            return _context.Roles.FirstOrDefault(r => r.nombreRol == nombreRol);
        }
    }
}