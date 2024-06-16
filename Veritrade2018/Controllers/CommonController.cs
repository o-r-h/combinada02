using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class CommonController : Controller
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly VeritradeServicios.ServiciosCorreo _wsc = new VeritradeServicios.ServiciosCorreo();
        private readonly string _path = SettingUtility.GetUrlBack();

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

            //if (respuesta == "OK")
            //{
            //    Session["IdUsuario"] = Usuario.GetIdUsuario(txtCodUsuario);
            //}
            //respuesta = "SESION_UNICA";
            return Json(new { respuesta }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login2(string txtCodUsuario2, string txtPassword2)
        {
            const string inicio = "ok";
            var respuesta = Validacion(txtCodUsuario2, txtPassword2, inicio);

            //if (respuesta == "OK")
            //{
            //    Session["IdUsuario"] = Usuario.GetIdUsuario(txtCodUsuario);
            //}
            //respuesta = "SESION_UNICA";
            return Json(new { respuesta }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
       {
            Dictionary<string, string> queryValues = (Dictionary<string, string>) Session["queryValues"];

            string url = (string)Session["url"]??"";
            Session.Clear();
            Session.Abandon();
            //OnlineUsers sessionActual;
            //ActiveSessions.Sessions.TryRemove(Session.SessionID, out sessionActual);
            Session["IdUsuario"] = null;

            return RedirectToAction("Index", "Home", new { url });
            //return Redirect(SettingUtility.GetUrlBack() + "/Veritrade/Error2.aspx");
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

                bool Valida = _ws. /*Funciones.*/ Valida2(codUsuario, password, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
                    ref CantUsuariosUnicos);
                if (!Valida)
                {
                    Respuesta = "INCORRECTO";
                    return Respuesta;
                }
                string Origen = Funciones.ObtieneOrigen(IdUsuario);
                if (codUsuario == "VRT-UNIV") inicio = "referid0";

                if (Valida && (codUsuario == "UPC" || Origen == "ULIMA" || Origen == "ESAN") && inicio != "referid0")
                {
                    Respuesta = "INCORRECTO";
                    return Respuesta;
                }
                if (!Funciones.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
                {
                    Session["LimiteVisitas"] = LimiteVisitas.ToString();
                    Respuesta = Resources.Resources.rpta4;
                }
                else if (CodSeguridad == "IP")
                {
                    if (DireccionIP == Properties.Settings.Default.IP_Veritrade || _ws.ValidaIPPais(IdUsuario, DireccionIP))
                    {
                    }
                    else
                    {
                        _ws.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "E1"); // Otro Pais
                        Respuesta = "OTROPAIS";
                        EnviaCorreoUsuario(Respuesta, codUsuario, DireccionIP);
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

                        //Session["IdUsuario"] = IdUsuario;
                    }
                    else if (inicio == "ok")
                    {
                        _ws.GrabaHistorial3(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            ProcessorID, MACAddress, ComputerName, UserName, VersionLogin, "E2"); // Sin Programa
                        Respuesta = Resources.Resources.rpta5;
                    }
                    else
                    {
                        _ws.GrabaHistorial3(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],
                            ProcessorID, MACAddress, ComputerName, UserName, VersionLogin, "E3"); // Otra PC
                        Respuesta = Resources.Resources.rpta6;
                    }
                }

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

        public void EnviaCorreoUsuario(string opcion, string codUsuario, string direccionIp)
        {
            var dr = Funciones.ObtieneUsuarioCorreo(codUsuario);
            var tipo = "";

            if (dr["IdTipo"].ToString() == "8") tipo = "[PRUEBA GRATIS] ";
            else if (dr["IdTipo"].ToString() == "10") tipo = "[CLIENTE] ";
            else if (dr["IdTipo"].ToString() == "49") tipo = "[GRATIS] ";
            else if (dr["IdTipo"].ToString() == "90") tipo = "[CONVENIO] ";

            var origen = dr["Origen"].ToString();

            string emailEnvio1 = "", emailEnvioNombre1 = "", emailEnvioPassword1 = "";
            Funciones.BuscaDatosCorreoEnvio("4", ref emailEnvio1, ref emailEnvioNombre1, ref emailEnvioPassword1);

            var fromName = emailEnvioNombre1;
            var fromEmail = emailEnvio1;
            var CcName = "Info Veritrade";
            var CcEmail = "info@veritrade-ltd.com";
            string toName, toEmail;
            var subject = opcion == "INACTIVO"
                ? "Intento ingreso usuario inactivo"
                : "Usuario ingreso restringido IP";

            subject = "[" + codUsuario + "] " + tipo + subject;

            var url = _path + "/Correos.aspx?Opcion=" + opcion + "&CodUsuario=" + codUsuario + "&DireccionIP=" +
                      direccionIp;

            //_wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, null, null, subject, url,
            //    emailEnvio1, emailEnvioPassword1);
            bool correoEnviado = false;

            // Ruben 202208
            if (tipo != "[CLIENTE] ")
            {
                toEmail = Funciones.BuscaCorreoAdmin(origen);
                if (toEmail != "")
                {
                    toName = "Ejecutivo Veritrade";
                    _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, CcName, CcEmail, null, null, subject, url,
                        emailEnvio1, emailEnvioPassword1);
                    correoEnviado = true;
                }

                /*
                if (tipo == "[CLIENTE] " && origen.ToLower() == "veritrade")
                {
                    toName = "Cynthia Medina";
                    toEmail = Funciones.BuscaCorreoAdmin("cmedina");
                    _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, CcName, CcEmail, null, null, subject, url,
                        emailEnvio1, emailEnvioPassword1);
                    correoEnviado = true;
                }
                */
            }
            else
            {
                string sql = "select E.Ejecutivo, E.UsuarioCorreo from Usuario UL, VeritradeAdmin.dbo.Usuario U, VeritradeAdmin.dbo.Cliente C, VeritradeAdmin.dbo.Ejecutivo E ";
                sql += "where UL.CodUsuario COLLATE DATABASE_DEFAULT = U.UsuarioCorreo COLLATE DATABASE_DEFAULT and U.IdCliente = C.IdCliente and C.IdEjecutivoA = E.IdEjecutivo and UL.CodUsuario = '" + codUsuario + "'";

                DataTable dt = Conexion.SqlDataTable(sql);

                toName = dt.Rows[0]["Ejecutivo"].ToString();
                toEmail = dt.Rows[0]["UsuarioCorreo"].ToString();

                _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, CcName, CcEmail, null, null, subject, url, emailEnvio1, emailEnvioPassword1);

                sql = "select Ejecutivo, UsuarioCorreo from VeritradeAdmin.dbo.Ejecutivo ";
                sql += "where CodTipoEjecutivo = 'JRA' ";

                DataTable dt2 = Conexion.SqlDataTable(sql);

                toName = dt2.Rows[0]["Ejecutivo"].ToString();
                toEmail = dt2.Rows[0]["UsuarioCorreo"].ToString();

                _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, CcName, CcEmail, null, null, subject, url, emailEnvio1, emailEnvioPassword1);

                correoEnviado = true;
            }

            if (!correoEnviado)
            {
                toName = CcName;
                toEmail = CcEmail;
                _wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, null, null, subject, url,
                    emailEnvio1, emailEnvioPassword1);
            }

            //toName = "Vanessa Soria";
            //toEmail = Funciones.BuscaCorreoAdmin("vsoria");
            //_wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, null, null, subject, url,
            //    emailEnvio1, emailEnvioPassword1);
        }
    }
}