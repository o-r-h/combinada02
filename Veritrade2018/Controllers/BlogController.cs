using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class BlogController : BaseController
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

            ViewBag.Menu = "blog";
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            Session.RemoveAll();

            if (c != "") Session["c"] = c;

            var postLanguage = new PostLanguage();

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["post_language"] = postLanguage.HomePostLanguages(culture, 4);
            ViewData["ultimo_post"] = postLanguage.LastPost(culture);
            ViewData["mas_visitados"] = postLanguage.MostVisited(culture);
            ViewData["post_publicados"] = postLanguage.OldPost(culture);

            return View();
        }

        [HttpGet]
        public ActionResult Post(string culture, int id)
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

            ViewBag.Menu = "blog";

            var post = new Post();

            if (!post.GetPost(culture, id))
            {
                return RedirectToRoute("Default");
            }

            post.SetPostVisit(id);

            var postLanguage = new PostLanguage();
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["post_description"] = new Post().GetPostDescription(id);
            ViewData["contenido_post"] = postLanguage.GetPostContent(culture, id);
            ViewData["mas_visitados"] = postLanguage.MostVisited(culture);
            ViewData["post_publicados"] = postLanguage.OldPost(culture);

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