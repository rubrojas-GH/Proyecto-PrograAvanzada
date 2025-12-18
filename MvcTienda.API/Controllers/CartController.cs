using System.Web.Http;
using System.Web.Http.Cors;
using MvcTienda.Aplicacion.Productos;

namespace MvcTienda.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/carrito")]
    public class CarritoController : ApiController
    {
        private readonly IProductoService _productoService;

        public CarritoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost]
        [Route("validar-item")]
        public IHttpActionResult ValidarItem([FromBody] CarritoRequest request)
        {
            if (request == null || request.IdProducto <= 0 || request.Cantidad <= 0)
                return BadRequest("Datos del producto no válidos.");

            var producto = _productoService.GetProductoConDetalles(request.IdProducto);

            if (producto == null)
                return NotFound();

            // Sumamos lo que ya hay en el carrito (si el JS lo envía) + lo nuevo
            int totalProyectado = request.Cantidad + request.CantidadPrevia;

            if (totalProyectado > producto.Stock)
            {
                return Ok(new
                {
                    success = false,
                    message = $"Stock insuficiente. Disponibles: {producto.Stock}. " +
                              (request.CantidadPrevia > 0 ? $"(Ya tienes {request.CantidadPrevia} en el carrito)" : "")
                });
            }

            return Ok(new
            {
                success = true,
                message = "Validación exitosa.",
                nombre = producto.Nombre,
                precio = producto.Precio
            });
        }
    }

    public class CarritoRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public int CantidadPrevia { get; set; }
    }
}