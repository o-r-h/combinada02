using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Planes
    {
        public int Id { get; set; }
        public Idiomas Idiomas { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int Orden { get; set; }

        public Planes()
        {
            Idiomas = new Idiomas();
        }

        public static string GetPlanPrecio(string codPais)
        {
            string planPrecio = string.Empty;

            var sql = "SELECT [PlanPrecio] FROM [dbo].[PlanPrecioPais] WHERE [CodPais] = '" + codPais + "'";

            var dt = Conexion.SqlDataTable(sql);
            DataRow row = dt.Rows[0];
            planPrecio = row["PlanPrecio"].ToString();

            return planPrecio;
        }

        public static List<Planes> GetPlanes(string idioma, string codPaisIp, string planPrecio)
        {
            var sql =
                "SELECT [planes].id as id, [planes].nombre, ISNULL([planes].descripcion,'') as descripcion FROM [dbo].[planes] " +
                "INNER JOIN [dbo].[idiomas] ON [idiomas].id = [planes].idioma_id " +
                "INNER JOIN [dbo].[planes_pais] ON [planes_pais].planes_id = [planes].id " +
                "WHERE [idiomas].nombre = '" + idioma +
                "' AND [planes].descripcion like '" + planPrecio +
                "' AND [planes_pais].codPais = '" + codPaisIp +
                "' AND [planes_pais].eliminado = 0";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<Planes>();
            var i = 1;

            foreach (DataRow row in dt.Rows)
            {
                var s = new Planes
                {
                    Id = Convert.ToInt16(row["id"]),
                    Nombre = Convert.ToString(row["nombre"]),
                    Descripcion = Convert.ToString(row["descripcion"]),
                    Orden = i
                };
                i++;
                model.Add(s);
            }
            return model;
        }

        public static Planes GetDetallePlan(int id)
        {
            var sql = "SELECT id, nombre, ISNULL(descripcion,'') as descripcion FROM [dbo].[planes] WHERE id = " + id;

            var dt = Conexion.SqlDataTable(sql, true);
            var s = new Planes();
            foreach (DataRow row in dt.Rows)
            {
                s.Id = Convert.ToInt16(row["id"]);
                s.Nombre = Convert.ToString(row["nombre"]);
                s.Descripcion = Convert.ToString(row["descripcion"]);
            }

            return s;
        }

        public static bool PlanExist(int id, string codPaisIp, string planPrecio)
        {
            var sql = "SELECT * FROM [dbo].[planes] " +
                      "INNER JOIN [dbo].[planes_pais] ON [planes_pais].planes_id = [planes].id " +
                      "WHERE [planes].id = " + id +
                      " AND  descripcion like '" + planPrecio +
                      "' AND [planes_pais].codPais = '" + codPaisIp +
                      "' AND [planes_pais].eliminado = 0";

            var dt = Conexion.SqlDataTable(sql, true);
            return dt.Rows.Count > 0;
        }
    }
}