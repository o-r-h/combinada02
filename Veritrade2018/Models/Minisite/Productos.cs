using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Productos
    {
        public int IdEmpresa { get; set; }
        public string Regimen { get; set; }
        public string Nandina { get; set; }
        public string Partida { get; set; }
        public decimal Valor { get; set; }
        public int Registros { get; set; }
        public double Porcentaje { get; set; }

        public static Productos GetMaxExport(int empresa)
        {
            var sql = "SELECT TOP (1) Nandina, Partida, Porcentaje  FROM [Productos] where idempresa = " + empresa +
                " ORDER BY Porcentaje DESC";
            var dt = Conexion.SqlDataTableMinisite(sql);

            var modelo = new Productos();
            foreach (DataRow row in dt.Rows)
            {
                modelo.Nandina = row["Nandina"].ToString().Trim();
                modelo.Partida = row["Partida"].ToString().Trim();
                modelo.Porcentaje = Convert.ToDouble(row["Porcentaje"].ToString());
            }

            return modelo;
        }

        public static List<Productos> GetData(int empresa, int tipo = 0)
        {
            var promedio = new List<Productos>();

            string sql;
            if (tipo == 0)
            {
                sql = "SELECT TOP (6) * FROM [Productos] WHERE Regimen = 'Importaciones' AND Idempresa = " + empresa;
            }
            else
            {
                sql = "SELECT TOP (6) * FROM [Productos] WHERE Regimen = 'Exportaciones' AND Idempresa = " + empresa;
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            foreach (DataRow row in dt.Rows)
            {
                var s = new Productos
                {
                    IdEmpresa = Convert.ToInt32(row["idempresa"].ToString()),
                    Regimen = row["Regimen"].ToString(),
                    Nandina = row["Nandina"].ToString(),
                    Partida = row["Partida"].ToString(),
                    Valor = Convert.ToDecimal(row["Valor"].ToString()),
                    Registros = Convert.ToInt32(row["Registros"].ToString()),
                    Porcentaje = Convert.ToDouble(row["Porcentaje"].ToString())
                };
                promedio.Add(s);
            }

            return promedio;
        }
    }
}