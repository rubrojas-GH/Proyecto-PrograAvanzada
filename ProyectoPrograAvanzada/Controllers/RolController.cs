using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services;
using System;
using System.Linq; // Necesario para Linq
using System.Web.Mvc;

namespace ProyectoPrograAvanzada.Controllers
{
    // Este controlador solo debería ser accesible por el Administrador (Rol ID = 1)
    [Authorize(Roles = "Administrador")]
    public class RolController : Controller
    {
        // Usaremos inyección de dependencia o inicializaremos el servicio
        private readonly RolService _rolService;

        // Constructor para inicializar el servicio (Asumimos el uso de ApplicationDbContext)
        public RolController()
        {
            // Nota: En una aplicación real, se usaría un contenedor IoC (Autofac, Unity)
            // para inyectar RolService. Aquí lo instanciamos directamente.
            _rolService = new RolService(new ApplicationDbContext());
        }

        //
        // GET: Rol/Index
        /// <summary>
        /// Muestra la lista de todos los roles.
        /// </summary>
        public ActionResult Index()
        {
            var roles = _rolService.GetAllRoles();
            return View(roles); // Pasamos la lista a la vista Index.cshtml
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
        public ActionResult Create(Rol rol)
        {
            if (ModelState.IsValid)
            {
                if (_rolService.CreateRol(rol))
                {
                    TempData["SuccessMessage"] = $"El rol '{rol.nombreRol}' ha sido creado exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("nombreRol", "Ya existe un rol con este nombre.");
                }
            }
            // Si hay errores de validación o duplicado, regresa a la vista con el modelo
            return View(rol);
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
        public ActionResult Edit(Rol rol)
        {
            if (ModelState.IsValid)
            {
                if (_rolService.UpdateRol(rol))
                {
                    TempData["SuccessMessage"] = $"El rol '{rol.nombreRol}' ha sido actualizado exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("nombreRol", "Ya existe otro rol con este nombre.");
                }
            }
            return View(rol);
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
                // Asumimos que RolService lanza InvalidOperationException si el rol está en uso
                if (_rolService.DeleteRol(id))
                {
                    TempData["SuccessMessage"] = "El rol ha sido eliminado exitosamente.";
                }
            }
            catch (InvalidOperationException ex)
            {
                // Maneja específicamente la excepción de lógica de negocio (rol en uso)
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                // Maneja errores inesperados de la base de datos (si el servicio no los maneja)
                TempData["ErrorMessage"] = "Ocurrió un error inesperado al eliminar el rol.";
            }

            return RedirectToAction("Index");
        }
    }
}