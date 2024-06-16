//using DevExpress.XtraExport;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Web.Mvc;
using Veritrade2017.Models;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
	public class InfoProductoArgentinaController  :BaseController
	{

		public ActionResult Index(string culture)
		{
			List<SubCategoriaProducto> listaOriginal = new List<SubCategoriaProducto>();



			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
			listaOriginal = new SubCategoriaProducto().GetTodasLasCategorias("AR");

			int partes = 4;
			List<SubCategoriaProducto> lista0 = new List<SubCategoriaProducto>();
			List<SubCategoriaProducto> lista1 = new List<SubCategoriaProducto>();
			List<SubCategoriaProducto> lista2 = new List<SubCategoriaProducto>();
			List<SubCategoriaProducto> lista3 = new List<SubCategoriaProducto>();

			//calcular el tamaño de cada lista
			int tamanoParte = (int)Math.Ceiling((double)listaOriginal.Count / partes);

			List<List<SubCategoriaProducto>> partesLista = listaOriginal
				.Select((item, index) => new { Item = item, Index = index })
				.GroupBy(x => x.Index / tamanoParte)
				.Select(group => group.Select(x => x.Item).ToList())
				.ToList();


			if (culture == "es")
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/es/info-productos-argentina";
				ViewBag.TituloInfoProducto = "Categorias de productos argentinos";
			}
			else
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/en/info-argentinian-products";
				ViewBag.TituloInfoProducto = "Argentine product categories";
			}
			for (int i = 0; i < partesLista.Count; i++)
			{
				Console.WriteLine(i.ToString());
				switch (i)
				{
					case 0:
						lista0 = partesLista[i];
						break;
					case 1:
						lista1 = partesLista[i];
						break;
					case 2:
						lista2 = partesLista[i];
						break;
					case 3:
						lista3 = partesLista[i];
						break;
					default:
						break;
				}
			}

			ViewBag.MiLista0 = lista0;
			ViewBag.MiLista1 = lista1;
			ViewBag.MiLista2 = lista2;
			ViewBag.MiLista3 = lista3;
			return View("~/views/Infoproductoargentina/Index.cshtml");

		}



	}
}
