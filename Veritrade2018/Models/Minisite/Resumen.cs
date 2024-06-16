using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Resumen
    {
        public int IdEmpresa { get; set; }
        public string Regimen { get; set; }
        public string Periodo { get; set; }
        public decimal Valor { get; set; }
        public int Embarques { get; set; }

        public static double GetPromedios(int empresa, int tipo = 0)
        {
            var promedio = 0.00;
            string sql;

            if (tipo == 0)
            {
                sql = "SELECT ISNULL(SUM(Valor) / COUNT(Valor), 0.00) as 'Promedio' FROM [Resumen] WHERE Regimen = 'Importaciones' AND Idempresa = " + empresa;
            }
            else
            {
                sql = "SELECT ISNULL(SUM(Valor) / COUNT(Valor), 0.00) as 'Promedio' FROM [Resumen] WHERE Regimen = 'Exportaciones' AND Idempresa = " + empresa;
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            foreach (DataRow row in dt.Rows)
            {
                promedio = Math.Round(Convert.ToDouble(row["Promedio"].ToString()));
            }

            return promedio;
        }

        public static List<Resumen> GetData(int empresa, int tipo = 0)
        {
            var promedio = new List<Resumen>();
            string sql;

            if (tipo == 0)
            {
                sql = "SELECT * FROM [Resumen] WHERE Regimen = 'Importaciones' AND Idempresa = " + empresa + " ORDER BY Periodo";
            }
            else
            {
                sql = "SELECT * FROM [Resumen] WHERE Regimen = 'Exportaciones' AND Idempresa = " + empresa + " ORDER BY Periodo";
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var s = new Resumen
                    {
                        IdEmpresa = Convert.ToInt32(row["Idempresa"].ToString()),
                        Regimen = row["Regimen"].ToString(),
                        Periodo = row["Periodo"].ToString(),
                        Valor = Convert.ToDecimal(row["Valor"].ToString()),
                        Embarques = Convert.ToInt32(row["Embarques"].ToString())
                    };
                    promedio.Add(s);
                }
            }

            return promedio;
        }

        public static int GetTotalEmbarques(int empresa)
        {
            var total = 0;
            var sql = "SELECT ISNULL(SUM(Embarques),0) as 'Embarques' FROM [Resumen] where Idempresa = " + empresa + " AND Periodo > YEAR(GETDATE()) - 5";

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToInt32(row["Embarques"].ToString());
                }
            }

            return total;
        }

        public static int GetTipoEmbarques(int empresa, int tipo = 0)
        {
            var total = 0;

            string sql;
            if (tipo == 0)
            {
                sql = "SELECT ISNULL(SUM(Embarques),0) as 'Embarques' FROM [Resumen] where Idempresa = " + empresa +
                    " AND Periodo > YEAR(GETDATE()) - 5 AND Regimen = 'Importaciones'";
            }
            else
            {
                sql = "SELECT ISNULL(SUM(Embarques),0) as 'Embarques' FROM [Resumen] where Idempresa = " + empresa +
                    " AND Periodo > YEAR(GETDATE()) - 5 AND Regimen = 'Exportaciones'";
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToInt32(row["Embarques"].ToString());
                }
            }

            return total;
        }

        public static double TotalTipoEmbarques(int empresa, int tipo = 0)
        {
            var total = 0.00;

            string sql;
            if (tipo == 0)
            {
                sql = "SELECT ISNULL(SUM(Valor),0) as 'Valor' FROM [Resumen] where Idempresa = " + empresa +
                      " AND Periodo > YEAR(GETDATE()) - 5 AND Regimen = 'Importaciones'";
            }
            else
            {
                sql = "SELECT ISNULL(SUM(Valor),0) as 'Valor' FROM [Resumen] where Idempresa = " + empresa +
                      " AND Periodo > YEAR(GETDATE()) - 5 AND Regimen = 'Exportaciones'";
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToDouble(row["Valor"].ToString());
                }
            }

            return total;
        }
    }
}