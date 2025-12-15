using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTienda.Domain.Entities
{
    public class Resena
    {
        [Key]
        public int idResena { get; set; }

        // Propiedades de la reseña
        // Anotaciones utilizadas tanto para validación básica
        // como para el mapeo con Entity Framework

        [Required(ErrorMessage = "El contenido de la reseña es obligatorio.")]
        [StringLength(500, ErrorMessage = "El contenido no debe exceder los 500 caracteres.")]
        public string contenido { get; set; }

        // --- CAMPO RECOMENDADO: CALIFICACIÓN ---
        [Required(ErrorMessage = "La calificación es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La calificación debe ser un valor entre 1 y 5.")]
        public int calificacion { get; set; } // Calificación de 1 a 5 estrellas
        // ----------------------------------------

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "El estado de la reseña es obligatorio.")]
        [StringLength(20)] // Valores esperados: Pendiente, Aprobada, Rechazada
        public string estado { get; set; }

        // --- Configuración de la Relación con Usuario y Producto (FKs) ---

        // FK a Usuario: quién escribió la reseña
        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [ForeignKey(nameof(Usuario))]
        public int idUsuario { get; set; }

        // FK a Producto: a qué producto aplica la reseña
        [Required(ErrorMessage = "El ID de producto es obligatorio.")]
        [ForeignKey(nameof(Producto))]
        public int idProducto { get; set; }

        // --- Propiedades de Navegación ---

        // Usuario que realizó la reseña
        public virtual Usuario Usuario { get; set; }

        // Producto reseñado
        public virtual Producto Producto { get; set; }
    }
}
