
using System.Web.Mvc;
using Veritrade2017.Models;
using System.Collections.Generic;
using Veritrade2017.Models.Minisite;
using Veritrade2017.Models.CountryProfile;
using System.Linq;
using Newtonsoft.Json;
using Veritrade2017.Models.Admin;
using System.Threading.Tasks;
using DevExpress.XtraExport;

namespace Veritrade2017.Controllers
{
	public class SandBoxController : BaseController
	{
		public ActionResult Index(string culture)
		{

			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
			return View("~/views/SandBox/Index.cshtml");
		}

	}
}