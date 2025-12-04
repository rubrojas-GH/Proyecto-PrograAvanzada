using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoPrograAvanzada.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoPrograAvanzada.Repositories.Interfaces
{
    internal interface IRolRepository
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
    }
}
