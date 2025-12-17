using MvcTienda.Aplicacion.Ordenes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Web.Models
{

    public class PerfilViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        public string Email { get; set; } // Solo lectura
        public string Rol { get; set; }   // Solo lectura

        // Seguridad
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; }

        public List<MvcTienda.Aplicacion.Ordenes.OrdenDto> MisOrdenes { get; set; } = new List<OrdenDto>();
    }
}