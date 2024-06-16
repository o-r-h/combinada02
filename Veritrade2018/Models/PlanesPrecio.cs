using System;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class PlanesPrecio
    {
        public int Id { get; set; }
        public Planes Plan { get; set; }
        public decimal Precio { get; set; }
        public Periodo Periodo { get; set; }
        public string Paises { get; set; }
        public string InfoComex { get; set; }
        public string Modulos { get; set; }
        public string Usuarios { get; set; }
        public string IngresosDescargas { get; set; }
        public string FavoritosGrupos { get; set; }
        public string Plantillas { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public PlanesPrecio()
        {
            Plan = new Planes();
            Periodo = new Periodo();
        }

        public static PlanesPrecio GetPrecioPlan(int planId)
        {
            var sql = "SELECT * FROM [dbo].[planes_precio] WHERE plan_id = " + planId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new PlanesPrecio();
            foreach (DataRow row in dt.Rows)
            {
                var s = new PlanesPrecio
                {
                    Id = Convert.ToInt32(row["id"]),
                    Precio = Convert.ToDecimal(row["precio"]),
                    Periodo = Periodo.GetPeriodo(Convert.ToInt32(row["periodo_id"])),
                    Paises = Convert.ToString(row["paises"]),
                    InfoComex = Convert.ToString(row["info_comex"]),
                    Modulos = Convert.ToString(row["modulos"]),
                    Usuarios = Convert.ToString(row["usuarios"]),
                    IngresosDescargas = Convert.ToString(row["ingresos_descargas"]),
                    FavoritosGrupos = Convert.ToString(row["favoritos_grupos"]),
                    Plantillas = Convert.ToString(row["plantillas"])
                };
                model = s;
            }

            return model;
        }
    }
}