using System.Web.Mvc;
using Veritrade2017.Models;
using System.Collections.Generic;
using Veritrade2017.Models.Minisite;
using Veritrade2017.Models.CountryProfile;
using System.Linq;
using Newtonsoft.Json;
using Veritrade2017.Models.Admin;
using System.Threading.Tasks;

namespace Veritrade2017.Controllers
{
	public class DescubreComercioExteriorImportController : BaseController
	{

		//public async Task<ActionResult> Index(string culture, int idPais, string paisNombre)
		//{
		//	ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
		//	ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
		//	ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

		//	PaisInfoGeneralDTO paisInfoGeneralDTO = new PaisInfoGeneralDTO();

		//	paisInfoGeneralDTO = await paisInfoGeneralDTO.GetData(idPais);
		//	ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();

		//	ViewBag.CultureValue = culture;
		//	ViewBag.Idpais = idPais;
		//	ViewBag.Pais = paisNombre;
		//	ViewBag.BannerEmpresa = await paisInfoGeneralDTO.ListadoBannerEmpresasRelacionadas(idPais, culture);
		//	ViewBag.BannerProducto = await paisInfoGeneralDTO.ListadoBannerProductosRelacionados(idPais, culture);

		//	string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0");
		//	string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0");
		//	ViewBag.TotalEmpresas = formattedTotalE;
		//	ViewBag.TotalProductos = formattedTotalP;

		//	ViewBag.ListadoExportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasExportaciones(idPais);
		//	ViewBag.ListadoExportacionesProducto = await exportacionCardInfoD.GetTopProductosExportaciones(idPais);

		//	ViewBag.ListadoImportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasImportaciones(idPais);
		//	ViewBag.ListadoImportacionesProducto = await exportacionCardInfoD.GetTopProductosImportaciones(idPais);

		//	PaisCardDTO paisCardDTO = new PaisCardDTO();

		//	var info = await paisCardDTO.GetInfoPais(idPais, culture);

		//	string impC = info.ImportadoresC.ToString("#,##0");
		//	string expC = info.ExportadoresC.ToString("#,##0");
		//	ViewBag.ImportacionEmpresasCount = impC;
		//	ViewBag.ExportacionEmpresasCount = expC;

		//	string impP = info.ImportadoresC.ToString("#,##0");
		//	string expP = info.ExportadoresC.ToString("#,##0");
		//	ViewBag.ImportacionProductosCount = impP;
		//	ViewBag.ExportacionProductosCount = expP;

		//	ViewBag.FlagPaisActual = info.fullNameBanderaUrl;

		//	return View("~/views/DescubreComercioExteriorImport/Index.cshtml");
		//}
		public async Task<ActionResult> Index(string culture,  string paisNombre)
		{
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			PaisInfoGeneralDTO paisInfoGeneralDTO = new PaisInfoGeneralDTO();
			int idPais = await paisInfoGeneralDTO.GetId(culture, paisNombre);
			paisInfoGeneralDTO = await paisInfoGeneralDTO.GetData(idPais);
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();

			InfoConsolidadaPais infoConsolidada = await paisInfoGeneralDTO.GetDataConsolidada(idPais, culture);
			int infoCo = 0;
			if (infoConsolidada != null & infoConsolidada.ListaLi.Count >= 3)
			{
				ViewBag.InfoCo1 = infoConsolidada.ListaLi[0].ToString();
				ViewBag.InfoCo2 = infoConsolidada.ListaLi[1].ToString();
				ViewBag.InfoCo3 = infoConsolidada.ListaLi[2].ToString();
				ViewBag.AnioAviso = infoConsolidada.FechaIni.Substring(0, 4);
				infoCo = 1;
			}
			ViewBag.InfoCo = infoCo;

			ViewBag.CultureValue = culture;
			ViewBag.Idpais = idPais;
			ViewBag.Pais = paisNombre;
			ViewBag.BannerEmpresa = await paisInfoGeneralDTO.ListadoBannerEmpresasRelacionadas(idPais, culture);
			ViewBag.BannerProducto = await paisInfoGeneralDTO.ListadoBannerProductosRelacionados(idPais, culture);

			string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0").Replace(".", ","); 
			string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalEmpresas = formattedTotalE;
			ViewBag.TotalProductos = formattedTotalP;

			ViewBag.ListadoExportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasExportaciones(idPais, culture);
			ViewBag.ListadoExportacionesProducto = await exportacionCardInfoD.GetTopProductosExportaciones(idPais, culture);

			ViewBag.ListadoImportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasImportaciones(idPais, culture);
			ViewBag.ListadoImportacionesProducto = await exportacionCardInfoD.GetTopProductosImportaciones(idPais, culture);

			PaisCardDTO paisCardDTO = new PaisCardDTO();

			var info = await paisCardDTO.GetInfoPais(idPais, culture);

			ViewBag.Pais = info.NombreES;
			ViewBag.PaisCap = info.NombreES;
			ViewBag.PaisCorto = info.NombreCorto;
			ViewBag.PaisLower = info.NombreES.ToLower();
			string impC = info.ImportadoresC.ToString("#,##0").Replace(".", ",");
			string expC = info.ExportadoresC.ToString("#,##0").Replace(".", ",");
			ViewBag.ImportacionEmpresasCount = impC;
			ViewBag.ExportacionEmpresasCount = expC;
			decimal totalEH = info.ImportadoresC + info.ExportadoresC;
			ViewBag.TotalEmpresasHeader = totalEH.ToString("#,##0").Replace(".", ",");
			

			string impP = info.ImportadoresP.ToString("#,##0").Replace(".", ",");
			string expP = info.ImportadoresP.ToString("#,##0").Replace(".", ",");
			decimal totalPH = info.ImportadoresP + info.ExportadoresP;
			ViewBag.totalPH = totalPH.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalProductosHeader = totalPH.ToString("#,##0").Replace(".", ",");
			ViewBag.ImportacionProductosCount = impP;
			ViewBag.ExportacionProductosCount = expP;

			ViewBag.FlagPaisActual = info.fullNameBanderaUrl;
			ViewBag.FriendlyNameES = info.FriendlyNameES;
			List<DataPoint> pointsI = new List<DataPoint>();
			List<DataPoint> pointsE = new List<DataPoint>();
			pointsI = await GetDataChartAsync(idPais, "I");
			pointsE = await GetDataChartAsync(idPais, "E");
			ViewBag.Archivo = info.Archivo;

			string json = JsonConvert.SerializeObject(pointsI);
			string json2 = JsonConvert.SerializeObject(pointsE);
			ViewData["GetImportDataChart"] = json;
			ViewData["GetExportDataChart"] = json2;
			
			string noCompanyTitle = "";
			if (culture == "es" && info.ExportadoresC == 0)
			{
				noCompanyTitle = $"Aquí encontrarás información de más de <b>{totalPH.ToString("#,##0").Replace(".", ",")}</b> productos importados y exportados de <b>{info.NombreES}</b>";
			}
			else
			{
				noCompanyTitle = $"Here you will find information on more than <b>{totalPH.ToString("#,##0").Replace(".", ",")}</b> imported and exported products from <b>{info.NombreES}</b>";
			}


			ViewBag.NoCompanyTitle = noCompanyTitle;

			ViewBag.FlagDescubre = "TRUE";
			ViewBag.LayoutCP = "CP1E";
			ViewBag.RutaCP_ES = $"/es/paises/{info.RutaFriendlyNameES}/importaciones";
			ViewBag.RutaCP_EN = $"/en/countries/{info.RutaFriendlyNameEN}/imports";



			return View("~/views/DescubreComercioExteriorImport/Index.cshtml");
		}

		public async Task<List<DataPoint>> GetDataChartAsync(int idpais, string regimen)
		{
			PaisGraficoDTO paisGraficoDTO = new PaisGraficoDTO();
			List<DataPoint> dp = new List<DataPoint>();
			dp = await paisGraficoDTO.GetInfoPaisGrafico(idpais, regimen);
			return dp;
		}

		[HttpGet]
		public async Task<ActionResult> GetListadoExportacionEmpresa(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetEmpresasExportacionesporNombreES(idPais, nombre, culture);

			var jsonData = JsonConvert.SerializeObject(lista);
			return Content(jsonData, "application/json");
		}


		[HttpGet]
		public async Task<ActionResult> GetListadoImportacionEmpresa(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetEmpresasImportacionesporNombreES(idPais, nombre, culture);

			var jsonData = JsonConvert.SerializeObject(lista);
			return Content(jsonData, "application/json");
		}


		[HttpGet]
		public async Task<ActionResult> GetListadoExportacionProducto(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetProductosExportacionesporNombreES(idPais, nombre, culture);

			var jsonData = JsonConvert.SerializeObject(lista);
			return Content(jsonData, "application/json");
		}


		[HttpGet]
		public async Task<ActionResult> GetListadoImportacionProducto(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetProductosImportacionesporNombreES(idPais, nombre, culture);

			var jsonData = JsonConvert.SerializeObject(lista);
			return Content(jsonData, "application/json");
		}
	}
}