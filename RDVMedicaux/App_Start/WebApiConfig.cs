using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace RDVMedicaux
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Défini les WebApi
        /// </summary>
        /// <param name="config">configuration http</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
