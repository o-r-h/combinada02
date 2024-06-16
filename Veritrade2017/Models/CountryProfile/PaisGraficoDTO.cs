using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.CountryProfile
{

	 public class PaisGraficoDTO
    {



		public async Task<List<DataPoint>> GetInfoPaisGrafico(int idPais, string regimen)
		{
			string sqlStr = $"select idpais ,  año, total,  format(total,'0,,') as totalB , regimen FROM Totales_anual where idpais = {idPais} and regimen = '{regimen}' order by año ";
			List<DataPoint> result = new List<DataPoint>();
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);

			foreach (DataRow row in dt.Rows)
			{
				DataPoint obj = new DataPoint
				{
					Value = Convert.ToDecimal(row["totalB"]),
					TotalB = Convert.ToDecimal(row["totalB"]),
					Year = Convert.ToInt32(row["año"])
				};
				result.Add(obj);
			}
			return result;
		}
	}

   
	

}