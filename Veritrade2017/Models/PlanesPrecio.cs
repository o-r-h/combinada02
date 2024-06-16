using System;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
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

        public static PlanesPrecio GetPrecioPlan(int planId, string codPais)
        {
            if(codPais != "CL") {
                return GetPrecioPlan(planId);
            }

            var sql = "SELECT " +
                "planes_precio.id as id" +
                ", planes_precio.precio as precio" +
                ", planes_precio.periodo_id as periodo_id" +
                ", planes_precio.paises as paises" +
                ", planes_precio.info_comex as info_comex" +
                ", planes_precio.modulos as modulos" +
                ", planes_precio.usuarios as usuarios" +
                ", planes_precio.ingresos_descargas as ingresos_descargas" +
                ", planes_precio.favoritos_grupos as favoritos_grupos" +
                ", planes_precio.plantillas as plantillas " +
                "FROM [dbo].[planes_precio] " +
                "WHERE [dbo].[planes_precio].plan_id = " + planId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new PlanesPrecio();
            foreach (DataRow row in dt.Rows)
            {

                var s = new PlanesPrecio
                {
                    Id = Convert.ToInt32(row["id"]),
                    Precio = Decimal.Truncate(Math.Round(Convert.ToDecimal(row["precio"]))),
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

        public static Decimal ConvertirUfAPeso(Decimal valorUf)
        {

            var sql = "SELECT "+
                      "  COALESCE( " +
                      "( " +
                      "           SELECT " +
                      "                valor " +
                      "            FROM " +
                      "                valor_uf " +
                      "            WHERE " +
                      "                fecha = CONVERT(DATE, GETDATE())), " +
                      "           ( " +
                      "               SELECT " +
                      "                   top 1 valor " +
                      "               FROM " +
                      "                   valor_uf " +
                      "               ORDER BY " +
                      "                   fecha DESC)) valor";

            var dt = Conexion.SqlDataTable(sql, true);
            Decimal Precio = 0;
            foreach (DataRow row in dt.Rows)
            {
                Precio = Convert.ToDecimal(row["valor"]);
            }

            return Decimal.Truncate(Math.Round(Precio * valorUf));
        }
    }
}