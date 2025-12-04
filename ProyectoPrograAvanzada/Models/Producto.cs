using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ProyectoPrograAvanzada.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }

        [Required]
        [StringLength(100)]
        public string nombreProducto { get; set; }

        // Campo faltante que debes agregar
        [StringLength(500)]
        public string descripcion { get; set; } 

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal precioProducto { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int stock { get; set; }

        [Required]
        public bool estadoProducto { get; set; } // true = Activo, false = Inactivo

        // Propiedades de navegación
        public virtual ICollection<ImagenProducto> Imagenes { get; set; }
        public virtual ICollection<Reseña> Reseñas { get; set; } = new List<Reseña>();
    }
}