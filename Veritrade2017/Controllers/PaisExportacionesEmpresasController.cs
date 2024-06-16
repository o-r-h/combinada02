
using DevExpress.Office.Utils;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Veritrade2017.Models;
using Veritrade2017.Models.CountryProfile;
using static iTextSharp.tool.xml.html.HTML;
using static Veritrade2017.Models.CountryProfile.Enums;

namespace Veritrade2017.Controllers
{
	public class PaisExportacionesEmpresasController : BaseController
	{

		public async Task<ActionResult> Index(string culture, string paisNombre)
		{

			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			PaisInfoGeneralDTO paisInfoGeneralDTO = new PaisInfoGeneralDTO();
			int idPais = await paisInfoGeneralDTO.GetId(culture, paisNombre);
			paisInfoGeneralDTO = await paisInfoGeneralDTO.GetData(idPais);
			ExportacionCardInfoD exportacionCardInfoD = new ExportacionCardInfoD();

			ViewBag.CultureValue = culture;
			ViewBag.Idpais = idPais;
			
			ViewBag.Pais = paisNombre;
			
			ViewBag.BannerEmpresa = await paisInfoGeneralDTO.ListadoBannerEmpresasRelacionadas(idPais, culture);
			ViewBag.BannerProducto = await paisInfoGeneralDTO.ListadoBannerProductosRelacionados(idPais, culture);

			string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0").Replace(".",",");
			string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalEmpresas = formattedTotalE;
			ViewBag.TotalProductos = formattedTotalP;

			CPSubcategoriasDTO cPSubcategoriaDTO = new CPSubcategoriasDTO();
			List<CPSubcategoria> lista = await cPSubcategoriaDTO.ListadoSubcategoria(idPais, "E", culture, Enums.TipoSubcateria.Empresas_subcategoria);
			ViewBag.ListaDatos = lista;
			int countCategoria = lista.Count;
			int countSubCategorias = lista.Sum(x=>x.Nsubcategorias);


			//var listaH = await cPSubcategoriaDTO.ListadoSubcategoriaDetalleHeader(idPais, "E", culture, "vegetales-y-frutos-y-legumbres", TipoSubcateriaDetalle.Empresas_subcategoria_detalle);

			//var ListaD = await cPSubcategoriaDTO.ListadoSubcategoriaDetalleEmpresaUrlSolo(idPais, "E", "es", 25);

			
			//List<SubDetalleHeaderyDetalle> ListaSubCatItem = new List<SubDetalleHeaderyDetalle>();
			//foreach (var item in listaH){
			//	SubDetalleHeaderyDetalle SDH = new SubDetalleHeaderyDetalle();
			//	SDH.IdCategoria = item.IdCategoria;
			//	SDH.IdSubcategoria = item.IdSubcategoria;
			//	SDH.SubCategoriaES = item.SubCategoriaES;
			//	SDH.CantEmpresas = item.CantEmpresas;
			//	SDH.TotalSuma = item.TotalSuma;
			//	SDH.ListaDetalle = ListaD.Where(x=>x.IdSubcategoria == item.IdSubcategoria).ToList();
			//	ListaSubCatItem.Add(SDH);
			//}
			


			ViewBag.countCategoria = countCategoria;
			ViewBag.countSubCategorias = countSubCategorias;

			PaisCardDTO paisCardDTO = new PaisCardDTO();

			var info = await paisCardDTO.GetInfoPais(idPais, culture);
			ViewBag.PaisCap = info.NombreES;
			ViewBag.PaisLower = info.NombreES.ToLower();
			ViewBag.FriendlyNameES = info.FriendlyNameES;
			ViewBag.FlagPaisActual = info.fullNameBanderaUrl;

			string impC = info.ImportadoresC.ToString("#,##0").Replace(".", ",");
			string expC = info.ExportadoresC.ToString("#,##0").Replace(".", ",");
			ViewBag.ImportacionEmpresasCount = impC;
			ViewBag.ExportacionEmpresasCount = expC;


			decimal totalPH = info.ImportadoresP + info.ExportadoresP;
			ViewBag.totalPH = totalPH.ToString("#,##0").Replace(".", ",");

			decimal totalEH = info.ImportadoresC + info.ExportadoresC;
			ViewBag.totalEH = totalEH.ToString("#,##0").Replace(".", ",");
			

			string SubtitleNro2 = "";
			if (culture != "es" )
			{
				SubtitleNro2 = $"Here you will find information by category of exporting companies in  <b>{info.NombreES}</b>";
			}
			else
			{
				SubtitleNro2 = $"Aquí encontrarás información por categorías de empresas exportadoras de <b>{info.NombreES}</b>";
			}

			ViewBag.SubtitleNro2 = SubtitleNro2;
			ViewBag.FlagDescubre = "TRUE";
			ViewBag.LayoutCP = "CP3ExportE";
			ViewBag.RutaCP_ES = $"/es/paises/{info.RutaFriendlyNameES}/exportaciones/empresas";
			ViewBag.RutaCP_EN = $"/en/countries/{info.RutaFriendlyNameEN}/exports/companies";

			return View("~/views/PaisExportacionesEmpresas/Index.cshtml");
		}


	}
}