using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class PostLanguage
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string ImagenPortada { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Anio { get; set; }
        public string Mes { get; set; }

        public PostLanguage()
        {
            Post = new Post();
        }

        public List<PostLanguage> HomePostLanguages(string idioma, int top = 0)
        {
            var limite = "";
            if (top > 0)
            {
                limite = "TOP " + top;
            }

            var sql = "SELECT " + limite +
                      " [post].id as id, [post].titulo, [post].descripcion, imagen_portada, fecha_publicacion " +
                      "FROM [dbo].[post_language] " +
                      "INNER JOIN [dbo].[post] ON [post].id = [post_language].post_id " +
                      "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [post].idioma_id " +
                      "WHERE [post_language].eliminado = 0 AND [post].eliminado = 0 AND [idiomas].nombre = '" +
                      idioma + "' " +
                      "ORDER BY fecha_publicacion DESC;";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<PostLanguage>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new PostLanguage();
                s.Id = Convert.ToInt32(row["id"]);
                s.Post.Titulo = row["titulo"].ToString();
                s.Post.Descripcion = row["descripcion"].ToString();
                s.ImagenPortada = row["imagen_portada"].ToString();
                s.FechaPublicacion = Convert.ToDateTime(row["fecha_publicacion"]);
                model.Add(s);
            }

            return model;
        }

        public PostLanguage LastPost(string idioma)
        {
            var sql =
                "SELECT TOP 1 [post].id as id,[post].titulo,[post].descripcion,imagen_portada,fecha_publicacion " +
                "FROM [dbo].[post_language] " +
                "INNER JOIN [dbo].[post] ON [post].id = [post_language].post_id " +
                "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [post].idioma_id " +
                "WHERE [post_language].eliminado = 0 AND [post].eliminado = 0 AND [idiomas].nombre = '" + idioma +
                "' ORDER BY fecha_publicacion DESC";

            var dt = Conexion.SqlDataTable(sql, true);
            var s = new PostLanguage();
            foreach (DataRow row in dt.Rows)
            {
                s.Id = Convert.ToInt32(row["id"]);
                s.Post.Titulo = row["titulo"].ToString();
                s.Post.Descripcion = row["descripcion"].ToString();
                s.ImagenPortada = row["imagen_portada"].ToString();
                s.FechaPublicacion = Convert.ToDateTime(row["fecha_publicacion"]);
            }

            return s;
        }

        public List<PostLanguage> MostVisited(string idioma)
        {
            var sql =
                "SELECT TOP 2 [post].id, [post].titulo, [post].descripcion, [imagen_portada], [fecha_publicacion] " +
                "FROM [post] " +
                "INNER JOIN[post_language] ON post.id = post_language.post_id " +
                "INNER JOIN[post_counter] ON post_language.post_id = post_counter.post_id " +
                "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [post].idioma_id " +
                "WHERE [post_language].eliminado = 0 AND [post].eliminado = 0 AND [idiomas].nombre = '" + idioma +
                "' ORDER BY visitas DESC";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<PostLanguage>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new PostLanguage();
                s.Id = Convert.ToInt32(row["id"]);
                s.Post.Titulo = row["titulo"].ToString();
                s.Post.Descripcion = row["descripcion"].ToString();
                s.ImagenPortada = row["imagen_portada"].ToString();
                s.FechaPublicacion = Convert.ToDateTime(row["fecha_publicacion"]);
                model.Add(s);
            }

            return model;
        }

        // Ruben 202305
        public PostLanguage GetPostContent(string idioma, int postId)
        {
            var sql =
                "SELECT [post_language].titulo,[post_language].contenido,imagen_portada,fecha_publicacion, campaña_prueba_gratis " + // Ruben 202305
                "FROM [dbo].[post_language] " +
                "INNER JOIN [dbo].[post] ON [post].id = [post_language].post_id " +
                "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [post].idioma_id " +
                "WHERE [post_language].eliminado = 0 AND [post].eliminado = 0 AND [idiomas].nombre = '" + idioma +
                "' AND post_id = " + postId;

            var dt = Conexion.SqlDataTable(sql, true);
            var s = new PostLanguage();
            foreach (DataRow row in dt.Rows)
            {                
                s.Titulo = row["titulo"].ToString();
                s.Contenido = row["contenido"].ToString();

                // Ruben 202305
                string Campaña_Prueba_Gratis = row["Campaña_Prueba_Gratis"].ToString();
                string Link = "https://www.veritradecorp.com/es/pruebagratis/" + Campaña_Prueba_Gratis;
                string Nuevo_Link = "javascript:prueba_gratis('" + Campaña_Prueba_Gratis + "');";

                s.Contenido = s.Contenido.Replace(Link, Nuevo_Link);

                s.ImagenPortada = row["imagen_portada"].ToString();
                s.FechaPublicacion = Convert.ToDateTime(row["fecha_publicacion"]);
            }

            return s;
        }

        public List<PostLanguage> OldPost(string idioma)
        {
            var sql = "SELECT post.id as id, post_language.titulo, " +
                      "YEAR([fecha_publicacion]) 'anio', MONTH([fecha_publicacion]) as 'mes', fecha_publicacion " +
                      "FROM [post_language] " +
                      "INNER JOIN [post] ON [post_language].post_id = [post].id " +
                      "INNER JOIN [idiomas] ON [idiomas].id = [post].idioma_id " +
                      "WHERE [post_language].eliminado = 0 AND [post].eliminado = 0 AND [idiomas].nombre = '" + idioma +
                      "' ORDER BY anio DESC, mes DESC; ";

            var dt = Conexion.SqlDataTable(sql, true);
            var modelo = new List<PostLanguage>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new PostLanguage();
                s.Id = Convert.ToInt32(row["id"]);
                s.Post.Titulo = row["titulo"].ToString();
                s.Anio = row["anio"].ToString();
                s.Mes = row["mes"].ToString();
                s.FechaPublicacion = Convert.ToDateTime(row["fecha_publicacion"]);

                modelo.Add(s);
            }

            return modelo;
        }
    }
}