using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class ValorDolar
    {
        public int Id { get; set; }
        public Double valor { get; set; }
        public DateTime fecha { get; set; }
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
    }
}