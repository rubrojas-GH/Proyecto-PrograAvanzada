using ProyectoPrograAvanzada.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ProyectoPrograAvanzada
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // 1. Verificar si el usuario está autenticado vía FormsAuthentication
            if (Context.User != null && Context.User.Identity.IsAuthenticated && Context.User.Identity is FormsIdentity)
            {
                // Obtener el ticket de autenticación. El FormsAuthenticationTicket contiene el nombre de usuario (email)
                FormsIdentity id = (FormsIdentity)Context.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;

                // El nombre de usuario (ticket.Name) es el email que usaste para loguearte
                string userEmail = ticket.Name;

                // 2. Instanciar el Repositorio/Servicio para obtener los roles
                // IMPORTANTE: Debes ajustar estas líneas si usas inyección de dependencias más avanzada.
                // Para este ejemplo, lo instanciamos directamente.
                var usuarioRepository = new UsuarioRepository();

                // 3. Obtener el nombre del rol del usuario
                // NECESITAS ESTE MÉTODO EN TU REPOSITORIO/SERVICIO
                string roleName = usuarioRepository.GetUserRoleNameByEmail(userEmail);

                if (!string.IsNullOrEmpty(roleName))
                {
                    // 4. Crear un nuevo IPrincipal con los roles adjuntos
                    string[] roles = new string[] { roleName };

                    // System.Security.Principal.GenericPrincipal permite que User.IsInRole funcione
                    Context.User = new System.Security.Principal.GenericPrincipal(id, roles);
                }
            }
        }
     }
}
