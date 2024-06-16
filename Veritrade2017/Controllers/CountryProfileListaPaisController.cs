
using System.Web.Mvc;
using Veritrade2017.Models;
using System.Collections.Generic;
using Veritrade2017.Models.Minisite;
using Veritrade2017.Models.CountryProfile;
using System.Threading.Tasks;
using DevExpress.Office.Utils;

namespace Veritrade2017.Controllers
{
	public class CountryProfileListaPaisController : BaseController
	{

		public async Task<ActionResult> Index(string culture)
		{
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
			

			//if (culture == "es")
			//{
			//	ViewBag.Canonical =
			//	"https://www.veritradecorp.com/es/paises-listado";

			//}
			//else
			//{
			//	ViewBag.Canonical =
			//	"https://www.veritradecorp.com/en/countries-list";
			//}

			Region region = new Region();
			PaisCardDTO paisCardDTO = new PaisCardDTO();
			List<List<PaisCardDTO>> listado  = new List<List<PaisCardDTO>>();



			//latam
			region = await Region.GetRegionByOrden(1);
			listado = await paisCardDTO.GetLineaPaisPorRegion(region.IdRegion, culture);
			ViewBag.Region1Nombre = culture =="es"? region.RegionNombre : region.RegionNombreEn;
			ViewBag.MiLista1 = listado;

			//ame
			region = await Region.GetRegionByOrden(2);
			listado = await paisCardDTO.GetLineaPaisPorRegion(region.IdRegion, culture);
			ViewBag.Region2Nombre = culture == "es" ? region.RegionNombre : region.RegionNombreEn;
			ViewBag.MiLista2 = listado;

			//ame
			region = await Region.GetRegionByOrden(3);
			listado = await paisCardDTO.GetLineaPaisPorRegion(region.IdRegion, culture);
			ViewBag.Region3Nombre = culture == "es" ? region.RegionNombre : region.RegionNombreEn;
			ViewBag.MiLista3 = listado;

			region = await Region.GetRegionByOrden(4);
			listado = await paisCardDTO.GetLineaPaisPorRegion(region.IdRegion, culture);
			ViewBag.Region4Nombre = culture == "es" ? region.RegionNombre : region.RegionNombreEn; ;
			ViewBag.MiLista4 = listado;



			return View("~/views/CountryProfileListaPais/Index.cshtml");
			
		}

	
	}
}