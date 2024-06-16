using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Controllers.Api.Telefonos;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class BaseController : Controller
    {
        // Ruben 202307
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = RouteData.Values["culture"] as string;

            // Ruben 202307
            //Functions3.Log(DateTime.Now.ToString() + "|1|" + (cultureName != null ? cultureName : "NULL"));

            // Attempt to read the culture cookie from Request
            if (cultureName == null)
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

            // Ruben 202307
            //Functions3.Log(DateTime.Now.ToString() + "|2|" + (cultureName != null ? cultureName : "NULL"));

            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Ruben 202307
            //Functions3.Log(DateTime.Now.ToString() + "|3|" + (cultureName != null ? cultureName : "NULL"));
            
            // Ruben 202304
            //RouteData.Values["culture"] = cultureName.ToLowerInvariant();
            //Response.RedirectToRoute(RouteData.Values);

            /*
            if (RouteData.Values["culture"] as string != cultureName)
            {
                // Force a valid culture in the URL
                RouteData.Values["culture"] = cultureName.ToLowerInvariant(); // lower case too

                // Redirect user
                Response.RedirectToRoute(RouteData.Values);
            }
            */

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            //if (Session["Idioma"] == null)
            //{
            //    if (Thread.CurrentThread.CurrentCulture.Name == "es" || Thread.CurrentThread.CurrentCulture.Name == "en")
            //    {
            //        Session["Idioma"] = Thread.CurrentThread.CurrentCulture.Name;
            //    }
            //}

            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["CodPaisIP"] == null)
            {
                string codPaisIp = "";
                var ws = new VeritradeAdmin.Seguridad();
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
                var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);
                Session["CodPaisIP"] = codPaisIp;                
            }

            string cultureName = "es";
            try
            {
                cultureName = filterContext.RouteData.Values["culture"] as string;

                // Attempt to read the culture cookie from Request
                if (cultureName == null)
                    cultureName = filterContext.HttpContext.Request.UserLanguages != null && filterContext.HttpContext.Request.UserLanguages.Length > 0 ? filterContext.HttpContext.Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

                // Validate culture name
                cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe                
            }
            catch (Exception ex)  { Console.WriteLine(ex.Message);  }


            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var bOmite = actionName.ToLower() == "preview" && controllerName.ToLower() == "pruebagratis";
            if (filterContext.HttpContext.Session["IdUsuario"] != null && !bOmite )
            {
                if (Functions.ObtieneOrigen(filterContext.HttpContext.Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
                {
                    Funciones.RemoveCacheOfContenidoGeneral();
                    //return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
                    filterContext.Result = new RedirectResult(SettingUtility.GetUrlBackHome(cultureName));
                }
                //filterContext.HttpContext.Session.RemoveAll();
            }
        }
        
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            var controller = ((string)RouteData.Values["controller"]).ToLower();
            var action = (RouteData.Values["action"] as string)?.ToLower();
            //if (controller?.ToLower() == "setculture") goto ReturnMe;

            // Ruben 202301
            string IdCliente = (RouteData.Values["idc"] as string) ?? "";
            if ((controller == "home" || controller == "planes") && IdCliente != "")
            {                
                if (controller == "home")
                {
                    string UrlVeritradeAdmin = "http://admin2018.veritrade.info/webforms/quieromasinfo?idc=" + IdCliente;
                    System.Net.WebClient Client = new System.Net.WebClient();
                    string Content = Client.DownloadString(UrlVeritradeAdmin);
                }
                if (controller == "planes")
                {
                    string Campaña = (RouteData.Values["campania"] as string) ?? "";
                    if (Campaña.ToLower() != "compra")
                    {
                        string UrlVeritradeAdmin = "http://admin2018.veritrade.info/webforms/quieromasinfo?idcc=" + IdCliente;
                        System.Net.WebClient Client = new System.Net.WebClient();
                        string Content = Client.DownloadString(UrlVeritradeAdmin);
                    }
                }
            }

            var _none = false;
            var sc = (RouteData.Values["campania"] as string) ?? (Session["c"] as string);
            //var param_camp = ;

            //if (!string.IsNullOrEmpty(sc)) sc = sc.Replace("m", "").Replace("I", "");
            //if (!string.IsNullOrEmpty(param_camp)) param_camp = param_camp.Replace("m", "").Replace("I", "");

            var ss_typeCamp = ((Campania.TYPE_CAMPANIA)((int?)Session["TypeCampania"] ?? 0));

            if (!Campania.CodesOrganics.Values.ToArray().Contains(sc)
                && !Campania.CodesWithCampanias.Values.ToArray().Contains(sc) && sc != null)
            {
                ss_typeCamp = Campania.TYPE_CAMPANIA.HOME_CAMPANIA;
                _none = true;
            }

            //var is_home = RouteData.Values["controller"] as string == "Home" &&
            //              action == "index";

            var noo = !Campania.CodesOrganics.Values. Contains(sc)
                      && new[] { "pruebagratis", "empresas", "planes", "blog", "home","productos","productoperfil" }.Contains(controller);

            if (sc != null && (noo
                                && ss_typeCamp == Campania.TYPE_CAMPANIA.ORGANIC))
                ss_typeCamp = Campania.TYPE_CAMPANIA.HOME_CAMPANIA;


            if ((_none || Campania.CodesWithCampanias.Values.ToArray().Contains(sc)) && !SsCampExpired() &&
                (ss_typeCamp != Campania.TYPE_CAMPANIA.ORGANIC || noo)) //&& 
                                                                        //(!Microsoft.VisualBasic.Strings.Left( Campania.CodesOrganics[Campania.GetTheControl(controller)], 5).Equals(sc) && sc != null)))
            {
                if (!_none) ss_typeCamp = Campania.TYPE_CAMPANIA.WITH_CAMPANIA;
                ViewData["CodesCampanias"] = Campania.CodesHomeCampanias(sc);
            }
            else
            {
                ss_typeCamp = Campania.TYPE_CAMPANIA.ORGANIC;
                ViewData["CodesCampanias"] = Campania.CodesOrganics;
                //sc = Campania.CodesOrganics[Campania.CONTROL.PRUEBA_GRATIS];
            }

ReturnMe:
            //var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            //var codes = ViewData["CodesCampanias"] as Dictionary<Campania.CONTROL, string>;

            //foreach (var entry in codes)
            //{
            //    codes[entry.Key] = this.SetCampMobilOrEnglish(entry.Value, (culture != "es"), ss_typeCamp, controller    );
            //}
            //ViewData["CodesCampanias"] = codes;

            Session["TypeCampania"] = (int)ss_typeCamp;
            Session["c"] = ViewData["CodCampaña"] = sc;
            Session["CodsCamp"] = ViewData["CodesCampanias"];
            
            base.OnActionExecuted(filterContext);
        }

        private bool SsCampExpired()
        {
            var ret = false;
            if (Session["tcamp"] == null)
            {
                Session["tcamp"] = DateTime.Now;
            }
            else
            {
                var tc = (DateTime)Session["tcamp"];
                ret = (DateTime.Now - tc).TotalMinutes >= Properties.Settings.Default.Campania_Expired;
                if (ret) Session["tcamp"] = null;
            }
            return ret;
        }

        [HttpPost]
        public virtual ActionResult SetCulture(string culture, string slug_pais_ruc_trib = "")
        {
            var controller = RouteData.Values["controller"] as string;

            // Ruben 202304
            if (controller == "Minisite")
            {
                string[] valores = slug_pais_ruc_trib.Split('|');
                RouteData.Values["slug"] = valores[0];
                RouteData.Values["pais"] = valores[1];
                RouteData.Values["ruc"] = valores[2];
                RouteData.Values["trib"] = valores[3];
            }

            // Ruben 202212
            foreach (KeyValuePair<string, object> kvp in RouteData.Values)
            {
                string key = kvp.Key;
                object value = kvp.Value;
                Functions3.Log("Inicio | " + key + " | " + value.ToString());
            }

            var sty = ((Campania.TYPE_CAMPANIA)((int?)Session["TypeCampania"] ?? 0));
            culture = CultureHelper.GetImplementedCulture(culture);
            
            var uriCamp = string.Empty;
            var values = RouteDataContext.RouteValuesFromUri(Request.UrlReferrer);
            if (values != null)
            {
                uriCamp = values["campania"] as string;
            }

            if (new string[] { "PruebaGratis" }.Contains(controller))
            {
                if (new string[] { "20100", "20100m", "20100I", "20100mI" }.Contains(uriCamp))
                {
                    controller = "Minisite";
                }
                else if (new string[] { "20200", "20200m", "20200I", "20200mI" }.Contains(uriCamp))
                {
                    controller = "ProductoPerfil";
                }            
            }
            RouteData.Values["culture"] = culture; // set culture 
            var _eng = culture != "es";

            if ((Session["c"] != null && string.IsNullOrEmpty(uriCamp)) &&
                !new[] {"pruebagratis", "empresas", "planes", "blog"}.Contains(controller?.ToLower()) &&
                sty != Campania.TYPE_CAMPANIA.ORGANIC)
            {
                var x  = Campania.SetCampMobilOrEnglish(Session["c"] as string, _eng, sty,
                    controller, Request.Browser.IsMobileDevice, true);
                if (RouteData.Values["campania"] == null)
                    Session["c"] = x;
                else
                    RouteData.Values["campania"] = x;
            }
            else if (!string.IsNullOrEmpty(uriCamp))
                RouteData.Values["campania"] = Campania.SetCampMobilOrEnglish(uriCamp, _eng, sty, controller, Request.Browser.IsMobileDevice);
            else if (sty == Campania.TYPE_CAMPANIA.ORGANIC)
                Session["c"] = null;

            //if (RouteData.Values["action"] == null)
                RouteData.Values["action"] = "Index";

            //if (RouteData.Values["campania"] != null &&
            //    new[] {"empresas", "pruebagratis"}.Contains(controller?.ToLower()))
            //    RouteData.Values["campania"] =
            //        Campania.SetCampMobilOrEnglish(RouteData.Values["campania"] as string, _eng, sty, controller, Request.Browser.IsMobileDevice);

            controller = RouteData.Values["controller"] as string;

            // Ruben 202212
            foreach (KeyValuePair<string, object> kvp in RouteData.Values)
            {
                string key = kvp.Key;
                object value = kvp.Value;
                Functions3.Log("Fin | " + key + " | " + value.ToString());
            }

            if (_eng && controller != "Home") // (_eng) Ruben 202212
            {
                var _route = controller + "US";                               

                if (RouteTable.Routes[_route] != null)
                {
                    return RedirectToRoute(_route, RouteData.Values);
                }
            }

            if (controller == "Home" && culture == "es")
                return Redirect("https://www.veritradecorp.com/es");

            return RedirectToRoute(RouteData.Values); 
        }

    }
}