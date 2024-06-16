using System.ComponentModel.DataAnnotations;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Solicitud
    {
        public string CodSolicitud { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_FullName_Required")]
        [StringLength(40, ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_FullName_StringLength", MinimumLength = 7)]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_Required")]
        [StringLength(12, ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_StringLength", MinimumLength = 7)]
        [RegularExpression("^([0-9-]+)$", ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_RegularExpression")]
        public string Telefono { get; set; }

        public string Empresa { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_EmailAddress")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Message_Required")]
        public string Mensaje { get; set; }

        public static void EnviaSolicitud(string codSolicitud, string nombres, string empresa, string telefono, string email1, string mensaje, string direccionIp)
        {
            var path = SettingUtility.GetUrlBackOld(); 
            var wsc = new VeritradeServicios.ServiciosCorreo();
            var ws = new VeritradeAdmin.Seguridad();

            var codPais = "";
            var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);

            var idSolicitud = Funciones.CreaSolicitud(codSolicitud, nombres, empresa, telefono, email1, mensaje, direccionIp, ubicacion);

            string emailEnvio1 = "", emailEnvioNombre1 = "", emailEnvioPassword1 = "";
            Funciones.BuscaDatosCorreoEnvio("4", ref emailEnvio1, ref emailEnvioNombre1, ref emailEnvioPassword1);

            string subject;

            var fromName = emailEnvioNombre1;
            var fromEmail = emailEnvio1;
            var toName = "Info Veritrade";
            var toEmail = "info@veritradecorp.com"; // Ruben 202306
            if (codSolicitud == "CONTACTENOS")
                subject = "Solicitud de Contacto o Soporte";
            else if (codSolicitud == "OLVIDASTE")
                subject = "Solicitud de Contraseña";
            else
                subject = "Solicitud de Cotización";
            var url = path + "/Correos.aspx?Opcion=SOLICITUD&IdSolicitud=" + idSolicitud;

            wsc.EnviaEmail3(fromName, fromEmail, toName, toEmail, null, null, null, null, subject, url, emailEnvio1, emailEnvioPassword1);
        }
    }
}