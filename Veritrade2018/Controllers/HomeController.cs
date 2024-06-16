using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string culture , string url)
        {
            if (Session["IdUsuario"] != null)
            {
                //if (Funciones.ObtieneOrigen(Session["IdUsuario"].ToString()).ToUpper() != "FREEMIUM")
                //{
                //    return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/MisBusquedas.aspx");
                //}
                return RedirectToAction("Index", "MisBusquedas");            
            }

            

            string idiomaT = "", codPaisIpt = "";
            if (Session["Idioma"] != null)
            {
                idiomaT = Session["Idioma"].ToString();
                codPaisIpt = Session["CodPaisIP"].ToString();
            }

            var c = "";
            if (Session["c"] != null) c = Session["c"].ToString();

            Session.RemoveAll();

            if (c != "") Session["c"] = c;

            if (idiomaT != "")
            {
                Session["Idioma"] = idiomaT;
                Session["CodPaisIP"] = codPaisIpt;
            }

            var codPaisIp = "";
            if (Session["Idioma"] == null)
            {
                var ws = new VeritradeAdmin.Seguridad();
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
                var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);
                var drPlanesPrecios = FuncionesBusiness.ObtienePlanesPrecios(codPaisIp);
                var idioma = drPlanesPrecios["IdiomaDefecto"].ToString();

                Session["CodPaisIP"] = codPaisIp;
                Session["Idioma"] = idioma;

                var currentCulture = CultureHelper.GetCurrentCulture();
                if (!idioma.Equals(currentCulture))
                {
                    return RedirectToAction("Index", "Home", new { culture = idioma });
                }
            }
            else
            {
                codPaisIp = Session["CodPaisIP"].ToString();
            }

            

            ViewData["url"] = url;
            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

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
            

            //COMENTADO
            //if (Session["CodPais"] == null) Session["CodPais"] = "PE";
            //AGREGADO 24-08
            if (Session["CodPais"] == null) Session["CodPais"] = codPaisIp;


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

            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Contactanos(Solicitud model)
        {
            if (ModelState.IsValid)
            {
                var codSolicitud = "CONTACTENOS";
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

        public XmlSitemapResult Xml()
        {
            var content = new SortedList<string, List<ISitemapItem>>();
            var items = new List<ISitemapItem>();

            // Data Rutas Generales
            const string sql = "SELECT * FROM configuraciones_seo";
            var dt = Conexion.SqlDataTable(sql, true);

            Fn_GenerarURLS(dt, ref items);
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
                Fn_GenerarURLS_Minisite(dt3, ref items);

                content.Add("sitemap" + count, items);
                count++;
                offset = offset + limit;
            }

            return new XmlSitemapResult(content);
        }

        private static void Fn_GenerarURLS(DataTable dt, ref List<ISitemapItem> items)
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

        private static void Fn_GenerarURLS_Minisite(DataTable dt, ref List<ISitemapItem> items)
        {
            for (var i = 0; i <= dt.Rows.Count - 1; i++)
            {
                var pais = dt.Rows[i]["PaisEmpresa"].ToString().Replace(" ", "%20");
                var uri = dt.Rows[i]["Uri"].ToString();
                var ruc = dt.Rows[i]["RUC"].ToString();
                var trib = dt.Rows[i]["Registrotrib"].ToString();

                var route = "/" + pais + "/importaciones-y-exportaciones-" + uri + "/" + trib + "-" + ruc;

                var initlang = "es";
                var hreflang = "en";

                var initUri = "/" + initlang + route;
                var alternateUri = "/" + hreflang + route;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });

                initlang = "en";
                hreflang = "es";
                initUri = "/" + initlang + route;
                alternateUri = "/" + hreflang + route;

                items.Add(
                    new SitemapItem(SettingUtility.GetUrlFront() + initUri, hreflang)
                    {
                        Href = SettingUtility.GetUrlFront() + alternateUri
                    });
            }
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