using ProyectoPrograAvanzada.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Services.Interfaces
{
    public interface IUsuarioService
    {
        bool RegisterUser(Usuario model, string password);
        Usuario LoginUser(string email, string password);
        Usuario GetUserById(int id);
        Usuario GetUsuarioByEmail(string email);
        Usuario ValidateCredentials(string email, string password);
        bool RegisterNewAsociado(Usuario usuario);
        IEnumerable<Usuario> GetAllUsers();
        void UpdateUser(Usuario usuario);
        void DeleteUser(int id);
        IEnumerable<Rol> GetAllRoles(); // Necesario para la lista desplegable de roles
    }
}
