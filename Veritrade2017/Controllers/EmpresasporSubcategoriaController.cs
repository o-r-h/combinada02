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
	public class EmpresasporSubcategoriaController : BaseController
	{

		public ActionResult Index(string culture, string codigoPais, int idSubcategoria)
		{
			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);

			List<EmpresasPorSubcategoria> listaOriginal = new List<EmpresasPorSubcategoria>();
			listaOriginal = new EmpresasPorSubcategoria().GetTodasLasEmpresas(codigoPais, idSubcategoria);
			int partes = 4;
			List<EmpresasPorSubcategoria> lista0 = new List<EmpresasPorSubcategoria>();
			List<EmpresasPorSubcategoria> lista1 = new List<EmpresasPorSubcategoria>();
			List<EmpresasPorSubcategoria> lista2 = new List<EmpresasPorSubcategoria>();
			List<EmpresasPorSubcategoria> lista3 = new List<EmpresasPorSubcategoria>();

			//calcular el tamaño de cada lista
			int tamanoParte = (int)Math.Ceiling((double)listaOriginal.Count / partes);
			List<List<EmpresasPorSubcategoria>> partesLista = listaOriginal
				.Select((item, index) => new { Item = item, Index = index })
				.GroupBy(x => x.Index / tamanoParte)
				.Select(group => group.Select(x => x.Item).ToList())
				.ToList();

			ViewBag.TituloEmpresa = "";
			if (partesLista.Count > 0){
				EmpresasPorSubcategoria  empresasPorSubcategoria = new EmpresasPorSubcategoria();

				ViewBag.SubCategoriaEmpresa = empresasPorSubcategoria.GetEmpresaCategoriaNombre(codigoPais, idSubcategoria).Subcategoria;
			}
			

			if (culture == "es")
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/es/productos-argentina-";
				ViewBag.TituloSubProducto = "Compañias";
			}
			else
			{
				ViewBag.Canonical =
				"https://www.veritradecorp.com/en/argentinian-products-";
				ViewBag.TituloSubProducto = "Companies";
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
			
			return View("~/views/EmpresasporSubcategoria/Index.cshtml");
		}

	}
}
