using MvcTienda.Domain.Entities;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MvcTienda.Infraestructura.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        // Inyección de Dependencias: El DbContext se pasa al constructor
        // Nota: Por ahora usamos 'new', pero luego lo inyectaremos correctamente.
        public UsuarioRepository(ApplicationDbContext context)
        {
            _db = context;
        }

        // --- Métodos de Acceso a Datos ---

        public Usuario GetUsuarioById(int id)
        {
            // Incluye la navegación al Rol para obtener el nombre del rol
            return _db.Usuarios
                      .Include(u => u.Rol)
                      .FirstOrDefault(u => u.idUsuario == id);
        }

        // Obtiene todos los usuarios con su rol asociado
        public IEnumerable<Usuario> GetAllUsuarios()
        {
            return _db.Usuarios
                      .Include(u => u.Rol)
                      .ToList();
        }

        public void AddUsuario(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            _db.SaveChanges();
        }

        public void UpdateUsuario(Usuario usuario)
        {
            // Verificamos si la entidad ya está siendo rastreada por el contexto
            var trackedEntity = _db.Usuarios.Local.FirstOrDefault(u => u.idUsuario == usuario.idUsuario);

            if (trackedEntity != null)
            {
                // Si ya está rastreada, Entity Framework ya conoce los cambios realizados
                // en el objeto 'usuario' (porque es la misma referencia). 
                // Solo nos aseguramos de que el estado sea Modified.
                _db.Entry(trackedEntity).State = EntityState.Modified;
            }
            else
            {
                // Si no está rastreada (por ejemplo, viene de otra sesión o contexto), 
                // la adjuntamos y la marcamos como modificada.
                _db.Usuarios.Attach(usuario);
                _db.Entry(usuario).State = EntityState.Modified;
            }

            _db.SaveChanges();
        }

        public void DeleteUsuario(int id)
        {
            var usuario = _db.Usuarios.Find(id);
            if (usuario != null)
            {
                _db.Usuarios.Remove(usuario);
                _db.SaveChanges();
            }
        }

        // --- Métodos Específicos para Autenticación (RF1) ---

        public Usuario GetUsuarioByEmail(string email)
        {
            return _db.Usuarios
                      .Include(u => u.Rol)
                      .FirstOrDefault(u => u.email == email);
        }

        public Usuario Authenticate(string email, string passwordHash)
        {
            // Busca al usuario por email y contraseña (ya cifrada/hasheada)
            return _db.Usuarios.Include(u => u.Rol)
                               .FirstOrDefault(u => u.email == email &&
                                                    u.contrasena == passwordHash &&
                                                    u.estado == true); // Verifica que esté activo
        }

        public Rol GetRolByName(string nombreRol)
        {
            return _db.Roles.FirstOrDefault(r => r.nombreRol == nombreRol);
        }

        public IEnumerable<Rol> GetAllRoles()
        {
            return _db.Roles.ToList();
        }

        public string GetUserRoleNameByEmail(string email)
        {
            // Usamos la misma lógica que otros métodos de búsqueda:
            var usuario = _db.Usuarios
                             .Include(u => u.Rol) // Incluir el Rol para acceder a 'nombreRol'
                             .FirstOrDefault(u => u.email == email);

            // Si el usuario existe y tiene un rol asignado, devolvemos el nombre del rol.
            return usuario?.Rol?.nombreRol;
        }
    }
}