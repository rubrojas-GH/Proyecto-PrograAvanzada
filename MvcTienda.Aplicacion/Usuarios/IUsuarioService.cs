using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Usuarios
{
    public interface IUsuarioService
    {
        // Registro y autenticación
        bool RegisterNewAsociado(UsuarioCreateDto dto);
        UsuarioDto ValidateCredentials(string email, string password);

        // Gestión de usuarios
        UsuarioDto GetUserById(int id);
        UsuarioDto GetUsuarioByEmail(string email);
        IEnumerable<UsuarioDto> GetAllUsers();

        void UpdateUser(UsuarioDto dto);
        void DeleteUser(int id);

        // Roles (para dropdowns)
        IEnumerable<string> GetAllRoles();

        // Gestión de Perfil
        void UpdateNombre(string email, string nuevoNombre);
        void ChangePassword(string email, string oldPassword, string newPassword);
    }
}
