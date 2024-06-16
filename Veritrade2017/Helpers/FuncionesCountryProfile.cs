using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Veritrade2017.Helpers
{
	public class FuncionesCountryProfile
	{

		public static string SearchPassword(string codUsuario)
		{
			string password = "";
			var sql = $"select password from Usuario where codusuario = '{codUsuario}'";

			var dt = Conexion.SqlDataTableCountryProfile(sql);
			foreach (DataRow row in dt.Rows)
			{
				password = row["password"].ToString();
			}

			return password;
		}

	}
}