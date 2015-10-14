using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using Pms.Data.Models;

namespace Pms.Data
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            // Web API configuration and services
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Parking>("Parking");
            builder.EntitySet<Camera>("Camera");
            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
