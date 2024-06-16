using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Veritrade2017.Models
{
    public class CorreoUsuarioPrueba
    {
        public string codUsuario { get; set; }
        public string pass { get; set; }
        public string nombres { get; set; }
        public string codCampania { get; set; }
        public string empresa { get; set; }
        public string telefono { get; set; }
        public string codTelefono { get; set; }
        public string direccionIp { get; set; }
        public string ubicacion { get; set; }
        public string fecRegistro { get; set; }
        public string publicidad { get; set; }
        public string ubicacionAnuncio { get; set; }
        public string tipoConsulta { get; set; }
        public string entidadConsultada { get; set; }
        public string nombrePais { get; set; }
    }
}