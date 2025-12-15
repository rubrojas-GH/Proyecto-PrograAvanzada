using System.Collections.Generic;
using MvcTienda.Domain.Entities;

namespace MvcTienda.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        // CRUD
        Usuario GetUsuarioById(int id);
        Usuario GetUsuarioByEmail(string email);
        IEnumerable<Usuario> GetAllUsuarios();

        void AddUsuario(Usuario usuario);
        void UpdateUsuario(Usuario usuario);
        void DeleteUsuario(int id);

        // Seguridad
        Usuario Authenticate(string email, string passwordHash);

        // Roles
        Rol GetRolByName(string nombreRol);
        IEnumerable<Rol> GetAllRoles();

        string GetUserRoleNameByEmail(string email);
    }
}
