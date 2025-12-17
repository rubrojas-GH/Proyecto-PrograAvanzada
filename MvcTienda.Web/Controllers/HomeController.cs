using System.Web.Mvc;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Categorias;
using System.Linq;

namespace MvcTienda.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        // Inyectamos ambos servicios en el constructor
        public HomeController(IProductoService productoService, ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        public ActionResult Index()
        {
            // 1. Cargamos las categorías (Este será el @model de la vista)
            var categorias = _categoriaService.GetAllCategorias();

            // 2. Cargamos los productos destacados (Se enviarán por ViewBag)
            var productosActivos = _productoService.GetCatalogoProductosActivos();

            // Tomamos solo los primeros 4 para que la sección "Ofertas" no sea infinita
            ViewBag.ProductosDestacados = productosActivos.Take(4).ToList();

            // 🚩 IMPORTANTE: Retornamos las categorías como el Modelo principal
            return View(categorias);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Página de información sobre nuestra tienda tipo bazar/librería.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Información de contacto y ubicación.";
            return View();
        }
    }
}