using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class AyudaController : BaseController
    {
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        [HttpGet]
        public ActionResult Index(string culture, string action, string codCampania)
        {
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }

            //    Session.RemoveAll();
            //}

            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null) c = Session["c"].ToString();

            //Session.RemoveAll();

            //if (c != "") Session["c"] = c;

            ViewBag.Menu = "ayuda";
            //ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";
            if (!string.IsNullOrEmpty(codCampania))
            { 
                ViewData["CodCampaña"] = codCampania;
                Session["c"] = ViewData["CodCampaña"];
            }

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/ayuda";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/help";

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

            ViewBag.Menu = "ayuda";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/ayuda";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/help";

            return View();
        }

        //[HttpPost]
        //public override ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    RouteData.Values["culture"] = culture; // set culture 

        //    if (Session["c"] != null) RouteData.Values["campania"] = Campania.CodFormated((Session["c"] as string), culture, Request.Browser.IsMobileDevice);

        //    if (culture == "es")
        //    {   
        //        return RedirectToRoute("Ayuda", new {culture, action = "Index"});
        //    }
        //    return RedirectToRoute("AyudaUS", new {culture, action = "Index"});
        //}
    }
}