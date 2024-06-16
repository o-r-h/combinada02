
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.Minisite
{
	public class SubCategoriaEmpresa
	{
		public int Idsubcategoria_expo { get; set; }
		public string Subcategoria { get; set; }
		public string CodigoPais { get; set; }

		public string CustomUrlES{
			get{
				return $"/views/es/example/{Idsubcategoria_expo}";
			}
		}

		public string CustomUrlEN
		{
			get
			{
				return $"/views/en/example/{Idsubcategoria_expo}";
			}
		}

		public List<SubCategoriaEmpresa> GetTodasLasCategorias(string abreviatura)
		{
			var sql = $"select distinct  idsubcategoria_expo, '{abreviatura}' as CodigoPais, subcategoria from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA where categoria is not  null and CP.idsubcategoria_expo=idsubcategoria  and codpais='AR' union select distinct idsubcategoria_impo,'{abreviatura}' as CodigoPais, subcategoria from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA where categoria is not  null and CP.idsubcategoria_impo=idsubcategoria and codpais='{abreviatura}' ";
			var dt = Conexion.SqlDataTableMinisite(sql);
			List<SubCategoriaEmpresa> result = new List<SubCategoriaEmpresa>();
			foreach (DataRow row in dt.Rows)
			{
				var s = new SubCategoriaEmpresa
				{
					Idsubcategoria_expo = Convert.ToInt32(row["idsubcategoria_expo"].ToString()),
					Subcategoria = row["subcategoria"].ToString(),
					CodigoPais = row["CodigoPais"].ToString()

				};
				result.Add(s);
			}

			return result.OrderBy(r=>r.Subcategoria).ToList();
		}
	}
}