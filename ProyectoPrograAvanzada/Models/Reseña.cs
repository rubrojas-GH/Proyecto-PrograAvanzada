using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPrograAvanzada.Models
{
    public class Reseña
    {
        [Key]
        public int idReseña { get; set; }

        // Propiedades de la reseña

        [Required(ErrorMessage = "El contenido de la reseña es obligatorio.")]
        [StringLength(500, ErrorMessage = "El contenido no debe exceder los 500 caracteres.")]
        public string contenido { get; set; }

        // --- CAMPO RECOMENDADO: CALIFICACIÓN ---
        [Required(ErrorMessage = "La calificación es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La calificación debe ser un valor entre 1 y 5.")]
        public int calificacion { get; set; } // Asume una calificación de 1 a 5 estrellas
        // ----------------------------------------

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "El estado de la reseña es obligatorio.")]
        [StringLength(20)] // Para valores como 'Pendiente', 'Aprobada', 'Rechazada'
        public string estado { get; set; }

        // --- Configuración de la Relación con Usuario y Producto (FKs) ---

        // FK a Usuario: Quién escribió la reseña
        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [ForeignKey("Usuario")]
        public int idUsuario { get; set; }

        // FK a Producto: A qué producto aplica la reseña
        [Required(ErrorMessage = "El ID de producto es obligatorio.")]
        [ForeignKey("Producto")]
        public int idProducto { get; set; }

        // --- Propiedades de Navegación ---

        // Objeto de navegación al Usuario que hizo la reseña
        public virtual Usuario Usuario { get; set; }

        // Objeto de navegación al Producto reseñado
        public virtual Producto Producto { get; set; }
    }
}
