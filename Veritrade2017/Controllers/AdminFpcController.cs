using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Models.Admin;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
    [FpcBasicAuthentication("admin", "admin")]
    public class AdminFpcController : Controller
    {
       
        public async Task<ActionResult> Index()
        {
            ViewData["nCantEmpresa"] = 0;//await Empresa.GetCountEmpresas();
            ViewData["nCantProducto"] = 0; //await MisProductos.GetCountProductos();
            ViewData["lstPaisesComp"] = await Empresa.GetListPais();
            ViewData["lstPaisesProd"] = await MisProductos.GetListPais();
            ViewData["nCantContenidoGeneral"] = await ConfiguracionesSeo.GetCountConfigSeo();
            return View(); 
        }

        #region COMPANY PROFILE
        [HttpPost]
        public async Task<JsonResult> GetCompanyByPos(int pos, string pais)
        {
            var obj = await Empresa.GetEmpresa(pos, pais);
            return Json(obj);
        }

        [HttpPost]
        public async Task<JsonResult> DoVisitaUrlCompanyEs(Empresa obj)
        {
            obj.AbsoluteUrlEs = Empresa.GetAbsoluteUrl(obj, Request.Url.GetLeftPart(UriPartial.Authority), "es");
            obj.UrlVisitadaEs = await UrlVisitada(obj.AbsoluteUrlEs);
            return Json(obj);
        }

        [HttpPost]
        public async Task<JsonResult> DoVisitaUrlCompanyEn(Empresa obj)
        {
            obj.AbsoluteUrlEn = Empresa.GetAbsoluteUrl(obj, Request.Url.GetLeftPart(UriPartial.Authority), "en");
            obj.UrlVisitadaEn = await UrlVisitada(obj.AbsoluteUrlEn);
            return Json(obj);
        }
        [HttpGet]
        public ActionResult RemoveCacheCompanyProfile()
        {
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems("Minisite");
            return RedirectToAction("Index");
        }
        #endregion

        #region PRODUCTO PROFILE
        [HttpPost]
        public async Task<JsonResult> GetProductByPos(int pos, int pais)
        {
            var obj = await MisProductos.GetProducto(pos, pais);
            return Json(obj);
        }

        [HttpPost]
        public async Task<JsonResult> DoVisitaUrlProductEs(Producto obj)
        {
            obj.AbsoluteUrlEs = MisProductos.GetAbsoluteUrl(obj, Request.Url.GetLeftPart(UriPartial.Authority), "es");
            obj.UrlVisitadaEs = await UrlVisitada(obj.AbsoluteUrlEs);
            return Json(obj);
        }

        [HttpPost]
        public async Task<JsonResult> DoVisitaUrlProductEn(Producto obj)
        {
            obj.AbsoluteUrlEn = MisProductos.GetAbsoluteUrl(obj, Request.Url.GetLeftPart(UriPartial.Authority), "en");
            obj.UrlVisitadaEn = await UrlVisitada(obj.AbsoluteUrlEn);
            return Json(obj);
        }
        [HttpGet]
        public ActionResult RemoveCacheProductProfile()
        {
            var cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems("ProductoPerfil");
            return RedirectToAction("Index");
        }
        #endregion

        #region CONTENIDO GENERAL
        [HttpGet]
        public ActionResult RemoveCacheContenidoGeneral()
        {
            Funciones.RemoveCacheOfContenidoGeneral();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> GeneraAndVisitaGC()
        {
            var lst = await ConfiguracionesSeo.GetListConfigSeo(Request.Url.GetLeftPart(UriPartial.Authority));
            return Json(lst);
        }

        #endregion

        [HttpPost]
        public async Task<JsonResult> UrlOk(string url)
        {
            return Json(new {ok = await this.UrlVisitada(url), url = url});
        }

        private async Task<bool> UrlVisitada(string url) {
            return await Extensiones.UrlIsReachable(url);
        }

    }
}