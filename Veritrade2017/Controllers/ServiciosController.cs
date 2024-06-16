using System.Web.Mvc;
using System.Web.WebPages;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class ServiciosController : BaseController
    {
        public static string Currentslug { get; set; }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Index(string culture, string slug, string codCampania)
        {
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //}

            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null) c = Session["c"].ToString();

            ////Session.RemoveAll();

            //if (c != "") Session["c"] = c;

            ViewBag.Menu = "servicios";
            
            if (string.IsNullOrEmpty(codCampania)) {
                ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";
            } else
            {
                ViewData["CodCampaña"] = codCampania;
                Session["c"] = ViewData["CodCampaña"];
            }
            //RouteData.Values["campania"] = ViewData["CodCampaña"];


            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["slug"] = !slug.IsEmpty() ? slug : "";

            Currentslug = !slug.IsEmpty() ? slug : "";

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/servicios/" + ViewData["slug"];
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/services/" + ViewData["slug"];

            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(string culture, string slug)
        {
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //    Session.RemoveAll();
            //}

            ViewBag.Menu = "servicios";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["slug"] = slug;

            Currentslug = !slug.IsEmpty() ? slug : "";

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/servicios/" + ViewData["slug"];
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/services/" + ViewData["slug"];

            return View();
        }

        [HttpPost]
        public override ActionResult SetCulture(string culture, string slug_pais_ruc_trib = "")
        {
            //var sty = ((Campania.TYPE_CAMPANIA)((int?)Session["TypeCampania"] ?? 0));
            //culture = CultureHelper.GetImplementedCulture(culture);
            //var controller = RouteData.Values["controller"] as string;
            //RouteData.Values["culture"] = culture;

            var prex = Servicios.ObtenerPrefijo(Currentslug);

            var slug = Servicios.ObtenerSlug(prex, culture);
            //if (Session["c"] != null) RouteData.Values["campania"] = Campania.CodFormated2((Session["c"] as string), culture, Request.Browser.IsMobileDevice, sty, Campania.GetTheControl(controller));

            //if (culture == "es")
            //{
            //    return RedirectToRoute("Servicios", new { slug, culture });
            //}
            //else
            //{
            //    return RedirectToRoute("ServiciosUS", new { slug, culture });
            //}

            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["slug"] = slug;

            return base.SetCulture(culture);

        }
    }
}