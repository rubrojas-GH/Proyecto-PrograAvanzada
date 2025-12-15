// OrdenItemDto.cs (Ajustado)
using System.ComponentModel.DataAnnotations;

namespace MvcTienda.Aplicacion.Ordenes
{
    public class OrdenItemDto
    {
        // El Id de la orden a la que pertenece (útil si se pasa como modelo individual)
        public int IdOrden { get; set; }

        [Required]
        public int IdProducto { get; set; }

        // Propiedad de solo lectura para la vista de confirmación
        public string NombreProducto { get; set; }

        // Precio al que se compró (CRÍTICO para auditoría, el precio del producto puede cambiar luego)
        [Range(0, 999999)]
        public decimal PrecioUnitario { get; set; }

        [Range(1, 999)]
        public int Cantidad { get; set; }

        // Propiedad calculada
        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }
    }
}