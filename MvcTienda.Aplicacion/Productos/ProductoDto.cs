using MvcTienda.Aplicacion.Resenas;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MvcTienda.Aplicacion.Productos
{
    public class ProductoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio."), StringLength(100)]
        [Display(Name = "Nombre del Producto")] // Para usar en las vistas
        public string Nombre { get; set; }

        // Añadir Descripción y Estado para Details y Edit
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required]
        [Display(Name = "Activo/Inactivo")]
        public bool EstadoProducto { get; set; } // Necesario para la administración

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; } // FK que se usará para Guardar/Editar

        public string NombreCategoria { get; set; }

        // Propiedades de navegación para la vista de Detalles (Details)
        public ICollection<ImagenProductoDto> Imagenes { get; set; } = new List<ImagenProductoDto>();
        // usar ResenaDto
        public ICollection<ResenaDto> Resenas { get; set; } = new List<ResenaDto>();
    }

    // DTO para manejar la información básica de una imagen en la aplicación
    public class ImagenProductoDto
    {
        public int Id { get; set; }
        public string UrlImagen { get; set; }
        // se puede añadir la FK del producto si es necesario para ciertas operaciones.
    }
}
