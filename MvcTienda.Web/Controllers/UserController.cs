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
        public UserController(IUsuarioService usuarioService, IOrdenService ordenService, IRolService rolService)
        {
            _usuarioService = usuarioService;
            _ordenService = ordenService;
            _rolService = rolService;
        }

        // =========================
        // REGISTRO DE USUARIOS
        // =========================

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool result = _usuarioService.RegisterNewAsociado(dto);

            if (!result)
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                return View(dto);
            }

            TempData["SuccessMessage"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }


        // =========================
        // LOGIN / LOGOUT
        // =========================

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

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

            // ID interno del usuario
            claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

            // Usamos ClaimTypes.Email para coincidir con Global.asax
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email));

            // El nombre que mostrará User.Identity.Name
            claims.Add(new Claim(ClaimTypes.Name, usuario.Email));

            // Rol para los atributos [Authorize(Roles="...")]
            claims.Add(new Claim(ClaimTypes.Role, usuario.Rol));

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            // 3. Crear el Ticket de Autenticación de OWIN
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;

            authenticationManager.SignIn(new AuthenticationProperties()
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? (DateTime?)DateTime.UtcNow.AddDays(7) : null
            }, identity);

            // 4. Redirección
            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize] // Cambiado a Post en la vista, pero este GET funciona para accesos directos
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // Si tu vista usa un FormMethod.Post para Logout, añade este:
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogoutPost()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // =========================
        // HISTORIAL DE COMPRAS
        // =========================

        [Authorize(Roles = "Asociado")]
        public ActionResult History()
        {
            string userEmail = User.Identity.Name;
            var usuarioDto = _usuarioService.GetUsuarioByEmail(userEmail);

            if (usuarioDto == null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                TempData["ErrorMessage"] = "Error de sesión. Por favor, inicia sesión de nuevo.";
                return RedirectToAction(nameof(Login));
            }

            int idUsuario = usuarioDto.Id;
            var historialOrdenesDto = _ordenService.GetHistorialUsuario(idUsuario);

            return View(historialOrdenesDto);
        }

        // =========================
        // ADMINISTRACIÓN DE USUARIOS
        // =========================

        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var usuarios = _usuarioService.GetAllUsers();
            return View(usuarios);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var usuario = _usuarioService.GetUserById(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = _rolService.GetAllRoles();
            return View(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _rolService.GetAllRoles();
                return View(dto);
            }

            _usuarioService.UpdateUser(dto);
            TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

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