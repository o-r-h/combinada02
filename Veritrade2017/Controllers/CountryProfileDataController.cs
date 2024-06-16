
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Veritrade2017.Models.CountryProfile;

namespace Veritrade2017.Controllers
{
	public class CountryProfileDataController : BaseController
	{

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

	}
}