using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers.Api.Telefonos
{
    [RoutePrefix("api/TelefonoPais")]
    public class TelefonoPaisController : ApiController
    {
        [HttpPost]
        [Route("ObtenerTelefonoPorIp")]
        public IHttpActionResult ObtenerTelefonoPorIp(string direccionIp)
        {
            var codPais = "";
            var ws = new VeritradeAdmin.Seguridad();

            ws.BuscaUbicacionIP2(direccionIp, ref codPais);

            string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            
            TelefonoPais telefonoPais = new TelefonoPais();
            telefonoPais = FuncionesBusiness.ObtenerTelefonoPaisPorCodPais(codPais, culture);
            return Ok(telefonoPais);
        }

        [HttpPost]
        [Route("ObtenerTelefonoPorPais")]
        public IHttpActionResult ObtenerTelefonoPorPais(string codPais, string culture)
        {
            TelefonoPais telefonoPais = new TelefonoPais();
            telefonoPais = FuncionesBusiness.ObtenerTelefonoPaisPorCodPais(codPais, culture);
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
