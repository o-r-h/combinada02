using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string PaisEmpresa { get; set; }
        public string Ruc { get; set; }
        public string Nombre { get; set; }

        public static bool ExistEmpresa(string slug, string ruc)
        {
            var sql = "SELECT TOP 1 * FROM [Empresa] WHERE Uri = '" + slug + "' and Ruc='"+ ruc +"' ";
            var dt = Conexion.SqlDataTableMinisite(sql);
            return dt.Rows.Count > 0;
        }

        public static Empresa GetEmpresa(string slug, string ruc)
        {
            var sql = "SELECT TOP 1 IdEmpresa, PaisEmpresa, RUC, Empresa as Nombre FROM [Empresa] WHERE Uri = '" + slug + "' AND Ruc='"+ ruc +"' ";
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