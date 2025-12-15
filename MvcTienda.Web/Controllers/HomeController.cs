using System.Web.Mvc;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;
using MvcTienda.Infraestructura.Repositories;
// using System.Linq; // Necesario para Linq en la acción Index (si se usa)

namespace MvcTienda.Web.Controllers
{
    public class HomeController : Controller
    {
        // Añadir el servicio necesario para la lógica de negocio
        private readonly IProductoService _productoService;

        public HomeController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        //
        // GET: /Home/Index
        // Esta será la página principal. Debe mostrar el catálogo de productos activos (RF 3.3, 1)
        public ActionResult Index()
        {
            // Requisito: Los usuarios no registrados solo pueden navegar por el catálogo de productos.
            // Por lo tanto, el Index debe cargar y mostrar los productos activos.
            var productos = _productoService.GetCatalogoProductosActivos();

            // Pasamos la lista de productos (DTOs) a la vista.
            return View(productos);
        }

        //
        // GET: /Home/About
        // Usada para mostrar la descripción/introducción de la tienda (bazar/librería) (RF 1)
        public ActionResult About()
        {
            ViewBag.Message = "Página de información sobre nuestra tienda tipo bazar/librería y los servicios exclusivos para asociados.";

            return View();
        }

        //
        // GET: /Home/Contact
        // Página de contacto general de la tienda
        public ActionResult Contact()
        {
            ViewBag.Message = "Información de contacto y ubicación.";

            return View();
        }

        // Opcional: Si quieres una página de errores personalizada
        public ActionResult Error()
        {
            return View();
        }
    }
}