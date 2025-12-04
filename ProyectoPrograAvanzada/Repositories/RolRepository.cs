using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProyectoPrograAvanzada.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly ApplicationDbContext _db;

        // Constructor, asume que inyectas o creas la instancia del contexto de la DB
        public RolRepository()
        {
            // Nota: Se recomienda inyectar el DBContext si usas Entity Framework
            _db = new ApplicationDbContext();
        }

        public IEnumerable<Rol> GetAllRoles()
        {
            // Lógica simple: Obtener todos los roles de la tabla Roles
            return _db.Roles.ToList();
        }

        public Rol GetRolById(int idRol)
        {
            return _db.Roles.Find(idRol);
        }

        public Rol GetRolByName(string nombreRol)
        {
            // Busca el rol por su nombre, ignorando mayúsculas/minúsculas si es necesario
            return _db.Roles.FirstOrDefault(r => r.nombreRol == nombreRol);
        }

        // --- Escritura (Implementación CRUD) ---

        public void AddRol(Rol rol)
        {
            _db.Roles.Add(rol);
            _db.SaveChanges(); // Persiste el cambio a la base de datos
        }

        public void UpdateRol(Rol rol)
        {
            // Marca la entidad como modificada para que EF la actualice.
            _db.Entry(rol).State = EntityState.Modified;
            _db.SaveChanges();
        }

        public void DeleteRol(int idRol)
        {
            var rol = _db.Roles.Find(idRol);
            if (rol != null)
            {
                _db.Roles.Remove(rol);
                _db.SaveChanges();
            }
            // Si el rol no existe, simplemente no pasa nada o se podria lanzar una excepción.
        }
    }
}