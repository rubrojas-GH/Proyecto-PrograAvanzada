using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ProyectoPrograAvanzada.Models
{
    public class Rol
    {
        [Key]
        // Anotación que define la llave primaria. (idRol: int)
        public int idRol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del rol no debe exceder los 50 caracteres.")]
        // Restricción: El nombre debe ser obligatorio y tener una longitud máxima.
        public string nombreRol { get; set; }

        // Propiedad de navegación: Un Rol puede tener muchos Usuarios
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}

