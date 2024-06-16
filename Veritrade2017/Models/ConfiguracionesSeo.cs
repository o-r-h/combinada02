using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Veritrade2017.Controllers;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
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
            // Ruben 202205
            //Functions.GrabaLog("", "", "", "0", "0", "GetConfig", idioma + " | " + uri);

            if(uri != "/" && uri != "/en")
            {
                uri += "%";
            }
            var sql = "SELECT title, description FROM [dbo].[configuraciones_seo] " +
                      "INNER JOIN [dbo].[idiomas] ON [configuraciones_seo].idioma_id = [idiomas].id " +
                      "WHERE [idiomas].nombre = '" + idioma + "' AND [configuraciones_seo].uri like '" + uri + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new ConfiguracionesSeo();
            foreach (DataRow row in dt.Rows)
            {
                model.Title = row["title"].ToString();
                model.Description = row["description"].ToString();

                // Ruben 202205
                //Functions.GrabaLog("", "", "", "", "", "", model.Title + " | " + model.Description);
            }

            return model;
        }

        public static async Task<Int64> GetCountConfigSeo()
        {
            var sql = "SELECT count(*) Cant FROM configuraciones_seo";
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataContentString, sql);

            Int64 cant = 0;
            foreach (DataRow row in dt.Rows)
            {
                cant = Convert.ToInt64(row["Cant"]);
            }
            return cant;
        }

        public static async Task<List<object>> GetListConfigSeo(string url_base)
        {
            var items = new List<object>();
            var sql = "SELECT *  FROM configuraciones_seo where right(uri, '5') <> '.aspx' ";
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataContentString, sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                items.Add(new
                {
                    Nro = (i+1),
                    Title = dt.Rows[i]["title"],
                    Url = url_base + dt.Rows[i]["uri"],
                    Visitada = false, //await Extensiones.UrlIsReachable(url_base + dt.Rows[i]["uri"]),
                });
            }
            return items;
        }

    }
}