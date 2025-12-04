using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories;
using ProyectoPrograAvanzada.Services;
using System.Web.Mvc;
using ProyectoPrograAvanzada.Repositories.Interfaces;

namespace ProyectoPrograAvanzada.Controllers
{
    // RF5: Solo el rol de Administrador puede acceder al Dashboard
    [Authorize(Roles = "Administrador")]
    public class DashboardController : Controller
    {
        private readonly IOrdenService _ordenService;
        private readonly IProductoService _productoService;
        private readonly IReseñaService _reseñaService;
        private readonly IUsuarioService _usuarioService; // Necesario para métricas de usuarios

        public DashboardController()
        {
            // Inicialización de Repositorios
            IOrdenRepository ordenRepository = new OrdenRepository();
            IProductoRepository productoRepository = new ProductoRepository();
            IReseñaRepository reseñaRepository = new ReseñaRepository();
            IUsuarioRepository usuarioRepository = new UsuarioRepository();

            // Inicialización de Servicios (Inyección de dependencias manual)
            _ordenService = new OrdenService(ordenRepository, productoRepository);
            _productoService = new ProductoService(productoRepository);
            _reseñaService = new ReseñaService(reseñaRepository, productoRepository);
            _usuarioService = new UsuarioService(usuarioRepository);
        }

        //
        // GET: /Dashboard
        // Muestra el Panel Central de Administración con métricas (RF5)
        public ActionResult Index()
        {
            // --- 1. Gestión de Reseñas (RF4) ---
            var reseñasPendientes = _reseñaService.GetReseñasPendientes();
            ViewBag.ReseñasPendientesCount = reseñasPendientes.Count();
            ViewBag.ReseñasPendientes = reseñasPendientes.Take(5); // Mostrar las 5 más recientes en el dashboard

            // --- 2. Gestión de Stock (RF2, RF5) ---
            // Asumimos un umbral de 'bajo stock' de 5 unidades.
            var allProducts = _productoService.GetCatalogoProductosActivos();
            var lowStockProducts = allProducts.Where(p => p.stock <= 5).ToList();

            ViewBag.LowStockCount = lowStockProducts.Count();
            ViewBag.LowStockProducts = lowStockProducts.OrderBy(p => p.stock).Take(5); // Mostrar los 5 con menor stock

            // --- 3. Métricas de Ventas (RF3, RF5) ---
            // NOTA: Asume que IOrdenService implementa GetAllOrdenes() para obtener todas las órdenes.
            var allOrders = _ordenService.GetAllOrdenes();

            // Cálculo del total de ventas
            decimal totalVentas = allOrders.Sum(o => o.total);

            // Cálculo de ventas del último mes (ejemplo)
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            decimal ventasUltimoMes = allOrders.Where(o => o.fecha >= lastMonth).Sum(o => o.total);

            ViewBag.TotalVentas = totalVentas.ToString("C", new System.Globalization.CultureInfo("es-CR")); // Formato de moneda
            ViewBag.VentasUltimoMes = ventasUltimoMes.ToString("C", new System.Globalization.CultureInfo("es-CR"));
            ViewBag.RecentOrders = allOrders.OrderByDescending(o => o.fecha).Take(5);

            // --- 4. Conteo de Usuarios (RF1, RF5) ---
            var allUsers = _usuarioService.GetAllUsers();
            ViewBag.TotalUsuarios = allUsers.Count();
            // Asume que la entidad Usuario tiene la navegación al Rol y el nombre del Rol es 'Administrador' o 'Asociado'
            ViewBag.TotalAdministradores = allUsers.Count(u => u.Rol != null && u.Rol.nombreRol == "Administrador");
            ViewBag.TotalAsociados = allUsers.Count(u => u.Rol != null && u.Rol.nombreRol == "Asociado");

            return View();
        }

        // Aquí podrías agregar acciones para ver listas completas, como /Dashboard/OrdersList
        // GET: /Dashboard/OrdersList
        public ActionResult OrdersList()
        {
            var allOrders = _ordenService.GetAllOrdenes().OrderByDescending(o => o.fecha);
            return View(allOrders);
        }
    }
}