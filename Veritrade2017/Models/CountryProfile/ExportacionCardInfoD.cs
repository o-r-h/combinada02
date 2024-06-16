
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{
	public class ExportacionCardInfoD
	{

        public int IdCategoria { get; set; }
		public string NombreES { get; set; }
		public string NombreEN { get; set; }
		public int NumeroEmpresa { get; set; }
		public string UriName { get; set; }
		public int IdPais { get; set; }
		public string Regimen { get; set; }
		public string ShortNombreES
		{
			get
			{
				if (NombreES != null)
				{
					if (NombreES.Length > 30)
					{
						return NombreES.Substring(0, 30) + "...";
					}
					else
					{
						return NombreES;
					}
				}
				return "";
			}
		}
		public string ShortNombreEN
		{
			get
			{
				if (NombreEN != null)
				{
					if (NombreEN.Length > 30)
					{
						return NombreEN.Substring(0, 30) + "...";
					}
					else
					{
						return NombreEN;
					}
				}
				return "";
			}
		}

		public string CategoriaSlugES { get; set; }
		public string CategoriaSlugEN { get; set; }

		//EMPRESA EXPORTACION
		public async Task<List<ExportacionCardInfoD>> GetTopEmpresasExportaciones(int idPais, string culture)
		{
			return await ListaEmpresas(idPais, culture,  "E");
		}

		public async Task<List<ExportacionCardInfoD>> GetEmpresasExportacionesporNombreES(int idPais,  string nombre,  string culture)
		{
			return await ListaEmpresasPorNombre(idPais, "E", nombre, culture);
		}

		//PRODUCTO EXPORTACION
		public async Task<List<ExportacionCardInfoD>> GetProductosExportacionesporNombreES(int idPais, string nombre, string culture)
		{
			return await ListaProductosPorNombre(idPais, "E", nombre, culture);
		}

		public async Task<List<ExportacionCardInfoD>> GetTopProductosExportaciones(int idPais, string culture)
		{
			return await ListaProductos(idPais,culture, "E");
		}

		//EMPRESA IMPORTACION
		public async Task<List<ExportacionCardInfoD>> GetTopEmpresasImportaciones(int idPais, string culture)
		{
			return await ListaEmpresas(idPais ,culture, "I");
		}

		public async Task<List<ExportacionCardInfoD>> GetEmpresasImportacionesporNombreES(int idPais, string nombre,  string culture)
		{
			return await ListaEmpresasPorNombre(idPais, "I", nombre, culture);
		}

		//PRODUCTO IMPORTACION
		public async Task<List<ExportacionCardInfoD>> GetProductosImportacionesporNombreES(int idPais, string nombre, string culture)
		{
			return await ListaProductosPorNombre(idPais, "I", nombre, culture);
		}

		public async Task<List<ExportacionCardInfoD>> GetTopProductosImportaciones(int idPais, string culture)
		{
			return await ListaProductos(idPais, culture, "I");
		}





		private static async Task<List<ExportacionCardInfoD>> ListaEmpresas(int id, string culture, string regimen)
		{
			List<ExportacionCardInfoD> result = new List<ExportacionCardInfoD>();
			string sqlStr = $"select id, Paiscorto, Pais, Regimen, idcategoria, CategoriaES, CategoriaEN, Nempresas,CategoriaSlugEN, CategoriaSlugES from  Empresas where id = {id} and Regimen = '{regimen}' order by  Nempresas desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				ExportacionCardInfoD region = new ExportacionCardInfoD
				{
					IdPais = Convert.ToInt32(row["Id"]),
					Regimen = row["Regimen"].ToString().Trim(),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					NombreES = culture == "es" ? row["CategoriaES"].ToString().Trim(): row["CategoriaEN"].ToString().Trim(),
					NombreEN = row["CategoriaEN"].ToString().Trim(),
					NumeroEmpresa =Convert.ToInt32(row["Nempresas"]),
					CategoriaSlugES= culture == "es"? row["CategoriaSlugES"].ToString().Trim(): row["CategoriaSlugEN"].ToString().Trim(),
					CategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim()

				};
				result.Add(region);

			}
			return result;
		}
		private static async Task<List<ExportacionCardInfoD>> ListaProductos(int id, string culture, string regimen)
		{
			List<ExportacionCardInfoD> result = new List<ExportacionCardInfoD>();
			string sqlStr = $"select id, Paiscorto, Pais, Regimen, idcategoria, CategoriaES, CategoriaEN, Nproductos,CategoriaSlugEN, CategoriaSlugES  from  Productos where id = {id} and Regimen = '{regimen}' order by  Nproductos desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				ExportacionCardInfoD region = new ExportacionCardInfoD
				{
					IdPais = Convert.ToInt32(row["Id"]),
					Regimen = row["Regimen"].ToString().Trim(),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					NombreES = culture == "es" ? row["CategoriaES"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					NombreEN = row["CategoriaEN"].ToString().Trim(),
					NumeroEmpresa = Convert.ToInt32(row["Nproductos"]),
					CategoriaSlugES = culture == "es" ? row["CategoriaSlugES"].ToString().Trim() : row["CategoriaSlugEN"].ToString().Trim(),
					CategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim()



				};
				result.Add(region);
			}
			return result;
		}
		private static async Task<List<ExportacionCardInfoD>> ListaEmpresasPorNombre(int id, string regimen, string nombre, string culture)
		{
			List<ExportacionCardInfoD> result = new List<ExportacionCardInfoD>();
			string sqlStr1 = $"select id, Paiscorto, Pais, Regimen, idcategoria, CategoriaES, CategoriaEN, Nempresas,CategoriaSlugEN, CategoriaSlugES  from  Empresas where id = {id} and Regimen = '{regimen}' order by  Nempresas desc";
			string sqlStr2 = culture == "es" ? $" and CategoriaES like '%{nombre}%'" : $" and CategoriaEN like '%{nombre}%'";
			string sqlStr = sqlStr1 + sqlStr2;
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				ExportacionCardInfoD region = new ExportacionCardInfoD
				{
					IdPais = Convert.ToInt32(row["Id"]),
					Regimen = row["Regimen"].ToString().Trim(),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					NombreES = culture == "es" ? row["CategoriaES"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					NombreEN = row["CategoriaEN"].ToString().Trim(),
					NumeroEmpresa = Convert.ToInt32(row["Nempresas"]),
					CategoriaSlugES = culture == "es" ? row["CategoriaSlugES"].ToString().Trim() : row["CategoriaSlugEN"].ToString().Trim(),
					CategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim()


				};
				result.Add(region);
			}
			return result;
		}
		private static async Task<List<ExportacionCardInfoD>> ListaProductosPorNombre(int id, string regimen, string nombre, string culture)
		{
			List<ExportacionCardInfoD> result = new List<ExportacionCardInfoD>();
			string sqlStr1 = $"select id, Paiscorto, Pais, Regimen, idcategoria, CategoriaES, CategoriaEN, Nproductos,CategoriaSlugEN, CategoriaSlugES from  Productos where id = {id} and Regimen = '{regimen}'";
			string sqlStr2 = culture == "es" ? $" and CategoriaES like '%{nombre}%'" : $" and CategoriaEN like '%{nombre}%'";
			string sqlStr = sqlStr1 + sqlStr2;
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				ExportacionCardInfoD region = new ExportacionCardInfoD
				{
					IdPais = Convert.ToInt32(row["Id"]),
					Regimen = row["Regimen"].ToString().Trim(),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					NombreES = culture == "es" ? row["CategoriaES"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					NombreEN = row["CategoriaEN"].ToString().Trim(),
					NumeroEmpresa = Convert.ToInt32(row["Nempresas"]),
					CategoriaSlugES = culture == "es" ? row["CategoriaSlugES"].ToString().Trim() : row["CategoriaSlugEN"].ToString().Trim(),
					CategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim()


				};
				result.Add(region);
			}
			return result;
		}
		private List<ExportacionCardInfoD> MockExportacionLista()
		{
			


			List<ExportacionCardInfoD> result = new List<ExportacionCardInfoD>();
			result.Add(new ExportacionCardInfoD { IdCategoria = 1, NombreES = "Agua", NombreEN = "Agua", NumeroEmpresa = 5, UriName = "urlxxx" , IdPais = 1});
			result.Add(new ExportacionCardInfoD { IdCategoria = 2, NombreES = "Fuego", NombreEN = "Agua",NumeroEmpresa = 15, UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 3, NombreES = "Tierra", NombreEN = "Agua", NumeroEmpresa = 123, UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 4, NombreES = "Aire", NombreEN = "Agua", NumeroEmpresa = 13, UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 5, NombreES = "Papa amarilla royal exportacion ultra", NombreEN = "Agua",NumeroEmpresa = 88, UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 6, NombreES = "Tomate", NumeroEmpresa = 93, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 7, NombreES = "Cebolla", NumeroEmpresa = 63, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 8, NombreES = "Palta", NumeroEmpresa = 45, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 9, NombreES = "Tomate", NumeroEmpresa = 9, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 10, NombreES = "Drones", NumeroEmpresa = 19, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 11, NombreES = "Telefonos", NumeroEmpresa = 44, NombreEN = "Agua", UriName = "urlxxx", IdPais = 1 });


			result.Add(new ExportacionCardInfoD { IdCategoria = 1, NombreES = "Piña", NombreEN = "Agua", NumeroEmpresa = 5, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 2, NombreES = "Golf", NombreEN = "Agua", NumeroEmpresa = 15, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 3, NombreES = "Frezzer", NombreEN = "Agua", NumeroEmpresa = 123, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 4, NombreES = "Congelador", NombreEN = "Agua", NumeroEmpresa = 13, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 5, NombreES = "Ron Pampero Edicion Especial xt", NombreEN = "Agua", NumeroEmpresa = 88, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 6, NombreES = "Tomate", NumeroEmpresa = 93, NombreEN = "Agua", UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 7, NombreES = "Cebolla", NumeroEmpresa = 63, NombreEN = "Agua", UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 8, NombreES = "Palta", NombreEN = "Agua", NumeroEmpresa = 45, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 9, NombreES = "Tomate", NombreEN = "Agua", NumeroEmpresa = 9, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 10, NombreES = "Drones", NombreEN = "Agua", NumeroEmpresa = 19, UriName = "urlxxx", IdPais = 2 });
			result.Add(new ExportacionCardInfoD { IdCategoria = 11, NombreES = "Telefonos", NombreEN = "Agua", NumeroEmpresa = 44, UriName = "urlxxx", IdPais = 2 });

			return result;
		}


	}


	
	

}