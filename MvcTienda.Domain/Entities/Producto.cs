using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTienda.Domain.Entities
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }

        [Required]
        [StringLength(100)]
        public string nombreProducto { get; set; }

        [StringLength(500)]
        public string descripcion { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal precioProducto { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int stock { get; set; }

        [Required]
        public bool estadoProducto { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [ForeignKey("Categoria")] // Indica que idCategoria es la llave foránea
        public int idCategoria { get; set; }

        // Propiedad de Navegación: Un Producto pertenece a una Categoria
        public virtual Categoria Categoria { get; set; }

        public virtual ICollection<ImagenProducto> Imagenes { get; set; }
        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    }
}
