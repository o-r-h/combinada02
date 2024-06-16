using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class PaisesController : BaseController
    {
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Index(string culture, string slug, string codCampania)
        {
            // Ruben 2023-08
            if (slug != "" && slug == slug.ToUpper())
            {
                string url;
                if (culture == "es")
                    url = "https://www.veritradecorp.com/es/paises/" + slug.ToLower();
                else
                    url = "https://www.veritradecorp.com/en/countries/" + slug.ToLower();

                Response.RedirectPermanent(url);
            }

            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //}

            ViewBag.Menu = "paises";
            if (string.IsNullOrEmpty(codCampania))
            {
                ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";
            }
            else
            {
                ViewData["CodCampaña"] = codCampania;
                Session["c"] = ViewData["CodCampaña"];
            }

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["paises"] = ServiciosPaises.GetList(culture);

            var c = "";
            if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            if (Session["c"] != null) c = Session["c"].ToString();

            //Session.RemoveAll();

            if (c != "") Session["c"] = c;

            if (!string.IsNullOrEmpty(slug))
            {
                ViewData["slug"] = slug;
                Session["paisSlug"] = slug;
            }
            else
            {
                ViewData["slug"] = "arg";
                Session["paisSlug"] = "arg";
            }

            // Ruben 202308
            var sql = "select nombre_pais from servicios_pais where abreviatura = '" + ViewData["slug"] + "' and idioma_id = " + (culture == "es" ? "1" : "2");

            var dt = Conexion.SqlDataTable(sql, true);

            var nombre_pais = dt.Rows[0]["nombre_pais"].ToString();

            if (culture == "es")
            {
                ViewBag.Title = "Importaciones y exportaciones de " + nombre_pais;
                ViewBag.Description = "Principales estadísticas de comercio exterior para descubrir nuevas oportunidades de importación y exportación de " + nombre_pais + ". ¡Empieza a investigar el mercado!";
                ViewBag.Canonical = "https://www.veritradecorp.com/es/paises/" + ViewData["slug"];
                
            }
            else
            {
                ViewBag.Title = "Imports and exports of " + nombre_pais;
                ViewBag.Description = "Main foreign trade statistics to discover new import and export opportunities of " + nombre_pais + ". Start researching the market!";
                ViewBag.Canonical = "https://www.veritradecorp.com/en/countries/" + ViewData["slug"];
            }

            return View();
        }

        [HttpPost]
        public override ActionResult SetCulture(string culture, string slug_pais_ruc_trib = "")
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            var slug = Session["paisSlug"]?.ToString() ?? "ARG";

            RouteData.Values["slug"] = slug;

            return base.SetCulture(culture);
        }
    }
}