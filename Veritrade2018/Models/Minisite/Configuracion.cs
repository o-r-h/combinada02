using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Minisite
{
    public class Configuracion
    {
        public int Pais { get; set; }
        public string AbreviaturaPais { get; set; }
        public string Registrotrib { get; set; }

        public static string GetRegistrotrib(string pais)
        {
            var registro = "";
            var sql = "SELECT Registrotrib FROM [Configuracion] Where [Pais] = '" + pais + "'";

            var dt = Conexion.SqlDataTableMinisite(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    registro = row["Registrotrib"].ToString();
                }
            }

            return registro;
        }
    }
}