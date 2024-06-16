
using System.Web.Mvc;
using Veritrade2017.Models;
using System.Collections.Generic;
using Veritrade2017.Models.CountryProfile;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Veritrade2017.Helpers;
using System.Web;
using System;
using static DevExpress.DataAccess.Native.Sql.QueryBuilder.ColumnDataItem;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
	public class DescubreComercioExteriorController : BaseController
	{

	
		public async Task<ActionResult> Index(string culture, string paisNombre)
		{
			
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			System.Web.HttpContext.Current.Session["paisNombreSession"] = paisNombre;
			

			PaisInfoGeneralDTO paisInfoGeneralDTO = new PaisInfoGeneralDTO();
			int idPais = await paisInfoGeneralDTO.GetId(culture, paisNombre);
			paisInfoGeneralDTO = await paisInfoGeneralDTO.GetData(idPais);
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();

			InfoConsolidadaPais infoConsolidada = await  paisInfoGeneralDTO.GetDataConsolidada(idPais, culture);
			int infoCo = 0;
			if (infoConsolidada !=null & infoConsolidada.ListaLi.Count >=3){
				ViewBag.InfoCo1 = infoConsolidada.ListaLi[0].ToString();
				ViewBag.InfoCo2 = infoConsolidada.ListaLi[1].ToString();
				ViewBag.InfoCo3 = infoConsolidada.ListaLi[2].ToString();
				ViewBag.AnioAviso = infoConsolidada.FechaIni.Substring(0, 4);
				infoCo = 1;
			}
			ViewBag.InfoCo = infoCo;


			ViewBag.CultureValue = culture;
			ViewBag.Idpais = idPais;
			
			ViewBag.BannerEmpresa = await paisInfoGeneralDTO.ListadoBannerEmpresasRelacionadas(idPais, culture);
			ViewBag.BannerProducto = await paisInfoGeneralDTO.ListadoBannerProductosRelacionados(idPais, culture);

			string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0").Replace(".",",");
			string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0").Replace(".", ","); ;
			ViewBag.TotalEmpresas = formattedTotalE;
			ViewBag.TotalProductos = formattedTotalP;

			ViewBag.ListadoExportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasExportaciones(idPais, culture);
			ViewBag.ListadoExportacionesProducto = await exportacionCardInfoD.GetTopProductosExportaciones(idPais, culture);

			ViewBag.ListadoImportacionesEmpresa = await exportacionCardInfoD.GetTopEmpresasImportaciones(idPais, culture);
			ViewBag.ListadoImportacionesProducto = await exportacionCardInfoD.GetTopProductosImportaciones(idPais, culture);

			PaisCardDTO paisCardDTO = new PaisCardDTO();

			var info = await paisCardDTO.GetInfoPais(idPais, culture);
			ViewBag.PaisCap = info.NombreES;
			ViewBag.Pais = info.NombreES;
			ViewBag.PaisLower = info.NombreES.ToLower();
			ViewBag.PaisCorto = info.NombreCorto;
			string impC = info.ImportadoresC.ToString("#,##0").Replace(".", ","); 
			string expC = info.ExportadoresC.ToString("#,##0").Replace(".", ","); 
			ViewBag.ImportacionEmpresasCount = impC;
			ViewBag.ExportacionEmpresasCount = expC;
			decimal totalEH = info.ImportadoresC + info.ExportadoresC;
			ViewBag.TotalEmpresasHeader = totalEH.ToString("#,##0").Replace(".", ","); 

			string impP = info.ImportadoresP.ToString("#,##0").Replace(".", ","); 
			string expP = info.ExportadoresP.ToString("#,##0").Replace(".", ",");
			decimal totalPH = info.ImportadoresP + info.ExportadoresP;
			ViewBag.totalPH = totalPH.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalProductosHeader = totalPH.ToString("#,##0").Replace(".", ",");
			ViewBag.ImportacionProductosCount = impP;
			ViewBag.ExportacionProductosCount = expP;
			ViewBag.Archivo = info.Archivo;

			ViewBag.FlagPaisActual = info.fullNameBanderaUrl;
			ViewBag.FriendlyNameES = info.FriendlyNameES;
			List<DataPoint> pointsI = new List<DataPoint>();
			List<DataPoint> pointsE = new List<DataPoint>();
			pointsI = await GetDataChartAsync(idPais, "I");
			pointsE = await GetDataChartAsync(idPais, "E");

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
				noCompanyTitle = $"Here you will find information on more than <b>{totalPH.ToString("#,##0").Replace(".", ",")}</b> imported and exported products from <b>{info.NombreES}</b>" ;
			}


			ViewBag.NoCompanyTitle = noCompanyTitle;

			ViewBag.FlagDescubre = "TRUE";
			ViewBag.LayoutCP = "CP1";
			ViewBag.RutaCP_ES = $"/es/paises/{info.RutaFriendlyNameES}/exportaciones";
			ViewBag.RutaCP_EN = $"/en/countries/{info.RutaFriendlyNameEN}/exports";

			
			return View("~/views/DescubreComercioExterior/Index.cshtml");
		}

		
		public async Task<List<DataPoint>> GetDataChartAsync( int idpais, string regimen){
			PaisGraficoDTO paisGraficoDTO = new PaisGraficoDTO();
			List<DataPoint> dp = new List<DataPoint>();
			dp = await paisGraficoDTO.GetInfoPaisGrafico(idpais, regimen);
			return dp;
		}


		public ActionResult SetCulture(string culture)
		{
			culture = CultureHelper.GetImplementedCulture(culture);
			RouteData.Values["culture"] = culture; // set culture 
			string paisNombre = System.Web.HttpContext.Current.Session["paisNombreSession"]  as string ;
			if (paisNombre == null)
			{
				paisNombre ="";
			}
			return RedirectToAction("Index", "DescubreComercioExterior", new { culture = culture, nombrePais = paisNombre });
	
		}


		[HttpGet]
		public async Task<ActionResult> GetListadoExportacionEmpresa(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD  exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetEmpresasExportacionesporNombreES( idPais, nombre,  culture);

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
		public  async Task<ActionResult> GetListadoExportacionProducto(string nombre, int idPais, string culture)
		{
			//TO DO logica para filtrar por idioma
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();
			List<ExportacionCardInfoD> lista = new List<ExportacionCardInfoD>();
			lista = await exportacionCardInfoD.GetProductosExportacionesporNombreES(idPais,nombre, culture);

			var jsonData = JsonConvert.SerializeObject(lista);
			return Content(jsonData, "application/json");
		}


		[HttpGet]
		public async Task<ActionResult> GetListadoImportacionProducto(string nombre, int idPais , string culture)
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