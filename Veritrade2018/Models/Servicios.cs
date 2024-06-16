using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Servicios
    {
        public int Id { get; set; }
        public Idiomas Idiomas { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Contenido { get; set; }
        public string Slug { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string ImgClass { get; set; }

        public Servicios()
        {
            Idiomas = new Idiomas();
        }

        public List<Servicios> GetList(string idioma)
        {
            var sql = "SELECT [servicios].*  FROM [servicios] INNER JOIN [idiomas] " +
                      "ON [idiomas].id = [servicios].idioma_id WHERE[idiomas].nombre = '" + idioma + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<Servicios>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Servicios
                {
                    Titulo = Convert.ToString(row["titulo"]),
                    Descripcion = Convert.ToString(row["descripcion"]),
                    Imagen = Convert.ToString(row["imagen"]),
                    Slug = Convert.ToString(row["slug"])
                };
                model.Add(s);
            }

            return model;
        }

        public List<Servicios> GetServicios(string idioma)
        {
            var sql = "SELECT [servicios].titulo, [servicios].imagen_contenido, [servicios].contenido, [servicios].slug " +
                      "FROM [servicios] INNER JOIN [idiomas] " +
                      "ON [idiomas].id = [servicios].idioma_id WHERE[idiomas].nombre = '" + idioma + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<Servicios>();
            var count = 1;
            foreach (DataRow row in dt.Rows)
            {
                var s = new Servicios
                {
                    Titulo = Convert.ToString(row["titulo"]),
                    Contenido = Convert.ToString(row["contenido"]),
                    Imagen = Convert.ToString(row["imagen_contenido"]),
                    Slug = Convert.ToString(row["slug"]),
                    ImgClass = "bot_ayuda0" + count
                };
                model.Add(s);
                count++;
            }

            return model;
        }

        public static string ObtenerPrefijo(string slug)
        {
            string prefijo = string.Empty;
            var sql = "SELECT prefijo FROM [servicios] WHERE slug = '" + slug + "'";

            var dt = Conexion.SqlDataTable(sql, true);

            DataRow row = dt.Rows[0];

            prefijo = row["prefijo"].ToString();

            return prefijo;
        }

        public static string ObtenerSlug(string prefijo, string culture)
        {

            string nuevoSlug = string.Empty;
            int idiomaId = 1;

            if (culture != "es") idiomaId = 2;

            var sql = "SELECT servicios.slug " +
                      "FROM [servicios] " +
                      "WHERE servicios.prefijo = '" + prefijo + "' and servicios.idioma_id =" + idiomaId;

            var dt = Conexion.SqlDataTable(sql, true);
            DataRow row = dt.Rows[0];

            nuevoSlug = row["slug"].ToString();

            return nuevoSlug;
        }
    }
}