using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Veritrade2017.Helpers;
using Veritrade2017.Models.Minisite;

namespace Veritrade2017.Models.Minisite
{
	public class ProductosporSubcategoria
	{

		public int Idsubcategoria { get; set; }
		public string Subcategoria { get; set; }
		public string CodProducto { get; set; }
		public string UriES { get; set; }
		public string UriEN { get; set; }
		public string DescripcionES { get; set; }
		public string DescripcionEN { get; set; }
		public string ShortDescripcionES
		{
			get
			{
			  if (DescripcionES != null){
			        if (DescripcionES.Length > 30){
					  return DescripcionES.Substring(0, 30) + "...";
					}else{
					  return DescripcionES;
					}    	
			  }
			  return "";
			} 
		}

		public string ShortDescripcionEN
		{
			get
			{
				if (DescripcionEN != null)
				{
					if (DescripcionEN.Length > 30)
					{
						return DescripcionEN.Substring(0, 30) + "...";
					}
					else
					{
						return DescripcionEN;
					}
				}
				return "";
			}
		}
		



		public List<ProductosporSubcategoria> GetTodasLosProductos(string codPais, int idSubcategoria)
		{
			List<ProductosporSubcategoria> result = new List<ProductosporSubcategoria>();
			var sql = $"select distinct Categoria.idsubcategoria, subcategoria, PP.codProducto,pp.DescripcionES, pp.DescripcionEN, 'https://www.veritradecorp.com/es/'+PaisAduana+'/importaciones-y-exportaciones/'+PP.UriES+'/'+codproducto as uries, 'https://www.veritradecorp.com/en/'+PaisAduana+'/imports-and-exports/'+PP.UriEn+'/'+codproducto  as urien from veritradeproductprofile..Producto PP, Categoria, veritradeproductprofile..paisaduana PA, veritradeproductprofile..Partida partida where categoria is not  null and Categoria.idsubcategoria=PP.idsubcategoria and PA.abreviatura2= '{codPais}' and Categoria.idsubcategoria='{idSubcategoria}'and PA.idpaisaduana=partida.IdpaisAduana and CodProducto=SUBSTRING(partida,1,6) Order by  pp.DescripcionES";

			var dt = Conexion.SqlDataTableMinisite(sql);

			foreach (DataRow row in dt.Rows)
			{
				var s = new ProductosporSubcategoria
				{
					Idsubcategoria = Convert.ToInt32(row["idsubcategoria"].ToString()),
					Subcategoria = row["subcategoria"].ToString(),
					CodProducto = row["codProducto"].ToString(),
					UriES = row["uries"].ToString(),
					UriEN = row["urien"].ToString(),
					DescripcionEN =row["DescripcionEN"].ToString(),
					DescripcionES = row["DescripcionES"].ToString(),
				};
				result.Add(s);
			}


			return result.OrderBy(r=>r.DescripcionES).ToList();

		}



		public ProductosporSubcategoria GetProductoNombre(string codPais, int idSubcategoria)
		{
			List<ProductosporSubcategoria> result = new List<ProductosporSubcategoria>();
			var sql = $"select top 1 Categoria.idsubcategoria, subcategoria, PP.codProducto,pp.DescripcionES, pp.DescripcionEN, 'https://www.veritradecorp.com/es/'+PaisAduana+'/importaciones-y-exportaciones/'+PP.UriES+'/'+codproducto as uries, 'https://www.veritradecorp.com/en/'+PaisAduana+'/imports-and-exports/'+PP.UriEn+'/'+codproducto  as urien from veritradeproductprofile..Producto PP, Categoria, veritradeproductprofile..paisaduana PA, veritradeproductprofile..Partida partida where categoria is not  null and Categoria.idsubcategoria=PP.idsubcategoria and PA.abreviatura2= '{codPais}' and Categoria.idsubcategoria='{idSubcategoria}'and PA.idpaisaduana=partida.IdpaisAduana and CodProducto=SUBSTRING(partida,1,6) Order by  pp.DescripcionES";

			var dt = Conexion.SqlDataTableMinisite(sql);

			foreach (DataRow row in dt.Rows)
			{
				var s = new ProductosporSubcategoria
				{
					Idsubcategoria = Convert.ToInt32(row["idsubcategoria"].ToString()),
					Subcategoria = row["subcategoria"].ToString(),
					CodProducto = row["codProducto"].ToString(),
					UriES = row["uries"].ToString(),
					UriEN = row["urien"].ToString(),
					DescripcionEN = row["DescripcionEN"].ToString(),
					DescripcionES = row["DescripcionES"].ToString(),
				};
				result.Add(s);
			}


			return result[0];

		}
	}
}