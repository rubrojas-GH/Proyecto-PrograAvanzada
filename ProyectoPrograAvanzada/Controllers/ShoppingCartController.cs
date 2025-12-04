using System;
using System.Web.Mvc;
using System.Collections.Generic;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.Repositories.Interfaces;
using ProyectoPrograAvanzada.Models;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace ProyectoPrograAvanzada.Controllers
{
    [Authorize(Roles = "Asociado")]
    public class ShoppingCartController : Controller
    {
        private readonly IOrdenService _ordenService;
        private readonly IUsuarioService _usuarioService; // Necesario para obtener el ID real del usuario

        // El carrito se gestionará en la Sesión (Diccionario: idProducto, Cantidad)
        private const string SessionCartKey = "ShoppingCart";

        public ShoppingCartController()
        {
            // Inicialización manual de dependencias
            IOrdenRepository ordenRepository = new OrdenRepository();
            IProductoRepository productoRepository = new ProductoRepository();
            _ordenService = new OrdenService(ordenRepository, productoRepository);

            IUsuarioRepository usuarioRepository = new UsuarioRepository();
            _usuarioService = new UsuarioService(usuarioRepository);
        }

        // --- Métodos de Gestión del Carrito (Sesión) ---

        //
        // GET: /ShoppingCart
        // Muestra el contenido actual del carrito
        public ActionResult Index()
        {
            // Lógica para obtener el carrito de la sesión
            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int> ?? new Dictionary<int, int>();

            // NOTA: Para la vista real, se tiene que usar el IProductoService.GetProductoById
            // para obtener el nombre, precio e imagen de cada producto en el diccionario.
            return View(cartItems);
        }

        //
        // POST: /ShoppingCart/AddItem
        // Agrega un producto al carrito
        [HttpPost]
        public ActionResult AddItem(int idProducto, int cantidad = 1) // Por defecto, agrega 1 unidad
        {
            if (cantidad <= 0)
            {
                return RedirectToAction("Index", "Producto");
            }

            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int> ?? new Dictionary<int, int>();

            if (cartItems.ContainsKey(idProducto))
            {
                cartItems[idProducto] += cantidad;
            }
            else
            {
                cartItems.Add(idProducto, cantidad);
            }

            Session[SessionCartKey] = cartItems;

            // Redirige al catálogo con un mensaje de éxito (ej: TempData["Success"] = "Producto añadido")
            return RedirectToAction("Index", "Producto");
        }

        //
        // POST: /ShoppingCart/RemoveItem
        // Elimina completamente un producto del carrito (o reduce la cantidad)
        [HttpPost]
        public ActionResult RemoveItem(int idProducto)
        {
            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int>;

            if (cartItems != null && cartItems.ContainsKey(idProducto))
            {
                // Remueve el producto completamente del diccionario del carrito
                cartItems.Remove(idProducto);
                Session[SessionCartKey] = cartItems;
            }

            return RedirectToAction("Index"); // Vuelve a la vista del carrito
        }

        // --- Método de Compra (RF3) ---

        //
        // POST: /ShoppingCart/Checkout
        // Procesa la compra y registra la orden
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout()
        {
            // 1. Obtener el carrito de la sesión
            Dictionary<int, int> cartItems = Session[SessionCartKey] as Dictionary<int, int>;

            // Verificar si el carrito está vacío
            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "El carrito de compras está vacío.");
                return View("Index");
            }

            // 2. Obtener el ID del usuario actual (LÓGICA CORREGIDA)
            string userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                // Si por alguna razón el usuario no tiene nombre (no debería pasar con [Authorize])
                return RedirectToAction("LogOff", "Account");
            }

            // Consultar el usuario real para obtener el idUsuario
            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuario == null)
            {
                // Fallo de seguridad/integridad: El usuario está autenticado pero no en la DB
                return RedirectToAction("LogOff", "Account");
            }

            int idUsuario = usuario.idUsuario; // ID REAL OBTENIDO

            try
            {
                // 3. Delegar el proceso transaccional al Servicio (RF3)
                Orden nuevaOrden = _ordenService.ProcesarCompra(idUsuario, cartItems);

                // 4. Limpiar el carrito de la sesión
                Session[SessionCartKey] = null;

                // 5. Redirigir a la confirmación de la orden
                return RedirectToAction("Confirmation", new { id = nuevaOrden.idOrden });
            }
            catch (InvalidOperationException ex)
            {
                // Capturar errores del servicio (ej. Stock insuficiente)
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
            catch (Exception ex)
            {
                // Error genérico (ej. Fallo de conexión a DB, etc.)
                // Loggear ex.Message en un sistema de registro de errores
                ModelState.AddModelError("", "Ocurrió un error inesperado al procesar la compra. Por favor, inténtelo de nuevo.");
                return View("Index");
            }
        }

        //
        // GET: /ShoppingCart/Confirmation/5
        public ActionResult Confirmation(int id)
        {
            var orden = _ordenService.GetOrdenById(id);

            // Obtener el ID del usuario actual de forma segura para validar la propiedad de la orden
            string userEmail = User.Identity.Name;
            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);
            int idUsuarioActual = usuario?.idUsuario ?? 0; // Si no existe, usamos 0

            // La orden debe existir Y el ID de la orden debe coincidir con el ID del usuario logueado
            if (orden == null || orden.idUsuario != idUsuarioActual)
            {
                return HttpNotFound();
            }
            return View(orden);
        }
    }
}