using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{
	public class Region
	{
		public int IdRegion { get; set; }
		public int OrdenRegion { get; set; }
		public string RegionNombre { get; set; }
		public string RegionNombreEn { get; set; }

		public static async Task<List<Region>> GetAllRegiones()
		{
		   List<Region> result = new List<Region>();
			string sqlStr = "SELECT Id,Orden,RegionES,RegionEN FROM dbo.RegionCountryProfile ORDER BY Orden";

			DataTable  dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				Region region = new Region
				{
					IdRegion = Convert.ToInt32(row["Id"]),
					RegionNombre = row["RegionES"].ToString().Trim(),
					RegionNombreEn = row["RegionEN"].ToString().Trim(),
					OrdenRegion = Convert.ToInt32(row["Orden"]),

				};
				result.Add(region);
			}
			return result;
		}


		public static async Task<Region> GetRegionById(int id)
		{
			List<Region> result = new List<Region>();
			string sqlStr = $"SELECT Id,Orden,RegionES,RegionEN FROM dbo.RegionCountryProfile WHERE id ={id}  ORDER BY Orden";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				Region region = new Region
				{
					IdRegion = Convert.ToInt32(row["Id"]),
					RegionNombre = row["RegionES"].ToString().Trim(),
					RegionNombreEn = row["RegionEN"].ToString().Trim(),
					OrdenRegion = Convert.ToInt32(row["Orden"]),

				};
				result.Add(region);
			}
			return result.FirstOrDefault();
		}


		public static async Task<Region> GetRegionByOrden(int orden)
		{
			List<Region> result = new List<Region>();
			string sqlStr = $"SELECT Id,Orden,RegionES,RegionEN FROM dbo.RegionCountryProfile WHERE orden ={orden}  ORDER BY Orden";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				Region region = new Region
				{
					IdRegion = Convert.ToInt32(row["Id"]),
					RegionNombre = row["RegionES"].ToString().Trim(),
					RegionNombreEn = row["RegionEN"].ToString().Trim(),
					OrdenRegion = Convert.ToInt32(row["Orden"]),

				};
				result.Add(region);
			}
			return result.FirstOrDefault();
		}



	}

	


}