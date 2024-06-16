using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Models.Admin;
using Veritrade2017.Models.ProductProfile;

namespace Veritrade2017.Controllers
{
    public class ProductosController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        //[DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        // GET: Productos
        public ActionResult Index(string culture)
        {
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewBag.Menu = culture.Equals("es") ? "buscar-productos" : "search-products";
            var c = "";
            if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();

            if (Session["c"] != null && c != "") c = Session["c"].ToString();

            if (c == "") c = "12100";

            if (Session["c"] == null) Session["c"] = c;
            if (culture.Equals("es"))
            {
                ViewData["last_searches"] = new BuscarProducto().GetLastSearches();
            }
            else
            {
                ViewData["last_searches"] = new BuscarProducto().GetLastSearchesEnglish();
            }

            ViewData["CountSearches"] = new BuscarProducto().CountJson();
            var campania = Session["c"].ToString();
            ViewData["cod_campaña"] = campania;

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/buscar-productos";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/search-products";

            return View();
        }
        [HttpPost]
        public JsonResult BuscarProducto(string description, string idioma, string codPais = "", string opcion = "")
        {
            string CodPaisIP = "";

#if DEBUG

            string DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif

            string Ubicacion = _ws.BuscaUbicacionIP2(DireccionIP, ref CodPaisIP);
            //string idioma = "es";
            var json = MisProductos.SearchProduct(description, CodPaisIP, idioma);
            return Json(json);
        }

        public JsonResult LastSearches()
        {
            var lastSearches = new BuscarProducto().GetLastSearches(0);
            string viewLastSearches = RenderViewToString(this.ControllerContext, "Productos/Partials/LastSearches", lastSearches);
            return Json(new
            {
                viewLastSearches
            });
        }
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            var c = "";
            if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();

            if (Session["c"] != null && c != "") c = Session["c"].ToString();

            if (c == "") c = "12100";

            if (Session["c"] == null) Session["c"] = c;

            var codCampania = Session["c"].ToString();
            ViewData["cod_campaña"] = codCampania;

            if (culture == "es")
            {
                return RedirectToRoute("Productos", new { culture, campania = codCampania, action = "Index" });
            }
            else
            {
                return RedirectToRoute("ProductosUS", new { culture, campania = codCampania, action = "Index" });

            }
            //return RedirectToAction("Index");
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