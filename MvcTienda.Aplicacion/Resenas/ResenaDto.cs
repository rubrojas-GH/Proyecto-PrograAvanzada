using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Resenas
{
    public class ResenaDto
    {
        // Identificadores
        public int Id { get; set; }

        [Required]
        public int IdProducto { get; set; }

        // Propiedades de la Reseña
        [Required, StringLength(500)]
        [Display(Name = "Comentario")]
        public string Comentario { get; set; }

        [Range(1, 5)]
        [Display(Name = "Calificación")]
        public int Calificacion { get; set; }

        public string Estado { get; set; } // Ejemplo: "Pendiente", "Aprobada", "Rechazada"

        public DateTime Fecha { get; set; }

        // Propiedades de Navegación/Visualización (para facilitar las vistas)

        [Display(Name = "Autor")]
        public string NombreUsuario { get; set; } // Nombre del Asociado (ej. para Moderation.cshtml)

        // Añadir el nombre del producto para Moderation.cshtml
        [Display(Name = "Producto")]
        public string NombreProducto { get; set; } // Nombre del Producto asociado (ej. para Moderation.cshtml)
    }
}