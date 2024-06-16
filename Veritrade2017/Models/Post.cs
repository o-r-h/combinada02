using System;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Post
    {
        public int Id { get; set; }
        public Idiomas Idioma { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public bool GetPost(string idioma, int id)
        {
            var sql = "SELECT * FROM [dbo].[post] " +
                      "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [post].idioma_id " +
                      "WHERE [idiomas].nombre = '" + idioma + "' " +
                      "AND [post].id = " + id;

            var dt = Conexion.SqlDataTable(sql, true);
            return dt.Rows.Count > 0;
        }

        public void SetPostVisit(int id)
        {
            var total = GetPostVisit(id);
            string sql;

            if (total > 0)
            {
                sql = "UPDATE [post_counter] SET [visitas] = [visitas] + 1 WHERE [post_id] = " + id;
            }
            else
            {
                sql = "INSERT INTO [post_counter] ([post_id],[visitas],[fecha_actualizacion]) " +
                      "VALUES (" + id + ", 1, getdate())";
            }

            Conexion.SqlExecute(sql, true);
        }

        public int GetPostVisit(int id)
        {
            var total = 0;
            var sql = "SELECT [visitas] FROM [dbo].[post_counter] WHERE [post_id] = " + id;

            var dt = Conexion.SqlDataTable(sql, true);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    total = Convert.ToInt32(row["visitas"]);
                }
            }
            else
            {
                total = 0;
            }

            return total;
        }

        public string GetPostDescription(int id)
        {
            var total = "";
            var sql = "SELECT [descripcion] FROM [dbo].[post] WHERE [id] = " + id;

            var dt = Conexion.SqlDataTable(sql, true);
            foreach (DataRow row in dt.Rows)
            {
                total = row["descripcion"].ToString();
            }

            return total;
        }
    }
}