using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Domain.Entities
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100)]
        public string nombreCategoria { get; set; }

        [StringLength(500)]
        public string descripcionCategoria { get; set; }

        // Propiedad de Navegación: Una categoría puede tener muchos productos
        public virtual ICollection<Producto> Productos { get; set; }
    }
}