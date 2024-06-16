using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Paises
    {
        public int IdEmpresa { get; set; }
        public string Regimen { get; set; }
        public string Nombre { get; set; }
        public decimal Valor { get; set; }
        public int Registros { get; set; }
        public double Porcentaje { get; set; }

        public static List<Paises> GetData(int empresa, int tipo = 0)
        {
            var promedio = new List<Paises>();

            string sql;
            if (tipo == 0)
            {
                sql = "SELECT TOP (6) * FROM [Paises] WHERE Regimen = 'Importaciones' AND Idempresa = " + empresa;
            }
            else
            {
                sql = "SELECT TOP (6) * FROM [Paises] WHERE Regimen = 'Exportaciones' AND Idempresa = " + empresa;
            }

            var dt = Conexion.SqlDataTableMinisite(sql);
            foreach (DataRow row in dt.Rows)
            {
                var s = new Paises
                {
                    IdEmpresa = Convert.ToInt32(row["Idempresa"].ToString()),
                    Regimen = row["Regimen"].ToString(),
                    Nombre = row["Paises"].ToString(),
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