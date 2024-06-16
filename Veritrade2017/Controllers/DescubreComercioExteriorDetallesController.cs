
using System.Web.Mvc;
using Veritrade2017.Models;
using System.Collections.Generic;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
	public  class DescubreComercioExteriorDetallesController : BaseController
	{
		public ActionResult Index(string culture)
		{
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			return View("~/views/DescubreComercioExteriorDetalles/Index.cshtml");
		}

	}
}