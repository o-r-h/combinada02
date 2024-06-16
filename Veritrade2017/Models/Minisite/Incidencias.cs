
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.Minisite
{
	public class Incidencias
	{

		public void SalvarIncidencia(string incidencia)
		{
			var sql = $"INSERT INTO dbo.INCIDENCIAS (TEXTO) VALUES ('{incidencia}')";
			Conexion.SqlExecute(sql);
		}


	}

	
}
