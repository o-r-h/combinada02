using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Veritrade2017 {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {

            config.Routes.MapHttpRoute(
               name: "DefaultActionApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional },
               constraints: new { action = @"^[a-zA-Z]+([\s][a-zA-Z]+)*$" }
           );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // api/Country/WithStates
            //config.Routes.MapHttpRoute(
            //    name: "ControllerAndActionOnly",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}