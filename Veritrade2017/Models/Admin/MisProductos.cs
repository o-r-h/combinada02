using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using System.Web;
using Veritrade2017.Helpers;
using Veritrade2017.Models.ProductProfile;

namespace Veritrade2017.Models.Admin
{
    public class MisProductos
    {
        public int IdProducto { get; set; }
        public String CodProducto { get; set; }
        public String Descripcion { get; set; }
        public String Uri { set; get; }
        public String PaisAduana { get; set; }
        public int IdPaisAduana { get; set; }
        public String Valor { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal CantTotal { get; set; }
        public decimal PreUnitTotal { get; set; }
        public String ValorTotalExp { get; set; }
        public String CantTotalExp { get; set; }
        public String PreUnitTotalExp { get; set; }
        public String RegimenActual { get; set; }
        public decimal TotalByRegimen { get; set; }
        

        public static BuscarProducto GetProducto(string uri, string codPartida, int IdPais, string idioma)
        {
            var sql = "";
            if (idioma.Equals("es"))
            {
                sql = " SELECT PR.IDPRODUCTO, Producto = PR.DescripcionES, PaisAduana= (SELECT PaisAduana FROM PAISADUANA WHERE IdPaisAduana = " + IdPais + "), Uri = PR.UriEs " +
                      " FROM PRODUCTO PR INNER JOIN TOTALES T ON PR.IdProducto = T.IdProducto" +
                      " WHERE PR.CodProducto = '" + codPartida + "' AND (UriES = '" + uri + "' OR UriEN = '" + uri + "') GROUP BY PR.IdProducto, PR.DescripcionES, PR.UriEs";

            }
            else
            {
                sql = "SELECT PR.IDPRODUCTO, Producto = PR.DescripcionEN, PaisAduana= (SELECT PaisAduana FROM PAISADUANA WHERE IdPaisAduana = " + IdPais + "), Uri = PR.UriEN " +
                      "FROM PRODUCTO PR INNER JOIN TOTALES T ON PR.IdProducto = T.IdProducto" +
                      " WHERE PR.CodProducto = '" + codPartida + "' AND (UriES = '" + uri + "' OR UriEN = '" + uri + "') GROUP BY PR.IdProducto, PR.DescripcionEN, PR.UriEN";

            }
            var dt = Conexion.SqlDataTableProductProfile(sql);

            BuscarProducto json = null;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    json = new BuscarProducto()
                    {
                        Producto = dr["Producto"].ToString().Trim(),
                        Pais = dr["PaisAduana"].ToString().Trim().ToLower(),
                        Uri = dr["Uri"].ToString(),
                        CodPartida = codPartida
                    };
                }
            }
            return json;
        }
        public static List<object> SearchProduct(string descripcion, string CodPaisIP, string culture)
        {
            string[] arrayPais = new string[] { "AR", "BO", "PE", "PY", "UY", "CR", "", "", "PA", "MX", "BR", "CL", "CO", "DO", "GT", "HN", "SV" };
            //INICIO - EXTRAYENDO TODOS LOS ID'S DE PRODUCTOS
            List<int> ListIdProducts= new List<int>();
            Char delimiterV = ' ';
            String[] substringsV = descripcion.Split(delimiterV);
            var sqlValidate = "";
            var sqlValidate2 = "";
            var sqlValidate3 = "";
            var tamDesc = substringsV.Length;
            if (culture.Equals("es"))
            {
                sqlValidate = "SELECT P.IdProducto  FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto IN (SELECT PR.IdProducto FROM (SELECT IdProducto, DescripcionEs, Partida = CodProducto FROM Producto " +
                              "UNION ALL SELECT IdProducto, DescripcionEs = Descripcion_ES, Partida FROM Partida) AS PR WHERE ";

                foreach (var substring in substringsV)
                {
                    sqlValidate2 += " DescripcionES LIKE  '" + substring + "%' OR Partida LIKE  '" + substring + "%' ";
                    if (tamDesc != 1)
                    {
                        sqlValidate2 += "OR ";
                        tamDesc--;
                    }
                }
                sqlValidate3 = " ) GROUP BY P.IdProducto ORDER BY SUM(T.Valor) DESC";

                sqlValidate += sqlValidate2 + sqlValidate3;
            }
            else
            {
                sqlValidate = "SELECT P.IdProducto  FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto IN (SELECT PR.IdProducto FROM (SELECT IdProducto, DescripcionEn, Partida = CodProducto FROM Producto " +
                              "UNION ALL SELECT IdProducto, DescripcionEn = Descripcion_EN, Partida FROM Partida) AS PR WHERE ";

                foreach (var substring in substringsV)
                {
                    sqlValidate2 += " DescripcionEN LIKE  '" + substring + "%' OR Partida LIKE  '" + substring + "%' OR ";
                }
                sqlValidate3 = " DescripcionEN LIKE 'ARTIFICIO%') GROUP BY P.IdProducto ORDER BY SUM(T.Valor) DESC";

                sqlValidate += sqlValidate2 + sqlValidate3;
            }

            var dtValidate = Conexion.SqlDataTableProductProfile(sqlValidate);
            if (dtValidate.Rows.Count > 0)
            {
                foreach (DataRow dr in dtValidate.Rows)
                {
                    ListIdProducts.Add(Convert.ToInt32(dr["IdProducto"]));
                }
            }
            //FIN - EXTRAYENDO TODOS LOS ID'S DE PRODUCTO

            var sql = "";
            var sql2 = "";
            var sql3 = "";
            var sqlPais = "";
            Char delimiter = ' ';
            String[] substrings = descripcion.Split(delimiter);
            DataTable dt = new DataTable();
            var json = new List<object>();
            sql = culture.Equals("es") ? "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionES, "
                                       : "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionEN, ";
            var sqlRegimen = "Regimen = (SELECT TOP 1 T.Regimen FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), ";
            var sqlUri = culture.Equals("es") ? "Uri = P.UriES FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto"
                                              : "Uri = P.UriEN FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto";
            var sqlGroup = culture.Equals("es") ? "GROUP BY P.IdProducto, P.CodProducto,P.DescripcionES,P.UriES ORDER BY SUM(T.Valor) DESC"
                                                 : "GROUP BY P.IdProducto, P.CodProducto,P.DescripcionEN,P.UriEN ORDER BY SUM(T.Valor) DESC";
            var sqlConsProducto = "";
            if (ListIdProducts.Count > 0)
            {            
                for (int i = 0; i < ListIdProducts.Count; i++)
                {
                    int IdPais = GetIdPais(ListIdProducts[i], CodPaisIP);
                    //for (int j = 0; j < arrayPais.Length; j++)
                    //{
                        if (IdPais != 0/*arrayPais.Contains(CodPaisIP) && ValidatePaisIP(ListIdProducts[i], IdPais)*/)
                        {                        
                            sqlPais = "PaisAduana = (SELECT PaisAduana FROM PaisAduana WHERE IdPaisAduana = " + IdPais + "), ";
                        }
                        else
                        {
                            sqlPais = "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), ";
                        }                    
                    //}
                    sqlConsProducto = "= " + ListIdProducts[i] + " ";

                    sql = sql + sqlPais + sqlRegimen + sqlUri + sqlConsProducto + sqlGroup;
                    dt = Conexion.SqlDataTableProductProfile(sql);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            json.Add(new
                            {
                                id = dr["IdProducto"].ToString(),
                                codPartida = dr["Partida"].ToString(),
                                value = dr["Descripcion"].ToString(),
                                pais = dr["PaisAduana"].ToString().ToLower(),
                                tipoOpe = dr["Regimen"].ToString(),
                                uri = dr["Uri"].ToString()
                            });
                        }
                    }
                    //else
                    //{
                    //    json.Add(new { id = 0, value = "-" });
                    //}
                    sql = culture.Equals("es") ? "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionES, "
                                               : "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionEN, ";
                }
            }
            else
            {
                json.Add(new { id = 0, value = "-" });
            }
            /*if (culture == "es")
            {
                sql = "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionES, " +
                      "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), " +
                      "Regimen = (SELECT TOP 1 T.Regimen FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), " +
                      "Uri = P.UriES FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto IN (SELECT PR.IdProducto FROM (SELECT IdProducto, DescripcionEs, Partida = CodProducto FROM Producto " +
                      "UNION ALL SELECT IdProducto, DescripcionEs = Descripcion_ES, Partida FROM Partida) AS PR WHERE ";

                foreach (var substring in substrings)
                {
                    sql2 += " DescripcionES LIKE  '" + substring + "%' OR Partida LIKE  '" + substring + "%' OR ";
                }
                
                sql3 = " DescripcionES LIKE 'ARTIFICIO%') GROUP BY P.IdProducto, P.CodProducto,P.DescripcionES,P.UriES ORDER BY SUM(T.Valor) DESC";

                sql += sql2 + sql3;
            }
            else
            {
                sql = "SELECT P.IdProducto, Partida = P.CodProducto, Descripcion = P.CodProducto + ' - ' + P.DescripcionEN, " +
                      "PaisAduana = (SELECT TOP 1 PA.PaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), " +
                      "Regimen = (SELECT TOP 1 T.Regimen FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana WHERE T.IDPRODUCTO = P.IDPRODUCTO ORDER BY T.VALOR DESC), " +
                      "Uri = P.UriEN FROM Producto P INNER JOIN Totales T ON P.IdProducto = T.IdProducto WHERE P.IdProducto IN (SELECT PR.IdProducto FROM (SELECT IdProducto, DescripcionEN, Partida = CodProducto FROM Producto " +
                      "UNION ALL SELECT IdProducto, DescripcionEN = Descripcion_EN, Partida FROM Partida) AS PR WHERE ";
                
                

                foreach (var substring in substrings)
                {
                    sql2 += " DescripcionEN LIKE  '" + substring + "%' OR Partida LIKE  '" + substring + "%' OR ";
                }

                sql3 = " DescripcionES LIKE 'ARTIFICIO%') GROUP BY P.IdProducto, P.CodProducto,P.DescripcionEN,P.UriEN ORDER BY SUM(T.Valor) DESC";

                sql += sql2 + sql3;
            }*/
            //var dt = Conexion.SqlDataTableProductProfile(sql);          

            return json;
        }
        public static DataTable SearchCountry(string pais)
        {
            var sql = "SELECT IdPaisAduana FROM PAISADUANA WHERE PaisAduana = '" + pais + "'";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            return dt;
        }
        public static DataTable SearchNameCountry(int idPais)
        {
            var sql = "SELECT PaisAduana FROM PAISADUANA WHERE IdPaisAduana = '" + idPais + "'";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            return dt;
        }
        public static DataTable SearchRegimen(int IdProducto)
        {
            var sql = "SELECT TOP 1 T.Regimen FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana = T.IdPaisAduana " +
                      "WHERE T.IDPRODUCTO = " + IdProducto + " ORDER BY T.VALOR DESC";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }
            return dt;
        }
        public static bool ExistProduct(int idProducto)
        {
            var sql = "SELECT TOP 1 * FROM PRODUCTO WHERE IdProducto = " + idProducto;
            var dt = Conexion.SqlDataTableProductProfile(sql);
            return dt.Rows.Count > 0;
        }
        public static int SearchProductByUri(string uri, string partida)
        {
            var sql = "SELECT IdProducto FROM PRODUCTO " +
                      " WHERE IdProducto IN(SELECT IdProducto FROM Totales) " +
                      " AND(UriES = '" + uri + "' OR UriEN = '" + uri + "') AND CodProducto = '" + partida + "'";

            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            //var IdProducto = 
            return Convert.ToInt32(dt.Rows[0]["IdProducto"]);
        }
        public static int GetIdPais(int IdProducto, string PaisAduana)
        {
            int idPais = 0;

            string sql = "SELECT PA.IdPaisAduana FROM TOTALES T INNER JOIN PAISADUANA PA ON PA.IdPaisAduana=T.IdPaisAduana WHERE T.IDPRODUCTO = " + IdProducto +
                " AND PA.abreviatura2 = '" + PaisAduana +"' GROUP BY PA.IdPaisAduana";

//"SELECT IdPaisAduana FROM TOTALES WHERE IDPRODUCTO = " +
//                         IdProducto + " AND IdpaisAduana = " + IdPaisAduana +
//                         " GROUP BY IdPaisAduana";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            if (dt.Rows.Count > 0){
                idPais = Convert.ToInt32(dt.Rows[0]["IdPaisAduana"]);
            }
            return idPais;
        }
        public static string TranslateUri(string uri, string culture, string codPartida)
        {
            var sql = "";
            if (culture.Equals("es"))
            {
                sql = "SELECT Uri = UriEn FROM PRODUCTO " +
                          " WHERE UriES = '" + uri + "' AND CodProducto = '" + codPartida + "'";
            }
            else
            {
                sql = "SELECT Uri = UriEs FROM PRODUCTO " +
                      " WHERE UriEN = '" + uri + "' AND CodProducto = '" + codPartida + "'";
            }


            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTableProductProfile(sql);
            }
            catch (Exception e)
            {
                dt = null;
            }

            //var IdProducto = 
            return Convert.ToString(dt.Rows[0]["Uri"]);
        }

        public static async Task<Producto> GetProducto(Int64 pos, int pais)
        {
            var sql = "select * from (select t.*, ROW_NUMBER() OVER(ORDER BY DescripcionES, PaisAduana ASC) AS Nro from (select distinct pr.*, rtrim( pa.PaisAduana) PaisAduana from dbo.Producto pr (nolock) inner join dbo.Totales tt (nolock) on pr.IDPRODUCTO = tt.IDPRODUCTO inner join dbo.PAISADUANA pa (nolock) on pa.IdPaisAduana = tt.IdPaisAduana where len(rtrim(isNull(UriES,''))) > 0 and len(rtrim(isNull(UriEN,''))) > 0 and pa.IdPaisAduana='"+ pais  +"'  )  t) tt where tt.Nro=" + pos;
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataProductProfileString, sql);

            Producto modelo = null;
            foreach (DataRow row in dt.Rows)
            {
                modelo = new Producto
                {
                    IdProducto = Convert.ToInt32(row["IdProducto"]),
                    CodProducto = (row["CodProducto"].ToString()),
                    DescripcionES = row["DescripcionES"].ToString().Trim(),
                    DescripcionEN = row["DescripcionEN"].ToString().Trim(),
                    UriES = row["UriES"].ToString().Trim(),
                    UriEN = row["UriEN"].ToString().ToLower().Trim(),
                    PaisAduana = row["PaisAduana"].ToString().Trim(),
                    Nro = Convert.ToInt64(row["Nro"]),
                };
            }
            return modelo;
        }

        public static async Task<Int64> GetCountProductos()
        {
            var sql = "select Count(*) AS Cant from (select distinct pr.*, rtrim( pa.PaisAduana) PaisAduana from dbo.Producto pr (nolock) inner join dbo.Totales tt (nolock) on pr.IDPRODUCTO = tt.IDPRODUCTO inner join dbo.PAISADUANA pa (nolock) on pa.IdPaisAduana = tt.IdPaisAduana ) t";
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataProductProfileString, sql);

            Int64 cant = 0;
            foreach (DataRow row in dt.Rows)
            {
                cant = Convert.ToInt64(row["Cant"]);
            }
            return cant;
        }

        public static async Task<List<Models.Minisite.Paises>> GetListPais()
        {
            var sql = "select upper(pa.PaisAduana) PaisAduana, pa.IdPaisAduana , Cant = count(distinct tt.IdProducto) from dbo.paisaduana pa (nolock) inner join dbo.Totales tt (nolock) on pa.IdPaisAduana=tt.IdPaisAduana   group by pa.PaisAduana, pa.IdPaisAduana order by 1";
            var dt = await Conexion.SqlDataTableAsync(Conexion.DataProductProfileString, sql);

            var lst = new List<Models.Minisite.Paises>();
            foreach (DataRow row in dt.Rows)
            {
                lst.Add(new Models.Minisite.Paises() { Nombre = row["PaisAduana"].ToString(), Registros = row.GetValue<int>("Cant"), Id = row.GetValue<int>("IdPaisAduana") });
            }
            return lst;
        }

        public static string GetAbsoluteUrl(Producto obj, string base_url, string culture)
        {
            string label = culture == "es" ? "importaciones-y-exportaciones" : "imports-and-exports";
            return $"{base_url}/{culture}/{obj.PaisAduana.ToLower()}/{label}/{(culture == "es" ? obj.UriES : obj.UriEN )}/{obj.CodProducto}";
        }
    }

    public class Producto
    {
        public int IdProducto { get; set; }
        public string CodProducto { get; set; }
        public string DescripcionES { get; set; }
        public string DescripcionEN { get; set; }
        public string UriES { get; set; }
        public string UriEN { get; set; }
        public string PaisAduana { get; set; }
        public Int64 Nro { get; set; }
        public string AbsoluteUrlEs { get; set; }
        public bool UrlVisitadaEs { get; set; }
        public string AbsoluteUrlEn { get; set; }
        public bool UrlVisitadaEn { get; set; }

        public string Descripcion => DescripcionES + " - " + PaisAduana.ToUpper();
    }
}