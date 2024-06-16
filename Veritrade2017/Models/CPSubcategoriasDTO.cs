using DevExpress.DashboardWeb.Native;
using iTextSharp.text.api;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veritrade2017.Helpers;
using Veritrade2017.Models.CountryProfile;
using static Veritrade2017.Models.CountryProfile.Enums;

namespace Veritrade2017.Models
{
	public class CPSubcategoriasDTO
	{

		public async Task<List<CPSubcategoria>> ListadoSubcategoria(int idPais, string regimen, string culture, TipoSubcateria tipoSubcateria)
		{

			string tablaname = "Productos_subcategoria";
			string campoadicional = ",Nproductos";
			if (tipoSubcateria == TipoSubcateria.Empresas_subcategoria)
			{
				tablaname = "Empresas_subcategoria";
				campoadicional = ",NEmpresas";
			}

			List<CPSubcategoria> result = new List<CPSubcategoria>();
			string sqlStr = $"SELECT idpais,Regimen,idcategoria,CategoriaES,CategoriaEN,Nsubcategorias,valor,CategoriaSlugEN,CategoriaSlugES {campoadicional} FROM {tablaname}  where idpais = {idPais} and Regimen = '{regimen}' order by valor desc";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoria obj = new CPSubcategoria
				{
					IdPais = Convert.ToInt32(row["idpais"]),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					Regimen = regimen,
					CategoriasES = culture == "es" ? row["CategoriaES"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					CategoriasEN = row["CategoriaEN"].ToString().Trim(),
					CategoriaSlugES = culture == "es" ? row["CategoriaSlugES"].ToString().Trim() : row["CategoriaSlugEN"].ToString().Trim(),
					Valor = Convert.ToDecimal(row["valor"]),
					NEmpresas = tipoSubcateria == TipoSubcateria.Empresas_subcategoria? Convert.ToInt32(row["NEmpresas"]): Convert.ToInt32(row["Nproductos"]),
					RutaCategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim(),
					RutaCategoriaSlugES = row["CategoriaSlugES"].ToString().Trim(),
					Nsubcategorias = Convert.ToInt32(row["Nsubcategorias"])
				};
				result.Add(obj);
			}
			return result;
		}



		public async Task<List<CPSubcategoriaDetalleHeader>> ListadoHeaderSubcategoriaDetalle(int idPais, string nombre, string regimen, string culture)
		{

			List<CPSubcategoriaDetalleHeader> result = new List<CPSubcategoriaDetalleHeader>();
			string sqlStr2 = "";
			string sqlStr = $"SELECT        SubCategoriaES, SubcategoriaEN, valor, idcategoria, idpais, Regimen FROM  " +
			$"dbo.Empresas_subcategoria_Detalle WHERE    (NombreES = '{nombre}') AND (idpais = {idPais}) AND (Regimen = '{regimen}') ORDER BY valor";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				 List<CPSubcategociriaDetalleEmpresa>  ccc = new List<CPSubcategociriaDetalleEmpresa>();
				CPSubcategoriaDetalleHeader obj = new CPSubcategoriaDetalleHeader(ccc)
				{
					IdPais = Convert.ToInt32(row["idpais"]),
					Regimen = regimen,
					SubCategoriaES = culture == "es" ? row["SubcategoriaEN"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					SubcategoriaEN = row["SubcategoriaEN"].ToString().Trim(),
					Valor = Convert.ToDecimal(row["valor"]),
					




				};
				result.Add(obj);

				foreach (var item in result)
				{
					sqlStr2 = $"SELECT Empresa,url,url_en,totalEmpresa FROM Empresas_subcategoria_Detalle_url where SubCategoriaES = '{item.SubCategoriaES}' AND idpais = {idPais} AND Regimen = '{regimen}'";
					DataTable dt2 = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr2);
					//item.ListaEmpresas = new List<CPSubcategociriaDetalleEmpresa>();
					foreach (DataRow row2 in dt2.Rows)
					{
						
						CPSubcategociriaDetalleEmpresa obj2 = new CPSubcategociriaDetalleEmpresa
						{
							EmpresaNombre = row2["Empresa"].ToString().Trim(),
							UrlVal = row2["url"].ToString().Trim(),
							UrlValEn = row2["url_en"].ToString().Trim(),
							Valor = Convert.ToDecimal(row2["totalEmpresa"]),
						};
						
						item.ListaEmpresas.Add(obj2);
					}
				}
				
			}

			return result;

		}




		public async Task<List<CPSubcategoria>> GetSubcategoria(int idPais, string regimen, string culture, TipoSubcateria tipoSubcateria,string descripcion)
		{

			string tablaname = "Productos_subcategoria";
			string campoadicional = ",Nproductos";
			string condicion = "";

			if (culture == "es"){
				 condicion = $" and CategoriaSlugES ='{descripcion}'";
			}
			else
			{
				condicion = $"and CategoriaSlugEN ='{descripcion}'";
			}
	

			
			
			
			if (tipoSubcateria == TipoSubcateria.Empresas_subcategoria)
			{
				tablaname = "Empresas_subcategoria";
				campoadicional = ",NEmpresas";
			}

			List<CPSubcategoria> result = new List<CPSubcategoria>();
			string sqlStr = $"SELECT idpais,Regimen,idcategoria,CategoriaES,CategoriaEN,Nsubcategorias,valor,CategoriaSlugEN,CategoriaSlugES {campoadicional} FROM {tablaname}  where idpais = {idPais} and Regimen = '{regimen}' {condicion}";
			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoria obj = new CPSubcategoria
				{
					IdPais = Convert.ToInt32(row["idpais"]),
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					Regimen = regimen,
					CategoriasES = culture == "es" ? row["CategoriaES"].ToString().Trim() : row["CategoriaEN"].ToString().Trim(),
					CategoriasEN = row["CategoriaEN"].ToString().Trim(),
					CategoriaSlugES = culture == "es" ? row["CategoriaSlugES"].ToString().Trim() : row["CategoriaSlugEN"].ToString().Trim(),
					Valor = Convert.ToDecimal(row["valor"]),
					NEmpresas = tipoSubcateria == TipoSubcateria.Empresas_subcategoria ? Convert.ToInt32(row["NEmpresas"]) : Convert.ToInt32(row["Nproductos"]),
					RutaCategoriaSlugEN = row["CategoriaSlugEN"].ToString().Trim(),
					RutaCategoriaSlugES = row["CategoriaSlugES"].ToString().Trim()
				};
				result.Add(obj);
			}
			return result;
		}




		public async Task<List<CPSubcategoriaDetalleHeaderSolo>> ListadoSubcategoriaDetalleHeader(int idPais, string regimen, string culture, string nombreSubCategoria , TipoSubcateriaDetalle tipoSubcateria)
		{

			string tablaname = "Productos_subcategoria_detalle";
			string campoadicional = "TotalProducto";
			string busquedaCustom = $" scu.NombreSlugES = '{nombreSubCategoria}'";

			if (culture != "es"){
				busquedaCustom = $" scu.NombreSlugEN = '{nombreSubCategoria}'";
			}


			if (tipoSubcateria == TipoSubcateriaDetalle.Empresas_subcategoria_detalle)
			{
				tablaname = "Empresas_subcategoria_detalle";
				campoadicional = "totalEmpresa";
			}


			List<CPSubcategoriaDetalleHeaderSolo> result = new List<CPSubcategoriaDetalleHeaderSolo>();
		
			string sqlStr2 = $"select idcategoria,  idsubcategoria,SubCategoriaES, SubcategoriaEN, cantEmpresas,TotalSuma FROM (SELECT scu.idcategoria, scu.idsubcategoria, scu.SubCategoriaES, scu.SubCategoriaEN, count(scu.idsubcategoria) cantEmpresas ,sum(scu.{campoadicional}) as TotalSuma FROM   {tablaname}_url scu WHERE  {busquedaCustom}  AND scu.idpais = {idPais} AND scu.Regimen = '{regimen}'  group by scu.idcategoria,  scu.idsubcategoria, scu.SubCategoriaES,  scu.SubCategoriaEN, scu.idsubcategoria) A order by TotalSuma desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr2);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoriaDetalleHeaderSolo obj = new CPSubcategoriaDetalleHeaderSolo
				{
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					IdSubcategoria = Convert.ToInt32(row["idsubcategoria"]),
					SubCategoriaES = culture == "es" ? row["SubCategoriaES"].ToString().Trim() : row["SubCategoriaEN"].ToString().Trim(),
					CantEmpresas =  Convert.ToInt32(row["cantEmpresas"]),
					TotalSuma = Convert.ToDecimal(row["TotalSuma"])
				};
				result.Add(obj);
			}
			return result;
		}


		public async Task<List<CPSubcategoriaDetalleUrlSolo>> ListadoSubcategoriaDetalleEmpresaUrlSolo(int idPais, string regimen, string culture, int idcategoria)
		{


			List<CPSubcategoriaDetalleUrlSolo> result = new List<CPSubcategoriaDetalleUrlSolo>();
			string sqlStr = $"SELECT distinct scu.idcategoria,  scu.idsubcategoria,scu.Empresa, scu.totalEmpresa,scu.url, scu.Url_en FROM  dbo.Empresas_subcategoria_Detalle sc inner join Empresas_subcategoria_Detalle_url scu on scu.idsubcategoria  = scu.idsubcategoria WHERE scu.idcategoria={idcategoria}  AND scu.idpais = {idPais} AND scu.Regimen = '{regimen}' ORDER BY idsubcategoria, TotalEmpresa desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoriaDetalleUrlSolo obj = new CPSubcategoriaDetalleUrlSolo
				{
					IdCategoria = idcategoria,
					IdSubcategoria = Convert.ToInt32(row["idsubcategoria"]),
					Nombre =  row["Empresa"].ToString().Trim(),
					Total = Convert.ToDecimal(row["totalEmpresa"]),
					UrlES = culture == "es" ? row["url"].ToString().Trim() : row["Url_en"].ToString().Trim(),
					UrlEN = row["Url_en"].ToString().Trim()
				};
				result.Add(obj);
			}
			return result;
		}

		public async Task<List<CPSubcategoriaDetalleUrlSolo>> ListadoSubcategoriaDetalleProductoUrlSolo(int idPais, string regimen, string culture, int idcategoria)
		{


			List<CPSubcategoriaDetalleUrlSolo> result = new List<CPSubcategoriaDetalleUrlSolo>();
			string sqlStr = $"SELECT distinct scu.idcategoria, scu.idsubcategoria,scu.descripcionES, scu.descripcionEN, sum(scu.TotalProducto) as Total,scu.url, scu.Url_en FROM  dbo.Productos_subcategoria_Detalle sc RIGHT OUTER JOIN  Productos_subcategoria_Detalle_url scu on sc.idsubcategoria  = scu.idsubcategoria WHERE  scu.idcategoria={idcategoria}  AND scu.idpais = {idPais} AND scu.Regimen = '{regimen}'  group by scu.idcategoria,  scu.idsubcategoria,scu.descripcionES,scu.descripcionEN, scu.url, scu.Url_en ORDER BY idsubcategoria desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoriaDetalleUrlSolo obj = new CPSubcategoriaDetalleUrlSolo
				{
					IdCategoria = idcategoria,
					IdSubcategoria = Convert.ToInt32(row["idsubcategoria"]),
					Nombre = culture == "es" ? row["descripcionES"].ToString().Trim() : row["descripcionEN"].ToString().Trim(),
					Total = Convert.ToDecimal(row["Total"]),
					UrlES = culture == "es" ? row["url"].ToString().Trim() : row["Url_en"].ToString().Trim(),
					UrlEN = row["Url_en"].ToString().Trim()
				};
				result.Add( obj);
			}
			return result;
		}


		public async Task<List<NombresItems>> VerificarIdiomaCategoria(int idPais)
		{


			List<NombresItems> result = new List<NombresItems>();
			string sqlStr = $"select distinct NombreSlugEN, NombreSlugES from  dbo.Empresas_subcategoria_Detalle where idpais = {idPais} ";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				NombresItems obj = new NombresItems
				{
					NombreEspanol = row["NombreSlugES"].ToString().Trim(),
					NombreIngles = row["NombreSlugEN"].ToString().Trim(),
				};
					result.Add(obj); 
			}
			return result;
		}

		public async Task<List<NombresItems>> VerificarIdiomaCategoriaProductos(int idPais)
		{


			List<NombresItems> result = new List<NombresItems>();
			string sqlStr = $"select distinct NombreSlugEN, NombreSlugES from  dbo.Productos_subcategoria_Detalle where idpais = {idPais} ";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				NombresItems obj = new NombresItems
				{
					NombreEspanol = row["NombreSlugES"].ToString().Trim(),
					NombreIngles = row["NombreSlugEN"].ToString().Trim(),
				};
				result.Add(obj);
			}
			return result;
		}



		public async Task<List<CPSubcategoriaDetalleHeaderSolo>> ListadoProductoSubcategoriaDetalleHeader(int idPais, string regimen, string culture, string nombreSubCategoria, TipoSubcateriaDetalle tipoSubcateria)
		{

			string busquedaCustom = $" sc.NombreSlugES = '{nombreSubCategoria}'";

			if (culture != "es")
			{
				busquedaCustom = $" sc.NombreSlugEN = '{nombreSubCategoria}'";
			}

			List<CPSubcategoriaDetalleHeaderSolo> result = new List<CPSubcategoriaDetalleHeaderSolo>();
			string sqlStr = $"select idcategoria,  idsubcategoria,SubCategoriaES, SubcategoriaEN, cantEmpresas,TotalSuma FROM (SELECT scu.idcategoria, scu.idsubcategoria, scu.SubCategoriaES, scu.SubCategoriaEN, count(scu.idsubcategoria) cantEmpresas ,sum(scu.TotalProducto) as TotalSuma FROM  dbo.Productos_subcategoria_detalle sc RIGHT OUTER JOIN  Productos_subcategoria_detalle_url scu on sc.idsubcategoria  = scu.idsubcategoria WHERE   {busquedaCustom} AND sc.idpais = {idPais} AND sc.Regimen = '{regimen}' group by scu.idcategoria,  scu.idsubcategoria, scu.SubCategoriaES,  scu.SubCategoriaEN, scu.idsubcategoria) A order by TotalSuma desc";

			DataTable dt = await Conexion.SqlDataTableAsync(Conexion.DataCountryProfileString, sqlStr);
			foreach (DataRow row in dt.Rows)
			{
				CPSubcategoriaDetalleHeaderSolo obj = new CPSubcategoriaDetalleHeaderSolo
				{
					IdCategoria = Convert.ToInt32(row["idcategoria"]),
					IdSubcategoria = Convert.ToInt32(row["idsubcategoria"]),
					SubCategoriaES = culture == "es" ? row["SubCategoriaES"].ToString().Trim() : row["SubCategoriaEN"].ToString().Trim(),
					CantEmpresas = Convert.ToInt32(row["cantEmpresas"]),
					TotalSuma = Convert.ToDecimal(row["TotalSuma"])
				};
				result.Add(obj);
			}
			return result;
		}


	


	}
}