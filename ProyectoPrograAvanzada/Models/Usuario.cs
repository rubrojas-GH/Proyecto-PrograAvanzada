using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPrograAvanzada.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }

        // --- Propiedades de la entidad Usuario ---

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        // Se puede añadir [Index(IsUnique = true)] si se usa Entity Framework 6 con configuraciones adicionales
        public string email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 255 caracteres.")]
        // Nota: En una aplicación real, este campo debe almacenar el hash de la contraseña, no la contraseña en texto plano.
        public string contrasena { get; set; }

        // El estado es un booleano (true=Activo, false=Inactivo). Es requerido.
        [Required]
        public bool estado { get; set; }

        // Se usa para las métricas de sesión. No se requiere en el formulario, por lo que no es necesario [Required].
        public DateTime ultimaConexion { get; set; }

        // --- Configuración de la Relación con Rol (FK) ---

        // Llave foránea que debe ser requerida
        [Required(ErrorMessage = "El rol es obligatorio.")]
        [ForeignKey("Rol")]
        public int idRol { get; set; }

        // Propiedad de Navegación a la entidad Rol (el objeto Rol asociado)
        public virtual Rol Rol { get; set; }

        // Propiedad de Navegación a la colección de Órdenes (Historial de Compras)
        public virtual ICollection<Orden> HistorialOrdenes { get; set; } = new List<Orden>();

        // Propiedad de Navegación a la colección de Reseñas
        public virtual ICollection<Reseña> Reseñas { get; set; }
    }
}