using System;
using System.Collections.Generic;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class PlanesPaisSuscription
    {
        public int Id { get; set; }
        public string PlanId { get; set; }
        public string CodPais { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public List<string> GetCodPaisesPlan(int planId)
        {
            var sql = "SELECT * FROM [planes_pais_suscription] WHERE planes_id = " + planId + " and eliminado = 0 ";

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                model.Add(Convert.ToString(row["codPais"]));
            }

            return model;
        }
    }
}