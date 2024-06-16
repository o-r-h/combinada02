using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class ServiciosPaisesDetalles
    {
        public int Id { get; set; }
        public ServiciosPaises ServiciosPaisId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Archivo { get; set; }
        public string Nota { get; set; }
        public string CodInfo { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public ServiciosPaisesDetalles()
        {
            ServiciosPaisId = new ServiciosPaises();
        }

        public static List<ServiciosPaisesDetalles> GetList(int servicioPais)
        {
            var sql = "SELECT titulo, descripcion, archivo, nota, codInfo " +
                      "FROM [dbo].[servicios_pais_detalle] WHERE servicios_pais_id = '" + servicioPais + "'";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<ServiciosPaisesDetalles>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new ServiciosPaisesDetalles
                {
                    Titulo = Convert.ToString(row["titulo"]),
                    Descripcion = Convert.ToString(row["descripcion"]),
                    Archivo = Convert.ToString(row["archivo"]),
                    Nota = Convert.ToString(row["nota"]),
                    CodInfo = Convert.ToString(row["codInfo"])
                };
                model.Add(s);
            }

            return model;
        }

        public static List<DateTime> GetDatesInfo(string codigo)
        {
            var sql = "SELECT Distinct CodPais,FechaIni,FechaFin FROM [BaseDatos] WHERE CodPais = '" + codigo + "'";

            var dt = Conexion.SqlDataTable(sql);
            var model = new List<DateTime>();
            foreach (DataRow row in dt.Rows)
            {

                model.Add(DateTime.ParseExact(row["FechaIni"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));
                model.Add(DateTime.ParseExact(row["FechaFin"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));
            }

            return model;
        }
    }
}