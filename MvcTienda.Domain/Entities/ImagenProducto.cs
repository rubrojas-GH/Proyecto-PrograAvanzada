using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTienda.Domain.Entities
{
    public class ImagenProducto
    {
        // Llave primaria (PK)
        [Key]
        public int idImagen { get; set; }

        // URL o path de la imagen
        [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
        [StringLength(500, ErrorMessage = "La URL no debe exceder los 500 caracteres.")]
        public string urlImagen { get; set; }

        // --- Configuración de la Relación con Producto (FK) ---

        // FK a Producto: A qué producto pertenece la imagen (Debe ser obligatoria)
        [Required(ErrorMessage = "El ID del producto asociado es obligatorio.")]
        [ForeignKey("Producto")]
        public int idProducto { get; set; }

        // --- Propiedad de Navegación ---

        // Objeto de navegación al Producto al que pertenece la imagen
        public virtual Producto Producto { get; set; }
    }
}