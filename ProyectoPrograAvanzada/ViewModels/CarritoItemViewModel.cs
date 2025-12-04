using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ProyectoPrograAvanzada.Models;

namespace ProyectoPrograAvanzada.ViewModels
{
    public class CarritoItemViewModel
    {
        // Datos de Producto (inmutables)
        public int idProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int StockDisponible { get; set; }

        // Datos del Carrito (mutables)

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        // Propiedad calculada
        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }
    }
}