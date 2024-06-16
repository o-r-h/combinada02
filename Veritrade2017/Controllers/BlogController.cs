using System.Linq;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class BlogController : BaseController
    {
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
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

            ViewBag.Menu = "blog";
            ////ViewData["CodCampaña"] = culture.Equals("es") ? "14100" : "14100I";

            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null)
            //{
            //    if (Session["c"].ToString() == "14100" || Session["c"].ToString() == "14100I")
            //    {
            //        c = Session["c"].ToString();
            //    }
            //}

            ////Session.RemoveAll();

            //if (c != "" && (c == "14100" || c =="14100I"))
            //{
            //    Session["c"] = c;
            //    ViewData["CodCampaña"] = c;
            //}               
            ////Nuevo código Gabriel
            //else
            //{
            //    var arr = new string[] {"51001", "51001I", "20100", "20100I" };
            //    if (!arr.Contains(c) && !string.IsNullOrEmpty(c)   )
            //    {
            //        Session["c"] = c;
            //        ViewData["CodCampaña"] = c;
            //    }
            //    else
            //    {
            //        ViewData["CodCampaña"] = culture.Equals("es") ? "14100" : "14100I";
            //        Session["c"] = ViewData["CodCampaña"].ToString();
            //    }
            //}            

            var postLanguage = new PostLanguage();

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["post_language"] = postLanguage.HomePostLanguages(culture, 4);
            ViewData["ultimo_post"] = postLanguage.LastPost(culture);
            ViewData["mas_visitados"] = postLanguage.MostVisited(culture);
            ViewData["post_publicados"] = postLanguage.OldPost(culture);

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/blog";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/blog";

            return View();
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Post(string culture, int id, string codCampania)
        {
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //    Session.RemoveAll();
            //}

            ViewBag.Menu = "blog";

            var post = new Post();

            if (!post.GetPost(culture, id))
            {
                //return RedirectToRoute("Default");
                return RedirectToRoute("Root");
            }

            post.SetPostVisit(id);

            if (!string.IsNullOrEmpty(codCampania))
            {
                ViewData["CodCampaña"] = codCampania;
                Session["c"] = ViewData["CodCampaña"];
            }


            var postLanguage = new PostLanguage();
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["post_description"] = new Post().GetPostDescription(id);
            ViewData["contenido_post"] = postLanguage.GetPostContent(culture, id);
            ViewData["mas_visitados"] = postLanguage.MostVisited(culture);
            ViewData["post_publicados"] = postLanguage.OldPost(culture);

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/blog/post/" + id;
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/blog/post/" + id;

            return View();
        }

        //[HttpPost]
        //public ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    if (Session["c"] != null) RouteData.Values["campania"] =   Campania.CodFormated((Session["c"] as string ), culture, Request.Browser.IsMobileDevice);
        //    RouteData.Values["action"] = "Index";
        //    RouteData.Values["culture"] = culture; // set culture 
        //    return RedirectToRoute(RouteData.Values); //RedirectToAction("Index");

        //    //var codCampania = "";

        //    //if (culture == "es")
        //    //{
        //    //    //Session["c"] = "20100";
        //    //    Session["c"] = Session["c"].ToString().Replace("I", "");
        //    //    codCampania = Session["c"].ToString();
        //    //    return RedirectToRoute("Blog", new { culture, campania = codCampania });
        //    //}
        //    //else
        //    //{
        //    //    //Session["c"] = "20100I";
        //    //    Session["c"] = Session["c"].ToString().Replace("I", "") + "I";
        //    //    codCampania = Session["c"].ToString();
        //    //    return RedirectToRoute("Blog", new {culture, campania = codCampania});

        //    //    //return RedirectToAction("Index");
        //    //}
        //}
    }
}