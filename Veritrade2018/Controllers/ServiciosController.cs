using System.Web.Mvc;
using System.Web.WebPages;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class ServiciosController : BaseController
    {
        // GET: Servicios
        public static string Currentslug { get; set; }

        [HttpGet]
        public ActionResult Index(string culture, string slug)
        {
            if (Session["IdUsuario"] != null)
            {
                if (Funciones.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
                {
                    //return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
                    return RedirectToAction("Index", "MisBusquedas", new { culture = culture });
                }
            }

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            Session.RemoveAll();
            if (c != "") Session["c"] = c;

            ViewBag.Menu = "servicios";
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["slug"] = !slug.IsEmpty() ? slug : "";

            Currentslug = !slug.IsEmpty() ? slug : "";

            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(string culture, string slug)
        {
            if (Session["IdUsuario"] != null)
            {
                if (Funciones.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
                {
                    //return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
                    return RedirectToAction("Index", "MisBusquedas", new { culture = culture });
                }
                Session.RemoveAll();
            }

            ViewBag.Menu = "servicios";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["slug"] = slug;

            Currentslug = !slug.IsEmpty() ? slug : "";

            return View();
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;

            var prex = Servicios.ObtenerPrefijo(Currentslug);

            var slug = Servicios.ObtenerSlug(prex, culture);

            return RedirectToAction("Index", new { slug });
        }
    }
}