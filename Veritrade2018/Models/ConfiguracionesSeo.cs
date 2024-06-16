using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class ConfiguracionesSeo
    {
        public Idiomas Idiomas { get; set; }
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ConfiguracionesSeo()
        {
            Idiomas = new Idiomas();
        }

        public static ConfiguracionesSeo GetConfig(string idioma, string uri)
        {
            var sql = "SELECT title, description FROM [dbo].[configuraciones_seo] " +
                      "INNER JOIN [dbo].[idiomas] ON [configuraciones_seo].idioma_id = [idiomas].id " +
                      "WHERE [idiomas].nombre = '" + idioma + "' AND [configuraciones_seo].uri like '" + uri + "%'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new ConfiguracionesSeo();
            foreach (DataRow row in dt.Rows)
            {
                model.Title = row["title"].ToString();
                model.Description = row["description"].ToString();
            }

            return model;
        }
    }
}