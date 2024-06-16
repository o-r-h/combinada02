using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers
{
    public class HomeController : BaseController
    {
        private void GetAddressIP(ref string codPaisIp)
        {   
            var ws = new VeritradeAdmin.Seguridad();
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
                var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            // Ruben 202206
            //codPaisIp = "ES";
        }

        // Ruben 202307
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Index(string culture, string url)//, string codUsuario, string password)
        {
            ////var arr = new string[] {"14100", "14100I", "20100", "20100I", "51001", "51001I"};
            //if (Session["IdUsuario"] != null)
            //{
            //    if (Functions.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
            //    {
            //        return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
            //    }
            //}

            var codPaisIp = "";
            string idiomaT = "", codPaisIpt = "";
            if (Session["Idioma"] != null)
            {
                idiomaT = Session["Idioma"].ToString();
                codPaisIp = codPaisIpt = Session["CodPaisIP"].ToString();
            }

            if (idiomaT != "")
            {
                Session["Idioma"] = idiomaT;
                Session["CodPaisIP"] = codPaisIpt;
            }
            else
            {   
                GetAddressIP(ref codPaisIp);

                var drPlanesPrecios = FuncionesBusiness.ObtienePlanesPrecios(codPaisIp);
                var idioma = drPlanesPrecios["IdiomaDefecto"].ToString();

                // Ruben 202206
                if (codPaisIp == "ES")
                    idioma = "es";
                
                Session["Idioma"] = idioma;
                Session["CodPaisIP"] = codPaisIp;
                
                var currentCulture = CultureHelper.GetCurrentCulture();

                // Ruben 202206
                //Functions.GrabaLog("0", "", "", "0", "0", "currentCulture", currentCulture);

                //Ruben 202305
                //return RedirectToRoute("RootCulture", new { culture = idioma });
                //return RedirectToRoute(new { controller = "Home", action = "Index", culture = idioma });

                // Ruben 202305
                if (!idioma.Equals(currentCulture))
                {
                    return RedirectToRoute(new { controller = "Home", action = "Index", culture = idioma });
                    //return RedirectToAction("Index", "Home", new { culture = idioma });
                }
            }

            var postLanguage = new PostLanguage();
            var paises = new Paises();
            var paisesContent = paises.GeListHome(culture);
            var ayuda = new Ayuda();
            var videos = new AyudaVideo().GetVideos(ayuda.GetAyuda(culture).Id);

                       
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["servicios"] = new Servicios().GetList(culture);

            ViewData["paises"] = paises.GeList(culture, true);
            ViewData["paises_list"] = paisesContent.GetRange(0, 9);
            ViewData["paises_popup"] = paisesContent;
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            ViewData["post_language"] = postLanguage.HomePostLanguages(culture, 2);
            ViewData["videos"] = videos;
            ViewData["codPaisIp"] = codPaisIp;
            ViewData["codUsuario"] = TempData["codUsuario"];
            ViewData["password"] = TempData["password"];
            ViewData["tokenUsado"] = TempData["tokenUsado"];

            if (Session["CodPais"] == null) Session["CodPais"] = "PE";

            ViewData["url"] = url;

            // Ruben 202308
            if (culture == "es")
            {
                ViewBag.Title = "Bases de datos de comercio exterior sobre importaciones y exportaciones mundiales";
                ViewBag.Description = "Accede a las mejores bases de datos de comercio exterior y obtén información sobre importaciones y exportaciones en todo el mundo. ¡Impulsa tu negocio internacional!";
            }
            else
            {
                ViewBag.Title = "World trade databases on global imports and exports";
                ViewBag.Description = "Access the best international trade databases and obtain information about imports and exports worldwide. Boost your international business!";
            }

            // Ruben 202308
            string Canonical = Request.Url.AbsoluteUri;
            ViewBag.Canonical = Canonical;

            // Ruben 202402
            if (Canonical != "https://www.veritradecorp.com/")
            {
                ViewBag.HreflangEs = "https://www.veritradecorp.com/es/";
                ViewBag.HreflangEn = "https://www.veritradecorp.com/en/";
            }

            /*
            if (Canonical != "https://www.veritradecorp.com/" && Canonical != "https://www.veritradecorp.com/es/" && Canonical != "https://www.veritradecorp.com/en/")
            {
                ViewBag.HreflangEs = "https://www.veritradecorp.com/es/";
                ViewBag.HreflangEn = "https://www.veritradecorp.com/en/";
            }
            */

            /*
            if (Request.Url.AbsoluteUri == "https://www.veritradecorp.com")
                ViewBag.Canonical = "https://www.veritradecorp.com/";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/" + culture + "/";
            */

            // Ruben 202303
            /*
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/";
            */

            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(string culture)
        {
            var clear = Request.Form["session"] ?? "none";
            if (clear == "end")
            {
                Session.RemoveAll();
            }

            var postLanguage = new PostLanguage();
            var paises = new Paises();
            var paisesContent = paises.GeListHome(culture);
            var ayuda = new Ayuda();
            var videos = new AyudaVideo().GetVideos(ayuda.GetAyuda(culture).Id);

            //ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";
            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null) c = Session["c"].ToString();

            //if (c != "") Session["c"] = c;
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["servicios"] = new Servicios().GetList(culture);

            ViewData["paises"] = paises.GeList(culture, true);
            ViewData["paises_list"] = paisesContent.GetRange(0, 9);
            ViewData["paises_popup"] = paisesContent;
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            ViewData["videos"] = videos;
            ViewData["post_language"] = postLanguage.HomePostLanguages(culture, 2);
            
            return View();
        }

        [HttpGet]
        public ActionResult Redirect()
        {
            return RedirectToRoute("RootCulture", new { culture = "es" }); // Ruben 202304
            //return RedirectToRoute(new { controller = "Home", action = "Index", culture = "es" }); // Ruben 202304
            //return RedirectToAction("Index", "Home", new { culture = "es" });
            //return RedirectPermanent("/Home/Index");
        }

        [HttpPost, ActionName("Redirect")]
        public ActionResult RedirectPost()
        {
            return RedirectToRoute("RootCulture", new { culture = "es" }); // Ruben 202304
            //return RedirectToRoute(new { controller = "Home", action = "Index", culture = "es" }); // Ruben 202304
            //return RedirectToAction("Index", "Home", new { culture = "es" });
            //return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Contactanos(Solicitud model)
        {
            if (ModelState.IsValid)
            {
                var codSolicitud = model.CodSolicitud; // Ruben 202306
                //var codSolicitud = "CONTACTENOS";
                var nombres = model.NombreCompleto;
                var empresa = model.Empresa;
                var telefono = model.Telefono;
                var email1 = model.Email;
                var mensaje = model.Mensaje;
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
                var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                Solicitud.EnviaSolicitud(codSolicitud, nombres, empresa, telefono, email1, mensaje, direccionIp);

                return Json(new
                {
                    EnableSuccess = true,
                    SuccessTitle = Resources.Resources.Request_Title,
                    SuccessMsg = Resources.Resources.Request_Message
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                EnableError = true,
                ErrorTitle = Resources.Resources.Request_Error_Title,
                ErrorMsg = Resources.Resources.Request_Error_Message
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SetLogin(string culture, string token)
        {
            if (token != null)
            {
                using (var client = new HttpClient())
                {
                    TempData["tokenUsado"] = true;
                    client.BaseAddress = new Uri(Properties.Settings.Default.UrlWeb);
                    //HTTP GET
                    var responseTask = client.GetAsync("/api/SesionUsuario/ValidarToken?token="+token);
                    responseTask.Wait();
                    var response = responseTask.Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        string codUsuario = data.Replace("\\", "").Replace("/", "").Replace("\"", "");
                        string password = FuncionesBusiness.SearchPassword(codUsuario);
                        TempData["codUsuario"] = codUsuario;
                        TempData["password"] = password;

                        // Ruben 202203
                        string sql = "update Usuario set CodEstado = 'A', ";
                        sql = sql + "FecInicio = " + DateTime.Today.ToString("yyyyMMdd") + ", FecFin = " + DateTime.Today.AddDays(5.0).ToString("yyyyMMdd") + ", FecFinAct = " + DateTime.Today.AddDays(5.0).ToString("yyyyMMdd") + " ";
                        sql = sql + "where CodUsuario = '" + codUsuario + "' ";
                        Conexion.SqlExecute(sql);

                        //return RedirectToRoute("Default", new { culture });
                        return RedirectToRoute("RootCulture", new { culture });
                    }
                }
            }
            //return RedirectToRoute("Default", new { culture});
            return RedirectToRoute("RootCulture", new { culture }); // 202212
        }
          
        public XmlSitemapResult Xml()
        {
            var content = new SortedList<string, List<ISitemapItem>>();
            var items = new List<ISitemapItem>();

            // Data Rutas Generales
            const string sql = "SELECT * FROM configuraciones_seo";
            var dt = Conexion.SqlDataTable(sql, true);

            fn_GenerarURLS(dt, ref items);
            content.Add("sitemap", items);
            
            // Rutas Minisite
            const string sql2 =
                "SELECT COUNT(*) AS Total FROM Empresa where ltrim(rtrim(Uri)) != '' and ltrim(rtrim(Uri)) != 'na'";
            var dt2 = Conexion.SqlDataTableMinisite(sql2);
            var total = Convert.ToInt64(dt2.Rows[0]["Total"]);

            const int limit = 10000;
            long offset = 0;
            int count = 1;

            for (var i = 0; i <= (total / limit) + 1; i++)
            {
                items = new List<ISitemapItem>();

                var sql3 =
                    "SELECT RUC, LOWER(PaisEmpresa) as PaisEmpresa, ltrim(rtrim(Uri)) as Uri, Registrotrib FROM Empresa " +
                    "INNER JOIN Configuracion ON Empresa.PaisEmpresa = Configuracion.Pais " +
                    "WHERE ltrim(rtrim(Uri)) != '' and ltrim(rtrim(Uri)) != 'na' " +
                    "ORDER BY IdEmpresa OFFSET " + offset + " ROWS FETCH NEXT " + limit + " ROWS ONLY ";

                var dt3 = Conexion.SqlDataTableMinisite(sql3);
                fn_GenerarURLS_Minisite(dt3, ref items);

                content.Add("sitemap" + count, items);
                count++;
                offset = offset + limit;
            }

            // Rutas ProductProfile
            const string sql4 = "SELECT T.IdProducto,T.IdPaisAduana FROM TOTALES T INNER JOIN Producto P ON P.IdProducto = T.IdProducto " +
                "WHERE LTRIM(RTRIM(P.UriES)) != '' AND LTRIM(RTRIM(P.UriEN)) != '' AND LTRIM(RTRIM(P.UriES)) != 'na' AND LTRIM(RTRIM(P.UriEN)) != 'na' " +
                "GROUP BY T.IdProducto, T.IdPaisAduana";
            var dt4 = Conexion.SqlDataTableProductProfile(sql4);
            var totalProduct = dt4.Rows.Count;

            const int limitProduct = 10000;
            long offsetProduct = 0;

            for (var i = 0; i <= (totalProduct / limitProduct) + 1; i++)
            {
                items = new List<ISitemapItem>();

                var sql5 =
                    "SELECT PaisAduana = PA.PaisAduana, P.UriES, P.UriEN, P.CodProducto , T.IdProducto,T.IdPaisAduana FROM TOTALES T " +
                    "INNER JOIN Producto P ON P.IdProducto = T.IdProducto " +
                    "INNER JOIN PaisAduana PA ON PA.IdPaisAduana = T.IdPaisAduana " +
                    "WHERE LTRIM(RTRIM(P.UriES)) != '' AND LTRIM(RTRIM(P.UriEN)) != '' AND LTRIM(RTRIM(P.UriES)) != 'na' AND LTRIM(RTRIM(P.UriEN)) != 'na' " +
                    "GROUP BY T.IdProducto, T.IdPaisAduana, P.UriEN, P.UriES, P.CodProducto, PA.PaisAduana " +
                    "ORDER BY T.IdProducto OFFSET " + offsetProduct + " ROWS FETCH NEXT " + limitProduct + " ROWS ONLY";

                var dt5 = Conexion.SqlDataTableProductProfile(sql5);
                fn_GenerarURLS_ProductProfile(dt5, ref items);

                content.Add("sitemap" + count, items);
                count++;
                offsetProduct = offsetProduct + limitProduct;
            }

            return new XmlSitemapResult(content);
        }

        private static void fn_GenerarURLS(DataTable dt, ref List<ISitemapItem> items)
        {
            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var idiomaId = dt.Rows[i]["idioma_id"].ToString();
                var uri = dt.Rows[i]["uri"].ToString();

                string hreflang;
                string alternateUri;
                if (idiomaId == "1")
                {
                    hreflang = "en";
                    alternateUri = dt.Rows[i + 1]["uri"].ToString();
                }
                else
                {
                    hreflang = "es";
                    alternateUri = dt.Rows[i - 1]["uri"].ToString();
                }
                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + uri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
            }
        }  

        private static void fn_GenerarURLS_Minisite(DataTable dt, ref List<ISitemapItem> items)
        {
            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var pais = dt.Rows[i]["PaisEmpresa"].ToString().Replace(" ", "%20");
                var uri = dt.Rows[i]["Uri"].ToString();
                var ruc = dt.Rows[i]["RUC"].ToString();
                var trib = dt.Rows[i]["Registrotrib"].ToString();

                // Ruben 202211
                // https://www.veritradecorp.com/es/importaciones-exportaciones-peru/alicorp/ruc-20100055237
                
                //var route = "/importaciones-exportaciones-" + pais + "/" + uri + "/" + trib + "-" + ruc; // Ruben 202211
                //var routeAlt = "/imports-exports-" + pais + "/" + uri + "/" + trib + "-" + ruc;

                var route = "/" + pais + "/importaciones-y-exportaciones-" + uri + "/" + trib + "-" + ruc;
                var routeAlt = "/" + pais + "/imports-and-exports-" + uri + "/" + trib + "-" + ruc;
                
                var initlang = "es";
                var hreflang = "en";

                var initUri = "/" + initlang + route;
                var alternateUri = "/" + hreflang + routeAlt;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
                initlang = "en";
                hreflang = "es";
                initUri = "/" + initlang + routeAlt;
                alternateUri = "/" + hreflang + route;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
            }
        }

        private static void fn_GenerarURLS_ProductProfile(DataTable dt, ref List<ISitemapItem> items)
        {
            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var pais = dt.Rows[i]["PaisAduana"].ToString().Replace(" ", "%20").ToLower();
                var uriEs = dt.Rows[i]["UriEs"].ToString();
                var uriEn = dt.Rows[i]["UriEn"].ToString();
                var partida = dt.Rows[i]["CodProducto"].ToString();

                var route = "/" + pais + "/importaciones-y-exportaciones/" + uriEs + "/" + partida;
                var routeAlt = "/" + pais + "/imports-and-exports/" + uriEn + "/" + partida;
                var initlang = "es";
                var hreflang = "en";

                var initUri = "/" + initlang + route;
                var alternateUri = "/" + hreflang + routeAlt;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
                initlang = "en";
                hreflang = "es";
                initUri = "/" + initlang + routeAlt;
                alternateUri = "/" + hreflang + route;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
            }
        }

        //[HttpPost]
        //public ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    RouteData.Values["culture"] = culture; // set culture 

        //    //if (!string.IsNullOrEmpty(RouteData.Values["campania"] as string))
        //    //{
        //    //    var campania = "";
        //    //    Session["c"] = culture.Equals("es")
        //    //        ? Session["c"].ToString().Replace("I", "")
        //    //        : Session["c"].ToString().Replace("I", "") + "I";
        //    //    campania = Session["c"].ToString();

        //    //    return RedirectToRoute("HomeCampania", new { culture, campania });
        //    //}
        //    return RedirectToAction("Index");
        //}
    }
}