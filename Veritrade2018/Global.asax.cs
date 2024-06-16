using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JobScheduler.JobScheduler.Start();
        }

        protected void Application_EndRequest()
        {
            if (Context.Items["AjaxPermissionDenied"] is bool)
            {
                Context.Response.StatusCode = 401;
                Context.Response.End();
            }
        }

        protected void Session_End()
        {
            ActiveSessions.Sessions.TryRemove(Session.SessionID, out _);
            if (Session["IdUsuario"] != null)
            {
                int idUsuario = int.Parse(Session["IdUsuario"].ToString());
                bool sesionActiva = false;
                ActualizarSesionUsuario(idUsuario, sesionActiva);
            }
        }

        protected void ActualizarSesionUsuario(int idUsuario, bool sesionActiva)
        {
            string baseurl = Properties.Settings.Default.UrlWeb;
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseurl)
            };

            int sesionUsuario = SesionUsuario.ObtenerSesionActivaPorUsuario(idUsuario);

            string urlApiSesion = $"api/SesionUsuario/?idUsuario={idUsuario}&sesionActiva={sesionActiva}";

            if (sesionUsuario > 0)
            {
                client.PutAsync(urlApiSesion, null);
            }
            else
            {
                client.PostAsync(urlApiSesion, null);
            }
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        protected void Application_Error()
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                /* When the request is ajax the system can automatically handle a mistake with a JSON response. 
                   Then overwrites the default response */
                if (requestContext.HttpContext.Request.IsAjaxRequest())
                {
                    httpContext.Response.Clear();
                    string controllerName = requestContext.RouteData.GetRequiredString("controller");
                    IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                    IController controller = factory.CreateController(requestContext, controllerName);
                    ControllerContext controllerContext = new ControllerContext(requestContext, (ControllerBase)controller);

                    JsonResult jsonResult = new JsonResult
                    {
                        Data = new { success = false, serverError = "500" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    jsonResult.ExecuteResult(controllerContext);
                    httpContext.Response.End();
                }                
            }
        }
    }

}
