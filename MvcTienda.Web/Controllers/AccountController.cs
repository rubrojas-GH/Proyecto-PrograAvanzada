using MvcTienda.Aplicacion.Usuarios; // Necesario solo si el controlador tuviera más lógica de gestión de cuenta
using System.Web.Mvc;
using Microsoft.Owin.Security; // Para IAuthenticationManager y SignOut de OWIN
using Microsoft.AspNet.Identity; // Para DefaultAuthenticationTypes
using System.Web; // Para HttpContext.GetOwinContext()

namespace MvcTienda.Web.Controllers
{
    public class AccountController : Controller
    {
        // Se mantiene la dependencia por si se utiliza para lógica de "Manage" o de terceros
        private readonly IUsuarioService _usuarioService;

        // 🟢 CORRECCIÓN 1: Usar Inyección de Dependencias (DI)
        // Eliminamos el constructor manual que rompía el patrón DI.
        public AccountController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Propiedad auxiliar para acceder al manager de autenticación de OWIN
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                // Usado por la clase auxiliar ChallengeResult
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // POST: /Account/LogOff
        // El único método de acción de autenticación principal que mantenemos.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // 🟢 Usar el método de deslogeo de OWIN/Claims.
            // Esto garantiza que la sesión basada en la cookie de OWIN sea terminada.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        // ====================================================================
        // CLASES Y CONSTANTES AUXILIARES (Para ManageController y Autenticación Externa)
        // ====================================================================

        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        // Clase auxiliar ChallengeResult requerida por la acción LinkLogin del ManageController.
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null) { }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}