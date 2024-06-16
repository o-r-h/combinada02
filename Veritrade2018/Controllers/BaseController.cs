using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            Console.WriteLine("controllerName >>" + controllerName);

            if (controllerName.ToLower() == "demo" || (filterContext.HttpContext.Request.IsAjaxRequest() && controllerName.ToLower() == "misbusquedas") )
            {
                return;
            }
             var IsDemo = Extensiones.GetCookie("IsDemo") == "1";

            var bOmite = (actionName.ToLower() == "index" && controllerName.ToLower() == "misbusquedas" &&
                            filterContext.HttpContext.Request.HttpMethod == "POST");

            

            if ((filterContext.HttpContext.Session["IdUsuario"] == null && !bOmite) )
            {
                Console.WriteLine("302 >> OnActionExecuting");
                var url = GiveMeUrlToRedirect(filterContext);

                filterContext.HttpContext.Session["IdUsuario"] = null;
                Extensiones.SetCookie("Mb", "0", TimeSpan.FromDays(-1));
                Extensiones.SetCookie("VLU", "0", TimeSpan.FromDays(-1)); //JANAQ 140720 Quitar Marca login Universidad
                Extensiones.SetCookie("IsDemo", "0", TimeSpan.FromDays(-1));
                filterContext.Result = new RedirectResult(SettingUtility.GetUrlFrontLogout() + ( !string.IsNullOrEmpty(url) ? "/" + url : "" ));
            } else
            {
                if (bOmite) filterContext.HttpContext.Session["IdUsuario"] = null;
            } 
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
             string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var bOmite = (actionName.ToLower() == "index" && controllerName.ToLower() == "misbusquedas" &&
                            filterContext.HttpContext.Request.HttpMethod == "POST");

            var IsMb = Extensiones.GetCookie("Mb") == "1";
            if (bOmite || IsMb) return;
            

            //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var IsDemo = Extensiones.GetCookie("IsDemo") == "1";

            if (!IsDemo && filterContext.HttpContext.Session["IdUsuario"] != null )
            {
                Console.WriteLine("302 >> OnActionExecuted");
                filterContext.HttpContext.Session["IdUsuario"] = null;
                Extensiones.SetCookie("IsDemo", "1", TimeSpan.FromDays(360));
                Extensiones.SetCookie("Mb", "0", TimeSpan.FromDays(-1));
                Extensiones.SetCookie("VLU", "0", TimeSpan.FromDays(-1)); //JANAQ 140720 Quitar Marca login Universidad
                filterContext.Result = new RedirectResult(SettingUtility.GetUrlFrontLogout());
            }

        }

        private string GiveMeUrlToRedirect(ActionExecutingContext filterContext)
        {
            var url = filterContext.HttpContext.Request.Url.AbsoluteUri;

            int pos = url.IndexOf("?");
            if (pos != -1)
            {
                return url.Substring(pos, url.Length - pos);
            }
            return string.Empty;
        }


        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;

            // Attempt to read the culture cookie from Request
            if (cultureName == null)
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            if (RouteData.Values["culture"] as string != cultureName)
            {

                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too

                // Redirect user
                Response.RedirectToRoute(RouteData.Values);
            }

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}