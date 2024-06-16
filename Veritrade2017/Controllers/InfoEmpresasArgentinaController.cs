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
	public class InfoEmpresasArgentinaController : BaseController
	{
		
		public ActionResult Index(string culture) {
			List<SubCategoriaEmpresa> listaOriginal = new List<SubCategoriaEmpresa>();

			

			ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
			ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
			ViewData["ayuda"] = new Ayuda().GetAyuda(culture);
			listaOriginal = new SubCategoriaEmpresa().GetTodasLasCategorias("AR");

			int partes = 4;
			List<SubCategoriaEmpresa> lista0 = new List<SubCategoriaEmpresa>();
			List<SubCategoriaEmpresa> lista1 = new List<SubCategoriaEmpresa>();
			List<SubCategoriaEmpresa> lista2 = new List<SubCategoriaEmpresa>();
			List<SubCategoriaEmpresa> lista3 = new List<SubCategoriaEmpresa>();

			//calcular el tamaño de cada lista
			int tamanoParte = (int)Math.Ceiling((double)listaOriginal.Count / partes);

			List<List<SubCategoriaEmpresa>> partesLista = listaOriginal
				.Select((item, index) => new { Item = item, Index = index })
				.GroupBy(x => x.Index / tamanoParte)
				.Select(group => group.Select(x => x.Item).ToList())
				.ToList();


			if (culture == "es")
				ViewBag.Canonical =
				"https://www.veritradecorp.com/es/info-empresas-argentina";
			else
				ViewBag.Canonical =
				"https://www.veritradecorp.com/en/info-argentinian-companies";


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
			return View("~/views/Infoempresasargentina/Index.cshtml");
		
		}

	}
}
