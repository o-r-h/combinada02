using System;
using System.Threading;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Models;
using Veritrade2017.Helpers;
using System.Security.Cryptography;
using System.Web;
using System.Globalization;
using Veritrade2017.Models.Admin;
using System.Data;
using System.Net.Http;

namespace Veritrade2017.Controllers
{
    public class PruebaGratisController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly VeritradeServicios.ServiciosCorreo _wsc = new VeritradeServicios.ServiciosCorreo();
        private const string PRODUCTO = "PROD";
        private const string EMPRESA = "EMP";

        private readonly string _path = SettingUtility.GetUrlBackOld();
        //comentario de prueba
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
            //}

            var codPaisIp = "";
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            //ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "12100I";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            
            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (c == "") c = "12100";
            //Session["c"] = c;

            ////if (Session["c"] != null && c != "") c = Session["c"].ToString();
            
            ////if (Session["c"] == null) Session["c"] = c;

            //var campania = Session["c"].ToString();

            //ViewData["CodCampaña"] = campania;

            //ViewData["cod_campaña"] = campania;
            ViewData["ListaPaises"] = new ListaPaises().Listado(culture);
            ViewData["paisIP"] = codPaisIp;

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/pruebagratis";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/freetrial";

            return View("Index");
        }

        [HttpPost]
        public ActionResult Preview(string culture)
        {
            if (Request.Form["txtCodUsuario"] == null) return RedirectToAction("Index");

            ViewData["CodUsuario"] = Request.Form["txtCodUsuario"];
            ViewData["Password"] = Request.Form["txtPassword"];
            ViewData["Script"] = "<script language='javascript'> document.forms[0].submit(); </script>";

            int idUsuario = int.Parse(Session["IdUsuario"].ToString());
            bool sesionActiva = true;
            ActualizarSesionUsuario(idUsuario, sesionActiva);

            return View("Preview");
        }

        public void ActualizarSesionUsuario(int idUsuario, bool sesionActiva)
        {
            string baseurl = Properties.Settings.Default.UrlWeb;
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseurl)
            };

            int sesionUsuario = SesionUsuario.ObtenerSesionActivaPorUsuario(idUsuario);

            string urlApiSesion = $"api/SesionUsuario/?idUsuario={idUsuario}&sesionActiva={sesionActiva}";

            if (sesionUsuario > 0)
            {
                client.PutAsync(urlApiSesion, null);
            }
            else
            {
                client.PostAsync(urlApiSesion, null);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AjaxForm(FreeTrial model, string culture)
        {
            if (ModelState.IsValid)
            {
                var email = model.FtEmail;
                //var nombres = model.FtNombreCompleto;
                //var nombres = String.Concat(model.FtNombres, " ", model.FtApellidos);
                var nombres = model.FtNombres;
                var apellidos = model.FtApellidos;
                var empresa = model.FtEmpresa;
                var telefono = model.FtTelefono;
                var pais = model.FtPais?.ToUpper();

                var consultaGratis = "OK";
                var codCampaña = model.FtCodCampania;

                var url = Request.RawUrl;
                var referido = "";
                if (Request.UrlReferrer != null)
                {
                    referido = Request.UrlReferrer.AbsoluteUri;
                }
                
                var respuesta = CreaUsuarioFreeTrial(email, nombres, apellidos,empresa, telefono, consultaGratis, codCampaña,
                    culture, url, referido, pais);
                Session["IdUsuario"] = Usuario.GetIdUsuario(email);

                return Json(new
                {
                    respuesta,
                    email
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                EnableError = true,
                ErrorTitle = "Error",
                ErrorMsg = "Something goes wrong, please try again later"
            });
        }

        public string CreaUsuarioFreeTrial(string email1, string nombres, string apellidos,string empresa, string telefono,
            string consultaGratis, string codCampaña, string idioma, string url, string referido, string pais = "")
        {
            string action = "Guardar";
            string idUsuario = string.Empty;
            if (FuncionesBusiness.ExisteCodUsuario(email1)) { 
                if (FuncionesBusiness.SearchIdTipoForCodUsusario(email1) == "8")
                {
                    action = "Actualizar";
                }
                else
                {
                    return "EXISTE";
                }
            }
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            var empPer = empresa != "" ? "EMP" : "PER";

            var codUsuario = email1.ToLower();

            var randomGenerator = RandomNumberGenerator.Create();
            byte[] data = new byte[6];
            randomGenerator.GetBytes(data);
            string password = BitConverter.ToString(data);

            password = password.Replace("-", "");

            long passInt = Convert.ToInt64(password, 16);

            password = passInt.ToString().Substring(0, 6);

            //if (!Functions.ExisteCodCampaña(codCampaña)) codCampaña = "0";

            if (action.Equals("Guardar"))
            {
                idUsuario = Functions.CrearUsuarioFreeTrial(empPer, codUsuario, password, nombres, apellidos, "", empresa, "", telefono, "",
                email1.ToLower(), "68", "", "67", direccionIp, codCampaña, url, referido, "", pais);
            }
            else
            {
                password = FuncionesBusiness.SearchPassword(codUsuario);

                idUsuario = Functions.ActualizarUsuarioFreeTrial(empPer, codUsuario, password, nombres, apellidos, "", empresa, "", telefono, "",
                 email1.ToLower(), "68", "", "67", direccionIp, codCampaña, url, referido, "", pais);
            }

            var campania = Session["c"];
            var rucEmpresa = Session["sRuc"];
            var slugEmpresa = Session["slugEmpresa"];
            var nombreEmpresa = Session["nombreEmpresa"];
            var codPartida = Session["CodPartida"];
            var nombreProducto = Session["DescProducto"];
            //var paisProducto = Session["Pais"];
            //var nombrePais = Session["paisEmpresa"];
            string tipo = "";

            var dr = Funciones.ObtieneUsuario(codUsuario);

            CorreoUsuarioPrueba usuarioPrueba = new CorreoUsuarioPrueba
            {
                codUsuario = codUsuario,
                pass = dr["Password"].ToString(),
                nombres = dr["Nombres"].ToString(),
                codCampania = dr["CodCampaña"].ToString(),
                empresa = dr["Empresa"].ToString(),
                telefono = dr["Telefono"].ToString(),
                codTelefono = dr["CodTelefono"].ToString(),
                direccionIp = dr["DireccionIp"].ToString(),
                ubicacion = dr["Ubicacion"].ToString()
            };

            var drCampania = Funciones.ObtieneCampaña(usuarioPrueba.codCampania);
            if (drCampania != null)
            {
                usuarioPrueba.publicidad = drCampania["Publicidad"].ToString();
                usuarioPrueba.ubicacionAnuncio = drCampania["UbicacionAnuncio"].ToString();
            }            

            DateTime fecRegistro = Convert.ToDateTime(dr["FecRegistro"]);
            usuarioPrueba.fecRegistro = fecRegistro.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);

            string nombrePais;

            //Asigna valores a modelo
            //Inserta relación de un producto o empresa con el código de usuario
            if (codPartida != null)
            {
                nombrePais = Session["Pais"].ToString();
                usuarioPrueba.tipoConsulta = PRODUCTO;
                usuarioPrueba.entidadConsultada = nombreProducto.ToString();
                usuarioPrueba.nombrePais = nombrePais[0].ToString().ToUpper() + nombrePais.Substring(1).ToLower();

                DataTable dtPartida =  FuncionesBusiness.BuscarPartidaPorIdProducto(codPartida.ToString());
                string descripcion = dtPartida.Rows[0].ItemArray[2].ToString();
                tipo = PRODUCTO;
                Funciones.CreaRelacionUsuarioPruebaGratis(codUsuario, tipo, usuarioPrueba.nombrePais, null, null, codPartida.ToString(), descripcion);
            }
            else if (rucEmpresa != null)
            {
                nombrePais = Session["paisEmpresa"].ToString();
                usuarioPrueba.tipoConsulta = EMPRESA;
                usuarioPrueba.entidadConsultada = rucEmpresa.ToString() + ": "+ nombreEmpresa.ToString();
                usuarioPrueba.nombrePais = nombrePais[0].ToString().ToUpper() + nombrePais.Substring(1).ToLower();

                tipo = EMPRESA;
                Funciones.CreaRelacionUsuarioPruebaGratis(codUsuario, tipo, usuarioPrueba.nombrePais, rucEmpresa.ToString(), slugEmpresa.ToString() , null, null);
            }

            string body = Funciones.RenderMailViewToString(this.ControllerContext, "Partials/Usuario_Prueba_Email", usuarioPrueba);
            //var thread = new Thread(EnviaCorreoFreeTrial);

            var parameters = new object[] { codUsuario, consultaGratis, idioma, codCampaña, codPartida, slugEmpresa, rucEmpresa, body };
            EnviaCorreoFreeTrial(parameters);
            //thread.Start(parameters);

            //JANAQ 160620 Creacion de usuario MixPanel
            FuncionesBusiness.CrearUsuarioMixPanel(idUsuario, Request.Url.Host);

            return password;
        }

        public void EnviaCorreoFreeTrial(object parameters)
        {
            var parameterArray = parameters as object[];
            if (parameterArray != null)
            {
                var codUsuario = (string)parameterArray[0];
                var consultaGratis = (string)parameterArray[1];
                if (consultaGratis != "") consultaGratis = "Consulta Gratis - ";
                var idioma = (string)parameterArray[2];
                var body = (string)parameterArray[7];

                var dr = Funciones.ObtieneUsuario(codUsuario);

                var nombres = dr["Nombres"].ToString();
                var empresa = dr["Empresa"].ToString();
                var email1 = dr["Email1"].ToString();
                var pais = dr["Pais"].ToString();

                string emailEnvio = "", emailEnvioNombre = "", emailEnvioPassword = "";
                Functions.BuscaDatosCorreoEnvio("4", ref emailEnvio, ref emailEnvioNombre, ref emailEnvioPassword);

                string subject;//, url;
                var fromName = emailEnvioNombre;
                var fromEmail = emailEnvio;
                var toName = nombres;
                var toEmail = email1;

                string bccName = "Info Veritrade";
                string bccEmail = "info@veritrade-ltd.com";

                if (idioma == "es")
                {
                    subject = "Veritrade - Bienvenido a la Prueba Gratis";
                }
                else
                {
                    subject = "Veritrade - Welcome to Free Trial";
                }
               
                var thread = new Thread(EmailUtils.EnviaEmail);
                toEmail = "info@veritrade-ltd.com";
                var tparameters = new object[] { fromName, fromEmail, toName, toEmail, null, null, null, null, subject, body,
                    emailEnvio, emailEnvioPassword };
                thread.Start(tparameters);

            }
        }

        //[HttpPost]
        //public override ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    RouteData.Values["culture"] = culture; // set culture 

        //    //var campania = "";
        //    //if (Session["c"] != null)
        //    //{
        //    //    Session["c"] = culture == "es" ? Session["c"].ToString().Replace("I","") 
        //    //        : Session["c"].ToString().Replace("I", "") + "I";
        //    //    campania = Session["c"].ToString();
        //    //}

        //    var campania = "";
        //    if (Session["c"] != null) campania = Session["c"].ToString();

        //    if (culture == "es")
        //    {
        //        return RedirectToRoute("PruebaGratis", new { campania });
        //    }
        //    else
        //    {
        //        return RedirectToRoute("PruebaGratisUS", new { campania });
        //    }

        //    //return RedirectToAction("Index", new { campania });
        //}
    }
}