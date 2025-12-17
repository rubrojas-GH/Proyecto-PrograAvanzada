using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Usuarios;
using MvcTienda.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    [Authorize(Roles = "Asociado")]
    public class ShoppingCartController : Controller
    {
        private readonly IOrdenService _ordenService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private const string SessionCartKey = "ShoppingCart";
        private readonly System.Globalization.CultureInfo _cultureCR = new System.Globalization.CultureInfo("es-CR");

        public ShoppingCartController(
            IOrdenService ordenService,
            IUsuarioService usuarioService,
            IProductoService productoService)
        {
            _ordenService = ordenService;
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        // GET: /ShoppingCart
        public ActionResult Index()
        {
            var cartItems = GetCartFromSession();
            var viewModelList = new List<CarritoItemViewModel>();

            foreach (var item in cartItems)
            {
                var productoDto = _productoService.GetProductoConDetalles(item.Key);
                if (productoDto != null)
                {
                    viewModelList.Add(new CarritoItemViewModel
                    {
                        IdProducto = item.Key,
                        NombreProducto = productoDto.Nombre,
                        PrecioUnitario = productoDto.Precio,
                        StockDisponible = productoDto.Stock,
                        Cantidad = item.Value
                    });
                }
            }

            return View(viewModelList);
        }

        // POST: /ShoppingCart/AddItem
        [HttpPost]
        public ActionResult AddItem(int idProducto, int cantidad = 1)
        {
            // 1. Buscamos el producto para ver su stock real en DB
            var producto = _productoService.GetProductoConDetalles(idProducto);
            if (producto == null || cantidad <= 0)
            {
                if (Request.IsAjaxRequest()) return Json(new { success = false, message = "Producto no encontrado." });
                return HttpNotFound();
            }

            // 2. Obtenemos lo que ya hay en el carrito
            var cartItems = GetCartFromSession();
            int cantidadActualEnCarrito = cartItems.ContainsKey(idProducto) ? cartItems[idProducto] : 0;

            // 3. VALIDACIÓN CRÍTICA: ¿Supera el stock disponible?
            if (cantidadActualEnCarrito + cantidad > producto.Stock)
            {
                string msgError = $"No puedes agregar más. Stock disponible: {producto.Stock}. Ya tienes {cantidadActualEnCarrito} en el carrito.";

                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = msgError });

                TempData["ErrorMessage"] = msgError;
                return RedirectToAction("Index", "Producto");
            }

            // 4. Si pasa la validación, actualizamos la sesión
            if (cartItems.ContainsKey(idProducto))
                cartItems[idProducto] += cantidad;
            else
                cartItems.Add(idProducto, cantidad);

            Session[SessionCartKey] = cartItems;

            // 5. Calculamos el nuevo total para el badge
            int totalCount = cartItems.Sum(x => x.Value);

            // 6. Respuesta según el tipo de petición
            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    count = totalCount,
                    message = "¡Producto añadido correctamente!"
                });
            }

            TempData["SuccessMessage"] = "Producto añadido al carrito.";
            return RedirectToAction("Index", "Producto");
        }

        // NUEVO: POST: /ShoppingCart/UpdateQuantity
        // Ajuste para permitir modificar cantidades directamente desde el carrito con validación de stock
        [HttpPost]
        public ActionResult UpdateQuantity(int idProducto, int cantidad)
        {
            var producto = _productoService.GetProductoConDetalles(idProducto);

            // 1. Validaciones básicas de existencia y cantidad positiva
            if (producto == null || cantidad <= 0)
            {
                return Json(new { success = false, message = "Cantidad o producto no válido." });
            }

            // 2. Validación de Stock Real contra la nueva cantidad solicitada
            if (cantidad > producto.Stock)
            {
                return Json(new
                {
                    success = false,
                    message = $"Límite excedido. Solo hay {producto.Stock} unidades disponibles.",
                    maxStock = producto.Stock
                });
            }

            // 3. Actualizar la sesión
            var cartItems = GetCartFromSession();
            if (cartItems.ContainsKey(idProducto))
            {
                cartItems[idProducto] = cantidad;
                Session[SessionCartKey] = cartItems;
            }

            // 4. Recalcular totales para refrescar la vista sin recargar
            decimal nuevoSubtotal = producto.Precio * cantidad;
            decimal nuevoTotalGeneral = cartItems.Sum(item => {
                var p = _productoService.GetProductoConDetalles(item.Key);
                return p.Precio * item.Value;
            });

            return Json(new
            {
                success = true,
                nuevoSubtotal = nuevoSubtotal.ToString("C", _cultureCR),
                nuevoTotal = nuevoTotalGeneral.ToString("C", _cultureCR),
                cartCount = cartItems.Sum(x => x.Value),
                message = "Cantidad actualizada."
            });
        }

        // POST: /ShoppingCart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int idProducto)
        {
            var cartItems = GetCartFromSession();
            if (cartItems.ContainsKey(idProducto))
            {
                cartItems.Remove(idProducto);
                Session[SessionCartKey] = cartItems;
            }
            return RedirectToAction("Index");
        }

        // POST: /ShoppingCart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            var cartItems = GetCartFromSession();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío.";
                return RedirectToAction("Index");
            }

            var usuario = _usuarioService.GetUsuarioByEmail(User.Identity.Name);
            if (usuario == null) return RedirectToAction("Login", "User");

            try
            {
                // Procesar la compra
                OrdenDto nuevaOrden = _ordenService.ProcesarCompra(usuario.Id, cartItems);

                // Limpiar carrito
                Session[SessionCartKey] = null;

                return RedirectToAction("Confirmation", new { id = nuevaOrden.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Confirmation(int id)
        {
            var orden = _ordenService.GetOrdenById(id);
            var usuario = _usuarioService.GetUsuarioByEmail(User.Identity.Name);

            if (orden == null || (usuario != null && orden.IdUsuario != usuario.Id))
            {
                return HttpNotFound();
            }

            return View(orden);
        }

        // Auxiliar para no repetir código de sesión
        private Dictionary<int, int> GetCartFromSession()
        {
            return Session[SessionCartKey] as Dictionary<int, int> ?? new Dictionary<int, int>();
        }

        // Nuevo: Para que el _LoginPartial obtenga el conteo vía JS
        [ChildActionOnly]
        public int GetCartCount()
        {
            var cart = GetCartFromSession();
            return cart.Sum(x => x.Value);
        }
    }
}