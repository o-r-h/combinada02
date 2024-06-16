using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Veritrade2018.Helpers;
namespace Veritrade2018.Models
{
    public class Popups
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public string Imagen { get; set; }
        public string Texto_2 { get; set; }
        public string Contactenos { get; set; }
        public string Despedida { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public Popups GetPopup(string idioma)
        {
            var sql = "SELECT TOP 1  * FROM [VeritradeContent].[dbo].[popups] " +
                      " WHERE getutcdate() between [fecha_inicio] and [fecha_fin] "+
                      " and popups.texto_Esp not like '%@UNIVERSIDAD%'";

            /*sql = "SELECT TOP 1  * FROM [VeritradeContent].[dbo].[popups] " +
                      " WHERE id= 1";*/

            var dt = Conexion.SqlDataTable(sql, true);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                Popups popup = null;

                foreach (DataRow row in dt.Rows)
                {
                    if (idioma == "es")
                    {
                         popup = new Popups
                        {

                            Id = Convert.ToInt16(row["id"]),
                            Imagen = row.GetValue<string>("imagen"),
                            Fecha_inicio = Convert.ToDateTime(row["fecha_inicio"]),
                            Fecha_fin = Convert.ToDateTime(row["fecha_fin"]),

                            Titulo = Convert.ToString(row["titulo_Esp"]),
                            Texto = Convert.ToString(row["texto_Esp"]),
                            Texto_2 = Convert.ToString(row["texto_2_Esp"]),
                            Contactenos = Convert.ToString(row["contactenos_Esp"]),
                            Despedida = Convert.ToString(row["despedida_Esp"]),


                        };

                    }
                    if (idioma == "en")
                    {
                        popup = new Popups
                        {

                            Id = Convert.ToInt16(row["id"]),
                            Imagen = row.GetValue<string>("imagen"),
                            Fecha_inicio = Convert.ToDateTime(row["fecha_inicio"]),
                            Fecha_fin = Convert.ToDateTime(row["fecha_fin"]),

                            Titulo = Convert.ToString(row["titulo_Eng"]),
                            Texto = Convert.ToString(row["texto_Eng"]),
                            Texto_2 = Convert.ToString(row["texto_2_Eng"]),
                            Contactenos = Convert.ToString(row["contactenos_Eng"]),
                            Despedida = Convert.ToString(row["despedida_Eng"]),


                        };

                    }
                }
                return popup;
            }

        }

        public Popups GetPopupUniversidad()
        {
            var sql = "select * from popups where popups.texto_Esp like '%@UNIVERSIDAD%' and getutcdate() between [fecha_inicio] and [fecha_fin]  ";

            /*sql = "SELECT TOP 1  * FROM [VeritradeContent].[dbo].[popups] " +
                      " WHERE id= 1";*/

            var dt = Conexion.SqlDataTable(sql, true);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                Popups popup = null;

                foreach (DataRow row in dt.Rows)
                {
                    popup = new Popups
                    {

                        Id = row["id"] != DBNull.Value ? Convert.ToInt16(row["id"]) : 0,
                        Imagen = row["imagen"] != DBNull.Value ? row.GetValue<string>("imagen") : null,
                        Fecha_inicio = row["fecha_inicio"] != DBNull.Value ? Convert.ToDateTime(row["fecha_inicio"]) : Convert.ToDateTime("2000-01-01"),
                        Fecha_fin = row["fecha_fin"] != DBNull.Value ? Convert.ToDateTime(row["fecha_fin"]) : Convert.ToDateTime("2000-01-01"),

                        Titulo = row["titulo_Esp"] != DBNull.Value ? Convert.ToString(row["titulo_Esp"]) : "",
                        Texto = row["texto_Esp"] != DBNull.Value ? Convert.ToString(row["texto_Esp"]) : "",
                        Texto_2 = row["texto_2_Esp"] != DBNull.Value ? Convert.ToString(row["texto_2_Esp"]) : "",
                        Contactenos = row["contactenos_Esp"] != DBNull.Value ? Convert.ToString(row["contactenos_Esp"]) : "",
                        Despedida = row["despedida_Esp"] != DBNull.Value ? Convert.ToString(row["despedida_Esp"]) : "",


                    };
                }
                return popup;
            }

        }

    }
}