using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Veritrade2017.Models;
using Veritrade2017.Models.CountryProfile;

namespace Veritrade2017.Controllers
{
	public class PaisImportacionesEmpresasController : BaseController
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

			string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0").Replace(".", ",");
			string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalEmpresas = formattedTotalE;
			ViewBag.TotalProductos = formattedTotalP;

			CPSubcategoriasDTO cPSubcategoriaDTO = new CPSubcategoriasDTO();
			List<CPSubcategoria> lista =  await cPSubcategoriaDTO.ListadoSubcategoria(idPais, "I", culture, Enums.TipoSubcateria.Empresas_subcategoria);

			ViewBag.ListaDatos = lista;
			int countCategoria = lista.Count;
			int countSubCategorias = lista.Sum(x => x.Nsubcategorias);

			ViewBag.countCategoria = countCategoria;
			ViewBag.countSubCategorias = countSubCategorias;

			PaisCardDTO paisCardDTO = new PaisCardDTO();

			var info = await paisCardDTO.GetInfoPais(idPais, culture);
			ViewBag.PaisCap = info.NombreES;
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
			if (culture != "es")
			{
				SubtitleNro2 = $"Here you will find information by category of importing companies in <b>{info.NombreES}</b>";
			}
			else
			{
				SubtitleNro2 = $"Aquí encontrarás información por categorías de empresas importadoras de <b>{info.NombreES}</b>";
			}

			ViewBag.SubtitleNro2 = SubtitleNro2;
			ViewBag.FlagDescubre = "TRUE";
			ViewBag.LayoutCP = "CP3ImportE";
			ViewBag.RutaCP_ES = $"/es/paises/{info.RutaFriendlyNameES}/importaciones/empresas";
			ViewBag.RutaCP_EN = $"/en/countries/{info.RutaFriendlyNameEN}/imports/companies";

			return View("~/views/PaisImportacionesEmpresas/Index.cshtml");
		}

	}
}