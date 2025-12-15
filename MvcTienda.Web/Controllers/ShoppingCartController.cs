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

        // El carrito se gestionará en la Sesión (Diccionario: idProducto, Cantidad)
        private const string SessionCartKey = "ShoppingCart";

        public ShoppingCartController(
            IOrdenService ordenService,
            IUsuarioService usuarioService,
            IProductoService productoService) // ¡Autofac inyecta todo!
        {
            _ordenService = ordenService;
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        // --- Métodos de Gestión del Carrito (Sesión) ---

        //
        // GET: /ShoppingCart
        // Muestra el contenido actual del carrito
        public ActionResult Index()
        {
            Dictionary<int, int> cartItems =
              Session[SessionCartKey] as Dictionary<int, int> ?? new Dictionary<int, int>();

            List<CarritoItemViewModel> viewModelList = new List<CarritoItemViewModel>();

            foreach (var item in cartItems)
            {
                int idProducto = item.Key; // idProducto del diccionario
                int cantidad = item.Value; // Cantidad del diccionario

                // Obtener detalles del producto (Nombre, Precio, Stock)
                var productoDto = _productoService.GetProductoConDetalles(idProducto);

                if (productoDto != null)
                {
                    viewModelList.Add(new CarritoItemViewModel
                    {
                        IdProducto = idProducto,
                        NombreProducto = productoDto.Nombre,
                        PrecioUnitario = productoDto.Precio,
                        StockDisponible = productoDto.Stock,
                        Cantidad = cantidad
                    });
                }
                // Si el producto no existe (ha sido eliminado), se omite
            }

            return View(viewModelList); // Devuelve la lista de ViewModels a la vista
        }

        //
        // POST: /ShoppingCart/AddItem
        // Agrega un producto al carrito
        [HttpPost]
        public ActionResult AddItem(int idProducto, int cantidad = 1) // idProducto en camelCase por Convención MVC
        {
            if (cantidad <= 0)
            {
                return RedirectToAction("Index", "Producto");
            }

            Dictionary<int, int> cartItems =
              Session[SessionCartKey] as Dictionary<int, int> ?? new Dictionary<int, int>();

            if (cartItems.ContainsKey(idProducto))
            {
                cartItems[idProducto] += cantidad;
            }
            else
            {
                cartItems.Add(idProducto, cantidad);
            }

            Session[SessionCartKey] = cartItems;

            return RedirectToAction("Index", "Producto");
        }

        //
        // POST: /ShoppingCart/RemoveItem
        // Elimina completamente un producto del carrito
        [HttpPost]
        public ActionResult RemoveItem(int idProducto)
        {
            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int>;

            if (cartItems != null && cartItems.ContainsKey(idProducto))
            {
                cartItems.Remove(idProducto);
                Session[SessionCartKey] = cartItems;
            }

            return RedirectToAction("Index");
        }

        // --- Método de Compra (RF3) ---

        //
        // POST: /ShoppingCart/Checkout
        // Procesa la compra y registra la orden
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int>;

            // ... (Validaciones de carrito vacío y usuario, no requieren ajuste)

            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "El carrito de compras está vacío.");
                // Necesitamos llamar a Index() para obtener el ViewModel, o replicar la lógica aquí
                // Para ser más limpios, redirigimos a Index GET si hay un error:
                TempData["ErrorMessage"] = "El carrito de compras está vacío.";
                return RedirectToAction("Index");
            }
            // Obtener el ID del usuario actual
            string userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("LogOff", "Account");
            }

            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuario == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            int idUsuario = usuario.Id;

            try
            {
                // Delegar el proceso transaccional al Servicio (RF3)
                OrdenDto nuevaOrden = _ordenService.ProcesarCompra(idUsuario, cartItems);

                // Limpiar el carrito de la sesión
                Session[SessionCartKey] = null;

                // Redirigir a la confirmación de la orden
                return RedirectToAction("Confirmation", new { id = nuevaOrden.Id });
            }
            catch (InvalidOperationException ex)
            {
                // Si ocurre un error de stock o lógica de negocio
                ModelState.AddModelError("", ex.Message);
                // Debe llamar a Index() para obtener el ViewModel correcto
                return Index(); // Llamar a Index GET para que la vista reciba el ViewModel correcto con el error
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al procesar la compra.");
                return Index(); // Llamar a Index GET
            }
        }

        //
        // GET: /ShoppingCart/Confirmation/5
        public ActionResult Confirmation(int id)
        {
            var orden = _ordenService.GetOrdenById(id);

            string userEmail = User.Identity.Name;
            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);
            int idUsuarioActual = usuario?.Id ?? 0;

            // Validar propiedad de la orden
            if (orden == null || orden.IdUsuario != idUsuarioActual) // Suponiendo que OrdenDto tiene IdUsuario
            {
                return HttpNotFound();
            }

            return View(orden);
        }
    }
}
