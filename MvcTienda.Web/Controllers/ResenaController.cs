using MvcTienda.Aplicacion.Productos;
using MvcTienda.Aplicacion.Resenas;
using MvcTienda.Aplicacion.Usuarios;
using System;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    public class ResenaController : Controller
    {
        private readonly IResenaService _resenaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService; // Necesario para obtener el producto

        public ResenaController(
            IResenaService resenaService,
            IUsuarioService usuarioService,
            IProductoService productoService)
        {
            _resenaService = resenaService;
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

        // =====================================================
        // ACCIONES PARA ASOCIADOS (CREACIÓN DE RESEÑAS)
        // =====================================================
        // Solo usuarios logueados (Asociados) pueden escribir reseñas.
        [Authorize(Roles = "Asociado")]
        //
        // GET: /Resena/Create/5
        // id = idProducto
        public ActionResult Create(int id)
        {
            var producto = _productoService.GetProductoConDetalles(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            // Pasar el producto a la vista para que el usuario sepa qué está reseñando
            ViewBag.Producto = producto;

            // Retorna un DTO pre-llenado con el ID del producto
            return View(new ResenaDto { IdProducto = id });
        }

        //
        // POST: /Resena/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Asociado")]
        public ActionResult Create(ResenaDto dto)
        {
            // Se obtiene el usuario autenticado a partir del email
            string userEmail = User.Identity.Name;
            var usuario = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuario == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // El ID del usuario autenticado se pasa explícitamente al servicio
                    // (El servicio NO conoce el contexto de autenticación)
                    _resenaService.CreateResena(dto, usuario.Id);

                    // Notificar al usuario que la reseña queda pendiente de aprobación (RF4)
                    TempData["SuccessMessage"] =
                        "Su reseña ha sido enviada y está pendiente de aprobación por el Administrador.";

                    return RedirectToAction("Details", "Producto", new { id = dto.IdProducto });
                }
                catch (InvalidOperationException ex)
                {
                    // Errores de lógica de negocio (producto inexistente, etc.)
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si ocurre un error, se recarga la información del producto
            ViewBag.Producto = _productoService.GetProductoConDetalles(dto.IdProducto);
            return View(dto);
        }

        // =====================================================
        // ACCIONES PARA ADMINISTRADORES (MODERACIÓN - RF4)
        // =====================================================
        [Authorize(Roles = "Administrador")]
        //
        // GET: /Resena/Moderation
        // Muestra la lista de reseñas pendientes de aprobación
        // NOTA: Esta acción solo carga la vista inicial. Las acciones de Aprobar/Rechazar
        // ahora se gestionan de forma asíncrona mediante JavaScript llamando a la Web API.
        public ActionResult Moderation()
        {
            var resenasPendientes = _resenaService.GetResenasPendientes();
            return View(resenasPendientes);
        }

        /* * MÉTODOS OBSOLETOS - REEMPLAZADOS POR WEB API (AJAX)
         * Se comentan para evitar colisiones y cumplir con el requisito de moderación asíncrona.
         * La lógica de estos métodos ahora reside en ResenasController dentro del proyecto API.
         */

        /*
        // POST: /Resena/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Approve(int id)
        {
            try
            {
                _resenaService.AprobarResena(id);
                TempData["SuccessMessage"] = "Reseña aprobada exitosamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Moderation");
        }
        */

        /*
        // POST: /Resena/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Reject(int id)
        {
            try
            {
                _resenaService.RechazarResena(id);
                TempData["SuccessMessage"] = "Reseña rechazada y eliminada.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Moderation");
        }
        */
    }
}