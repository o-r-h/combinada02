//using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.Minisite
{
	public class EmpresasPorSubcategoria
	{
	public int Idsubcategoria_expo { get; set; }
	public string Subcategoria { get; set; }
	public string Empresa { get; set; }
	public string UriES { get; set; }
	public string UriEN { get; set; }

	public List<EmpresasPorSubcategoria> GetTodasLasEmpresas(string codPais, int idSubcategoria){
	   List<EmpresasPorSubcategoria> result = new List<EmpresasPorSubcategoria> ();

			var sql = $"select distinct idsubcategoria_expo, subcategoria, empresa,'https://www.veritradecorp.com/es/' + lower(pais) + '/importaciones-y-exportaciones-' + uri + '/' + registrotrib + '-' + RUC as uries, 'https://www.veritradecorp.com/en/' + lower(pais) + '/imports-and-exports-' + uri + '/' + registrotrib + '-' + RUC as urien from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA, Configuracion where categoria is not null and CP.idsubcategoria_expo = idsubcategoria and codpais = '{codPais}' and Categoria.idsubcategoria ={ idSubcategoria} and Configuracion.AbreviaturaPais = '{codPais}' union select distinct idsubcategoria_impo, subcategoria, empresa,'https://www.veritradecorp.com/es/' + lower(pais) + '/importaciones-y-exportaciones-' + uri + '/' + registrotrib + '-' + RUC as uries, 'https://www.veritradecorp.com/en/' + lower(pais) + '/imports-and-exports-' + uri + '/' + registrotrib + '-' + RUC as urien from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA, Configuracion where categoria is not null and CP.idsubcategoria_impo = idsubcategoria and codpais = '{codPais}'  and Categoria.idsubcategoria ={ idSubcategoria} and Configuracion.AbreviaturaPais = '{codPais}' ";

			var dt = Conexion.SqlDataTableMinisite(sql);
			
			foreach (DataRow row in dt.Rows)
			{
				var s = new EmpresasPorSubcategoria
				{
					Idsubcategoria_expo = Convert.ToInt32(row["idsubcategoria_expo"].ToString()),
					Subcategoria = row["subcategoria"].ToString(),
					Empresa = row["empresa"].ToString(),
					UriES = row["uries"].ToString(),
					UriEN = row["urien"].ToString()
				};
				result.Add(s);
			}


			return result;

	}


		public EmpresasPorSubcategoria GetEmpresaCategoriaNombre(string codPais, int idSubcategoria)
		{
			List<EmpresasPorSubcategoria> result = new List<EmpresasPorSubcategoria>();

			var sql = $"select Top 1  idsubcategoria_expo, subcategoria, empresa,'https://www.veritradecorp.com/es/' + lower(pais) + '/importaciones-y-exportaciones-' + uri + '/' + registrotrib + '-' + RUC as uries, 'https://www.veritradecorp.com/en/' + lower(pais) + '/imports-and-exports-' + uri + '/' + registrotrib + '-' + RUC as urien from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA, Configuracion where categoria is not null and CP.idsubcategoria_expo = idsubcategoria and codpais = '{codPais}' and Categoria.idsubcategoria ={idSubcategoria} and Configuracion.AbreviaturaPais = '{codPais}' union select Top 1   idsubcategoria_impo, subcategoria, empresa,'https://www.veritradecorp.com/es/' + lower(pais) + '/importaciones-y-exportaciones-' + uri + '/' + registrotrib + '-' + RUC as uries, 'https://www.veritradecorp.com/en/' + lower(pais) + '/imports-and-exports-' + uri + '/' + registrotrib + '-' + RUC as urien from Empresa CP, Categoria , veritradeproductprofile..paisaduana PA, Configuracion where categoria is not null and CP.idsubcategoria_impo = idsubcategoria and codpais = '{codPais}'  and Categoria.idsubcategoria ={idSubcategoria} and Configuracion.AbreviaturaPais = '{codPais}' ";

			var dt = Conexion.SqlDataTableMinisite(sql);

			foreach (DataRow row in dt.Rows)
			{
				var s = new EmpresasPorSubcategoria
				{
					Idsubcategoria_expo = Convert.ToInt32(row["idsubcategoria_expo"].ToString()),
					Subcategoria = row["subcategoria"].ToString(),
					Empresa = row["empresa"].ToString(),
					UriES = row["uries"].ToString(),
					UriEN = row["urien"].ToString()
				};
				result.Add(s);
			}


			return result[0];

		}

	}
}