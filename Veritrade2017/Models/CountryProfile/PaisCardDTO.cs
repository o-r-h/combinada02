using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{
	public class PaisCardDTO
	{
		public int IdPais { get; set; }
		public string NombreCorto { get; set; }
		public string NombreCorto3 { get; set; }
		public string NombreES { get; set; }
		public string NombreEN { get; set; }
		public string BanderaUri { get; set; }
		public int IdRegion { get; set; }
		public decimal ExportadoresP { get; set; }
		public decimal ImportadoresP { get; set; }
		public decimal ExportadoresC { get; set; }
		public decimal ImportadoresC { get; set; }
		public string FriendlyNameES { get; set; }
		public string FriendlyNameEN { get; set; }

		public string ExportadoresPstr { get { return this.ExportadoresP.ToString("#,##0").Replace(".", ","); } }
		public string ImportadoresPstr { get { return this.ImportadoresP.ToString("#,##0").Replace(".", ","); } }
		public string ExportadoresCstr { get { return this.ExportadoresC.ToString("#,##0").Replace(".", ","); } }
		public string ImportadoresCstr { get { return this.ImportadoresC.ToString("#,##0").Replace(".", ","); } }
		public string Archivo { get; set; }

		public string RutaFriendlyNameES { get; set; }
		public string RutaFriendlyNameEN { get; set; }


		public string fullNameBanderaUrl {
			get { return $"~/Content/Images/flags/{this.BanderaUri}"; }
		}

		public async Task<List<PaisCardDTO>> GetAllPaisesByRegionAsync(int idRegion)
		{
			List<PaisCardDTO> result = new List<PaisCardDTO>();

			string sqlStr = $"SELECT PaisesCountryProfile.Id,NombreCorto,NombreCorto3,NombreES,NombreEN,BanderaUri,FriendlyNameES, FriendlyNameEN,IdRegion,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'P' ),0) as ExportadoresP ,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'P' ),0) as ImportadoresP,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'C' ),0) as ExportadoresC,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'C' ),0) as ImportadoresC\r\n  FROM PaisesCountryProfile   \r\n  WHERE IdRegion = {idRegion} order by NombreCorto";



			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				PaisCardDTO obj = new PaisCardDTO
				{
					IdPais = Convert.ToInt32(row["Id"]),
					NombreCorto = row["NombreCorto"].ToString().Trim(),
					NombreCorto3 = row["NombreCorto3"].ToString().Trim(),
					NombreES = row["NombreES"].ToString().Trim(),
					NombreEN = row["NombreEN"].ToString().Trim(),
					BanderaUri = row["BanderaUri"].ToString().Trim(),
					IdRegion = Convert.ToInt32(row["IdRegion"]),
					ImportadoresC = Convert.ToDecimal(row["ImportadoresC"]),
					ExportadoresC = Convert.ToDecimal(row["ExportadoresC"]),
					ImportadoresP = Convert.ToDecimal(row["ImportadoresP"]),
					ExportadoresP = Convert.ToDecimal(row["ExportadoresP"]),
					FriendlyNameES = row["FriendlyNameES"].ToString().Trim(),
					FriendlyNameEN = row["FriendlyNameEN"].ToString().Trim()
				};
				result.Add(obj);
			}
			return result;
		}



		public async Task<GroupListPaisCardDTO> ListadoPaisesCuatroColumnas(int idRegion)
		{
			GroupListPaisCardDTO result = new GroupListPaisCardDTO();

			List<PaisCardDTO> listaOriginal = await GetAllPaisesByRegionAsync(idRegion);
			int partes = 4;
			List<PaisCardDTO> lista0 = new List<PaisCardDTO>();
			List<PaisCardDTO> lista1 = new List<PaisCardDTO>();
			List<PaisCardDTO> lista2 = new List<PaisCardDTO>();
			List<PaisCardDTO> lista3 = new List<PaisCardDTO>();

			//calcular el tamaño de cada lista
			int tamanoParte = (int)Math.Ceiling((double)listaOriginal.Count / partes);

			List<List<PaisCardDTO>> partesLista = listaOriginal
				.Select((item, index) => new { Item = item, Index = index })
				.GroupBy(x => x.Index / tamanoParte)
				.Select(group => group.Select(x => x.Item).ToList())
			.ToList();



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

			result.Lista0 = lista0;
			result.Lista1 = lista1;
			result.Lista2 = lista2;
			result.Lista3 = lista3;
			
			return result;

		}


		public async Task<List<List<PaisCardDTO>>> GetLineaPaisPorRegion(int idRegion, string culture){
			List<PaisCardDTO> result = new List<PaisCardDTO>();

			string sqlStr = $"SELECT PaisesCountryProfile.Id,NombreCorto,NombreCorto3,NombreES,NombreEN,Archivo, FriendlyNameES, FriendlyNameEN,BanderaUri,IdRegion,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'P' ),0) as ExportadoresP ,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'P' ),0) as ImportadoresP,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'C' ),0) as ExportadoresC,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'C' ),0) as ImportadoresC\r\n  FROM PaisesCountryProfile   \r\n  WHERE IdRegion = {idRegion} order by NombreES";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				PaisCardDTO obj = new PaisCardDTO
				{
					IdPais = Convert.ToInt32(row["Id"]),
					NombreCorto = row["NombreCorto"].ToString().Trim(),
					NombreCorto3 = row["NombreCorto3"].ToString().Trim(),
					NombreES = culture =="es" ? row["NombreES"].ToString().Trim() : row["NombreEN"].ToString().Trim(),
					NombreEN = row["NombreEN"].ToString().Trim(),
					BanderaUri = row["BanderaUri"].ToString().Trim(),
					IdRegion = Convert.ToInt32(row["IdRegion"]),
					ImportadoresC = Convert.ToDecimal(row["ImportadoresC"]) ,
					ExportadoresC = Convert.ToDecimal(row["ExportadoresC"]) ,
					ImportadoresP = Convert.ToDecimal(row["ImportadoresP"]) ,
					ExportadoresP = Convert.ToDecimal(row["ExportadoresP"]) ,
					FriendlyNameES = culture == "es" ? row["FriendlyNameES"].ToString().Trim(): row["FriendlyNameEN"].ToString().Trim(),
					FriendlyNameEN = row["FriendlyNameEN"].ToString().Trim(),
					RutaFriendlyNameES = row["FriendlyNameES"].ToString().Trim(),
					RutaFriendlyNameEN = row["FriendlyNameEN"].ToString().Trim(),
					Archivo = row["Archivo"].ToString().Trim()

				};
				result.Add(obj);
			}


			List<List<PaisCardDTO>> sublistas = SplitList(result, 4);
			return sublistas;

		}


		public async Task<PaisCardDTO> GetInfoPais(int idPais, string culture){
			string sqlStr = $"SELECT PaisesCountryProfile.Id,NombreCorto,NombreCorto3,NombreES,NombreEN,FriendlyNameES, Archivo, FriendlyNameEN,BanderaUri,IdRegion,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'P' ),0) as ExportadoresP ,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'P' ),0) as ImportadoresP,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='E' and tipo= 'C' ),0) as ExportadoresC,\r\n  isnull((SELECT Total from Totales where Idpais = id AND Regimen ='I' and tipo= 'C' ),0) as ImportadoresC\r\n  FROM PaisesCountryProfile   \r\n  WHERE PaisesCountryProfile.Id = {idPais} ";
			List<PaisCardDTO> result = new List<PaisCardDTO>();
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				PaisCardDTO obj = new PaisCardDTO
				{
					IdPais = Convert.ToInt32(row["Id"]),
					NombreCorto = row["NombreCorto"].ToString().Trim(),
					NombreCorto3 = row["NombreCorto3"].ToString().Trim(),
					NombreES = culture == "es" ? row["NombreES"].ToString().Trim() : row["NombreEN"].ToString().Trim(),
					NombreEN = row["NombreEN"].ToString().Trim(),
					BanderaUri = row["BanderaUri"].ToString().Trim(),
					IdRegion = Convert.ToInt32(row["IdRegion"]),
					ImportadoresC = Convert.ToDecimal(row["ImportadoresC"]),
					ExportadoresC = Convert.ToDecimal(row["ExportadoresC"]),
					ImportadoresP = Convert.ToDecimal(row["ImportadoresP"]),
					ExportadoresP = Convert.ToDecimal(row["ExportadoresP"]),
					FriendlyNameES = culture == "es" ? row["FriendlyNameES"].ToString().Trim() : row["FriendlyNameEN"].ToString().Trim(),
					FriendlyNameEN = row["FriendlyNameEN"].ToString().Trim(),
					Archivo = row["Archivo"].ToString().Trim(),
					RutaFriendlyNameES = row["FriendlyNameES"].ToString().Trim(),
					RutaFriendlyNameEN = row["FriendlyNameEN"].ToString().Trim()

				};
				result.Add(obj);
			}
			return result.FirstOrDefault();	
		}


		private List<List<T>> SplitList<T>(List<T> source, int chunkSize)
		{
			return source
				.Select((value, index) => new { value, index })
				.GroupBy(x => x.index / chunkSize)
				.Select(group => group.Select(x => x.value).ToList())
				.ToList();
		}



	}

	

	



}