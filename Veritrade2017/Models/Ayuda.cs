using System;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Ayuda
    {
        public int Id { get; set; }
        public Idiomas Idiomas { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public Ayuda()
        {
            Idiomas = new Idiomas();
        }

        public Ayuda GetAyuda(string idioma)
        {
            var sql = "SELECT [ayuda].id, [ayuda].titulo, [ayuda].descripcion FROM [dbo].[ayuda] " +
                      "INNER JOIN[dbo].[idiomas] ON [ayuda].idioma_id = [idiomas].id " +
                      "WHERE [idiomas].nombre = '" + idioma + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new Ayuda();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Ayuda
                {
                    Id = Convert.ToInt16(row["id"]),
                    Titulo = Convert.ToString(row["titulo"]),
                    Descripcion = Convert.ToString(row["descripcion"])
                };
                model = s;
            }

            return model;
        }
    }
}