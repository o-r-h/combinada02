using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2017.Helpers;

namespace Veritrade2017.Controllers
{
    public class LoginAlertsController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        // GET: LoginAlerts
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string inicio, string txtCodUsuario2, string txtPassword2,
            string compra, string ruta)
        {
            var cultura = RouteData.Values["culture"] as string;  //cultura
            if (Session["IdUsuario"] == null)
            {
                if (string.IsNullOrEmpty(inicio) ||
                    string.IsNullOrEmpty(txtCodUsuario2) ||
                    string.IsNullOrEmpty(txtPassword2))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Si la validación es correcta se asigna los valores de sesión como por ejemplo el IdUsuario
                    Validacion(txtCodUsuario2, txtPassword2, inicio);
                }

                if (Session["IdUsuario"] == null)
                {
                    Response.BufferOutput = true;
                    if (Session["Error"] != null)
                    {
                        return RedirectToAction("Mostrar", "Error", new { cultura });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    if(ruta.ToLower().Contains("acc") && (ruta.ToLower().Contains("acp") || ruta.ToLower().Contains("apc")))
                        return RedirectToAction("Index", "MisBusquedas", new { compra, ruta });
                    else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("amp"))
                        return RedirectToAction("Index", "MisProductos", new { compra, ruta });
                    else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("amc"))
                        return RedirectToAction("Index", "MisCompanias", new { compra, ruta });
                    else if (ruta.ToLower().Contains("acc") && ruta.ToLower().Contains("ss"))
                        return RedirectToAction("Index", "MisAlertasFavoritos", new { compra, ruta });
                    else
                    {
                        return RedirectToAction("Index", "MisBusquedas", new { compra });
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "MisBusquedas", new { compra });
            }
        }

        private ActionResult Validacion(string txtCodUsuario, string txtPassword, string inicio)
        {
            string IdUsuario = "", IdAplicacion = "", CodSeguridad = "";
            int CantUsuariosUnicos = 0;

            string CodUsuario = txtCodUsuario.ToUpper();

#if DEBUG
            string DireccionIP = Properties.Settings.Default.IP_Debug;

#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif
            if (Funciones.BuscaCodEstado(CodUsuario) == "I")
            {
                Session["Error"] = "INACTIVO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            bool Valida = _ws. /*Funciones.*/ Valida2(CodUsuario, txtPassword, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
                ref CantUsuariosUnicos);

            if (!Valida)
            {
                Session["Error"] = "INCORRECTO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            string Origen = Funciones.ObtieneOrigen(IdUsuario);


            if (Valida && (CodUsuario == "UPC" || Origen == "ULIMA" || Origen == "ESAN" || Origen == "ADEX" ||
                           Origen == "UPN") &&

                inicio != "referid0")
            {
                Session["Error"] = "INCORRECTO";

                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }


            if (CodSeguridad == "IP" && DireccionIP != Properties.Settings.Default.IP_Veritrade &&
                !_ws.ValidaIPPais(IdUsuario, DireccionIP))

            {
                Session["Error"] = "OTRO_PAIS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            if (CodSeguridad != "Off")
            {
                if (Funciones.SessionUnica(CodUsuario) && Funciones.ExisteUsuarioEnLinea(IdUsuario))
                {
                    Session["Error"] = "SESION_UNICA";
                    Session["CodUsuarioTemp"] = CodUsuario;
                    //return RedirectToAction("Logout", "Common");
                    return RedirectToAction("Index", "Error");
                }
            }





            int LimiteVisitas = 0, Visitas = 0;

            if (!Funciones.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
            {

                Session["Error"] = "LIMITE_VISITAS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            FuncionesBusiness.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");


            Session["IdUsuario"] = IdUsuario;


            var onlineUser = new OnlineUsers
            {
                IdSesion = Session.SessionID,
                IdUsuario = Session["IdUsuario"].ToString()

            };

            ActiveSessions.Sessions.AddOrUpdate(Session.SessionID, onlineUser, (k, v) => v = onlineUser);



            Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(IdUsuario);
            Session["Plan"] = Funciones.ObtienePlan(IdUsuario).ToUpper();
            Session["Origen"] = Funciones.ObtieneOrigen(IdUsuario).ToUpper();

            if (Session["Origen"].ToString() == "ULIMA" || Session["Origen"].ToString() == "UPC" ||
                Session["Origen"].ToString() == "ESAN" || Session["Origen"].ToString() == "ADEX" ||

                Session["Origen"].ToString() == "UPN" || Session["Origen"].ToString() == "UPT" ||
                Session["Origen"].ToString() == "UNIV") Session["Plan"] = "UNIVERSIDADES";
            else if (Session["Plan"].ToString() == "ESENCIAL" || Session["Plan"].ToString() == "PERU UNO" ||
                     Session["Plan"].ToString() == "ECUADOR UNO" || Session["Plan"].ToString() == "PERU IMEX" ||
                     Session["Plan"].ToString() == "ECUADOR IMEX")
            {

                string CodPais = "", TipoOpe = "";
                Funciones.BuscaDatosPlanEspecial(IdUsuario, ref CodPais, ref TipoOpe);
                Session["CodPais"] = CodPais;
                Session["TipoOpe"] = TipoOpe;
            }


            return null;
        }
    }
}