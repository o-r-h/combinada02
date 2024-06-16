using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Veritrade2018.Helpers;
using Veritrade2018.Models;

namespace Veritrade2018.Controllers.Admin
{
    public class ContactanosController : Controller
    {
        // GET: Contactanos
      
        [HttpGet]
        public ActionResult Index(string culture)
        {
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
            return View("Index");
        }
    }
}