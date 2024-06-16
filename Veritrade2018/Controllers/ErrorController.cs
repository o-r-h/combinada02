using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Resources;
using Veritrade2018.Helpers;

namespace Veritrade2018.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            if (Session["Error"] != null)
                return RedirectToAction("Mostrar");
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
       
        public ActionResult Mostrar(string cultura)
        {
            string tipoError = "";
            if (Session["Error"] != null)
                tipoError = Session["Error"].ToString();
            else
            {
                return RedirectToAction("Index", "Home");
            }
                
            ViewData["MensajeError"] = GetMensajeError(tipoError, cultura);
            return View("Index");
        }

        private string GetMensajeError(string tipoError, string cultura)
        {
            string mensajeError = "";
            string tituloError = "";

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultura);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            switch (tipoError)
            {
                case "INCORRECTO":
                    //tituloError = "Usuario y/o Clave incorrectos";
                    tituloError = Resources.Resources.Success_title1;
                    //mensajeError = "Su Usuario y/o Contraseña no han sido ingresados correctamente.<br/><br/>Por favor vuelva a intentarlo.< br />< br />Si olvidó su Usuario y/ o Clave, por favor";
                    mensajeError = Resources.Resources.desc_title1 + "<br/><br/>" + Resources.Resources.desc_title2 + "<br/><br/>" + Resources.Resources.desc_title3;
                    break;
                case "INACTIVO":
                    //tituloError = "Usuario Inactivo";
                    tituloError = Resources.Resources.Success_title2;
                    //mensajeError = " No se pude ingresar debido a que su usuario se encuentra Inactivo en Veritrade.<br/><br/>Si desea activarlo, por favor";
                    mensajeError = Resources.Resources.desc_title4 + "<br/><br/>" + Resources.Resources.desc_title5;
                    break;
                case "OTRO_PAIS":
                    //tituloError = "Acceso no autorizado";
                    tituloError = Resources.Resources.Success_title3;
                    //mensajeError = "Lo sentimos, este usuario no tiene acceso desde este país.<br/><br/>Si desea mayor información, por favor";
                    mensajeError = Resources.Resources.desc_title6 + "<br/><br/>" + Resources.Resources.desc_title7;
                    break;
                case "SESION_UNICA":
                    string codUsuarioTemp = "";
                    if (Session["CodUsuarioTemp"] != null)
                        codUsuarioTemp = Session["CodUsuarioTemp"].ToString();
                    tituloError = Resources.Resources.Success_title5;
                    //tituloError = "Ya existe el Usuario en Línea";
                    //mensajeError = " En este momento ya hay una sesión abierta con el usuario <b>"+codUsuarioTemp+"</b>.<br/><br/>Veritrade, por la seguridad de la información de su cuenta, sólo permite una sesión abierta por usuario.<br/><br/>Si usted salió de su navegador sin apretar el botón <b> \"Cerrar Sesión\" </b> deberá esperar 30 minutos para volver a ingresar nuevamente.<br/><br/>Si desea mayor información, por favor";
                    mensajeError = Resources.Resources.desc_title11+"<b>" + codUsuarioTemp + "</b>.<br/><br/>" + Resources.Resources.desc_title12 + "<br/><br/>" + Resources.Resources.desc_title13 + "<b> \" "+ Resources.Resources.desc_title14 + " \" </b>" + Resources.Resources.desc_title15 + "<br/><br/>" + Resources.Resources.desc_title7;
                    break;
                case "LIMITE_VISITAS":
                    //tituloError = "Se llegó al Límite de Visitas";
                    tituloError = Resources.Resources.Success_title6;
                    //mensajeError = " Su usuario ya alcanzó el máximo contratado de ingresos al mes.<br/><br/>Si desea mayor información, por favor";
                    mensajeError = Resources.Resources.desc_title16 + "<br/><br/>" + Resources.Resources.desc_title7;
                    break;
            }

            ViewBag.TituloError = tituloError;
            return mensajeError;
        }
    }
}