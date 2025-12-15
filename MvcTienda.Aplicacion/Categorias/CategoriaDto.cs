using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Categorias
{
    public class CategoriaDto
    {
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [Display(Name = "Nombre de la Categoría")]
        [StringLength(100)]
        public string NombreCategoria { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(500)]
        public string DescripcionCategoria { get; set; }
    }
}