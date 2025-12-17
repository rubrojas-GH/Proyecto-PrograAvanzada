using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web.Helpers;
using System.Security.Claims;

namespace MvcTienda.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configuración de la cookie de autenticación para nuestro sistema de usuarios
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,

                // Ruta corregida para apuntar a tu controlador de usuarios
                LoginPath = new PathString("/User/Login"),

                // Sesión persistente por 7 días
                ExpireTimeSpan = TimeSpan.FromDays(7),
                SlidingExpiration = true
            });

            // Cookie necesaria para manejar estados de sesión externos si se requirieran
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // IMPORTANTE: Soluciona el error de "NameIdentifier" al usar Claims personalizados.
            // Esto le indica al sistema que use el Email como identificador único para los tokens de seguridad.
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
        }
    }
}