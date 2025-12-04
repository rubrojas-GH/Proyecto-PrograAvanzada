using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Services.Interfaces
{
    public interface IRolService
    {
        // Método para obtener todos los roles, usado principalmente para poblar DropDownLists
        IEnumerable<Rol> GetAllRoles();

        // Opcional: Si necesitas obtener un rol por su ID o nombre
        Rol GetRolById(int idRol);
        Rol GetRolByName(string nombreRol);
    }
}
