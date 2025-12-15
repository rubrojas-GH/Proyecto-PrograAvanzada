using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Usuarios
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        // Role name instead of Role ID for better readability
        [Required]
        public string Rol { get; set; }

        public int IdRol { get; set; }

        public bool Estado { get; set; }

        public DateTime UltimaConexion { get; set; }

        public string ContrasenaActual { get; set; }
    }

    public class UsuarioCreateDto
    {
        // ... Propiedad Nombre (que mapea a nombre en la entidad)
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        // ... Propiedad Email (que mapea a email en la entidad)
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        // === PROPIEDADES DE AUTENTICACIÓN QUE DEBEN EXISTIR ===

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 255 caracteres.")]
        public string Password { get; set; } // <<-- Debe existir para mapear a 'contrasena'

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        // **Este atributo es crucial para que la validación del lado del cliente funcione**
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class UsuarioLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
