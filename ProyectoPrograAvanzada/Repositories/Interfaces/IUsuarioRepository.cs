using ProyectoPrograAvanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        // Métodos CRUD básicos
        Usuario GetUsuarioById(int id);
        IEnumerable<Usuario> GetAllUsuarios();
        void AddUsuario(Usuario usuario);
        void UpdateUsuario(Usuario usuario);
        void DeleteUsuario(int id);

        // Métodos específicos para RF1 y RNF2 (Seguridad)
        Usuario GetUsuarioByEmail(string email);
        Usuario Authenticate(string email, string passwordHash); // Para iniciar sesión
        Rol GetRolByName(string nombreRol); // Para asignar roles

        // Métodos para la Administración de Usuarios (RF1)
        IEnumerable<Rol> GetAllRoles();
    }
}
