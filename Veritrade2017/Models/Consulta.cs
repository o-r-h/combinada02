using System;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class Consulta
    {
        public string IdAutocompletado { get; set; }
        public string Interes { get; set; }
        public string Regimen { get; set; }
        public string Busqueda { get; set; }
        public string Filtro { get; set; }
        public string Importador { get; set; }
        public string Exportador { get; set; }

        /// <summary>
        /// Verifica si el nombre de la variable existe dentro de la configuracion
        /// </summary>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <param name="tipoOpe">identificador de operacion</param>
        /// <param name="variable"></param>
        /// <returns>Retorna un valor del tipo Boolean</returns>
        public static bool ExisteVariable(string codPais, string tipoOpe, string variable)
        {
            var sql = "SELECT DISTINCT [CodPais],[TipoOpe],[Variable1] FROM [dbo].[VariableAgrupTest] " +
                      "WHERE CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "' and Variable1 = '" + variable + "'";

            var dt = Conexion.SqlDataTable(sql);

            var aux = dt != null && dt.Rows.Count > 0;
            return aux;
        }

        /// <summary>
        /// Verifica si tiene una descripción comercial dentro de las configuraciones exista el valor de Descripcion comercial
        /// </summary>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <param name="tipoOpe">identificador de operacion</param>
        /// <returns>Retorna un valor del tipo Boolean</returns>
        public static bool FlagDesComercial(string codPais, string tipoOpe)
        {
            var aux = "";
            var sql = "select isnull(FlagDesComercial, '') as FlagDesComercial from BaseDatos where CodPais = '" + codPais + "' and TipoOpe = '" + tipoOpe + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                aux = Convert.ToString(row["FlagDesComercial"]).ToUpper();
            }

            return aux == "S";
        }

        public static bool EsStopWord(string palabra)
        {
            var cant = 0;
            var sql = "select count(*) as Cant from StopWords where StopWord = '" + palabra + "'";
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                cant = Convert.ToInt32(row["Cant"]);
            }

            return (cant > 0);
        }

        public static DataTable BuscaPartidas(string nandina, string codPais, bool flagConsultaGratis)
        {
            var cant = 0;
            var cantCaracteres = 90;

            if (!flagConsultaGratis) cantCaracteres = 60;

            var sql = "select count(*) as Cant from Partida_" + codPais + " ";
            sql += "where Nandina like '" + nandina + "%' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                cant = Convert.ToInt32(row["Cant"]);
            }

            var todos = "TODOS";
            var subCapitulo = "SubCapitulo";
            var hts6 = "HTS6";
            if (codPais == "BR" || codPais == "CN" || codPais == "IN" || codPais == "US")
            {
                todos = "ALL";
                subCapitulo = "SubCapituloEN";
                hts6 = "HTS6_EN";
            }

            sql = "";
            if (cant > 0)
            {
                sql += "select CodSubCapitulo as Nandina2, '" + nandina + "' as Nandina, replace(convert(char(13), '" + nandina + "'), ' ', '_') + ' [" + todos + " 4] ' + substring(" + subCapitulo + ", 1, " + cantCaracteres + ") as Partida ";
                sql += "from SubCapitulo where CodSubCapitulo = '" + nandina + "' union ";
                sql += "select CodHTS6 as Nandina2, CodHTS6 as Nandina, replace(convert(char(13), CodHTS6), ' ', '_') + ' [" + todos + " 6] ' + substring(" + hts6 + ", 1, " + cantCaracteres + ") as Partida ";
                sql += "from HTS6 H, Partida_" + codPais + " P where H.CodHTS6 = substring(P.Nandina, 1, 6) and CodHTS6 like '" + nandina + "%' union ";
            }
            sql += "select Nandina as Nandina2, Nandina, replace(convert(char(13), Nandina), ' ', '_') + ' ' + substring(Partida, 1, " + (cantCaracteres + 10).ToString() + ") as Partida from Partida_" + codPais + " ";
            sql += "where Nandina like '" + nandina + "%' or Partida like '%" + nandina + "%' ";
            sql += "order by Nandina";

            var dt2 = Conexion.SqlDataTable(sql);
            return dt2;
        }

        public static DataTable BuscaEmpresas(string empresa, string codPais)
        {
            var tabla = (codPais == "PE") ? "Empresa_PE_con_datos" : "Empresa_" + codPais;
            var ruc = (codPais == "PE" || codPais == "CL") ? "RUC" : "'' as RUC";
            var condRuc = (codPais == "PE" || codPais == "CL") ? "or RUC like '" + empresa + "%'" : "";

            var sql = "select * from (select top 50 1 as Orden, IdEmpresa, Empresa, " + ruc + " from " + tabla + " ";
            sql += "where Empresa like '" + empresa + "%' " + condRuc + " order by Empresa) T union ";

            sql += "select * from (select top 50 2 as Orden, IdEmpresa, Empresa, " + ruc + " from " + tabla + " ";
            sql += "where 1 = 1 ";

            for (var index = 0; index < empresa.Split(' ').Length; index++)
            {
                var emp = empresa.Split(' ')[index];
                sql += "and Empresa like '%" + emp + "%' ";
            }
            sql += "and IdEmpresa not in (select IdEmpresa from " + tabla + " where Empresa like '" + empresa + "%') ";
            sql += "order by Empresa) T2 order by 1, 3 ";

            var dt2 = Conexion.SqlDataTable(sql);
            return dt2;
        }

        public static string BuscaIdPartida(string nandina, string codPais)
        {
            var idPartida = "";
            var sql = "select IdPartida from Partida_" + codPais + " where Nandina = '" + nandina + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idPartida = row["IdPartida"].ToString();
            }

            return idPartida;
        }

        public static string BuscaSubCapitulo(string codSubCapitulo, string codIdioma = "")
        {
            var subCapitulo = "";
            var sql = "select " + (codIdioma == "" ? "SubCapitulo" : "SubCapituloEN") + " as SubCapitulo from SubCapitulo where CodSubCapitulo = '" + codSubCapitulo + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                subCapitulo = row["SubCapitulo"].ToString();
            }

            return subCapitulo;
        }

        public static string BuscaHts6(string codHts6, string codIdioma = "")
        {
            var hts6 = "";
            var sql = "select " + (codIdioma == "" ? "HTS6" : "HTS6_EN") + " as HTS6 from HTS6 where CodHTS6 = '" + codHts6 + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                hts6 = row["HTS6"].ToString();
            }

            return hts6;
        }

        /// <summary>
        /// Retorna los datos de la Partida
        /// </summary>
        /// <param name="idPartida"> id de la Partida</param>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaPartida(string idPartida, string codPais)
        {
            var partida = "";
            var sql = "select Nandina + ' ' + Partida as Partida from Partida_" + codPais + " where IdPartida = " + idPartida;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                partida = row["Partida"].ToString();
            }

            return partida;
        }
        
        /// <summary>
        /// Retorna el nombre de la empresa
        /// </summary>
        /// <param name="idEmpresa">Identificador de la tabla empresa_(CodPais)</param>
        /// <param name="codPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaEmpresa(string idEmpresa, string codPais)
        {
            var empresa = "";
            var sql = "select Empresa from Empresa_" + codPais + " where IdEmpresa = " + idEmpresa;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                empresa = row["Empresa"].ToString();
            }

            return empresa;
        }

        public static DataTable AgrupadoDataPie(string Filtro, string CodPais, string CIFTot,
           string SessionSqlFiltro, string tabla, string DUA)
        {
            string sql = "";
            string lsCifTot = CIFTot;
            if (CodPais == "BR" || CodPais == "IN")
                lsCifTot = "convert(decimal(19,2), " + CIFTot + ")";

            if (Filtro == "Partida")
            {
                sql = "select P.IdPartida, P.Nandina, P.Nandina + ' ' + P.Partida as Partida, CantReg, " + CIFTot + " ";
                sql += "from Partida_" + CodPais + " P, (select IdPartida, count(*) as CantReg, sum(" + lsCifTot +
                       ") as " + CIFTot + " ";
            }
            else if (Filtro == "Modelo")
                sql = "select IdModelo, Marca + ' - ' + Modelo as Modelo, count(*) as CantReg, sum(" + lsCifTot +
                      ") as " + CIFTot + " ";
            else if (Filtro == "AduanaDUA")
                sql = "select IdAduana, Aduana, " +
                      " sum(" + lsCifTot + ") as " + CIFTot + " ";
            else
                sql = "select Id" + Filtro + ", rtrim(" + Filtro + ") as " + Filtro + ", count(*) as CantReg, sum(" +
                      lsCifTot + ") as " + CIFTot + " ";

            sql += "from " + tabla + " T where 1 = 1 ";
            //sql += Session["SqlFiltro"].ToString();
            sql += SessionSqlFiltro;


            if (Filtro == "Partida")
            {
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
                sql += "order by 5 desc";
            }
            else if (Filtro == "Modelo")
                sql += "group by IdModelo, Marca, Modelo order by 4 desc";
            else if (Filtro == "Importador" || Filtro == "Exportador")
            {
                sql += "group by Id" + Filtro + ", " + Filtro + " ";
                sql += "order by 4 desc";
            }
            else if (Filtro == "AduanaDUA")
            {
                sql += "group by IdAduana, Aduana ";
                sql += "order by 3 desc";
            }
            else
            {
                sql += "group by Id" + Filtro + ", " + Filtro + " ";
                sql += "order by 4 desc";
            }

            return Conexion.SqlDataTable(sql);
        }

        public static DataTable Agrupado(string Filtro, string CodPais, string CIFTot,
            string SessionSqlFiltro, string tabla, string DUA)
        {
            string sql = "";
            string lsCifTot = CIFTot;
            if (CodPais == "BR" || CodPais == "IN")
                lsCifTot = "convert(decimal(19,2), " + CIFTot + ")";

            if (Filtro == "Partida")
            {
                sql = "select P.IdPartida, P.Nandina, P.Nandina + ' ' + P.Partida as Partida, CantReg, " + CIFTot + " ";
                sql += "from Partida_" + CodPais + " P, (select IdPartida, count(*) as CantReg, sum(" + lsCifTot +
                       ") as " + CIFTot + " ";
            }
            else if (Filtro == "Modelo")
                sql = "select IdModelo, Marca + ' - ' + Modelo as Modelo, count(*) as CantReg, sum(" + lsCifTot +
                      ") as " + CIFTot + " ";
            else if (Filtro == "AduanaDUA")
                sql = "select IdAduana, Aduana, " + DUA +
                      " as DUA, convert(varchar(3), IdAduana) + '-' + convert(varchar(20), " + DUA +
                      ") as IdAduanaDUA,  Aduana + ' ' + convert(varchar(20), " + DUA +
                      ") as AduanaDUA, count(*) as CantReg, sum(" + lsCifTot + ") as " + CIFTot + " ";
            else if(Filtro  == "PaisEmbarque")
            {
                //sql = "select IdPaisEmbarque, Pais as PaisEmbarque, count(*) as CantReg, ISNULL(" + lsCifTot +
                //     ",0) as " + CIFTot + " ";
                sql = "select IdPaisEmbarque, Pais as PaisEmbarque, CantReg, ISNULL(" + lsCifTot + ",0) AS " + CIFTot +
                      ", ROW_NUMBER() over (order by IdPaisEmbarque) as Nro ";
                sql += "from Pais_" + CodPais + " P, (select IdPaisEmbarque, count(*) as CantReg, sum(ISNULL(" + lsCifTot +
                       ",0)) / 1000 as " + CIFTot + " ";
            }
            else
                sql = "select Id" + Filtro + ", rtrim(" + Filtro + ") as " + Filtro + ", count(*) as CantReg, sum(" +
                      lsCifTot + ") as " + CIFTot + " ";

            sql += "from " + tabla + " T where 1 = 1 ";
            //sql += Session["SqlFiltro"].ToString();
            sql += SessionSqlFiltro;


            if (Filtro == "Partida")
            {
                sql += "group by IdPartida) T where P.IdPartida = T.IdPartida ";
                sql += "order by 5 desc";
            }
            else if (Filtro == "Modelo")
                sql += "group by IdModelo, Marca, Modelo order by 4 desc";
            else if (Filtro == "Importador" || Filtro == "Exportador")
            {
                sql += "group by Id" + Filtro + ", " + Filtro + " ";
                sql += "order by 4 desc";
            }
            else if (Filtro == "AduanaDUA")
            {
                sql += "group by IdAduana, Aduana, " + DUA + " ";
                sql += "order by 7 desc";
            }
            else if (Filtro == "PaisEmbarque")
            {
                sql += "group by IdPaisEmbarque) T where P.IdPais = T.IdPaisEmbarque ";
                sql += "order by 4 desc";
            }
            else
            {
                sql += "group by Id" + Filtro + ", " + Filtro + " ";
                sql += "order by 4 desc";
            }

            return Conexion.SqlDataTable(sql);
        }

        public static DataTable BuscarRegistrosDetalle(string SessionSqlFiltroR1, string tabla)
        {
            string sql = "";
            sql = "select * ";
            sql += "from " + tabla + " where 1 = 1 ";
            sql += SessionSqlFiltroR1;
            sql += "order by FechaNum desc";

            return Conexion.SqlDataTable(sql);
        }

    }


}