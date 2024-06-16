using System;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Configuraciones
    {
        public Idiomas Idiomas { get; set; }
        public string Campo { get; set; }
        public string Valor { get; set; }

        public Configuraciones()
        {
            Idiomas = new Idiomas();
        }

        public static Configuraciones GetConfig(string idioma, string campo)
        {
            var sql = "SELECT campo, valor FROM [dbo].[configuraciones] " +
                      "INNER JOIN [dbo].[idiomas] ON [configuraciones].idioma_id = [idiomas].id " +
                      "WHERE [idiomas].nombre = '" + idioma + "' AND [configuraciones].campo = '" + campo + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new Configuraciones();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Configuraciones
                {
                    Campo = Convert.ToString(row["campo"]),
                    Valor = Convert.ToString(row["valor"])
                };
                model = s;
            }

            return model;
        }
    }
}