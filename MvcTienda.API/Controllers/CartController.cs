using MvcTienda.Aplicacion.Productos;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MvcTienda.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/cart")]
    public class CartController : ApiController
    {
        private readonly IProductoService _productoService;
        private const string SessionCartKey = "ShoppingCart";

        public CartController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // POST api/cart/add
        [HttpPost]
        [Route("add")]
        public IHttpActionResult Add(AddToCartRequest request)
        {
            if (request == null || request.IdProducto <= 0 || request.Cantidad <= 0)
                return BadRequest("Datos inválidos.");

            var producto = _productoService.GetProductoConDetalles(request.IdProducto);
            if (producto == null)
                return NotFound();

            var cart = GetCart();
            int cantidadActual = cart.ContainsKey(request.IdProducto)
                ? cart[request.IdProducto]
                : 0;

            if (cantidadActual + request.Cantidad > producto.Stock)
            {
                return Ok(new
                {
                    success = false,
                    message = $"Stock insuficiente. Disponible: {producto.Stock}"
                });
            }

            if (cart.ContainsKey(request.IdProducto))
                cart[request.IdProducto] += request.Cantidad;
            else
                cart.Add(request.IdProducto, request.Cantidad);

            SaveCart(cart);

            return Ok(new
            {
                success = true,
                cartCount = cart.Sum(x => x.Value),
                message = "Producto agregado al carrito"
            });
        }

        // POST api/cart/update
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(UpdateCartRequest request)
        {
            if (request == null || request.Cantidad <= 0)
                return BadRequest("Datos inválidos.");

            var producto = _productoService.GetProductoConDetalles(request.IdProducto);
            if (producto == null)
                return NotFound();

            if (request.Cantidad > producto.Stock)
            {
                return Ok(new
                {
                    success = false,
                    message = $"Solo hay {producto.Stock} unidades disponibles.",
                    maxStock = producto.Stock
                });
            }

            var cart = GetCart();
            if (!cart.ContainsKey(request.IdProducto))
                return BadRequest("Producto no está en el carrito.");

            cart[request.IdProducto] = request.Cantidad;
            SaveCart(cart);

            decimal nuevoTotal = cart.Sum(i => {
                var p = _productoService.GetProductoConDetalles(i.Key);
                return p.Precio * i.Value;
            });

            return Ok(new
            {
                success = true,
                nuevoSubtotal = (producto.Precio * request.Cantidad).ToString("C"),
                nuevoTotal = nuevoTotal.ToString("C"),
                cartCount = cart.Sum(x => x.Value)
            });
        }

        public class UpdateCartRequest
        {
            public int IdProducto { get; set; }
            public int Cantidad { get; set; }
        }


        // =======================
        // Helpers de sesión
        // =======================
        private Dictionary<int, int> GetCart()
        {
            var session = HttpContext.Current.Session;
            return session[SessionCartKey] as Dictionary<int, int>
                   ?? new Dictionary<int, int>();
        }

        private void SaveCart(Dictionary<int, int> cart)
        {
            HttpContext.Current.Session[SessionCartKey] = cart;
        }
    }

    // DTO del request
    public class AddToCartRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
