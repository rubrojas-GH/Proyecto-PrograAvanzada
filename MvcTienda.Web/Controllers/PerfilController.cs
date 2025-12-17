using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Usuarios;
using MvcTienda.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IOrdenService _ordenService;

        // Inyección de dependencias mediante el constructor
        public PerfilController(IUsuarioService usuarioService, IOrdenService ordenService)
        {
            _usuarioService = usuarioService;
            _ordenService = ordenService;
        }

        // GET: /Perfil
        public ActionResult Index()
        {
            // 1. Obtener datos del usuario logueado
            var usuarioDto = _usuarioService.GetUsuarioByEmail(User.Identity.Name);
            if (usuarioDto == null) return HttpNotFound();

            // 2. Mapear datos básicos al ViewModel
            var model = new PerfilViewModel
            {
                Nombre = usuarioDto.Nombre,
                Email = usuarioDto.Email,
                Rol = usuarioDto.Rol ?? "Asociado",
                MisOrdenes = new List<OrdenDto>()
            };

            // 3. Cargar historial de compras si es Asociado usando el nombre correcto: GetHistorialUsuario
            if (User.IsInRole("Asociado"))
            {
                try
                {
                    var historial = _ordenService.GetHistorialUsuario(usuarioDto.Id);
                    // Ordenamos por fecha descendente para que la más reciente aparezca primero
                    model.MisOrdenes = historial.OrderByDescending(o => o.Fecha).ToList();
                }
                catch (Exception ex)
                {
                    // Loguear error si fuera necesario y notificar a la vista
                    ViewBag.OrderError = "No se pudo cargar el historial de órdenes.";
                }
            }

            return View(model);
        }

        // POST: /Perfil/ActualizarNombre
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarNombre(PerfilViewModel model)
        {
            if (string.IsNullOrEmpty(model.Nombre))
            {
                TempData["ErrorMessage"] = "El nombre no puede estar vacío.";
                return RedirectToAction("Index");
            }

            try
            {
                _usuarioService.UpdateNombre(User.Identity.Name, model.Nombre);
                TempData["SuccessMessage"] = "Tu nombre ha sido actualizado con éxito.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /Perfil/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPassword(PerfilViewModel model)
        {
            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                TempData["ErrorMessage"] = "Debes completar todos los campos de contraseña.";
                return RedirectToAction("Index");
            }

            // Aquí se asume que la validación de igualdad (New vs Confirm) se hace en el modelo o cliente
            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Las nuevas contraseñas no coinciden.";
                return RedirectToAction("Index");
            }

            try
            {
                _usuarioService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                TempData["SuccessMessage"] = "Contraseña actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}