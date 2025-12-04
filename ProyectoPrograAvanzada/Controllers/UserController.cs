using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security; // Necesario para FormsAuthentication

using ProyectoPrograAvanzada.Models;
using ProyectoPrograAvanzada.Services.Interfaces;
using ProyectoPrograAvanzada.Repositories;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.Repositories.Interfaces;

namespace ProyectoPrograAvanzada.Controllers
{
    // Por defecto, todas las acciones del controlador están protegidas,
    // salvo que se indique lo contrario con [AllowAnonymous].
    //[Authorize(Roles = "Administrador")]
    public class UserController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService; // Necesario para cargar roles en Edit

        public UserController()
        {
            // Inicialización Manual de las dependencias.
            IUsuarioRepository repository = new UsuarioRepository();
            _usuarioService = new UsuarioService(repository);

            // Suponemos que tienes un RolService accesible para listar roles
            _rolService = new RolService(new ApplicationDbContext());
        }

        //
        // RNF1: Acciones de Gestión (Solo Administrador)
        // ----------------------------------------------------

        // GET: /User
        // Muestra la lista de todos los usuarios
        public ActionResult Index()
        {
            var usuarios = _usuarioService.GetAllUsers();
            return View(usuarios);
        }

        // GET: /User/Details/5
        public ActionResult Details(int id)
        {
            var usuario = _usuarioService.GetUserById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: /User/Edit/5
        // Muestra el formulario para editar un usuario y cambiar su rol
        public ActionResult Edit(int id)
        {
            var usuario = _usuarioService.GetUserById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Cargar la lista de Roles para el DropDownList
            ViewBag.RolesList = _rolService.GetAllRoles().Select(r => new SelectListItem { Value = r.idRol.ToString(), Text = r.nombreRol, Selected = r.idRol == usuario.idRol });

            return View(usuario);
        }

        // POST: /User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _usuarioService.UpdateUser(usuario); // <--- Quitar el 'if' si el servicio es 'void'
                TempData["SuccessMessage"] = $"El usuario {usuario.nombre} fue actualizado exitosamente.";
                return RedirectToAction("Index");
                
            }

            // Volver a cargar la lista de roles si la validación falla
            ViewBag.RolesList = _rolService.GetAllRoles().Select(r => new SelectListItem { Value = r.idRol.ToString(), Text = r.nombreRol, Selected = r.idRol == usuario.idRol });
            return View(usuario);
        }

        // POST: /User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _usuarioService.DeleteUser(id);
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                // Idealmente, manejar excepciones específicas de la BD
                TempData["ErrorMessage"] = $"Error al eliminar el usuario: {ex.Message}";
            }
            return RedirectToAction("Index");
        }

        //
        // Seguridad Pública (Permitido a TODOS)
        // ----------------------------------------------------

        // GET: User/Register
        // Permite el acceso a usuarios no autenticados
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Usuario model)
        {
            if (ModelState.IsValid)
            {
                // CRÍTICO: Llama al servicio para crear el usuario (asignando Rol Asociado por defecto)
                if (_usuarioService.RegisterNewAsociado(model))
                {
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor, inicie sesión.";
                    return RedirectToAction("Login", "User");
                }
                else
                {
                    ModelState.AddModelError("email", "El correo electrónico ya se encuentra registrado.");
                }
            }
            return View(model);
        }

        // GET: User/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Producto");
            }
            return View("~/Views/User/Login.cshtml");
        }

        // POST: User/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            var user = _usuarioService.ValidateCredentials(email, password);

            if (user != null)
            {
                // 1. Crear el ticket de autenticación
                FormsAuthentication.SetAuthCookie(user.email, false);

                // 2. Redirección basada en Rol o URL
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Producto");
                }
            }

            ModelState.AddModelError("", "Correo electrónico o contraseña incorrectos.");
            return View();
        }

        // POST: User/LogOff
        // Usamos [HttpPost]
        // y [ValidateAntiForgeryToken] por seguridad, aunque [AllowAnonymous] no es necesario si el usuario ya está autenticado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}