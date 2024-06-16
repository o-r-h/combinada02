using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
    public class EmpresasController : BaseController
    {
        //[DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        // GET: Empresas
        public ActionResult Index(string culture)
        {
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //    Session.RemoveAll();
            //}
            ViewBag.Menu = culture.Equals("es") ? "buscar-empresas" : "search-companies";
            //ViewData["CodCampaña"] = culture.Equals("es") ? "20100" : "20100I";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            //ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
            ViewData["last_searches"] = new BuscaEmpresaModel().GetLastSearches();

            //PARTE AGREGADA
            //var c = "";
            //if (RouteData.Values["campania"] != null)
            //{
            //    c = RouteData.Values["campania"].ToString();
            //}

            //if (Session["c"] != null && c != "")
            //{
            //    c = Session["c"].ToString();
            //}

            //if (c == "")
            //{
            //    c = culture == "es" ? "20100" : "20100I";
            //}

            ////if (Session["c"] == null) Session["c"] = c;

            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null)
            //{
            //    if (Session["c"].ToString() == "20100" || Session["c"].ToString() == "20100I")
            //    {
            //        c = Session["c"].ToString();
            //    }                   
            //}
            //if (c != "" && (c == "20100" || c == "20100I"))
            //{
            //    Session["c"] = c;
            //    ViewData["CodCampaña"] = c;
            //}

            //else
            //{
            //    var arr = new string[] { "51001", "51001I", "14100", "14100I" };
            //    if (!arr.Contains(c) && !string.IsNullOrEmpty(c))
            //    {
            //        Session["c"] = c;
            //        ViewData["CodCampaña"] = c;
            //    }
            //    else
            //    {
            //        ViewData["CodCampaña"] = culture.Equals("es") ? "20100" : "20100I";
            //        Session["c"] = ViewData["CodCampaña"].ToString();
            //    }
            //}

            //var campania = Session["c"].ToString();
            //ViewData["cod_campaña"] = campania;
            ViewData["CountSearches"] = new BuscaEmpresaModel().CountJson();

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/buscar-empresas";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/search-companies";

            return View();
        }

        //[HttpPost]
        //public override ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);

        //    var sc = (RouteData.Values["campania"] as string) ?? (Session["c"] as string);

        //    if (culture == "es")
        //    {
        //        if (sc == "20100") Session["c"] = RouteData.Values["campania"] = "20100";
        //    }
        //    else
        //    {
        //        if (sc == "12100") Session["c"] = RouteData.Values["campania"] = "13100";
        //    }

        //    return base.SetCulture(culture);
        //}
        public JsonResult LastSearches(string culture)
        {
            List<BuscaEmpresaModel> lastSearches = null;
            lastSearches = new BuscaEmpresaModel().GetLastSearches();

            string viewLastSearches = RenderViewToString(this.ControllerContext, "Partials/LastSearches", lastSearches);
            return Json(new
            {
                viewLastSearches
            });
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