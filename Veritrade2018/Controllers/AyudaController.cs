using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class AyudaController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string culture)
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

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            Session.RemoveAll();

            if (c != "") Session["c"] = c;

            ViewBag.Menu = "ayuda";
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

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

            ViewBag.Menu = "ayuda";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

            return View();
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }
    }
}