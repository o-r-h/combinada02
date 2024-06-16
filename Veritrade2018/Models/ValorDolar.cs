using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class ValorDolar
    {
        public int Id { get; set; }
        public Double Valor { get; set; }
        public DateTime Fecha { get; set; }
        public static Decimal traerValorActual()
        {
            var sql = "SELECT " +
                      "  COALESCE( " +
                      "            ( " +
                      "            SELECT " +
                      "                valor " +
                      "            FROM " +
                      "                valor_dolar " +
                      "            WHERE " +
                      "                fecha = CONVERT(DATE, GETDATE())), " +
                      "           ( " +
                      "               SELECT " +
                      "                   top 1 valor " +
                      "               FROM " +
                      "                   valor_dolar " +
                      "               ORDER BY " +
                      "                   fecha DESC)) valor";

            var dt = Conexion.SqlDataTable(sql, true);
            
            foreach (DataRow row in dt.Rows)
            {
                return Convert.ToDecimal(row["valor"]);
            }
            return 0;
        }

        public static DateTime traerUltimaFecha()
        {
            var sql = "            SELECT " +
                      "                fecha " +
                      "            FROM " +
                      "                valor_dolar " +
                      "            ORDER BY " +
                      "                fecha DESC";

            var dt = Conexion.SqlDataTable(sql, true);

            foreach (DataRow row in dt.Rows)
            {
                return Convert.ToDateTime(row["fecha"]);
            }
            return DateTime.Today;
        }

        public static DateTime traerUltimaFechaIngresada()
        {
            var sql = "            SELECT " +
                      "                fecha " +
                      "            FROM " +
                      "                valor_dolar " +
                      "            ORDER BY " +
                      "                fecha DESC";

            var dt = Conexion.SqlDataTable(sql, true);

            foreach (DataRow row in dt.Rows)
            {
                return Convert.ToDateTime(row["fecha"]);
            }
            return Convert.ToDateTime("2000-01-01");
        }

        public static void guardarValores(IList<ValorDolar> valores)
        {
            var values = "";
            foreach (var valorDolar in valores){
                values = $"('{valorDolar.Fecha.ToString("yyyy-MM-dd")}',{valorDolar.Valor.ToString().Replace(',','.')})";
                if (traerUltimaFechaIngresada() < valorDolar.Fecha) {
                    var sql = $"insert into valor_dolar (fecha, valor) values {values}";
                    Conexion.SqlExecute(sql, true);
                }
            }

            
        }
    }
}