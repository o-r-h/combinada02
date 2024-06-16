using System;
using System.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Controllers.Api.Usuarios;
using System.Net.Http;
using System.Net;
using System.IO;

namespace Veritrade2017.Controllers
{
    public class CommonController : Controller
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly VeritradeServicios.ServiciosCorreo _wsc = new VeritradeServicios.ServiciosCorreo();
        private readonly string _path = SettingUtility.GetUrlBackOld();

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetPais(string pais)
        {
            var listado = new ListaPaises().Listado();
            return Json(new { listado }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsValidCountrie(string pais)
        {
            if (new ListaPaises().IsValid(pais))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json("El valor seleccionado no es válido", JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsValidPlan(int chargeId)
        {
            string codPaisIp = "";
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            string planPrecio = string.Empty;
            planPrecio = Planes.GetPlanPrecio(codPaisIp);

            if (Planes.PlanExist(chargeId, codPaisIp, planPrecio))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json("El valor seleccionado no es válido", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string txtCodUsuario, string txtPassword)
        {
            const string inicio = "ok";
            var respuesta = Validacion(txtCodUsuario, txtPassword, inicio);
            if (respuesta == "OK")
            {
                Console.WriteLine(">> logged: " + txtCodUsuario );
                string idUsuario = Usuario.GetIdUsuario(txtCodUsuario);
                bool sesionActiva = true;
                Session["IdUsuario"] = idUsuario;
                ActualizarSesionUsuario(int.Parse(idUsuario), sesionActiva);
            }

            return Json(new { respuesta }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login2(string txtCodUsuario2, string txtPassword2)
        {
            const string inicio = "ok";
            var respuesta = Validacion(txtCodUsuario2, txtPassword2, inicio);

            if (respuesta == "OK")
            {
                Console.WriteLine(">> logged: " + txtCodUsuario2);
                Session["IdUsuario"] = Usuario.GetIdUsuario(txtCodUsuario2);
            }
            return Json(new { respuesta }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            if (Session["IdUsuario"] != null)
            {
                int idUsuario = int.Parse(Session["IdUsuario"].ToString());
                bool sesionActiva = false;
                ActualizarSesionUsuario(idUsuario, sesionActiva);
            }

            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            var url = Request.Url.AbsoluteUri;
            int pos = url.IndexOf("?");
            if (pos != -1)
                url = url.Substring(pos, url.Length - pos);
            else
                url = string.Empty;

            Session.Abandon();
            Session.Clear();
            Session["IdUsuario"] = null;
            //return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/Error2.aspx");
            Extensiones.SetCookie("Mb", "0", TimeSpan.FromDays(-1));
            Extensiones.SetCookie("VLU", "0", TimeSpan.FromDays(-1)); //JANAQ 140720 Quitar Marca login Universidad
            return RedirectToAction("Index", "Home", new { culture, url } );
        }

        public string Validacion(string codUsuario, string password, string inicio)
        {
            string Respuesta = "OK";

            string IdUsuario = "", IdAplicacion = "", CodSeguridad = "";
            int LimiteVisitas = 0, Visitas = 0;
            int CantUsuariosUnicos = 0;

#if DEBUG
            var DireccionIP = Properties.Settings.Default.IP_Debug;
#else
            var DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif
            try
            {
                string CodEstado = BuscaCodEstado(codUsuario);
                if (CodEstado == "I")
                {
                    Respuesta = "INACTIVO";
                    EnviaCorreoUsuario(Respuesta, codUsuario, DireccionIP);
                    return Respuesta;
                }

                bool Valida = _ws.Valida2(codUsuario, password, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
                    ref CantUsuariosUnicos);
                if (!Valida)
                {
                    Respuesta = "INCORRECTO";
                    return Respuesta;
                }

                // Ruben 202207
                string UrlVeritradeAdmin = "http://admin2018.veritrade.info/webforms/usuarioonline?u=" + codUsuario;
                WebClient Client = new WebClient();
                string Content = Client.DownloadString(UrlVeritradeAdmin);

                string Origen = Functions3.ObtieneOrigen(IdUsuario);
                if (codUsuario == "VRT-UNIV") inicio = "referid0";

                //string Aplicacion = "";
                //if (IdAplicacion == "1") Aplicacion = "Business"; else Aplicacion = "Express"; 
                //Session["Aplicacion"] = Aplicacion;

                // Ruben 202210
                if (Valida && (codUsuario == "UPC" || Origen == "ULIMA" || Origen == "ESAN" || Origen == "CERTUS" || Origen == "SISE" || Origen == "UCV") && inicio != "referid0")
                {
                    Respuesta = "INCORRECTO";
                    return Respuesta;
                }
                //int limite_visitas_free_trial = 25; //Convert.ToInt32(ConfigurationManager.AppSettings["limite_visitas_free_trial"]);
                //string TipoUsuario = Functions.BuscaTipoUsuario(IdUsuario);
                //int Contador = Functions.BuscaContador(IdUsuario);

                //Session["Plan"] = Functions.ObtienePlan(IdUsuario);

                /*
                if (Functions.ExisteUsuarioEnLinea(IdUsuario))
                {
                    Session["MensajeError"] = "EXISTE_USUARIO_EN_LINEA";
                    Session["CodUsuarioTemp"] = Request.Form["txtCodUsuario"];
                    Response.Redirect("Error.aspx");
                }
                else 
                */
                /*
                if (TipoUsuario == "Free Trial" && Contador >= limite_visitas_free_trial)
                {
                    Respuesta = "Período de Prueba finalizado.";
                    //Session["MensajeError"] = "LIMITE";
                    //Response.Redirect("Error.aspx");
                }
                else
                */
                if (!Functions3.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
                {
                    //Session["CodUsuarioTemp"] = Request.Form["txtCodUsuario"].ToUpper();
                    Session["LimiteVisitas"] = LimiteVisitas.ToString();
                    Respuesta = Resources.Resources.rpta4;
                    //Session["MensajeError"] = "LIMITE_VISITAS";
                    //Response.Redirect("Error.aspx");
                }
                /*
                else if (Session["Plan"].ToString() == "Express" && Functions.ExisteUsuarioEnLinea(IdUsuario))
                {
                    Respuesta = "SESIONABIERTA";
                    //Session["CodUsuarioTemp"] = Request.Form["txtCodUsuario"].ToUpper();
                    //Session["MensajeError"] = "EXISTE_USUARIO_EN_LINEA";
                    //Response.Redirect("Error.aspx");
                }
                */
                else if (CodSeguridad == "Off")
                {
                    //Session["IdUsuario"] = IdUsuario;
                    //ws.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");
                }
                /*
                else if (CodSeguridad == "Aviso")
                {
                    Session["IdUsuario"] = IdUsuario;
                    FuncionesSeguridad.GrabaHistorial(IdUsuario, Request.ServerVariables["REMOTE_ADDR"], Request.ServerVariables["HTTP_USER_AGENT"], "");
                    mpupexAviso3.Show();
                }
                */
                else if (CodSeguridad == "IP")
                {
                    if (DireccionIP == Properties.Settings.Default.IP_Veritrade || _ws.ValidaIPPais(IdUsuario, DireccionIP))
                    {
                        //Session["IdUsuario"] = IdUsuario;
                        //ws.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");
                    }
                    else
                    {
                        _ws.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            "E1"); // Otro Pais
                        Respuesta = "OTROPAIS";
                        EnviaCorreoUsuario(Respuesta, codUsuario, DireccionIP);
                        //Session["MensajeError"] = "OTROPAIS";
                        //Response.Redirect("ErrorLogin.aspx");
                    }
                }
                else if (CodSeguridad == "Programa")
                {
                    string ProcessorID = Request.Form["hdfProcessorID"];
                    string MACAddress = Request.Form["hdfMACAddress"];
                    string ComputerName = Request.Form["hdfComputerName"];
                    string UserName = Request.Form["hdfUserName"];

                    string VersionLogin;
                    if (inicio == "Programa") VersionLogin = "VL1";
                    else if (inicio == "Programa2") VersionLogin = "VL2";
                    else if (inicio == "Programa3") VersionLogin = "VL3";
                    else VersionLogin = "Web";

                    if ((inicio == "Programa" || inicio == "Programa2" || inicio == "Programa3") && _ws.ValidaSeguridad(
                            IdUsuario, CantUsuariosUnicos, ProcessorID, MACAddress, ComputerName, UserName, VersionLogin))
                    {
                        _ws.GrabaHistorial3(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            ProcessorID, MACAddress, ComputerName, UserName, VersionLogin, "");
                        Session["IdUsuario"] = IdUsuario;
                    }
                    else if (inicio == "ok")
                    {
                        _ws.GrabaHistorial3(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            ProcessorID, MACAddress, ComputerName, UserName, VersionLogin, "E2"); // Sin Programa
                        Respuesta = Resources.Resources.rpta5;
                        //Session["MensajeError"] = "SINPROGRAMA";
                        //Response.Redirect("ErrorLogin.aspx");
                    }
                    else
                    {
                        _ws.GrabaHistorial3(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            ProcessorID, MACAddress, ComputerName, UserName, VersionLogin, "E3"); // Otra PC
                        Respuesta = Resources.Resources.rpta6;
                        //Session["MensajeError"] = "OTRAPC";
                        //Response.Redirect("ErrorLogin.aspx");
                    }
                }

                //Functions.IncrementaContador(IdUsuario);
                /*
                else if (Aplicacion == "Express" && Valida)
                {
                    if (Functions.ExisteUsuarioEnLinea(IdUsuario))
                    {
                        Session["CodUsuarioTemp"] = Request.Form["txtCodUsuario"].ToUpper();
                        Session["MensajeError"] = "EXISTE_USUARIO_EN_LINEA";                        
                        Response.Redirect("Error.aspx");
                    }
                    else if (!Functions.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
                    {
                        Session["CodUsuarioTemp"] = Request.Form["txtCodUsuario"].ToUpper();
                        Session["LimiteVisitas"] = LimiteVisitas.ToString();
                        Session["MensajeError"] = "LIMITE_VISITAS";
                        Response.Redirect("Error.aspx");
                    }
                    else
                    {
                        Session["IdUsuario"] = IdUsuario;
                        string CodPais = "", TipoOpe = "";
                        Functions.BuscaDatosExpress(IdUsuario, ref CodPais, ref TipoOpe);
                        Session["CodPais"] = CodPais;
                        Session["TipoOpe"] = TipoOpe;
                        Session["lang"] = "es";

                        ws.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");
                        Response.Redirect("MyVeritrade.aspx");
                    }
                }

                else
                {
                    Respuesta = "INCORRECTO";
                    //Session["MensajeError"] = "Se ha ingresado el Usuario y/o Contraseña incorrectos ó su Cuenta no está activa";
                    //Session["MensajeError"] = "You have introduced an invalid user or password or your account is inactive";
                    //Response.Redirect("Error.aspx");
                }
                */

                var datosCliente = new Dictionary<string, string>
                {
                    { "codUsuario", codUsuario },
                    { "password", password },
                    { "inicio", inicio },
                    { "IdUsuario", IdUsuario },
                    { "IdAplicacion", IdAplicacion },
                    { "CodSeguridad", CodSeguridad },
                    { "LimiteVisitas", LimiteVisitas.ToString() },
                    { "Visitas", Visitas.ToString() },
                    { "DireccionIP", DireccionIP },
                    { "Respuesta", Respuesta }
                };
                ExceptionUtility.LogSignUp(datosCliente);

                return Respuesta;
            }
            catch (Exception ex)
            {
                var datosCliente = new Dictionary<string, string>
                {
                    { "codUsuario", codUsuario },
                    { "password", password },
                    { "inicio", inicio },
                    { "IdUsuario", IdUsuario },
                    { "IdAplicacion", IdAplicacion },
                    { "CodSeguridad", CodSeguridad },
                    { "LimiteVisitas", LimiteVisitas.ToString() },
                    { "Visitas", Visitas.ToString() },
                    { "DireccionIP", DireccionIP },
                    { "Respuesta", Respuesta }
                };

                string errorHandler = Request.QueryString["handler"];
                ExceptionUtility.LogException(ex, errorHandler, datosCliente);

                return "ERROR";
            }
        }

        public string BuscaCodEstado(string codUsuario)
        {
            var codEstado = "";

            var sql = "select CodEstado from Usuario where CodUsuario = '" + codUsuario + "' and IdAplicacion = 1";
            var dt = Conexion.SqlDataTable(sql);

            foreach (DataRow row in dt.Rows)
            {
                codEstado = row["CodEstado"].ToString();
            }

            return codEstado;
        }

        void EnviaCorreoUsuario(string Opcion, string CodUsuario, string DireccionIP)
        {
            DataRow dr = Funciones.ObtieneUsuarioCorreo(CodUsuario);
            string Tipo = "";
            if (dr["IdTipo"].ToString() == "8") Tipo = "[PRUEBA GRATIS] ";
            else if (dr["IdTipo"].ToString() == "10") Tipo = "[CLIENTE] ";
            else if (dr["IdTipo"].ToString() == "49") Tipo = "[GRATIS] ";
            else if (dr["IdTipo"].ToString() == "90") Tipo = "[CONVENIO] ";

            string Origen = dr["Origen"].ToString();

            string EmailEnvio1 = "", EmailEnvioNombre1 = "", EmailEnvioPassword1 = "";
            Funciones.BuscaDatosCorreoEnvio("4", ref EmailEnvio1, ref EmailEnvioNombre1, ref EmailEnvioPassword1);

            string FromName, FromEmail, ToName, ToEmail, Subject, URL;

            FromName = EmailEnvioNombre1; FromEmail = EmailEnvio1;
            string CcName = "Info Veritrade"; 
            string CcEmail = "info@veritrade-ltd.com";
            if (Opcion == "INACTIVO")
                Subject = "Intento ingreso usuario inactivo";
            else
                Subject = "Usuario ingreso restringido IP";

            Subject = "[" + CodUsuario + "] " + Tipo + Subject;

            URL = _path + "/Correos.aspx?Opcion=" + Opcion + "&CodUsuario=" + CodUsuario + "&DireccionIP=" + DireccionIP;
            //_wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, null, null, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);

            bool correoEnviado = false;

            // Ruben 202208
            if (Tipo != "[CLIENTE] ")
            {
                ToEmail = Funciones.BuscaCorreoAdmin(Origen);
                if (ToEmail != "")
                {
                    ToName = "Ejecutivo Veritrade";
                    _wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, CcName, CcEmail, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);
                    correoEnviado = true;
                }

                /*
                if (Tipo == "[CLIENTE] " && Origen.ToLower() == "veritrade")
                {
                    //Ruben 2017-07-17
                    ToName = "Cynthia Medina";
                    ToEmail = Funciones.BuscaCorreoAdmin("cmedina");
                    _wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, CcName, CcEmail, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);
                    correoEnviado = true;
                }
                */
            }
            else
            {
                string sql = "select E.Ejecutivo, E.UsuarioCorreo from Usuario UL, VeritradeAdmin.dbo.Usuario U, VeritradeAdmin.dbo.Cliente C, VeritradeAdmin.dbo.Ejecutivo E ";
                sql += "where UL.CodUsuario COLLATE DATABASE_DEFAULT = U.UsuarioCorreo COLLATE DATABASE_DEFAULT and U.IdCliente = C.IdCliente and C.IdEjecutivoA = E.IdEjecutivo and UL.CodUsuario = '" + CodUsuario + "'";

                DataTable dt = Conexion.SqlDataTable(sql);

                ToName = dt.Rows[0]["Ejecutivo"].ToString();
                ToEmail = dt.Rows[0]["UsuarioCorreo"].ToString();

                _wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, CcName, CcEmail, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);

                sql = "select Ejecutivo, UsuarioCorreo from VeritradeAdmin.dbo.Ejecutivo ";
                sql += "where CodTipoEjecutivo = 'JRA' ";

                DataTable dt2 = Conexion.SqlDataTable(sql);

                ToName = dt2.Rows[0]["Ejecutivo"].ToString();
                ToEmail = dt2.Rows[0]["UsuarioCorreo"].ToString();

                _wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, CcName, CcEmail, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);

                correoEnviado = true;
            }

            if (!correoEnviado)
            {
                ToName = CcName;
                ToEmail = CcEmail;
                _wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, null, null, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);
            }

            //ToName = "Vanessa Soria";
            //ToEmail = Funciones.BuscaCorreoAdmin("vsoria");
            //_wsc.EnviaEmail3(FromName, FromEmail, ToName, ToEmail, null, null, null, null, Subject, URL, EmailEnvio1, EmailEnvioPassword1);
        }

    }
}