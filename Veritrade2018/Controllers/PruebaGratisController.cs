using System;
using System.Security.Cryptography;
using System.Threading;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class PruebaGratisController : BaseController
    {
        // GET: PruebaGratis
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly VeritradeServicios.ServiciosCorreo _wsc = new VeritradeServicios.ServiciosCorreo();

        private readonly string _path = SettingUtility.GetUrlBack();

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
            }

            var codPaisIp = "";
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            ViewData["CodCampaña"] = culture.Equals("es") ? "12100" : "13100";

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            var c = "";
            if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();

            if (Session["c"] != null && c != "") c = Session["c"].ToString();

            if (c == "") c = "12100";

            if (Session["c"] == null) Session["c"] = c;

            var campania = Session["c"].ToString();
            ViewData["cod_campaña"] = campania;
            ViewData["ListaPaises"] = new ListaPaises().Listado(culture);
            ViewData["paisIP"] = codPaisIp;

            return View("Index");
        }

        [HttpPost]
        public ActionResult Preview(string culture)
        {
            if (Request.Form["txtCodUsuario"] == null) return RedirectToAction("Index");

            ViewData["CodUsuario"] = Request.Form["txtCodUsuario"];
            ViewData["Password"] = Request.Form["txtPassword"];
            ViewData["Script"] = "<script language='javascript'> document.forms[0].submit(); </script>";

            return View("Preview");
        }

        [ValidateAntiForgeryToken]
        public ActionResult AjaxForm(FreeTrial model, string culture)
        {
            if (ModelState.IsValid)
            {
                var email = model.FtEmail;
                var nombres = model.FtNombreCompleto;
                var empresa = model.FtEmpresa;
                var telefono = model.FtTelefono;
                var pais = model.FtPais;

                var consultaGratis = "OK";
                var codCampaña = model.FtCodCampania;

                var url = Request.RawUrl;
                var referido = "";
                if (Request.UrlReferrer != null)
                {
                    referido = Request.UrlReferrer.AbsoluteUri;
                }

                var respuesta = CreaUsuarioFreeTrial(email, nombres, empresa, telefono, consultaGratis, codCampaña,
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

        public string CreaUsuarioFreeTrial(string email1, string nombres, string empresa, string telefono,
            string consultaGratis, string codCampaña, string idioma, string url, string referido, string pais = "")
        {
            if (_ws.ExisteCodUsuario(email1)) return "EXISTE";
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

            if (!Funciones.ExisteCodCampaña(codCampaña)) codCampaña = "0";

            Funciones.CrearUsuarioFreeTrial(empPer, codUsuario, password, nombres, "", "", empresa, "", telefono, "",
                email1.ToLower(), "68", "", "67", direccionIp, codCampaña, url, referido, "", pais);

            var thread = new Thread(EnviaCorreoFreeTrial);
            var parameters = new object[] { codUsuario, consultaGratis, idioma, codCampaña };
            thread.Start(parameters);

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

                var dr = Funciones.ObtieneUsuario(codUsuario);

                var nombres = dr["Nombres"].ToString();
                var empresa = dr["Empresa"].ToString();
                var email1 = dr["Email1"].ToString();
                var pais = dr["Pais"].ToString();

                string emailEnvio = "", emailEnvioNombre = "", emailEnvioPassword = "";
                Funciones.BuscaDatosCorreoEnvio("4", ref emailEnvio, ref emailEnvioNombre, ref emailEnvioPassword);

                string subject, url;
                var fromName = emailEnvioNombre;
                var fromEmail = emailEnvio;
                var toName = nombres;
                var toEmail = email1;

                string bccName = "Info Veritrade";
                string bccEmail = "info@veritrade-ltd.com";

                if (idioma == "es")
                {
                    subject = "Veritrade - Bienvenido a la Prueba Gratis";
                    url = _path + "/Correos.aspx?Opcion=BIENVENIDO&CodUsuario=" + codUsuario;
                }
                else
                {
                    subject = "Veritrade - Welcome to Free Trial";
                    url = _path + "/Correos.aspx?Opcion=BIENVENIDO_EN&CodUsuario=" + codUsuario;
                }

                _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, bccName, bccEmail, subject, url,
                    emailEnvio, emailEnvioPassword);


                var drCampania = Funciones.ObtieneCampaña((string)parameterArray[3]);

                var paisCampania = drCampania["Pais"].ToString();

                string paisComp = string.Empty;
                if (string.IsNullOrEmpty(paisCampania))
                {
                    paisComp = pais != string.Empty ? " - " + pais : "";
                }
                else
                {
                    paisComp = " - " + paisCampania;
                }
                if (empresa != "")
                    subject = "Prueba Gratis - Empresa - " + empresa + paisComp;
                else
                    subject = "Prueba Gratis - Persona - " + nombres + paisComp;
                url = _path + "/Correos.aspx?Opcion=REGISTRO&CodUsuario=" + codUsuario;

                toEmail = "info@veritrade-ltd.com";
                _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, null, null, subject, url, emailEnvio,
                    emailEnvioPassword);
            }
        }

        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            var campania = "";
            if (Session["c"] != null) campania = Session["c"].ToString();

            return RedirectToAction("Index", new { campania });
        }
    }
}