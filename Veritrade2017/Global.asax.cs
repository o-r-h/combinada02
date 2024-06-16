using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Org.BouncyCastle.Asn1.Cmp;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Register global filter
            //GlobalFilters.Filters.Add(new MyRedirectToSessionActionFilterAttribute());


            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
            
            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;

            
        }

        protected void Application_End()
        {

        }

        protected void Session_End()
        {
            if (Session["IdUsuario"] != null)
            {
                int idUsuario = int.Parse(Session["IdUsuario"].ToString());
                bool sesionActiva = false;
                ActualizarSesionUsuario(idUsuario, sesionActiva);
            }
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
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
    }

    public class MyRedirectToSessionActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string cultureName = "es";
            try
            {
                cultureName = context.RouteData.Values["culture"] as string;

                // Attempt to read the culture cookie from Request
                if (cultureName == null)
                    cultureName = context.HttpContext.Request.UserLanguages != null && context.HttpContext.Request.UserLanguages.Length > 0 ? context.HttpContext.Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            // Ruben 202307
            //cultureName = "fr";

            if (context.HttpContext.Session["IdUsuario"] != null)
            {
                if (Functions.ObtieneOrigen(context.HttpContext.Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
                {
                    Funciones.RemoveCacheOfContenidoGeneral();
                    context.Result = new RedirectResult(SettingUtility.GetUrlBackHome(cultureName) );
                }

                //context.HttpContext.Session.RemoveAll();
            }
            base.OnResultExecuting(context);
        }
    }
}