using System.Collections.Generic;
using MvcTienda.Domain.Entities;

namespace MvcTienda.Domain.Repositories
{
    public interface IRolRepository
    {
        // Retorna todos los roles disponibles en la base de datos
        IEnumerable<Rol> GetAllRoles();

        // Retorna un rol por su identificador
        Rol GetRolById(int idRol);

        // Retorna un rol por su nombre (ej: "Administrador", "Asociado")
        Rol GetRolByName(string nombreRol);

        void AddRol(Rol rol);
        void UpdateRol(Rol rol);
        void DeleteRol(int idRol);

        // Verifica si un nombre de rol ya existe, excluyendo opcionalmente un ID específico
        bool ExistsRolName(string nombreRol, int? excludeId = null);
        bool HasUsuarios(int idRol);
    }
}
