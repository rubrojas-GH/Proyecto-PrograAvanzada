using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.Repositories.Interfaces;

namespace ProyectoPrograAvanzada.Controllers
{
    public class ReseñaController : Controller
    {
        private readonly IReseñaService _reseñaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService; // Necesario para obtener el producto

        public ReseñaController()
        {
            // Inicialización manual de dependencias

            // 1. Repositorios
            IReseñaRepository reseñaRepository = new ReseñaRepository();
            IProductoRepository productoRepository = new ProductoRepository();
            IUsuarioRepository usuarioRepository = new UsuarioRepository();

            // 2. Servicios
            // El servicio de reseñas depende del Repositorio de Productos
            _reseñaService = new ReseñaService(reseñaRepository, productoRepository);
            _usuarioService = new UsuarioService(usuarioRepository);
            _productoService = new ProductoService(productoRepository);
        }

        // --- ACCIONES PARA ASOCIADOS (CREACIÓN DE RESEÑAS) ---
        // Solo usuarios logueados (Asociados) pueden escribir reseñas.
        [Authorize(Roles = "Asociado, Administrador")]
        //
        // GET: /Reseña/Create/5
        // id = idProducto
        public ActionResult Create(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            // Pasar el producto a la vista para que el usuario sepa lo que está reseñando
            ViewBag.Producto = producto;

            // Retorna un modelo de Reseña pre-llenado con el ID del producto
            return View(new Reseña { idProducto = id });
        }

        //
        // POST: /Reseña/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Asociado, Administrador")]
        public ActionResult Create(Reseña reseña)
        {
            // Se debe obtener el ID del usuario logueado
            string userEmail = User.Identity.Name;
            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuario == null)
            {
                // Debería ser capturado por [Authorize], pero es una doble verificación.
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    reseña.idUsuario = usuario.idUsuario;
                    _reseñaService.CreateReseña(reseña);

                    // Notificar al usuario que la reseña ha sido enviada para aprobación (RF4)
                    TempData["SuccessMessage"] = "Su reseña ha sido enviada y está pendiente de aprobación por el Administrador.";

                    return RedirectToAction("Details", "Producto", new { id = reseña.idProducto });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Recargar la información del producto si algo falla
            ViewBag.Producto = _productoService.GetProductoConDetalles(reseña.idProducto);
            return View(reseña);
        }

        // --- ACCIONES PARA ADMINISTRADORES (MODERACIÓN - RF4) ---
        [Authorize(Roles = "Administrador")]
        //
        // GET: /Reseña/Moderation
        // Muestra la lista de reseñas pendientes de aprobación
        public ActionResult Moderation()
        {
            var reseñasPendientes = _reseñaService.GetReseñasPendientes();
            return View(reseñasPendientes);
        }

        //
        // POST: /Reseña/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Approve(int id)
        {
            try
            {
                _reseñaService.AprobarReseña(id);
                TempData["SuccessMessage"] = "Reseña aprobada exitosamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Moderation");
        }

        //
        // POST: /Reseña/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Reject(int id)
        {
            try
            {
                _reseñaService.RechazarReseña(id);
                TempData["SuccessMessage"] = "Reseña rechazada y eliminada.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Moderation");
        }
    }
}