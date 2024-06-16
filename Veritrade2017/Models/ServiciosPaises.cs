using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class ServiciosPaises
    {
        public int Id { get; set; }
        public Idiomas Idiomas { get; set; }
        public string NombrePais { get; set; }
        public string Abreviatura { get; set; }
        public string ImagenBandera { get; set; }
        public string ImagenServicio { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public ServiciosPaises()
        {
            Idiomas = new Idiomas();
        }

        public static List<ServiciosPaises> GetList(string idioma)
        {
            var sql = "SELECT [servicios_pais].id as id, nombre_pais, abreviatura, imagen_bandera, imagen_servicio " +
                      "FROM [dbo].[servicios_pais] INNER JOIN [dbo].[idiomas] " +
                      "ON [idiomas].id = [servicios_pais].idioma_id WHERE [idiomas].nombre = '" + idioma + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<ServiciosPaises>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new ServiciosPaises
                {
                    Id = Convert.ToInt32(row["id"]),
                    NombrePais = Convert.ToString(row["nombre_pais"]),
                    Abreviatura = Convert.ToString(row["abreviatura"]),
                    ImagenBandera = Convert.ToString(row["imagen_bandera"]),
                    ImagenServicio = Convert.ToString(row["imagen_servicio"])
                };
                model.Add(s);
            }

            return model;
        }

        public static string GetCodigoPais(string codigo)
        {
            var s = "";
            var sql = "SELECT CodPais FROM AdminPais where CodPais3 = '" + codigo + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                s = Convert.ToString(row["CodPais"]);
            }

            return s;
        }
    }
}