using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models.Minisite
{
    public class Empresa
    {
        public int Nro { get; set; }
        public string Uri { get; set; }
        public string RegistroTrib { get; set; }
        public int IdEmpresa { get; set; }
        public string PaisEmpresa { get; set; }
        public string Ruc { get; set; }
        public string Nombre { get; set; }
        public bool UrlVisitadaEs { get; set; }
        public bool UrlVisitadaEn { get; set; }
        public string AbsoluteUrlEs { get; set; }
        public string AbsoluteUrlEn { get; set; }

        // Ruben 202211
        // https://www.veritradecorp.com/es/importaciones-exportaciones-peru/alicorp/ruc-20100055237 // Ruben 202211
        // https://www.veritradecorp.com/es/PERU/importaciones-y-exportaciones-bellucci-international-japan-eirl/RUC-20492112365

        // Ruben 202211
        public static string GetAbsoluteUrl(Empresa obj, string base_url, string culture)
        {
            
            //string label = culture == "es" ? "importaciones-exportaciones" : "imports-exports"; // Ruben 202211
            string label = culture == "es" ? "importaciones-y-exportaciones" : "imports-and-exports";

            //return $"{base_url}/{culture}/{label}-{obj.PaisEmpresa}/{obj.Uri}/{obj.RegistroTrib}-{obj.Ruc}";
            return $"{base_url}/{culture}/{obj.PaisEmpresa}/{label}-{obj.Uri}/{obj.RegistroTrib}-{obj.Ruc}";
        }

        public static bool ExistEmpresa(string slug, string ruc)
        {
            var sql = "SELECT TOP 1 * FROM [Empresa] WHERE Uri = '" + slug + "' And Ruc='" + ruc + "'";
            
            var dt = Conexion.SqlDataTableMinisite(sql);

            return dt.Rows.Count > 0;
        }
        public static Empresa GetEmpresa(string slug, string ruc)
        {
            var sql = "SELECT TOP 1 IdEmpresa, PaisEmpresa, RUC, Empresa as Nombre FROM [Empresa] WHERE Uri = '" + slug + "' and ruc='" + ruc + "'";
            
            var dt = Conexion.SqlDataTableMinisite(sql);

            var modelo = new Empresa();
            foreach (DataRow row in dt.Rows)
            {
                modelo.IdEmpresa = Convert.ToInt32(row["IdEmpresa"].ToString());
                modelo.PaisEmpresa = row["PaisEmpresa"].ToString().Trim();
                modelo.Ruc = row["RUC"].ToString().Trim();
                modelo.Nombre = row["Nombre"].ToString().Trim();
            }

            return modelo;
        }

        public static async Task<Empresa> GetEmpresa(int pos, string pais)
        {
            var sql = "Select * From (SELECT ROW_NUMBER() OVER(ORDER BY IdEmpresa ASC) AS Nro, IdEmpresa, RUC, LOWER(PaisEmpresa) as PaisEmpresa, ltrim(rtrim(Uri)) as Uri, Empresa, LOWER(Registrotrib) as Registrotrib " +
                      "FROM Empresa with(nolock) INNER JOIN Configuracion with(nolock) ON Empresa.PaisEmpresa = Configuracion.Pais where Uri <>'na' and PaisEmpresa='" + pais + "'  ) T Where Nro = " + pos;
            
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataMinisiteString, sql);

            Empresa modelo = null;
            foreach (DataRow row in dt.Rows)
            {
                modelo = new Empresa
                {
                    Nro = Convert.ToInt32(row["Nro"]),
                    IdEmpresa = Convert.ToInt32(row["IdEmpresa"].ToString()),
                    PaisEmpresa = row["PaisEmpresa"].ToString().Trim(),
                    Ruc = row["RUC"].ToString().Trim(),
                    Nombre = row["Empresa"].ToString().Trim(),
                    Uri = row["Uri"].ToString().ToLower().Trim(),
                    RegistroTrib = row["Registrotrib"].ToString().ToLower().Trim()
                };
            }
            return modelo;
        }

        public static async Task<Int64> GetCountEmpresas()
        {            
            var sql = "SELECT count(Empresa.IdEmpresa ) Cant FROM Empresa with(nolock) INNER JOIN Configuracion with(nolock) ON Empresa.PaisEmpresa = Configuracion.Pais ";
            
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataMinisiteString, sql);

            Int64 cant = 0;
            foreach (DataRow row in dt.Rows)
            {
                cant = Convert.ToInt64(row["Cant"]);
            }
            return cant;
        }

        public static async Task<List<Paises>> GetListPais()
        {
            var sql = "select c.*, Cant = (Select count(*) from dbo.Empresa e where e.PaisEmpresa=c.Pais) from dbo.Configuracion c order by 1 asc";
            
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataMinisiteString, sql);

            var lst = new List<Models.Minisite.Paises>();
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(new Paises() { Nombre =  row["Pais"].ToString(), Registros =  row.GetValue<int>("Cant")   } );
            }
            return lst;
        }

        public static BuscaEmpresaModel GetEmpresaFound(string slug, string ruc)
        {
            var sql = "SELECT Top 1 RUC, LOWER(PaisEmpresa) as PaisEmpresa, ltrim(rtrim(Uri)) as Uri, Empresa, LOWER(Registrotrib) as Registrotrib " +
                      "FROM Empresa INNER JOIN Configuracion ON Empresa.PaisEmpresa = Configuracion.Pais " +
                      "WHERE Uri ='" + slug + "' and empresa.ruc='" + ruc + "'";

            var dt = Conexion.SqlDataTableMinisite(sql);

            BuscaEmpresaModel json=null;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json = new BuscaEmpresaModel()
                    {
                        Empresa = dr["Empresa"].ToString().Trim(),
                        Pais = dr["PaisEmpresa"].ToString().Trim(),
                        Ruc = dr["RUC"].ToString().Trim(),
                        Trib = dr["Registrotrib"].ToString().Trim(),
                        Uri = dr["Uri"].ToString().Trim()
                    };
                }
            }

            return json;
        } 

        public static List<object> SearchEmpresa(string slug)
        {
            var sql = "SELECT TOP (1000) RUC, LOWER(PaisEmpresa) as PaisEmpresa, ltrim(rtrim(Uri)) as Uri, Empresa, LOWER(Registrotrib) as Registrotrib " +
                      "FROM Empresa INNER JOIN Configuracion ON Empresa.PaisEmpresa = Configuracion.Pais " +
                      "WHERE Empresa like '%" + slug + "%' OR RUC like '%" + slug + "%' OR PaisEmpresa like '%" + slug + "%'";

            var dt = Conexion.SqlDataTableMinisite(sql);

            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        id = new
                        {
                            uri = dr["Uri"].ToString(),
                            ruc = dr["RUC"].ToString(),
                            trib = dr["Registrotrib"].ToString(),
                            pais = dr["PaisEmpresa"].ToString().ToLower()
                        },
                        value = dr["Empresa"].ToString().Trim() + " - " + dr["PaisEmpresa"]
                    });
                }
            }
            else
            {
                json.Add(new { id = 0, value = "-", ruc = "", uri = "" });
            }

            return json;
        }

    }

}