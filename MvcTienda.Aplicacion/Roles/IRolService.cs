using System.Collections.Generic;

namespace MvcTienda.Aplicacion.Roles
{
    public interface IRolService
    {
        IEnumerable<RolDto> GetAllRoles();
        RolDto GetRolById(int idRol);
        RolDto GetRolByName(string nombreRol);

        void CreateRol(RolDto dto);
        void UpdateRol(RolDto dto);
        void DeleteRol(int idRol);
    }
}
