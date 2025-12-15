using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcTienda.Domain.Entities
{
    public class DetalleOrden
    {
        [Key]
        public int idDetalleOrden { get; set; }

        // Propiedades

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser un valor positivo.")]
        public decimal precioUnitario { get; set; }

        // Subtotal del detalle (cantidad * precioUnitario)
        // Se calcula en la capa de Aplicación antes de persistir
        [Required(ErrorMessage = "El subtotal es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser un valor positivo.")]
        public decimal subtotal { get; set; }

        // --- Configuración de las Relaciones (PK/FK) ---

        // FK a Orden: A qué orden pertenece este detalle
        [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
        [ForeignKey("Orden")]
        public int idOrden { get; set; }

        // FK a Producto: Qué producto se compró
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [ForeignKey("Producto")]
        public int idProducto { get; set; }

        // --- Propiedades de Navegación ---

        public virtual Orden Orden { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
