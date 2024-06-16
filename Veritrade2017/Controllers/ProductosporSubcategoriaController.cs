using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Veritrade2017.Models;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Controllers
{
	public class ProductosporSubcategoriaController : BaseController
	{

		public ActionResult Index(string culture, string codigoPais, int idSubcategoria)
		{
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			List<ProductosporSubcategoria> listaOriginal = new List<ProductosporSubcategoria>();
			listaOriginal = new ProductosporSubcategoria().GetTodasLosProductos(codigoPais,idSubcategoria);
			int partes = 4;
			List<ProductosporSubcategoria> lista0 = new List<ProductosporSubcategoria>();
			List<ProductosporSubcategoria> lista1 = new List<ProductosporSubcategoria>();
			List<ProductosporSubcategoria> lista2 = new List<ProductosporSubcategoria>();
			List<ProductosporSubcategoria> lista3 = new List<ProductosporSubcategoria>();

			//calcular el tamaño de cada lista
			int tamanoParte = (int)Math.Ceiling((double)listaOriginal.Count / partes);
			List<List<ProductosporSubcategoria>> partesLista = listaOriginal
				.Select((item, index) => new { Item = item, Index = index })
				.GroupBy(x => x.Index / tamanoParte)
				.Select(group => group.Select(x => x.Item).ToList())
				.ToList();

			



			if (culture == "es")
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/es/productos-argentina-";
				ViewBag.TituloSubProducto = "Productos";
			}
			else
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/en/argentinian-products-";
				ViewBag.TituloSubProducto = "SubCategory Names";
			}

			ViewBag.SubcategoriaProducto = "";
			if (partesLista.Count > 0)
			{
				EmpresasPorSubcategoria empresasPorSubcategoria = new EmpresasPorSubcategoria();

				ViewBag.SubcategoriaProducto = empresasPorSubcategoria.GetEmpresaCategoriaNombre(codigoPais, idSubcategoria).Subcategoria;
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

			return View("~/views/ProductosporSubcategoria/Index.cshtml");
		}
	}
}
