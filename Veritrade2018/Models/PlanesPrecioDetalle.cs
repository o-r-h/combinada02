using System;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class PlanesPrecioDetalle
    {
        public int Id { get; set; }
        public PlanesPrecio PlanesPrecio { get; set; }
        public string PaisesDescripcion { get; set; }
        public string InfoComexDescripcion { get; set; }
        public string ModulosDescripcion { get; set; }
        public string UsuariosDescripcion { get; set; }
        public string IngresosDescargasDescripcion { get; set; }
        public string FavoritosGruposDescripcion { get; set; }
        public string PlantillasDescripcion { get; set; }
        public bool Eliminado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public string TituloPlan { get; set; }

        public PlanesPrecioDetalle()
        {
            PlanesPrecio = new PlanesPrecio();
        }

        public static string GetPaisesDescripcion(int id)
        {
            return GetAtributo(id, "paises_descripcion");
        }

        public static string GetInfoComexDescripcion(int id)
        {
            return GetAtributo(id, "info_comex_descripcion");
        }

        public static string GetModulosDescripcion(int id)
        {
            return GetAtributo(id, "modulos_descripcion");
        }

        public static string GetUsuariosDescripcion(int id)
        {
            return GetAtributo(id, "usuarios_descripcion");
        }

        public static string GetIngresosDescargasDescripcion(int id)
        {
            return GetAtributo(id, "ingresos_descargas_descripcion");
        }

        public static string GetFavoritosGruposDescripcion(int id)
        {
            return GetAtributo(id, "favoritos_grupos_descripcion");
        }

        public static string GetPlantillasDescripcion(int id)
        {
            return GetAtributo(id, "plantillas_descripcion");
        }

        public static string GetAtributo(int planId, string campo)
        {
            var sql = "SELECT " + campo + " FROM [dbo].[planes_precio_detalle] WHERE plan_precio_id = " + planId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = "";
            foreach (DataRow row in dt.Rows)
            {
                model = Convert.ToString(row[campo]);
            }
            return model;
        }

        public static PlanesPrecioDetalle GetPlanDetalle(int planId)
        {
            var sql = "SELECT [planes].nombre, [planes_precio_detalle].* FROM [dbo].[planes_precio_detalle] " +
                      "INNER JOIN[dbo].[planes_precio] ON[planes_precio].id = [planes_precio_detalle].plan_precio_id " +
                      "INNER JOIN[dbo].[planes] ON[planes].id = [planes_precio].plan_id " +
                      "WHERE plan_precio_id = " + planId;

            var dt = Conexion.SqlDataTable(sql, true);
            var model = new PlanesPrecioDetalle();
            foreach (DataRow row in dt.Rows)
            {
                model.TituloPlan = Convert.ToString(row["nombre"]);
                model.PaisesDescripcion = Convert.ToString(row["paises_descripcion"]);
                model.InfoComexDescripcion = Convert.ToString(row["info_comex_descripcion"]);
                model.ModulosDescripcion = Convert.ToString(row["modulos_descripcion"]);
                model.UsuariosDescripcion = Convert.ToString(row["usuarios_descripcion"]);
                model.IngresosDescargasDescripcion = Convert.ToString(row["ingresos_descargas_descripcion"]);
                model.FavoritosGruposDescripcion = Convert.ToString(row["favoritos_grupos_descripcion"]);
                model.PlantillasDescripcion = Convert.ToString(row["plantillas_descripcion"]);
            }
            return model;
        }
    }
}