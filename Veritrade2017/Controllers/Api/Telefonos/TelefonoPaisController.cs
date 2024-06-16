using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers.Api.Telefonos
{
    [RoutePrefix("api/TelefonoPais")]
    public class TelefonoPaisController : ApiController
    {
        [HttpPost]
        [Route("ObtenerTelefonoPorIp")]
        public IHttpActionResult ObtenerTelefonoPorIp(string direccionIp)
        {
			var inci = new Incidencias();
			string xx = direccionIp != null ? direccionIp.ToString() : "nulo";
			inci.SalvarIncidencia($"1 ObtenerTelefonoPorPais - direccionIp  :{xx}");

			TelefonoPais telefonoPais = new TelefonoPais();
		
				var codPais = "";
				var ws = new VeritradeAdmin.Seguridad();

				ws.BuscaUbicacionIP2(direccionIp, ref codPais);

				string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
				
                xx = culture != null ? culture.ToString() : "nulo";
				inci.SalvarIncidencia($"1 ObtenerTelefonoPorPais - culture  :{xx}");


				telefonoPais = FuncionesBusiness.ObtenerTelefonoPaisPorCodPais(codPais, culture);


				xx = telefonoPais.NombrePais.ToString() != null ? telefonoPais.NombrePais.ToString() : "nulo";
				inci.SalvarIncidencia($"1 ObtenerTelefonoPorPais - telefonoPais NombrePais  :{xx}");

				xx = telefonoPais.Telefono.ToString() != null ? telefonoPais.Telefono.ToString() : "nulo";
				inci.SalvarIncidencia($"1 ObtenerTelefonoPorPais - telefonoPais telefono  :{xx}");




           
           
            return Ok(telefonoPais);
        }

        [HttpPost]
        [Route("ObtenerTelefonoPorPais")]
        public IHttpActionResult ObtenerTelefonoPorPais(string codPais, string culture)
        {
			TelefonoPais telefonoPais = new TelefonoPais();
			try
            {
				var inci = new Incidencias();
				string xx = codPais != null ? codPais.ToString() : "nulo";
				inci.SalvarIncidencia($"1 ObtenerTelefonoPorPais - codPais  :{xx}");
				xx = culture != null ? culture.ToString() : "nulo";
				inci.SalvarIncidencia($"2 ObtenerTelefonoPorPais - culture  :{xx}");

				
				telefonoPais = FuncionesBusiness.ObtenerTelefonoPaisPorCodPais(codPais, culture);


				xx = telefonoPais != null ? telefonoPais.ToString() : "nulo";
				inci.SalvarIncidencia($"3 ObtenerTelefonoPorPais - telefonoPais  :{xx}");

				
			}
            catch (Exception ex)
            {

				var inci = new Incidencias();
				inci.SalvarIncidencia($"3 ObtenerTelefonoPorPais - error  :{ex.Message +  ex.InnerException.Message}");
			}

			return Ok(telefonoPais);
		}

        [HttpPost]
        [Route("ObtenerTelefonosPorId")]
        public IHttpActionResult ObtenerTelefonosPorId(int telefonoId, string culture)
        {
            TelefonoPais telefonoPais = new TelefonoPais();
            telefonoPais = FuncionesBusiness.ObtenerTelefonosPorId(telefonoId, culture);
            return Ok(telefonoPais);
        }

        [HttpPost]
        [Route("ObtenerTelefonosPaises")]
        public IHttpActionResult ObtenerTelefonosPaises(string culture)
        {
            //Obtiene los números telefónicos de Veritrade independiente del país
            List<TelefonoPais> telefonos = new List<TelefonoPais>();
            telefonos = FuncionesBusiness.ObtenerTelefonosPaises(culture);
            return Ok(telefonos);
        }
    }
}
