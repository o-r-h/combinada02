using System;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Periodo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Idiomas Idiomas { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public Periodo()
        {
            Idiomas = new Idiomas();
        }

        public static Periodo GetPeriodo(int periodoId)
        {
            var sql = "SELECT * FROM [dbo].[periodo] WHERE id = " + periodoId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new Periodo();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Periodo
                {
                    Id = Convert.ToInt32(row["id"]),
                    Descripcion = Convert.ToString(row["descripcion"]),
                    Eliminado = Convert.ToBoolean(row["eliminado"])

                };
                model = s;
            }

            return model;
        }
    }
}