using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity; // Mantener solo para DefaultAuthenticationTypes

namespace MvcTienda.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // ELIMINAR Identity Contexts y Managers.
            // app.CreatePerOwinContext(...)

            // Permitir que la aplicación use una cookie para almacenar información del usuario.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // El AuthenticationType debe coincidir con el tipo de autenticación que usarás.
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

                // Ajustar la ruta de Login. 
                // Asumiendo que tu controlador es 'User' y la acción es 'Login'.
                LoginPath = new PathString("/User/Login"),

                // ELIMINAR el SecurityStampValidator, ya que depende de ASP.NET Identity.
                // Provider = new CookieAuthenticationProvider { ... }

                // Opcional: Configurar el tiempo de expiración
                ExpireTimeSpan = TimeSpan.FromDays(7), // Ejemplo: Recordarme por 7 días
                SlidingExpiration = true
            });

            // Si no usas inicios de sesión externos, puedes eliminar estas líneas.
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            // app.UseTwoFactorSignInCookie(...)
            // app.UseTwoFactorRememberBrowserCookie(...)

            // Desactivar o eliminar proveedores de terceros si no se usan
            // app.UseGoogleAuthentication(...)
        }
    }
}