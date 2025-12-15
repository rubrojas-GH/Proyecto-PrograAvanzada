using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Resenas;
using MvcTienda.Aplicacion.Usuarios;
using MvcTienda.Domain.Repositories;
using MvcTienda.Infraestructura.Data;
using MvcTienda.Infraestructura.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    // RF5: Solo el rol de Administrador puede acceder al Dashboard
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly IOrdenService _ordenService;
        private readonly IProductoService _productoService;
        private readonly IResenaService _resenaService;
        private readonly IUsuarioService _usuarioService; // Necesario para métricas de usuarios

        public DashboardController(
            IOrdenService ordenService,
            IProductoService productoService,
            IResenaService resenaService,
            IUsuarioService usuarioService)
        {
            _ordenService = ordenService;
            _productoService = productoService;
            _resenaService = resenaService;
            _usuarioService = usuarioService;
        }

        //
        // GET: /Dashboard
        // Muestra el Panel Central de Administración con métricas (RF5)
        public ActionResult Index()
        {
            // --- 1. Gestión de Resenas (RF4) ---
            var resenasPendientes = _resenaService.GetResenasPendientes();
            ViewBag.ResenasPendientesCount = resenasPendientes.Count();
            ViewBag.ResenasPendientes = resenasPendientes.Take(5); // Mostrar las 5 más recientes en el dashboard

            // --- 2. Gestión de Stock (RF2, RF5) ---
            // Productos con bajo nivel de inventario.
            // Asumimos un umbral de 'bajo stock' de 5 unidades.
            var allProducts = _productoService.GetCatalogoProductosActivos();
            var lowStockProducts = allProducts.Where(p => p.Stock <= 5).ToList();

            ViewBag.LowStockCount = lowStockProducts.Count();
            ViewBag.LowStockProducts = lowStockProducts
                .OrderBy(p => p.Stock)
                .Take(5);

            // --- 3. Métricas de Ventas (RF3, RF5) ---
            // NOTA: Asume que IOrdenService implementa GetAllOrdenes() para obtener todas las órdenes.
            var allOrders = _ordenService.GetAllOrdenes();

            // Cálculo del total de ventas
            decimal totalVentas = allOrders.Sum(o => o.Total);

            // Cálculo de ventas del último mes (ejemplo)
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            decimal ventasUltimoMes = allOrders.Where(o => o.Fecha >= lastMonth).Sum(o => o.Total);

            ViewBag.TotalVentas = totalVentas.ToString("C", new System.Globalization.CultureInfo("es-CR")); // Formato de moneda
            ViewBag.VentasUltimoMes = ventasUltimoMes.ToString("C", new System.Globalization.CultureInfo("es-CR"));
            ViewBag.RecentOrders = allOrders.OrderByDescending(o => o.Fecha).Take(5);

            // --- 4. Conteo de Usuarios (RF1, RF5) ---
            var allUsers = _usuarioService.GetAllUsers(); // allUsers es IEnumerable<UsuarioDto>
            ViewBag.TotalUsuarios = allUsers.Count(); // Total de usuarios registrados (activos e inactivos).

            // CORRECCIÓN: Usar la propiedad string 'Rol' del DTO.
            ViewBag.TotalAdministradores = allUsers.Count(u => u.Rol == "Administrador");
            ViewBag.TotalAsociados = allUsers.Count(u => u.Rol == "Asociado");

            return View();
        }

        // Aquí podrías agregar acciones para ver listas completas, como /Dashboard/OrdersList
        // GET: /Dashboard/OrdersList
        public ActionResult OrdersList()
        {
            var allOrders = _ordenService.GetAllOrdenes().OrderByDescending(o => o.Fecha);
            return View(allOrders);
        }
    }
}