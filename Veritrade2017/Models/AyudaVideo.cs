using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class AyudaVideo
    {
        public int Id { get; set; }
        public Ayuda Ayuda { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string UrlVideo { get; set; }
        public string TituloVideo { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public AyudaVideo()
        {
            Ayuda = new Ayuda();
        }

        public List<AyudaVideo> GetVideos(int ayudaId)
        {
            var sql = "SELECT titulo, descripcion, url_video, titulo_video " +
                      "FROM [dbo].[ayuda_video] WHERE ayuda_id = " + ayudaId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<AyudaVideo>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new AyudaVideo
                {
                    Titulo = Convert.ToString(row["titulo"]),
                    Descripcion = Convert.ToString(row["descripcion"]),
                    UrlVideo = Convert.ToString(row["url_video"]),
                    TituloVideo = Convert.ToString(row["titulo_video"])
                };
                model.Add(s);
            }

            return model;
        }

        public List<AyudaVideo> GetVideos2(int ayudaId)
        {
            var sql = "SELECT titulo, descripcion, url_video, titulo_video, duracion " +
                      "FROM [dbo].[ayuda_video2] WHERE eliminado = 'False' and ayuda_id = " + ayudaId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<AyudaVideo>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new AyudaVideo
                {
                    Titulo = Convert.ToString(row["titulo"]),
                    Descripcion = Convert.ToString(row["duracion"]),
                    UrlVideo = Convert.ToString(row["url_video"]),
                    TituloVideo = Convert.ToString(row["titulo_video"])
                };
                model.Add(s);
            }

            return model;
        }
    }
}