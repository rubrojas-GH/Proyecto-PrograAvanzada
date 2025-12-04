using ProyectoPrograAvanzada.Models; // Para acceder a las clases de modelo

using ProyectoPrograAvanzada.Repositories;

using ProyectoPrograAvanzada.Repositories.Interfaces;

using ProyectoPrograAvanzada.Services;

using ProyectoPrograAvanzada.Services.Interfaces;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Web;

using System.Web.Mvc;

using System.Web.Security;



namespace ProyectoPrograAvanzada.Controllers

{

    public class AccountController : Controller

    {

        //Declarar la dependencia de la interfaz de Servicio

        private readonly IUsuarioService _usuarioService;



        public AccountController()

        {

            // NOTA: Esto es una INICIALIZACIÓN MANUAL de las dependencias.

            // para inyectar estas dependencias de forma automática.

            IUsuarioRepository repository = new UsuarioRepository();

            _usuarioService = new UsuarioService(repository);

        }



        //

        // GET: /Account/Register

        // Muestra el formulario de registro de un nuevo usuario

        public ActionResult Register()

        {

            return View();

        }



        //

        // POST: /Account/Register

        // Procesa los datos del formulario de registro

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Register(Usuario model)

        {

            if (ModelState.IsValid)

            {

                // 1. Delegar la lógica de negocio (cifrado, asignación de rol, guardado) al Servicio.

                // Le pasamos el objeto 'Usuario' y la contraseña (que está en 'model.contraseña').

                try

                {

                    if (_usuarioService.RegisterUser(model, model.contrasena))

                    {

                        // Registro exitoso, redirigir al Login

                        return RedirectToAction("Login", "Account");

                    }

                    else

                    {

                        // El servicio retorna 'false' si el email ya existe

                        ModelState.AddModelError("email", "El correo electrónico ya está registrado.");

                    }

                }

                catch (InvalidOperationException ex)

                {

                    // Capturar errores del servicio (ej. Rol 'Asociado' no encontrado)

                    ModelState.AddModelError("", ex.Message);

                }

            }



            // Si el modelo no es válido o el registro falló, regresa a la vista

            return View(model);

        }



        //

        // GET: /Account/Login

        // Muestra el formulario de inicio de sesión

        public ActionResult Login()

        {

            return View();

        }



        //

        // POST: /Account/Login

        // Procesa los datos de inicio de sesión

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginViewModel model, string returnUrl)

        {

            if (ModelState.IsValid)

            {

                // 1. Delegar la autenticación al Servicio

                Usuario authenticatedUser = _usuarioService.LoginUser(model.Email, model.Password);



                if (authenticatedUser != null)

                {

                    // 2. Autenticación exitosa

                    // Crea el ticket de autenticación

                    FormsAuthentication.SetAuthCookie(authenticatedUser.email, model.RememberMe);



                    // Redirigir al usuario

                    return RedirectToLocal(returnUrl);

                }



                // Si la autenticación falla

                ModelState.AddModelError("", "Credenciales no válidas o usuario inactivo.");

            }



            return View(model);

        }



        //

        // POST: /Account/LogOff

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult LogOff()

        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");

        }



        // Función auxiliar (para el LoginViewModel)

        private ActionResult RedirectToLocal(string returnUrl)

        {

            if (Url.IsLocalUrl(returnUrl))

            {

                return Redirect(returnUrl);

            }

            return RedirectToAction("Index", "Home");

        }



        protected override void Dispose(bool disposing)

        {

            base.Dispose(disposing);

        }

    }

}