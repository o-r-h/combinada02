using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Newtonsoft.Json;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Models.CountryProfile;
using Veritrade2017.Models.Minisite;
using Paises = Veritrade2017.Models.Minisite.Paises;

namespace Veritrade2017.Controllers
{
    public class MinisiteController : BaseController
    {   
        // GET: Minisite 
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheCompanyProfile")]
        public ActionResult Index(string culture, string slug, string pais, string ruc, string trib)
        {
            if (Session["IdUsuario"] != null)
            {
                var idusuario = Convert.ToInt32(Session["IdUsuario"].ToString());
                var usuarioRegistrado = Usuario.GetUsuario(idusuario);
                ViewData["registrado"] = usuarioRegistrado.Nombres;
            }
            
			// Ruben 202311
            // https://www.veritradecorp.com/es/peru/importaciones-y-exportaciones-importaciones-y-exportaciones-anaid-eirl/ruc-20532843023
            if (culture == "aa")
            {
                culture = "es";
                slug = "importaciones-y-exportaciones-" + slug;
            }
            // Ruben 202303
            // https://www.veritradecorp.com/es/peru/importaciones-y-exportaciones-alicorp-saa/ruc-20100055237

            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/" + pais + "/importaciones-y-exportaciones-" + slug + "/" + trib + "-" + ruc;
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/" + pais + "/imports-and-exports-" + slug + "/" + trib + "-" + ruc;

            // Ruben 202306
            pais = pais.Replace("-", " ");

            /*
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/" + pais + "/" + slug + "/" + trib + "-" + ruc;
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/" + pais + "/" + slug + "/" + trib + "-" + ruc;
            */

            // Ruben 202304
            /*
            string impo_expo = "importaciones-y-exportaciones-";
            if (slug.Length > 30 && slug.Substring(0, 30) == impo_expo)
                slug = slug.Substring(30, slug.Length - 30);
            else
            {
                impo_expo = "imports-and-exports-";
                if (slug.Length > 20 && slug.Substring(0, 20) == impo_expo)
                    slug = slug.Substring(20, slug.Length - 20);
            }
            */

            /*
            string impo_expo = "importaciones-y-exportaciones-";
            if (slug.Substring(0, 30) == impo_expo)
                slug = slug.Substring(30, slug.Length - 30);
            else
            {
                impo_expo = "imports-and-exports-";
                if (slug.Substring(0, 20) == impo_expo)
                    slug = slug.Substring(20, slug.Length - 20);
            }
            */

            Session["sRuc"] = ruc;

            ViewBag.Menu = "planes";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            //_ruc = ruc;

            var sql = "SELECT * FROM [Configuracion] WHERE Pais = '" + pais + "' And RegistroTrib = '" + trib + "'";
            var dt = Conexion.SqlDataTableMinisite(sql);
            bool ExisteTrib = dt.Rows.Count > 0;

            if (!ExisteTrib)
            {
                //return RedirectToRoute("Default");
                return RedirectToRoute("Root");
            }

            if (!Empresa.ExistEmpresa(slug, ruc))
            {
                //return RedirectToRoute("Default");
                return RedirectToRoute("Root");
            }
            else //saving for last searches
            {
                new BuscaEmpresaModel().SaveLastSearches(slug, ruc);
            }

            ////var c = "";
            ////if (Session["c"] != null) c = Session["c"].ToString();

            ////if (c == "") c = culture.Equals("es") ? "20100" : "20100I";

            ////if (Session["c"] == null) Session["c"] = c;

            ////ViewData["CodCampaña"] = c;

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

            var empresa = Empresa.GetEmpresa(slug, ruc);
            ViewData["nombreEmpresa"] = empresa.Nombre;
            Session["nombreEmpresa"] = empresa.Nombre;
            ViewData["rucEmpresa"] = empresa.Ruc;
            ViewData["paisEmpresa"] = empresa.PaisEmpresa;
            ViewData["paisImagen"] = "flag_" + empresa.PaisEmpresa.ToLower() + ".png";
            ViewData["registriTrib"] = Configuracion.GetRegistrotrib(empresa.PaisEmpresa);
            ViewData["impEmbarques"] = Resumen.GetTipoEmbarques(empresa.IdEmpresa);
            ViewData["expEmbarques"] = Resumen.GetTipoEmbarques(empresa.IdEmpresa, 1);

            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);
            ViewData["impTotalEmbarques"] = hasImport;
            ViewData["expTotalEmbarques"] = hasExport;
            if (hasImport > hasExport)
            {
                ViewData["FobTotal"] = Detalle.GetTotalFob(empresa.IdEmpresa, 1);
                ViewData["CifTotal"] = Detalle.GetTotalCif(empresa.IdEmpresa);
            }
            else
            {
                ViewData["FobTotal"] = Detalle.GetTotalFob(empresa.IdEmpresa, 1);
                ViewData["CifTotal"] = Detalle.GetTotalCif(empresa.IdEmpresa);
            }

            ViewData["totalEmbarques"] = Resumen.GetTotalEmbarques(empresa.IdEmpresa);
            ViewData["slug"] = slug;

            Session["slugEmpresa"] = slug;
            Session["paisEmpresa"] = pais;
            Session["rucEmpresa"] = ruc;
            Session["tribEmpresa"] = trib;

            return View();
        }

        [ChildActionOnly]
        public ActionResult ChartImpMinisite(string slug, string culture)
        {
            //ViewData["CodCampaña"] = culture.Equals("es") ? "20100" : "20100I";
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var model = Resumen.GetData(empresa.IdEmpresa);

            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);

            if (hasImport > hasExport)
            {
                ViewData["themeexp"] = "Origin";
                ViewData["theme"] = "Default";
            }
            else
            {
                ViewData["themeexp"] = "Default";
                ViewData["theme"] = "Origin";
            }
            return PartialView("_ChartImpMinisite", model);
        }

        [ChildActionOnly]
        public ActionResult ChartExpMinisite(string slug, string culture)
        {
            //ViewData["CodCampaña"] = culture.Equals("es") ? "20100" : "20100I";
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var model = Resumen.GetData(empresa.IdEmpresa, 1);
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);

            if (hasImport > hasExport)
            {
                ViewData["themeexp"] = "Origin";
                ViewData["theme"] = "Default";
            }
            else
            {
                ViewData["themeexp"] = "Default";
                ViewData["theme"] = "Origin";
            }
            return PartialView("_ChartExpMinisite", model);
        }

        [ValidateInput(false)]
        [ChildActionOnly]
        public ActionResult GridView1Partial(string slug, string culture, string my_campania = "")
        {
            //Session["c"] = culture == "es" ? "20100" : "20100I";
            //ViewData["CodCampaña"] = Session["c"].ToString();
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);

            List<Productos> model;
            if (hasImport > hasExport)
            {
                model = Productos.GetData(empresa.IdEmpresa);
            }
            else
            {
                model = Productos.GetData(empresa.IdEmpresa, 1);
            }
            ViewData["my_campania"] = my_campania;
            return PartialView("_GridView1Partial", model);
        }

        [ValidateInput(false)]
        [ChildActionOnly]
        public ActionResult GridView2Partial(string slug, string culture, string my_campania = "")
        {
            //Session["c"] = culture == "es" ? "20100" : "20100I";
            //ViewData["CodCampaña"] = Session["c"].ToString();
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);

            List<Paises> model;
            if (hasImport > hasExport)
            {
                model = Paises.GetData(empresa.IdEmpresa);
            }
            else
            {
                model = Paises.GetData(empresa.IdEmpresa, 1);
            }
            ViewData["my_campania"] = my_campania;
            return PartialView("_GridView2Partial", model);
        }

        [ValidateInput(false)]
        [ChildActionOnly]
        public ActionResult GridView3Partial(string slug, string culture, string my_campania = "")
        {
            //Session["c"] = culture == "es" ? "20100" : "20100I";
            //ViewData["CodCampaña"] = Session["c"].ToString();
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);

            string tipo;
            List<Detalle> model;
            if (hasImport > hasExport)
            {
                model = Detalle.GetData(empresa.IdEmpresa);
                tipo = "importaciones";
            }
            else
            {
                model = Detalle.GetData(empresa.IdEmpresa, 1);
                tipo = "exportaciones";
            }

            ViewData["paisEmpresa"] = empresa.PaisEmpresa;
            ViewData["tipo"] = tipo;
            ViewData["my_campania"] = my_campania;
            return PartialView("_GridView3Partial", model);
        }

        [ChildActionOnly]
        public ActionResult ModalPartial(string slug, string culture, string my_campania = "")
        {
            //Session["c"] = culture == "es" ? "20100" : "20100I";
            //ViewData["CodCampaña"] = Session["c"].ToString();
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var model = Detalle.GetDataPartida(empresa.IdEmpresa);
            var detalle = Detalle.GetFirstDetalle(empresa.IdEmpresa);
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);
            var tipo = hasImport > hasExport ? "importaciones" : "exportaciones";

            ViewData["partida"] = detalle.Nandina;
            ViewData["descripcion"] = detalle.PartidaDesc;
            ViewData["tipo"] = tipo;
            Session["nombreEmpresa"] = empresa.Nombre;
            ViewData["empresa"] = empresa.Nombre;
            ViewData["my_campania"] = my_campania;
            return PartialView("Modals/Modal_Partida", model);
        }

        [ChildActionOnly]
        public ActionResult ModalEmbarque(string slug, string culture, string my_campania = "")
        {
            //Session["c"] = culture == "es" ? "20100" : "20100I";
            //ViewData["CodCampaña"] = Session["c"].ToString();
            var empresa = Empresa.GetEmpresa(slug, Session["sRuc"]?.ToString());
            var model = Detalle.GetDataEmbarque(empresa.IdEmpresa);
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);
            var tipo = hasImport > hasExport ? "importaciones" : "exportaciones";

            ViewData["tipo"] = tipo;

            ViewData["my_campania"] = my_campania;
            return PartialView("Modals/Modal_Embarque", model);
        }

        [HttpPost]
        public JsonResult BuscarEmpresa(string nandina, string codPais = "", string opcion = "")
        {
          var json = Empresa.SearchEmpresa(nandina);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public override ActionResult SetCulture(string culture, string slug_pais_ruc_trib = "")
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
            //ViewData["CodCampaña"] = culture.Equals("es")
            //    ? Session["c"].ToString().Replace("I", "")
            //    : Session["c"].ToString().Replace("I", "") + "I";
            var slug = "";
            if (Session["slugEmpresa"] != null)
                slug = Session["slugEmpresa"].ToString();

            var pais = "";
            if (Session["paisEmpresa"] != null)
                pais = Session["paisEmpresa"].ToString();

            var ruc = "";
            if (Session["rucEmpresa"] != null)
                ruc = Session["rucEmpresa"].ToString();

            var trib = "";
            if (Session["tribEmpresa"] != null)
                trib = Session["tribEmpresa"].ToString();
            
            //if (culture == "es")
            //{
            //    return RedirectToRoute("Minisite", new { culture, slug, pais, ruc, trib });
            //}
            //else
            //{
            //    return RedirectToRoute("MinisiteUS", new { culture, slug, pais, ruc, trib });
            //}
            ////return RedirectToAction("Index", "Minisite", new { culture, slug, pais, ruc, trib });
            ///

            RouteData.Values["slug"] = slug;
            RouteData.Values["pais"] = pais;
            RouteData.Values["ruc"] = ruc;
            RouteData.Values["trib"] = trib;

            //var sc = (RouteData.Values["campania"] as string) ?? (Session["c"] as string);

            //if (culture == "es")
            //{
            //    if (sc == "13100") Session["c"] = RouteData.Values["campania"] = "12100";
            //}
            //else
            //{
            //    if (sc == "12100") Session["c"] = RouteData.Values["campania"] = "13100";
            //}

            return base.SetCulture(culture, slug_pais_ruc_trib);
        }

		
		
	}
}