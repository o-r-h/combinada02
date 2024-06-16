using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;
using Veritrade2018.Models.Minisite;
using Paises = Veritrade2018.Models.Minisite.Paises;

namespace Veritrade2018.Controllers
{
    public class MinisiteController : BaseController
    {
        private string _ruc;
        // GET: Minisite
        [HttpGet]
        public ActionResult Index(string culture, string slug, string pais, string ruc, string trib)
        {
            if (Session["IdUsuario"] != null)
            {
                var idusuario = Convert.ToInt32(Session["IdUsuario"].ToString());
                var usuarioRegistrado = Usuario.GetUsuario(idusuario);
                ViewData["registrado"] = usuarioRegistrado.Nombres;
            }

            ViewBag.Menu = "planes";


            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            _ruc = ruc;

            if (!Empresa.ExistEmpresa(slug, ruc))
            {
                return RedirectToRoute("Default");
            }

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            if (c == "") c = culture.Equals("es") ? "20100" : "20100I";

            if (Session["c"] == null) Session["c"] = c;

            ViewData["CodCampaña"] = c;

            var empresa = Empresa.GetEmpresa(slug, _ruc);
            ViewData["nombreEmpresa"] = empresa.Nombre;
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

            ViewData["modelImp"] = Resumen.GetData(empresa.IdEmpresa);
            ViewData["modelExp"] = Resumen.GetData(empresa.IdEmpresa, 1);

            Session["slugEmpresa"] = slug;
            Session["paisEmpresa"] = pais;
            Session["rucEmpresa"] = ruc;
            Session["tribEmpresa"] = trib;

            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridView1Partial(string slug, string culture)
        {
            ViewData["CodCampaña"] = Session["c"];
            var empresa = Empresa.GetEmpresa(slug, _ruc);
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

            return PartialView("_GridView1Partial", model);
        }

        [ValidateInput(false)]
        public ActionResult GridView2Partial(string slug, string culture)
        {
            ViewData["CodCampaña"] = Session["c"];
            var empresa = Empresa.GetEmpresa(slug, _ruc);
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

            return PartialView("_GridView2Partial", model);
        }

        [ValidateInput(false)]
        public ActionResult GridView3Partial(string slug, string culture)
        {
            ViewData["CodCampaña"] = Session["c"];
            var empresa = Empresa.GetEmpresa(slug, _ruc);
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
            return PartialView("_GridView3Partial", model);
        }

        public ActionResult ModalPartial(string slug, string culture)
        {
            ViewData["CodCampaña"] = Session["c"];
            var empresa = Empresa.GetEmpresa(slug, _ruc);
            var model = Detalle.GetDataPartida(empresa.IdEmpresa);
            var detalle = Detalle.GetFirstDetalle(empresa.IdEmpresa);
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);
            var tipo = hasImport > hasExport ? "importaciones" : "exportaciones";

            ViewData["partida"] = detalle.Nandina;
            ViewData["descripcion"] = detalle.PartidaDesc;
            ViewData["tipo"] = tipo;
            ViewData["empresa"] = empresa.Nombre;

            return PartialView("Modals/Modal_Partida", model);
        }

        public ActionResult ModalEmbarque(string slug, string culture)
        {
            ViewData["CodCampaña"] = Session["c"];
            var empresa = Empresa.GetEmpresa(slug, _ruc);
            var model = Detalle.GetDataEmbarque(empresa.IdEmpresa);
            var hasImport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa);
            var hasExport = Resumen.TotalTipoEmbarques(empresa.IdEmpresa, 1);
            var tipo = hasImport > hasExport ? "importaciones" : "exportaciones";

            ViewData["tipo"] = tipo;

            return PartialView("Modals/Modal_Embarque", model);
        }

        [HttpPost]
        public JsonResult BuscarEmpresa(string nandina, string codPais = "", string opcion = "")
        {
            var json = Empresa.SearchEmpresa(nandina);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 
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

            if (culture == "es")
            {
                return RedirectToRoute("Minisite", new { culture, slug, pais, ruc, trib });
            }
            else
            {
                return RedirectToRoute("MinisiteUS", new { culture, slug, pais, ruc, trib });
            }

            //return RedirectToAction("Index", "Minisite", new { culture, slug, pais, ruc, trib });
        }
    }
}