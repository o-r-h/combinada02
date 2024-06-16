using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Dapper;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Models;

namespace Veritrade2017.Helpers
{
    public static class Funciones
    {
        public static string BuscaPartida(string IdPartida, string CodPais)
        {
            var partida = "";
            var sql = "select Nandina + ' ' + Partida as Partida from Partida_" + CodPais + " where IdPartida = " + IdPartida;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                partida = row["Partida"].ToString();
            }

            return partida;
        }

        public static string BuscaSubCapitulo(string CodSubCapitulo, string CodIdioma = "")
        {
            var subCapitulo = "";
            var sql = "select " + (CodIdioma == "" ? "SubCapitulo" : "SubCapituloEN") + " as SubCapitulo from SubCapitulo where CodSubCapitulo = '" + CodSubCapitulo + "' ";

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

        public static string BuscaIdPartida(string Nandina, string CodPais)
        {
            string IdPartida = "";
            var sql = "select IdPartida from Partida_" + CodPais + " where Nandina = '" + Nandina + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                IdPartida = row["IdPartida"].ToString();
            }

            return IdPartida;
        }

        /// <summary>
        /// Retorna el nombre de la empresa
        /// </summary>
        /// <param name="IdEmpresa">Identificador de la tabla empresa_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaEmpresa(string IdEmpresa, string CodPais)
        {
            string Empresa = "";
            var sql = "select Empresa from Empresa_" + CodPais + " where IdEmpresa = " + IdEmpresa;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Empresa = row["Empresa"].ToString();
            }

            return Empresa;
        }

        /// <summary>
        /// Retorna el nombre del Proveedor
        /// </summary>
        /// <param name="IdProveedor">Identificador de la tabla Proveedor_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaProveedor(string IdProveedor, string CodPais)
        {
            string Proveedor = "";
            var sql = "select Proveedor from Proveedor_" + CodPais + " where IdProveedor = " + IdProveedor;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Proveedor = row["Proveedor"].ToString();
            }

            return Proveedor;
        }

        /// <summary>
        /// Retorna el nombre del Imporator/Exportador
        /// </summary>
        /// <param name="IdImportadorExp">Identificador de la tabla ImportadorExp_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaImportadorExp(string IdImportadorExp, string CodPais)
        {
            string ImportadorExp = "";
            var sql = "select ImportadorExp from ImportadorExp_" + CodPais + " where IdImportadorExp = " + IdImportadorExp;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                ImportadorExp = row["ImportadorExp"].ToString();
            }

            return ImportadorExp;
        }

        /// <summary>
        /// Retorna el nombre del pais
        /// </summary>
        /// <param name="IdPais">Identificador de la tabla Pais_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaPais(string IdPais, string CodPais)
        {
            string Pais = "";
            var sql = "select Pais from Pais_" + CodPais + " where IdPais = " + IdPais;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Pais = row["Pais"].ToString();
            }

            return Pais;
        }

        /// <summary>
        /// Retorna la información sobre vias de tranporte
        /// </summary>
        /// <param name="IdViaTransp">Identificador de la tabla ViaTransp_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaVia(string IdViaTransp, string CodPais)
        {
            string ViaTransp = "";
            var sql = "select ViaTransp from ViaTransp_" + CodPais + " where IdViaTransp = " + IdViaTransp;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                ViaTransp = row["ViaTransp"].ToString();
            }

            return ViaTransp;
        }

        /// <summary>
        /// Retorna informacion sobre las aduanas
        /// </summary>
        /// <param name="IdAduana">Identificador de la tabla Aduana_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaAduana(string IdAduana, string CodPais)
        {
            string Aduana = "";
            var sql = "select Aduana from Aduana_" + CodPais + " where IdAduana = " + IdAduana;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Aduana = row["Aduana"].ToString();
            }

            return Aduana;
        }

        public static string Incoterm(string CodPais, string TipoOpe)
        {
            string aux = "";
            var sql = "select isnull(Incoterm, '') as Incoterm from BaseDatos ";
            sql += "where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                aux = row["Incoterm"].ToString();
            }

            return aux;
        }

        public static bool FlagDesComercial(string CodPais, string TipoOpe)
        {
            string aux = "";
            var sql = "select isnull(FlagDesComercial, '') as FlagDesComercial from BaseDatos where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                aux = row["FlagDesComercial"].ToString().ToUpper();
            }

            return aux == "S";
        }

        /// <summary>
        /// Retorna verdadero en el caso de encontrar la variable solicitada según la operación enviada
        /// </summary>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <param name="TipoOpe">identificador de operacion</param>
        /// <param name="Variable1">variable a buscar</param>
        /// <returns></returns>
        public static bool ExisteVariable(string CodPais, string TipoOpe, string Variable1)
        {
            bool aux;
            var sql = "select distinct codpais, tipoope, variable1 from VariableAgrupTest where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and Variable1 = '" + Variable1 + "' ";

            var dt = Conexion.SqlDataTable(sql);
            aux = dt.Rows.Count > 0;

            return aux;
        }

        public static string CampoPeso(string CodPais, string TipoOpe)
        {
            string aux = "";
            var sql = "select isnull(CampoPeso, '') as CampoPeso from BaseDatos ";
            sql += "where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                aux = row["CampoPeso"].ToString();
            }

            return aux;
        }

        public static string InfoEnLinea(string CodPais, string TipoOpe)
        {
            string FechaIni = "", FechaFin = "";
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-PE");
            var sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                FechaIni = row["FechaIni"].ToString();
                FechaIni = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-" + FechaIni.Substring(6, 2);
                FechaIni = Convert.ToDateTime(FechaIni).ToString("dd-MMM-yyyy", culture).ToUpper();
                FechaFin = row["FechaFin"].ToString();
                FechaFin = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-" + FechaFin.Substring(6, 2);
                FechaFin = Convert.ToDateTime(FechaFin).ToString("dd-MMM-yyyy", culture).ToUpper();
            }

            return "Información en línea desde " + FechaIni + " hasta " + FechaFin;
        }

        /// <summary>
        /// Retorna el rango de fecha de operación de un pais en las variables por referencia
        /// </summary>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <param name="TipoOpe">identificador de operacion</param>
        /// <param name="AñoIni">ref string AñoIni</param>
        /// <param name="MesIni">ref string MesIni</param>
        /// <param name="AñoFin">ref string AñoFin</param>
        /// <param name="MesFin">ref string MesFin</param>
        public static void Rango(string CodPais, string TipoOpe, ref string AñoIni, ref string MesIni, ref string AñoFin, ref string MesFin)
        {
            string FechaIni, FechaFin;
            var sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                FechaIni = row["FechaIni"].ToString();
                AñoIni = FechaIni.Substring(0, 4);
                MesIni = FechaIni.Substring(4, 2);
                FechaFin = row["FechaFin"].ToString();
                AñoFin = FechaFin.Substring(0, 4);
                MesFin = FechaFin.Substring(4, 2);
            }
        }

        public static string ListaItems(ArrayList Lista)
        {
            string aux = "(";
            for (int i = 0; i < Lista.Count; i++)
                aux += Lista[i].ToString() + ", ";
            aux = aux.Substring(0, aux.Length - 2);
            aux += ")";
            return aux;
        }

        //Revisar
        //public static ArrayList GuardaSeleccionados(string CodPais, string TipoOpe, GridView gdv, ArrayList IDsSeleccionados, ListBox lstSeleccionados)
        //{
        //    string ID, Tipo = "", Nombre = "";

        //    if (IDsSeleccionados == null) IDsSeleccionados = new ArrayList();
        //    foreach (GridViewRow row in gdv.Rows)
        //    {
        //        if (gdv.ID != "gdvAduanaDUAs")
        //            ID = gdv.DataKeys[row.RowIndex].Value.ToString();
        //        else
        //            ID = gdv.DataKeys[row.RowIndex].Values[0].ToString() + '-' + gdv.DataKeys[row.RowIndex].Values[1].ToString();

        //        if (ID.Substring(0, 1) == "F" || ID.Substring(0, 1) == "G")
        //            ID = ID.Substring(1, ID.Length - 1);
        //        bool FlagSeleccionado = ((CheckBox)row.FindControl("chkSel")).Checked;
        //        if (FlagSeleccionado)
        //        {
        //            if (!IDsSeleccionados.Contains(ID))
        //            {
        //                IDsSeleccionados.Add(ID);
        //                switch (gdv.ID)
        //                {
        //                    case "gdvPartidas":
        //                    case "gdvPartidas2": Tipo = "2PA"; Nombre = "[Partida] " + BuscaPartida(ID, CodPais); break;
        //                    case "gdvImportadores":
        //                    case "gdvImportadores2":
        //                        if (TipoOpe == "I")
        //                        { Tipo = "3IM"; Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais); }
        //                        else
        //                        { Tipo = "3EX"; Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais); }
        //                        break;
        //                    case "gdvProveedores":
        //                    case "gdvProveedores2":
        //                        if (TipoOpe == "I")
        //                        { Tipo = "4PR"; Nombre = "[Exportador] " + BuscaProveedor(ID, CodPais); }
        //                        else
        //                        { Tipo = "4IE"; Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais); }
        //                        break;
        //                    case "gdvPaisesOrigen":
        //                    case "gdvPaisesOrigen2":
        //                        if (TipoOpe == "I")
        //                        { Tipo = "5PO"; Nombre = "[País Origen] " + BuscaPais(ID, CodPais); }
        //                        else
        //                        { Tipo = "5PD"; Nombre = "[País Destino] " + BuscaPais(ID, CodPais); }
        //                        break;
        //                    case "gdvViasTransp":
        //                    case "gdvViasTransp2": Tipo = "6VT"; Nombre = "[Vía Transporte] " + BuscaVia(ID, CodPais); break;
        //                    case "gdvAduanaDUAs": Tipo = "7AD"; Nombre = "[Aduana DUA] " + BuscaAduana(gdv.DataKeys[row.RowIndex].Values[0].ToString(), CodPais) + ' ' + gdv.DataKeys[row.RowIndex].Values[1].ToString(); break;
        //                }
        //                lstSeleccionados.Items.Add(new ListItem(Nombre, Tipo + ID));
        //            }
        //        }
        //        //else
        //        //    IDsSeleccionados.Remove(ID);
        //    }
        //    return IDsSeleccionados;
        //}

        public static DataTable BuscaFavoritos(string IdUsuario, string CodPais, string TipoOpe, string TipoFavorito, string FavoritoB, bool flagSeleccione)
        {
            var sql = "";
            if (flagSeleccione)
                sql += "select 0 as IdFavorito, '[ Seleccione]' as Favorito union ";
            if (TipoFavorito == "Partida")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = 'PA' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Nandina + ' ' + case when len(Partida) > 80 then substring(Partida, 1, 80) else Partida end as Favorito ";
                sql += "from V_FavUnicos F, Partida_PE P ";
                sql += "where F.IdFavorito = P.IdPartida ";
                sql += "and F.IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and F.TipoOpe = '" + TipoOpe + "' and TipoFav = 'PA' ";
                if (FavoritoB != "")
                    sql += "and Partida like '%" + FavoritoB + "%' ";
                sql += "order by 2";
            }
            else if (TipoFavorito == "Importador" || TipoFavorito == "Exportador")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito  ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Empresa as Favorito ";
                sql += "from V_FavUnicos F, Empresa_" + CodPais + " E ";
                sql += "where F.IdFavorito = E.IdEmpresa ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' and TipoFav = '" + (TipoOpe == "I" ? "IM" : "EX") + "' ";
                if (FavoritoB != "")
                    sql += "and Empresa like '%" + FavoritoB + "%' ";
                sql += "order by 2";
            }
            else if (TipoFavorito == "Proveedor")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql += "select IdFavorito, Proveedor as Favorito ";
                sql += "from V_FavUnicos F, Proveedor_" + CodPais + " P ";
                sql += "where F.IdFavorito = P.IdProveedor ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'PR' ";
                if (FavoritoB != "")
                    sql += "and Proveedor like '%" + FavoritoB + "%' ";
                sql += "order by 2 ";
            }
            else if (TipoFavorito == "ImportadorExp")
            {
                sql += "select IdGrupo as IdFavorito, '[G] ' + Grupo as Favorito ";
                sql += "from Grupo ";
                sql += "where IdUsuario = " + IdUsuario + " and CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                if (FavoritoB != "")
                    sql += "and Grupo like '%" + FavoritoB + "%' ";
                sql += "union ";
                sql = "select IdFavorito, ImportadorExp as Favorito ";
                sql += "from V_FavUnicos F, ImportadorExp_" + CodPais + " I ";
                sql += "where F.IdFavorito = I.IdImportadorExp ";
                sql += "and IdUsuario = " + IdUsuario + " and F.CodPais = '" + CodPais + "' and TipoFav = 'IE' ";
                if (FavoritoB != "")
                    sql += "and ImportadorExp like '%" + FavoritoB + "%' ";
                sql += "order by 2";
            }

            var dt = Conexion.SqlDataTable(sql);
            return dt;
        }

        public static DataTable ListaPaisesRegimen(bool FlagTodos, string CodIdioma = "")
        {
            string PaisRegimen = "PaisRegimen";
            if (CodIdioma == "en") PaisRegimen = "PaisRegimenEN";

            var sql = "select distinct " + (FlagTodos ? "" : "top 9") + " M.CodPais, " + PaisRegimen + " as PaisRegimen, Bandera, FechaFin ";
            sql += "from PaisRegimen M, BaseDatos B ";
            sql += "where M.CodPais = B.CodPais order by FechaFin desc, " + PaisRegimen;

            var dt = Conexion.SqlDataTable(sql);
            return dt;
        }

        public static void BuscaDatosCorreoEnvio(string IdCorreo, ref string Correo, ref string Nombre, ref string Clave)
        {
            var sql = "select Correo, Nombre, Clave from Correo where IdCorreo = " + IdCorreo;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Correo = row["Correo"].ToString();
                Nombre = row["Nombre"].ToString();
                Clave = row["Clave"].ToString();
            }
        }

        public static string CreaSolicitud(string CodSolicitud, string Nombres, string Empresa, string Telefono, string Email1, string Mensaje, string DireccionIP, string Ubicacion)
        {
            string IdSolicitud = "";

            var sql = "insert into Solicitud(CodSolicitud, Nombres, Empresa, Telefono, Email1, Mensaje, DireccionIP, Ubicacion, Fecha) values ";
            sql += "('" + CodSolicitud + "', '" + Nombres + "', '" + Empresa + "', '" + Telefono + "', '" + Email1 + "', '" + Mensaje + "', ";
            sql += "'" + DireccionIP + "', '" + Ubicacion + "', getdate())";

            Conexion.SqlExecute(sql);

            sql = "select max(IdSolicitud) as IdSolicitud from Solicitud";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                IdSolicitud = row["IdSolicitud"].ToString();
            }

            return IdSolicitud;
        }

        public static DataRow ObtieneSolicitud(string IdSolicitud)
        {
            var sql = "select * from Solicitud where IdSolicitud = " + IdSolicitud;

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public static DataRow ObtieneUsuario(string CodUsuario)
        {
            var sql = "select EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, P.Pais, Password, DireccionIP, IdTipo, T.Valor as Tipo, O.Valor as Origen, CodCampaña, FecFin, FecRegistro, ImporteUSD, Ubicacion, CodTelefono ";
            sql += "from Usuario U, AdminPais P, AdminValor T, AdminValor O, AdminPaisN N ";
            sql += "where U.CodPais = P.CodPais and U.IdTipo = T.IdAdminValor and U.IdOrigen = O.IdAdminValor and CodUsuario = '" + CodUsuario + "' and U.CodPais = N.CodPais";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        public static DataRow ObtenerPartidaPorIdProducto(string IdProducto)
        {
            var sql =   "SELECT IdPartida, IdProducto, IdpaisAduana, Partida, Descripcion_ES, Descripcion_EN, UriES, UriEN ";
                sql +=  "FROM partida ";
                sql += $"WHERE IdProducto='{IdProducto}'";
            var dt = Conexion.SqlDataTable(sql);

            return dt.Rows[0];
        }

        //Crea relación entre un código de empresa o de producto con el código del usuario prueba creado
        public static bool CreaRelacionUsuarioPruebaGratis(
            string codUsuario, string tipoConsulta, string nomPais, string rucEmpresa, string nombreEmpresa, string codProducto, string descripcion)
        {
            bool result;
            try 
            { 
                var sql = "INSERT INTO ConsultaUsuarioPrueba(CodUsuario, Tipo, NombrePais, RucEmpresa, Empresa, CodProducto, Descripcion) VALUES ";
                sql += $"('{codUsuario}', '{tipoConsulta}', '{nomPais}', '{rucEmpresa}','{nombreEmpresa}', '{codProducto}' , '{descripcion}')";

                Conexion.SqlExecute(sql);
                result = true;
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                result = false;
            }
            return result;
        }

        public static DataRow ObtieneUsuarioCorreo(string CodUsuario)
        {
            var sql = "select EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, P.Pais, Password, DireccionIP, FecInicio, FecFin, P1.CodTelefono, IdTipo, T.Valor as Tipo, O.Valor as Origen, ";
            sql += "U.CodCampaña, isnull(C.Pais, '') as PaisCampaña, PC.CodTelefono as CodTelefonoC, FecFin, FecRegistro, u.idUsuario ";
            sql += "from Usuario U, AdminPais P, AdminPaisN P1, AdminValor T, AdminValor O, Campaña C, AdminPaisN PC ";
            sql += "where U.CodPais = P.CodPais and U.CodPais = P1.CodPais and U.IdTipo = T.IdAdminValor and U.IdOrigen = O.IdAdminValor and U.CodCampaña = C.CodCampaña and C.CodPaisC = PC.CodPais ";
            sql += "and CodUsuario = '" + CodUsuario + "' ";

            var dt = Conexion.SqlDataTable(sql);

            return dt.Rows[0];
        }

        /*Metodo que permite obtener datos del usuario para su registro en MixPanel
        JANAQ 160620  
       */
        public static UsuarioMixPanel ObtieneDatosUsuarioPorUsuario(string idUsuario)
        {
            using (var db = new ConexProvider().Open)
            {
                UsuarioMixPanel result = db.QueryFirstOrDefault<UsuarioMixPanel>("dbo.USP_ObtenerDatosUsuarioMixPanelPorUsuario", new
                {
                    IdUsuario = idUsuario
                }, commandType: CommandType.StoredProcedure);


                return result;
            }
        }

        public static List<string> ObtenerPaisesUsuario(int idUsuario, string idioma)
        {
            List<string> retorno = new List<string>();
            string campoDescripcion = "descripcion_eng";
            if (idioma == "es")
            {
                campoDescripcion = "descripcion";
            }
            string sql = $"select variableGeneral.{campoDescripcion} from suscripcion "
                            + $"inner join variableGeneral on suscripcion.codPais = variableGeneral.idvariable "
                            + $"where variableGeneral.estado = 1 and suscripcion.idusuario = {idUsuario}";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {

                    retorno.Add(row[campoDescripcion].ToString());
                }
            }
            catch (Exception e)
            {
                ExceptionUtility.LogException(e, "ObtenerPaisesUsuario");
            }
            return retorno;
        }

        public static DataRow ObtieneCampaña(string codCampaña)
        {
            var sql = "select * from Campaña where CodCampaña = '" + codCampaña + "'";

            var dt = Conexion.SqlDataTable(sql);
            if(dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }

        public static string BuscaCorreoAdmin(string usuarioAdmin)
        {
            var correo = "";
            var sql = "select Correo from [Admin] where UsuarioAdmin = '" + usuarioAdmin + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                correo = row["Correo"].ToString();
            }

            return correo;
        }

        public static void RemoveCacheOfContenidoGeneral()
        {
            var cacheManager = new OutputCacheManager();

            //remove a single cached action output (Index action)
            cacheManager.RemoveItems("Ayuda");
            cacheManager.RemoveItems("Blog");
            cacheManager.RemoveItems("Empresas");
            cacheManager.RemoveItems("Home");
            cacheManager.RemoveItems("Paises");
            cacheManager.RemoveItems("Planes");
            cacheManager.RemoveItems("Productos");
            cacheManager.RemoveItems("PruebaGratis");
            cacheManager.RemoveItems("Servicios");
        }

        public static string InfoEnLinea(string CodPais, string TipoOpe, string idioma)
        {
            string FechaIni = "", FechaFin = "", formatoFecha = "";
            //System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-PE");

            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(idioma);

            if (culture.ToString().Equals("es"))
            {
                formatoFecha = "dd-MMM-yyyy";
            }
            else
            {
                formatoFecha = "MMM dd, yyyy";
            }

            var sql = "select FechaIni, FechaFin from BaseDatos where CodPais ='" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                FechaIni = row["FechaIni"].ToString();
                FechaIni = FechaIni.Substring(0, 4) + "-" + FechaIni.Substring(4, 2) + "-" + FechaIni.Substring(6, 2);
                FechaIni = Convert.ToDateTime(FechaIni).ToString(formatoFecha, culture).ToUpper();
                FechaFin = row["FechaFin"].ToString();
                FechaFin = FechaFin.Substring(0, 4) + "-" + FechaFin.Substring(4, 2) + "-" + FechaFin.Substring(6, 2);
                FechaFin = Convert.ToDateTime(FechaFin).ToString(formatoFecha, culture).ToUpper();
            }

            string infoEnLinea = "";
            if (culture.ToString().Equals("es"))
                infoEnLinea = "Información en línea desde " + FechaIni + " hasta " + FechaFin;
            else
                infoEnLinea = "Online information from " + FechaIni + " to " + FechaFin;
            return infoEnLinea;
        }

        public static string ListaItemsS(ArrayList Lista)
        {
            string aux = "(";
            for (int i = 0; i < Lista.Count; i++)
                aux += "'" + Lista[i].ToString() + "', ";
            aux = aux.Substring(0, aux.Length - 2);
            aux += ")";
            return aux;
        }

        public static ArrayList EliminaSeleccion(ArrayList Acumulado, string Id)
        {
            if (Acumulado != null)
            {
                if (Acumulado.Contains(Id)) Acumulado.Remove(Id);
                if (Acumulado.Count == 0) Acumulado = null;
                return Acumulado;
            }
            return null;
        }

        public static string BuscaMarca(string IdMarca, string CodPais)
        {
            var marca = "";
            var sql = "select Marca from Marca_" + CodPais + " where IdMarca = " + IdMarca;
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                marca = row["Marca"].ToString();
            }
            return marca;
        }

        public static string BuscaMarca(string idMarca)
        {
            string sql = "select Marca from Marca_PEB where IdMarca = " + idMarca;
            string marca = "";
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    marca = (dt.Rows[0])["Marca"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return marca;
        }

        public static string BuscaModelo(string IdModelo, string CodPais)
        {
            var modelo = "";
            var sql = "select Marca, Modelo from Modelo_" + CodPais + " where IdModelo = " + IdModelo;
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                modelo = row["Marca"].ToString() + " - " + row["Modelo"].ToString();
            }
            return modelo;
        }

        public static string BuscaModelo(string idModelo)
        {
            string modelo = "";
            string sql = "select Marca, Modelo from Modelo_PEB where IdModelo = " + idModelo;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    modelo = (dt.Rows[0])["Marca"].ToString() + " - " + (dt.Rows[0])["Modelo"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return modelo;
        }

        public static string BuscaDistrito(string IdDistrito, string CodPais)
        {
            var distrito = "";
            var sql = "select Distrito from Distrito_" + CodPais + " where IdDistrito = " + IdDistrito;
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                distrito = row["Distrito"].ToString();
            }
            return distrito;
        }

        public static ArrayList GuardaSeleccionados(string CodPais, string TipoOpe, string filtro,
                                                string[] listSeleccionados, ArrayList IDsSeleccionados, ref List<object> listNuevosFiltros)
        {
            var json = new List<object>();

            string ID, Tipo = "", Nombre = "";

            if (IDsSeleccionados == null)
                IDsSeleccionados = new ArrayList();

            for (int i = 0; i < listSeleccionados.Length; i++)
            {
                ID = listSeleccionados[i];

                if (ID.Substring(0, 1) == "F" || ID.Substring(0, 1) == "G")
                    ID = ID.Substring(1, ID.Length - 1);
                if (!IDsSeleccionados.Contains(ID))
                {
                    IDsSeleccionados.Add(ID);
                    switch (filtro)
                    {
                        case "Partida":
                            Tipo = "2PA";
                            Nombre = "[Partida] " + BuscaPartida(ID, CodPais);
                            break;
                        case "Marca":
                            Tipo = "2MA";
                            Nombre = "[Marca] " + BuscaMarca(ID, CodPais);
                            break;
                        case "Modelo":
                            Tipo = "2MO";
                            Nombre = "[Modelo] " + BuscaModelo(ID, CodPais);
                            break;
                        case "Importador":
                            Tipo = "3IM";
                            Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais);
                            break;
                        case "Exportador":
                            Tipo = "3EX"; Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais);
                            break;
                        //case "Importador": case "ImportadorExp":
                        //    if (TipoOpe == "I"){
                        //        Tipo = "3IM"; Nombre = "[Importador] " + BuscaEmpresa(ID, CodPais);
                        //    }else{
                        //        Tipo = "3EX"; Nombre = "[Exportador] " + BuscaEmpresa(ID, CodPais);
                        //    }
                        //    break;
                        case "gdvNotificados":
                            //Tipo = "3NO"; Nombre = "[Notificado] " + BuscaNotificado(ID, CodPais);
                            break;
                        case "Proveedor":
                            Tipo = "4PR";
                            Nombre = ((CodPais != "CL") ? "[Exportador] " : "[Marca] ") + BuscaProveedor(ID, CodPais);
                            break;
                        case "ImportadorExp":
                            Tipo = "4IE";
                            Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais);
                            break;
                        //case "Proveedor": case "Exportador":
                        //    if (TipoOpe == "I"){
                        //        Tipo = "4PR"; Nombre = ((CodPais != "CL") ? "[Exportador] " : "[Marca] ") + BuscaProveedor(ID, CodPais);
                        //    }else{
                        //        Tipo = "4IE"; Nombre = "[Importador] " + BuscaImportadorExp(ID, CodPais);
                        //    }
                        //    break;
                        case "PaisOrigen":
                        case "PaisDestino":
                            if (TipoOpe == "I" && CodPais != "USI")
                            {
                                Tipo = "5PO"; Nombre = "[País Origen] " + BuscaPais(ID, CodPais);
                            }
                            else if (TipoOpe == "I" && CodPais == "USI")
                            {
                                Tipo = "5PE"; Nombre = "[Último País Embarque] " + BuscaPais(ID, CodPais);
                            }
                            else
                            {
                                Tipo = "5PD"; Nombre = "[País Destino] " + BuscaPais(ID, CodPais);
                            }
                            break;
                        case "ViaTransp":
                            if (CodPais == "USI")
                            {
                                Tipo = "6PD"; Nombre = "[Puerto Descarga] " + BuscaPuerto(ID, CodPais);
                            }
                            else if (CodPais == "PEE")
                            {
                                Tipo = "6DE"; Nombre = "[Puerto Destino] " + BuscaPuerto(ID, CodPais);
                            }
                            else if (CodPais == "USE" || CodPais == "PEI")
                            {
                                Tipo = "6PE"; Nombre = "[Último Puerto Embarque] " + BuscaPuerto(ID, CodPais);
                            }
                            else
                            {
                                Tipo = "6VT"; Nombre = "[Vía Transporte] " + BuscaVia(ID, CodPais);
                            }
                            break;
                        case "AduanaDUA":
                            Tipo = "7AD";
                            string[] ids = ID.Split(new Char[] { '-' });
                            Nombre = "[Aduana DUA] " + BuscaAduana(ids[0], CodPais) + ' ' + ids[1];
                            break;
                        case "Aduana":
                            Tipo = "7AA";
                            Nombre = "[Aduana] " + BuscaAduana(ID, CodPais);
                            break;
                        case "Distrito":
                            Tipo = "8DI";
                            Nombre = "[Distrito] " + BuscaDistrito(ID, CodPais);
                            break;
                        case "gdvManifiestos":
                            //verifcar con pruebas
                            break;
                    }
                    json.Add(new
                    {
                        valueOption = Tipo + ID,
                        labelOption = Nombre
                    });
                }

            }

            listNuevosFiltros = json;
            return IDsSeleccionados;
        }

        public static string BuscaPuerto(string IdPto, string CodPais)
        {
            string Puerto = "";
            var sql = "select Puerto from Puerto_" + CodPais + " where IdPto = " + IdPto;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Puerto = row["Puerto"].ToString();
            }
            return Puerto;
        }

        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public static string RenderMailViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);
            var viewResult = ViewEngines.Engines.FindPartialView(context, viewName).View;


            //viewPage.ViewData = new ViewDataDictionary(viewData);
            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    var viewContext = new ViewContext(context, viewResult, viewData, new TempDataDictionary(), tw);
                    //ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };
                    viewResult.Render(viewContext, tw);
                }

                //viewResult.View.Render(viewContext, sw);


            }
            return sb.ToString();
        }

    }
}