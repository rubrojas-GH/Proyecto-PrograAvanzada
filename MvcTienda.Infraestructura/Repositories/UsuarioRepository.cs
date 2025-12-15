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
            _db.Entry(usuario).State = EntityState.Modified;
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

        public Producto GetProductoById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Producto> GetAllProductos()
        {
            throw new System.NotImplementedException();
        }

        public void AddProducto(Producto producto)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateProducto(Producto producto)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteProducto(int id)
        {
            throw new System.NotImplementedException();
        }

        public void AddImagen(ImagenProducto imagen)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteImagen(int idImagen)
        {
            throw new System.NotImplementedException();
        }

        public Usuario GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Usuario GetByEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Usuario> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public void Add(Usuario usuario)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Usuario usuario)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}