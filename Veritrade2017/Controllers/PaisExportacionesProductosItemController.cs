﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Veritrade2017.Models.CountryProfile;
using Veritrade2017.Models;
using static Veritrade2017.Models.CountryProfile.Enums;

namespace Veritrade2017.Controllers
{
	public class PaisExportacionesProductosItemController :BaseController
	{

		public async Task<ActionResult> Index(string culture, string paisNombre, string nombreitem)
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
			ViewBag.NombreItem = nombreitem;
			ViewBag.BannerEmpresa = await paisInfoGeneralDTO.ListadoBannerEmpresasRelacionadas(idPais, culture);
			ViewBag.BannerProducto = await paisInfoGeneralDTO.ListadoBannerProductosRelacionados(idPais, culture);

			string formattedTotalE = paisInfoGeneralDTO.TotalEmpresasCantidad.ToString("#,##0").Replace(".", ",");
			string formattedTotalP = paisInfoGeneralDTO.TotalProductosCantidad.ToString("#,##0").Replace(".", ",");
			ViewBag.TotalEmpresas = formattedTotalE;
			ViewBag.TotalProductos = formattedTotalP;



			CPSubcategoriasDTO cPSubcategoriaDTO = new CPSubcategoriasDTO();

			List<CPSubcategoria> lista = await cPSubcategoriaDTO.ListadoSubcategoria(idPais, "E", culture, Enums.TipoSubcateria.Productos_subcategoria);
			ViewBag.ListaDatos = lista;
			int countCategoria = lista.Count;
			int countSubCategorias = lista.Sum(x => x.Nsubcategorias);

			ViewBag.countCategoria = countCategoria;
			ViewBag.countSubCategorias = countSubCategorias;

			PaisCardDTO paisCardDTO = new PaisCardDTO();

			var info = await paisCardDTO.GetInfoPais(idPais, culture);
			ViewBag.FriendlyNameES = info.FriendlyNameES;
			ViewBag.FlagPaisActual = info.fullNameBanderaUrl;
			ViewBag.PaisCap = info.NombreES;


			var listaH = await cPSubcategoriaDTO.ListadoSubcategoriaDetalleHeader(idPais, "E", culture, nombreitem, TipoSubcateriaDetalle.Productos_subcategoria_detalle);
			int idDetalle = 0;
			if (listaH.Count > 0)
			{
				idDetalle = listaH[0].IdCategoria;
			}

			var ListaD = await cPSubcategoriaDTO.ListadoSubcategoriaDetalleProductoUrlSolo(idPais, "E", culture, idDetalle);

			List<SubDetalleHeaderyDetalle> ListaSubCatItem = new List<SubDetalleHeaderyDetalle>();
			foreach (var item in listaH)
			{
				SubDetalleHeaderyDetalle SDH = new SubDetalleHeaderyDetalle();
				SDH.IdCategoria = item.IdCategoria;
				SDH.IdSubcategoria = item.IdSubcategoria;
				SDH.SubCategoriaES = item.SubCategoriaES;
				
				//SDH.TotalSuma = item.TotalSuma;
				SDH.ListaDetalle = ListaD.Where(x => x.IdSubcategoria == item.IdSubcategoria).OrderByDescending(o=>o.Total).ToList();
				SDH.CantEmpresas = SDH.ListaDetalle.Count();   //item.CantEmpresas;
				SDH.TotalSuma = SDH.ListaDetalle.Sum(x => x.Total);
				ListaSubCatItem.Add(SDH);
			}

			ViewBag.LSCT = ListaSubCatItem;

			List<CPSubcategoria> Lsub = await cPSubcategoriaDTO.GetSubcategoria(idPais, "E", culture, TipoSubcateria.Productos_subcategoria, nombreitem);

			string categorievar = "";
			string empresavar = "";
			string valorvar = "";
			if (Lsub.Count > 0)
			{
				categorievar = Lsub[0].CategoriasES;
				empresavar = ListaD.Count().ToString("#,##0").Replace(".", ",");
				valorvar = ListaD.Sum(x=>x.Total).ToString("#,##0").Replace(".", ",");
			}


			if (Lsub != null)
			{
				if (culture != "es")
				{

					ViewBag.categoriaNombreCajaPan = categorievar;
					ViewBag.icategoria = $"<h4 class='custom-h4  custom-h4-mobile'>Categorie: <span> {categorievar}</span> </h4>";
					ViewBag.iempresa = $"<h5 class='custom-h5'>Products: <span>{empresavar}</span></h5> ";
					ViewBag.isubcategoria = $"<h5 class='custom-h5'>Subcategories: <span>{listaH.Count}</span></h5> ";
					ViewBag.iusd = $" <h5 class='custom-h5'>USD: <span>${valorvar}</span></h5>";
				}
				else
				{

					ViewBag.categoriaNombreCajaPan = categorievar;
					ViewBag.icategoria = $"<h4 class='custom-h4  custom-h4-mobile'>Categoría: <span> {categorievar}</span> </h4>";
					ViewBag.iempresa = $"<h5 class='custom-h5'>Productos: <span>{empresavar}</span></h5> ";
					ViewBag.isubcategoria = $"<h5 class='custom-h5'>Subcategorías: <span>{listaH.Count}</span></h5> ";
					ViewBag.iusd = $" <h5 class='custom-h5'>USD: <span>${valorvar}</span></h5>";
				}
			}

			NombresItems ningles = new NombresItems();
			List<NombresItems> nombresItems = await cPSubcategoriaDTO.VerificarIdiomaCategoriaProductos(idPais);
			if (culture == "es")
			{
				ningles = nombresItems.Where(x => x.NombreEspanol == nombreitem).FirstOrDefault();
			}
			else
			{
				ningles = nombresItems.Where(x => x.NombreIngles == nombreitem).FirstOrDefault();
			}

			if (ningles == null)
			{
				ningles = new NombresItems();
				ningles.NombreEspanol = "";
				ningles.NombreIngles = "";
			}

			ViewBag.FlagDescubre = "TRUE";
			ViewBag.LayoutCP = "CP3ExportPitem";
			ViewBag.RutaCP_ES = $"/es/paises/{info.RutaFriendlyNameES}/exportaciones/productos/{ningles.NombreEspanol}";
			ViewBag.RutaCP_EN = $"/en/countries/{info.RutaFriendlyNameEN}/exports/products/{ningles.NombreIngles}";


					

			

			return View("~/views/PaisExportacionesProductosItem/Index.cshtml");
		}
	}
}