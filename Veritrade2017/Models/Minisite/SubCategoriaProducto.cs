//using DevExpress.Office.Utils;
using DevExpress.Web.Mvc.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.Minisite
{
	public class SubCategoriaProducto
	{
        public int IdSubcategoria { get; set; }
		public string SubCategoria { get; set; }
		public string CodigoPais { get; set; }


		public string CustomUrlES
		{
			get
			{
				return $"/views/es/example/{IdSubcategoria}";
			}
		}

		public string CustomUrlEN
		{
			get
			{
				return $"/views/en/example/{IdSubcategoria}";
			}
		}

		public  List<SubCategoriaProducto> GetTodasLasCategorias(string abreviatura)
		{
			var sql = $"select distinct Categoria.idsubcategoria, subcategoria, '{abreviatura}' as CodigoPais from veritradeproductprofile..Producto PP, Categoria, veritradeproductprofile..paisaduana PA, veritradeproductprofile..Partida partida where categoria is not null and Categoria.idsubcategoria = PP.idsubcategoria and PA.abreviatura2 = '{abreviatura}' and PA.idpaisaduana = partida.IdpaisAduana and CodProducto = SUBSTRING(partida, 1, 6)";
			var dt = Conexion.SqlDataTableMinisite(sql);
			List<SubCategoriaProducto> result = new List<SubCategoriaProducto>();
			foreach (DataRow row in dt.Rows)
			{
				var s = new SubCategoriaProducto
				{
					IdSubcategoria = Convert.ToInt32(row["idsubcategoria"].ToString()),
					SubCategoria = row["subcategoria"].ToString(),
					CodigoPais = row["CodigoPais"].ToString()

				};
				result.Add(s);
			}

			return result.OrderBy(r=>r.SubCategoria).ToList();
		}
	}

	
}