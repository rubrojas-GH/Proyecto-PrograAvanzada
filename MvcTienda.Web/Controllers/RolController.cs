using System;
using System.Web.Mvc;
using MvcTienda.Aplicacion.Roles;

namespace MvcTienda.Web.Controllers
{
    // Este controlador solo debería ser accesible por el Administrador
    [Authorize(Roles = "Administrador")]
    public class RolController : Controller
    {
        // Servicio de aplicación para la gestión de roles
        private readonly IRolService _rolService;

        // Constructor con inyección de dependencias
        // En una aplicación real, este servicio se inyecta desde el contenedor IoC
        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        //
        // GET: Rol/Index
        /// <summary>
        /// Muestra la lista de todos los roles.
        /// </summary>
        public ActionResult Index()
        {
            var roles = _rolService.GetAllRoles();
            return View(roles); // Se envían DTOs a la vista
        }

        //
        // GET: Rol/Create
        /// <summary>
        /// Muestra el formulario para crear un nuevo rol.
        /// </summary>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: Rol/Create
        /// <summary>
        /// Procesa la creación del nuevo rol.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RolDto dto)
        {
            // Validación de datos del formulario
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                _rolService.CreateRol(dto);
                TempData["SuccessMessage"] = $"El rol '{dto.Nombre}' ha sido creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: nombre duplicado)
                ModelState.AddModelError("Nombre", ex.Message);
                return View(dto);
            }
        }

        //
        // GET: Rol/Edit/5
        /// <summary>
        /// Muestra el formulario para editar un rol existente.
        /// </summary>
        public ActionResult Edit(int id)
        {
            var rol = _rolService.GetRolById(id);
            if (rol == null)
            {
                return HttpNotFound();
            }

            return View(rol);
        }

        //
        // POST: Rol/Edit/5
        /// <summary>
        /// Procesa la actualización del rol.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RolDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                _rolService.UpdateRol(dto);
                TempData["SuccessMessage"] = $"El rol '{dto.Nombre}' ha sido actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: nombre duplicado o rol inexistente)
                ModelState.AddModelError("Nombre", ex.Message);
                return View(dto);
            }
        }

        //
        // POST: Rol/Delete/5
        /// <summary>
        /// Elimina un rol por su ID.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                // El servicio lanza excepción si el rol tiene usuarios asociados
                _rolService.DeleteRol(id);
                TempData["SuccessMessage"] = "El rol ha sido eliminado exitosamente.";
            }
            catch (InvalidOperationException ex)
            {
                // Maneja específicamente la excepción de lógica de negocio (rol en uso)
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                // Maneja errores inesperados
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al eliminar el rol.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
