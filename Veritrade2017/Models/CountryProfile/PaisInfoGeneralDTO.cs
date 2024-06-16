using Microsoft.Web.Services3.Addressing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{
	public class PaisInfoGeneralDTO
	{

        public int IdPais { get; set; }
		public int TotalEmpresasCantidad { get; set; }
		public int TotalProductosCantidad { get; set; }

		public int EquivServicioES { get; set; }
		public int EquivServicioEN { get; set;}


		public  async Task<PaisInfoGeneralDTO> GetData(int idPais)
		{
			List<PaisInfoGeneralDTO> result = new List<PaisInfoGeneralDTO>();
			string sqlStr = $"SELECT TotalEmpresasCantidad,TotalProductosCantidad\r\n  FROM Totales_suma where Idpais = {idPais}";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				PaisInfoGeneralDTO obj = new PaisInfoGeneralDTO
				{
					IdPais = idPais,
					TotalEmpresasCantidad = Convert.ToInt32(row["TotalEmpresasCantidad"]),
					TotalProductosCantidad = Convert.ToInt32(row["TotalProductosCantidad"]),
					

				};
				result.Add(obj);
			}
			return result.FirstOrDefault();
		}

		public async Task<int> GetId(string culture, string friendlyName)
		{
			List<int> result = new List<int>();
			string nombreCampo = culture == "es" ? "FriendlyNameES" : "FriendlyNameEN";
			string sqlStr = $"SELECT id  FROM PaisesCountryProfile where {nombreCampo} ='{friendlyName}'";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			
			foreach (DataRow row in dt.Rows)
			{
				result.Add(Convert.ToInt32(row["id"]));
			};
				
			return result.FirstOrDefault();
		}

		
		public async Task<List<BannerGeneral>> ListadoBannerProductosRelacionados(int idPais, string culture) {
			List<BannerGeneral> result = new List<BannerGeneral>();
			string sqlStr = $"select idpais, tipo, texto,url ,texto_en, url_en from marquesina where Idpais={idPais} and tipo = 'P'";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				BannerGeneral obj = new BannerGeneral
				{
					IdPais = idPais,
					NombreES = culture == "es" ? row["texto"].ToString().Trim() : row["texto_en"].ToString().Trim(),
					NombreEN = row["texto"].ToString().Trim(),
					//Uri = row["url"].ToString().Trim()
					Uri = culture == "es" ? row["url"].ToString().Trim() : row["url_en"].ToString().Trim()

				};
				result.Add(obj);
			}
			List<BannerGeneral> mergedList = new List<BannerGeneral>();
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			return mergedList;
			
			//List<BannerGeneral> lista = new List<BannerGeneral>();
			//lista.Add(new BannerGeneral { NombreES = "La Abeja", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Umbrella", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Stark ", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "ACME", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "WarnerBross", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Nintendo", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Sony", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Toyota", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });

			//lista.Add(new BannerGeneral { NombreES = "Barajas", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Token", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Sea doo ", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Viral", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Play Hard", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Loquendo", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Alfajorx", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Ford", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });


			//return lista.Where(r => r.IdPais == idPais).ToList();
		}

		public async Task<List<BannerGeneral>> ListadoBannerEmpresasRelacionadas(int idPais, string culture)
		{

			List<BannerGeneral> result = new List<BannerGeneral>();
			string sqlStr = $"select idpais, tipo, texto, texto_en,url, url_en from marquesina where Idpais={idPais} and tipo = 'C'";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				BannerGeneral obj = new BannerGeneral
				{
					IdPais = idPais,
					NombreES = culture =="es" ? row["texto"].ToString().Trim(): row["texto_en"].ToString().Trim(),
					NombreEN = row["texto"].ToString().Trim(),
					//Uri =   row["url"].ToString().Trim()
					Uri = culture == "es" ? row["url"].ToString().Trim() : row["url_en"].ToString().Trim()

				};
				result.Add(obj);
			}
			List<BannerGeneral> mergedList = new List<BannerGeneral>();
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			mergedList.AddRange(result);
			return mergedList;


			//lista.Add(new BannerGeneral { NombreES = "Papas", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Cebollas", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Cable electrico", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Condensador de flujo electrotrifasico", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Chaquetas", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Hierro", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Sodio", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Paño de lana", NombreEN = "The bee", IdPais = 1, Uri = "www.abeja.com" });

			//lista.Add(new BannerGeneral { NombreES = "Papel tipografico A4", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Libros", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Piel de vaca curtida ", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Flores", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Protectores de voltaje", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Eje camion Ford ", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Medicinas", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//lista.Add(new BannerGeneral { NombreES = "Gel Cabello", NombreEN = "The bee", IdPais = 2, Uri = "www.abeja.com" });
			//return lista.Where(r => r.IdPais == idPais).ToList();
		}



		public async Task<InfoConsolidadaPais> GetDataConsolidada(int idPais, string culture)
		{
			List<PaisInfoGeneralDTO> result = new List<PaisInfoGeneralDTO>();
			string sqlStr = $"SELECT id, EquivServicioES, EquivServicioEN  FROM PaisesCountryProfile  where Id = {idPais}";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				PaisInfoGeneralDTO obj = new PaisInfoGeneralDTO
				{
					IdPais = idPais,
					EquivServicioES = Convert.ToInt32(row["EquivServicioES"]),
					EquivServicioEN = Convert.ToInt32(row["EquivServicioEN"]),


				};
				result.Add(obj);
			}


			int idioma = culture != "es"? result[0].EquivServicioEN : result[0].EquivServicioES;
			List<InfoConsolidadaPais> result2 = new List<InfoConsolidadaPais>();
			sqlStr = $"SELECT  titulo, descripcionCP, archivo, nota, codInfo,codInfo, BD.FechaIni,BD.FechaFin FROM veritradecontent..servicios_pais_detalle SPD INNER JOIN VeritradeBusiness..basedatos BD ON SPD.codInfo = CodPais where  servicios_pais_id ={idioma}";
			DataTable dt2 = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt2.Rows)
			{
				InfoConsolidadaPais obj2 = new InfoConsolidadaPais
				{
					Titulo = (row["titulo"]).ToString(),
					Descripcion  = (row["descripcionCP"]).ToString(),
					FechaIni = (row["FechaIni"]).ToString(),
					FechaFin = (row["FechaFin"]).ToString(),
					Nota = (row["Nota"]).ToString(),
					Culture = culture
				

				};
				result2.Add(obj2);
			}

			if (result2.Count > 0) {
				return result2.FirstOrDefault();
			}

			
			
			sqlStr = $"SELECT  titulo, descripcion, archivo, nota, codInfo,codInfo, BD.FechaIni,BD.FechaFin FROM veritradecontent..servicios_pais_detalle SPD INNER JOIN VeritradeBusiness..basedatos BD ON SPD.codInfo = CodPais where codinfo = 'UE'";
			DataTable dt3 = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			List<InfoConsolidadaPais> result3 = new List<InfoConsolidadaPais>();
			foreach (DataRow row in dt3.Rows)
			{
				InfoConsolidadaPais obj3 = new InfoConsolidadaPais
				{
					Titulo = (row["titulo"]).ToString(),
					Descripcion = (row["descripcionCP"]).ToString(),
					FechaIni = (row["FechaIni"]).ToString(),
					FechaFin = (row["FechaFin"]).ToString(),
					Nota = (row["Nota"]).ToString(),
					Culture = culture


				};
				result3.Add(obj3);
			}

			if (culture =="es"){
				return result3[0];
			}

			return result3[1];
		}

	}
}