using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Clientes
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Logo { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public static List<Clientes> GetClientes()
        {
            var sql = "SELECT id, nombre, logo FROM [clientes] WHERE eliminado = 0";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<Clientes>();
            foreach (DataRow row in dt.Rows)
            {
                var s = new Clientes
                {
                    Id = Convert.ToInt16(row["id"]),
                    Nombre = Convert.ToString(row["nombre"]),
                    Logo = Convert.ToString(row["logo"])
                };
                model.Add(s);
            }

            return model;
        }
    }
}