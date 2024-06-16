using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Veritrade2017.Models.CountryProfile
{
	public class InfoConsolidadaPais
	{
		public string Titulo { get; set; }
		public string Descripcion { get; set; }
		public string Nota { get; set; }
		public string FechaIni { get; set; }
		public string FechaFin { get; set; }
		public string Culture { get; set; }
		public List<string> ElementosLi { get; set; }

		public string FechaTraduccion{ 
			get{
				return ConvertirFecha(this.FechaIni, this.Culture);
			}
		}

		public List<string> ListaLi
		{
			get
			{
				return ObtenerElementosLi();
			}
		}




		private List<string> ObtenerElementosLi()
		{
			List<string> elementos = new List<string>();

			string fechaFormateadaInicialI = ConvertirFecha(this.FechaIni, this.Culture);
			string fechaFormateadaInicialF = ConvertirFecha(this.FechaFin, this.Culture);

			string patron = @"<li>(.*?)</li>";
			MatchCollection coincidencias = Regex.Matches(this.Descripcion, patron);
			
			foreach (Match coincidencia in coincidencias)
			{

				elementos.Add(coincidencia.Groups[1].Value.Replace("</ul>", "").Replace("<ul>", "").Replace("{0}", fechaFormateadaInicialI).Replace("{1}", fechaFormateadaInicialF));
			}

			return elementos;
		}


		static string ConvertirFecha(string fechaStr, string culture)
		{
			// Convertir la cadena a un objeto DateTime
			DateTime fecha = DateTime.ParseExact(fechaStr, "yyyyMMdd", CultureInfo.InvariantCulture);

			// Diccionario para mapear los números de mes a nombres de meses en español
			string[] meses = {
			"enero", "febrero", "marzo", "abril", "mayo", "junio",
			"julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"
		};

			string[] mesesIngles = {
			"January", "February", "March", "abril", "May", "June",
			"July", "August", "September ", "October", "November ", "December"
		};

			// Formatear la fecha en el formato deseado

			string fechaFormateada = "";
			if (culture == "es")
			{
				return fechaFormateada = string.Format("{0} de {1} del {2}", fecha.Day, meses[fecha.Month - 1], fecha.Year);
			}
			fechaFormateada = string.Format("{0} {1}, {2}", mesesIngles[fecha.Month - 1], fecha.Day, fecha.Year);
			return fechaFormateada;
		}



	}
}