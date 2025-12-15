using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MvcTienda.Aplicacion.Ordenes;
using MvcTienda.Aplicacion.Roles;
using MvcTienda.Aplicacion.Usuarios;
using MvcTienda.Web.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MvcTienda.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IOrdenService _ordenService;
        private readonly IRolService _rolService;

        // =========================
        // CONSTRUCTOR
        // =========================
        // Inyección de dependencias de ambos Servicios
        public UserController(IUsuarioService usuarioService, IOrdenService ordenService, IRolService rolService)
        {
            _usuarioService = usuarioService;
            _ordenService = ordenService;
            _rolService = rolService;
        }

        // =========================
        // REGISTRO DE USUARIOS
        // =========================

        // GET: /User/Register
        // Muestra el formulario de registro de nuevos usuarios (Asociados)
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        // Registra un nuevo usuario Asociado (RF1)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UsuarioCreateDto dto)
        {
            // Validación de datos del formulario
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            // Llamada al servicio de aplicación (método correcto)
            bool result = _usuarioService.RegisterNewAsociado(dto);

            if (!result)
            {
                // Email ya existe
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }


        // =========================
        // LOGIN / LOGOUT
        // =========================

        // GET: /User/Login
        // Muestra el formulario de inicio de sesión
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Creamos una instancia del ViewModel si el controller usa LoginViewModel
            return View(new LoginViewModel());
        }

        // POST: /User/Login
        // Autentica al usuario y crea la sesión (RF1, RNF2)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Validar Credenciales
            var usuario = _usuarioService.ValidateCredentials(model.Email, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(model);
            }

            // 2. Crear las Claims (Identidad del Usuario)
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, usuario.Email)); // El nombre de la identidad es el email
            claims.Add(new Claim(ClaimTypes.Role, usuario.Rol)); // ¡CRÍTICO para [Authorize(Roles="...")]

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // 3. Crear el Ticket de Autenticación de OWIN (Sign In)
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;

            // Crear la cookie con la opción "Remember Me" (IsPersistent)
            authenticationManager.SignIn(new AuthenticationProperties()
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? (DateTime?)DateTime.UtcNow.AddDays(7) : null // 7 días si RememberMe es true
            }, identity);

            // 4. Redirección
            return RedirectToLocal(returnUrl);
        }

        // Agregar función auxiliar faltante que sí estaba en AccountController.cs
        // o usar la de AccountController si decides borrar Login/Register de AccountController
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Producto");
        }



        // GET: /User/Logout
        // Cierra la sesión del usuario
        [Authorize]
        public ActionResult Logout()
        {
            // Usar el método de deslogeo de OWIN.
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Opcionalmente, puedes redirigir a Home en lugar de Login.
            return RedirectToAction("Index", "Home");
        }

        // =========================
        // HISTORIAL DE COMPRAS (RF 3.3)
        // =========================

        // GET: /User/History
        [Authorize(Roles = "Asociado")]
        public ActionResult History()
        {
            // 1. Obtener el Email del usuario autenticado
            string userEmail = User.Identity.Name;

            // 2. Usar IUsuarioService para obtener el ID del usuario
            // USANDO EL NOMBRE EXACTO DEL MÉTODO DE TU INTERFAZ
            var usuarioDto = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuarioDto == null)
            {
                // Usar OWIN SignOut
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                // Redirigimos al login con un mensaje claro
                TempData["ErrorMessage"] = "Error de sesión. Por favor, inicia sesión de nuevo.";
                return RedirectToAction(nameof(Login));
            }

            // 3. Obtener el ID del usuario
            // Asumo que tu UsuarioDto tiene una propiedad llamada 'Id'
            int idUsuario = usuarioDto.Id;

            // 4. Llamar al Servicio de Órdenes usando el ID
            var historialOrdenesDto = _ordenService.GetHistorialUsuario(idUsuario);

            // 5. Devolver la vista con los DTOs de órdenes.
            return View(historialOrdenesDto);
        }

        // =========================
        // ADMINISTRACIÓN DE USUARIOS
        // =========================

        // GET: /User
        // Muestra la lista de usuarios (solo Administrador)
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var usuarios = _usuarioService.GetAllUsers();
            return View(usuarios);
        }

        // GET: /User/Edit/5
        // Muestra el formulario para editar un usuario
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var usuario = _usuarioService.GetUserById(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            // <<-- LÓGICA DE ROLES  -->>
            // 1. Obtener la lista de todos los roles disponibles (RolDto)
            var roles = _rolService.GetAllRoles();

            // 2. Pasar la lista de roles a la vista usando ViewBag
            // La vista Edit.cshtml espera esta lista bajo el nombre "Roles".
            ViewBag.Roles = roles;

            return View(usuario);
        }

        // POST: /User/Edit/5
        // Actualiza la información de un usuario
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Si la validación falla, DEBEMOS recargar la lista de roles
                // para que el DropDownList no se caiga al volver a renderizar.
                ViewBag.Roles = _rolService.GetAllRoles();
                return View(dto);
            }

            // Manejo de la actualización (la lógica ya está en UsuarioService)
            _usuarioService.UpdateUser(dto);
            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /User/Delete/5
        // Desactiva o elimina un usuario (según reglas de negocio)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _usuarioService.DeleteUser(id);
            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
