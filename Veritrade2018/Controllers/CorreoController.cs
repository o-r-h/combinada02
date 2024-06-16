using System.Web.Mvc;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers
{
    public class CorreoController : Controller
    {        
        // GET: Correo
        public ActionResult Index()
        {
            string Opcion = Request.QueryString["Opcion"].ToString();
            string IdSolicitud = Request.QueryString["IdSolicitud"].ToString();
            setViewVariableByOption(Opcion, IdSolicitud);
            return View();
        }

        private void setViewVariableByOption(string option, string IdSolicitud)
        {
            Correo objCorreo;
            switch (option)
            {
                case "SOLICITUD":
                    objCorreo = new Correo();
                    var dr = FuncionesBusiness.ObtieneSolicitud(IdSolicitud);
                    string codSolicitud = dr["CodSolicitud"].ToString();
                    if (codSolicitud == "CONTACTENOS")
                        objCorreo.Titulo = "Solicitud de Contacto o Soporte";

                    if (codSolicitud == "CONTACTENOS2")
                    {
                        objCorreo.Titulo = "Solicitud de Contacto o Soporte - Usuario";
                        objCorreo.CodUsuario = dr["CodUsuario"].ToString();
                    }
                    else if (codSolicitud == "OLVIDASTE")
                        objCorreo.Titulo = "Solicitud de Contraseña";
                    else
                        objCorreo.Titulo = "Solicitud de Cotización";

                    objCorreo.Nombres = dr["Nombres"].ToString();
                    objCorreo.Empresa = dr["Empresa"].ToString();
                    objCorreo.Telefono = dr["Telefono"].ToString();
                    objCorreo.Email1 = dr["Email1"].ToString();
                    objCorreo.Mensaje = dr["Mensaje"].ToString();
                    objCorreo.DireccionIP = dr["DireccionIP"].ToString();
                    objCorreo.Ubicacion = dr["Ubicacion"].ToString();
                    break;
                default:
                    objCorreo = new Correo();
                    break;
            }
            ViewData["objCorreo"] = objCorreo;
        }
        
    }
}
