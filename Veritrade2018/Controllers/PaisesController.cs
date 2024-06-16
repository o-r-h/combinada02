using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class PaisesController : BaseController
    {
        // GET: Paises
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

            ViewBag.Menu = "paises";
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["paises"] = ServiciosPaises.GetList(culture);

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            Session.RemoveAll();

            if (c != "") Session["c"] = c;

            if (!string.IsNullOrEmpty(slug))
            {
                ViewData["slug"] = slug;
                Session["paisSlug"] = slug;
            }
            else
            {
                ViewData["slug"] = "ARG";
                Session["paisSlug"] = "ARG";
            }            

            return View();
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            var slug = Session["paisSlug"]?.ToString() ?? "ARG";

            return RedirectToAction("Index", new { slug });
        }
    }
}