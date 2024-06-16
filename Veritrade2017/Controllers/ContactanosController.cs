using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Veritrade2017.Helpers;
using Veritrade2017.Models;

namespace Veritrade2017.Controllers.Admin
{
    public class ContactanosController : BaseController
    {
        // GET: Contactanos
        [AuthorizedCultureContact]
        [HttpGet]
        public ActionResult Index(string culture)
        {
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
            ViewData["Portada"] = culture == "es" ? "PORTADA_LANDING_CONTACT.png" : "PORTADA_LANDING_CONTACT_EN.png";
            return View("Index");
        }
    }
}