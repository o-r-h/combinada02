using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlTypes;
using System.Web;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models.Admin
{
    public class MisProductos
    {
        public int IdProducto { get; set; }
        public String CodProducto { get; set; }
        public String Descripcion { get; set; }
        public String PaisAduana { get; set; }
        public int IdPaisAduana { get; set; }
        public String Valor { get; set; }
        public int Cantidad { get; set; }
        public String ValorTotal { get; set; }
        public String CantidadTotal { get; set; }
        public String PrecioUnitTotal { get; set; }

        public static List<object> SearchProduct(string descripcion, string culture)
        {
            var sql = "";
            if (culture=="es")
            {
                sql = "SELECT TOP(1000) IdProducto, DescripcionES AS Descripcion " + "FROM PRODUCTO" +
                          " WHERE DescripcionES LIKE '%' + '" + descripcion + "' + '%'";
            }
            else
            {
                sql = "SELECT TOP(1000) IdProducto, DescripcionEN AS Descripcion " + " FROM PRODUCTO" +
                          " WHERE DescripcionEN LIKE '%' + '" + descripcion + "'+ '%'";
            }
            var dt = Conexion.SqlDataTableProductProfile(sql);

            var json = new List<object>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json.Add(new
                    {
                        id = new
                        {
                            uri = dr["IdProducto"].ToString()
                        },
                        value = dr["Descripcion"].ToString()
                    });
                }
            }
            else
            {
                json.Add(new { id = 0, value = "-"});
            }

            return json;
        }
    }
}