using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcTienda.Domain.Entities
{
    public class Orden
    {
        // Llave primaria (PK)
        [Key]
        public int idOrden { get; set; }

        // Propiedades de la orden

        [Required(ErrorMessage = "La fecha de la orden es obligatoria.")]
        public DateTime fecha { get; set; }

        // Total (se calcula sumando los subtotales de los detalles)
        [Required(ErrorMessage = "El total de la orden es obligatorio.")]
        [Range(0.00, double.MaxValue, ErrorMessage = "El total debe ser un valor no negativo.")]
        public decimal total { get; set; }

        // --- Configuración de la Relación con Usuario (FK) ---

        // FK a Usuario: Quién realizó la orden
        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        [ForeignKey("Usuario")]
        public int idUsuario { get; set; }

        // --- Propiedades de Navegación ---

        // Objeto de navegación al Usuario que hizo la orden
        public virtual Usuario Usuario { get; set; }

        // Colección de navegación a los detalles de la orden (el listado de productos comprados)
        public virtual ICollection<DetalleOrden> DetallesOrden { get; set; } = new List<DetalleOrden>();
    }
}