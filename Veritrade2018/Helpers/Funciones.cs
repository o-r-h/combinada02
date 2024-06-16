using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Veritrade2018.Models;
using Veritrade2018.Models.Admin;
using Veritrade2018.Util;

namespace Veritrade2018.Helpers
{
    public class Funciones
    {
        public const string ruta_descarga = "/VeritradeDownloads/";

        //public const int fav_x_pag = 20;
        public const int fav_x_pag = 10;

        public static IEnumerable<SelectListItem> CargarTrimestres(string TrimIni, string TrimFin ,string Idioma)
        {
            string lTrimestre = "TrimestreES";
            if (Idioma == "en") lTrimestre = "Trimestre";

            string sql = "select IdAño * 10 + IdTrimestre as IdAñoTrimestre, Trimestre + ' ' + convert(varchar(4), IdAño) as AñoTrimestre ";
            sql += "from Año, (select distinct IdTrimestre, " + lTrimestre + " as Trimestre from Mes) T ";
            sql += "where IdAño * 10 + IdTrimestre between " + TrimIni + " and " + TrimFin + " ";
            sql += "order by 1";
            var lista = new List<SelectListItem>();
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new SelectListItem { Value = row["IdAñoTrimestre"].ToString(), Text = @row["AñoTrimestre"].ToString()});
            }

            return lista;

        }

        public static IEnumerable<SelectListItem> CargarAños(string AñoIni, string AñoFin)
        {

            string sql = "select IdAño ";
            sql += "from Año ";
            sql += "where IdAño between " + AñoIni + " and " + AñoFin + " ";
            sql += "order by 1";
            var lista = new List<SelectListItem>();
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new SelectListItem { Value = row["IdAño"].ToString(), Text = @row["IdAño"].ToString() });
            }

            return lista;

        }

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

        public static bool BuscarVariableGeneral(string idVariable , string codPais)
        {
            bool flag = false;

            var sql = $@" SELECT IdParent,Valores
                            FROM VariableGeneral
                            WHERE IdVariable = '{idVariable}' and Estado = 1";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = (row["Valores"].ToString() == "1") && row["IdParent"].ToString() == codPais;                
            }


            return flag;
        }


        public static bool VisualizarExcel()
        {
            bool validar = false;
            var sql = string.Format(@"SELECT Estado 
                                        FROM VariableGeneral
                                        WHERE IdVariable = 'VED'");

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                validar = Convert.ToBoolean(row["Estado"]);
            }

            return validar;
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

        public static string BuscaIdTables(string campo, string tabla, string filtro,string campo2)
        {
            string IdPartida = "''";
            var sql = $"select {campo} from {tabla} where {campo2} like '{filtro}'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                IdPartida += ','+row[campo].ToString();
            }

            return IdPartida;
        }

        public static string BuscaCampoPorFiltro(string campo, string tabla, string filtro)
        {
            string IdPartida = "''";
            var sql = $"select {campo} from {tabla} where {filtro}";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                IdPartida += ',' + row[campo].ToString();
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

        public static string BuscaNotificado(string IdNotificado, string CodPais)
        {
            string sql;

            sql = "select Notificado from Notificado_" + CodPais + " where IdNotificado = " + IdNotificado;
            string Notificado = string.Empty;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Notificado = row["Notificado"].ToString();
            }

            return Notificado;
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
        /// <summary>
        /// Retorna el codigo del pais
        /// </summary>
        /// <param name="IdPais">Identificador de la tabla Pais_(CodPais)</param>
        /// <param name="CodPais">SIGLA internacional de un pais</param>
        /// <returns></returns>
        public static string BuscaCodPais(string IdPais, string CodPais)
        {
            string codPais = "";
            var sql = $"select t2.CodPais codPais from pais_{CodPais} t1 inner join AdminPaisN t2 on (t1.pais = t2.pais or t1.pais = t2.paisEn) where t1.IdPais = {IdPais}";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                codPais = row["codPais"].ToString();
            }

            return codPais;
        }


/// <summary>
/// Retorna la información sobre vias de tranporte
/// </summary>
/// <param name="IdMarca">Identificador de la tabla Marca_(CodPais)</param>
/// <param name="CodPais">SIGLA internacional de un pais</param>
/// <returns></returns>
public static string BuscaMarcaEC(string IdMarca, string CodPais)
        {
            string Marcas = "";
            var sql = "select Marca from Marca_" + CodPais + " where IdMarca = " + IdMarca;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Marcas = row["Marca"].ToString();
            }

            return Marcas;
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

            if(dt!=null)
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

            if (CodPais == "PEP")
                CodPais = "PE";

            var sql = "select distinct codpais, tipoope, variable1 from VariableAgrupTest where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
            sql += "and Variable1 = '" + Variable1 + "' ";

            var dt = Conexion.SqlDataTable(sql);
            aux = dt.Rows.Count > 0;

            return aux;
        }

        ///// <summary>
        ///// Retorna verdadero en el caso de encontrar la variable solicitada según la operación enviada V2
        ///// </summary>
        ///// <param name="CodPais">SIGLA internacional de un pais</param>
        ///// <param name="TipoOpe">identificador de operacion</param>
        ///// <param name="Variable1">variable a buscar</param>
        ///// <returns></returns>
        //public static bool ExisteVariable(string CodPais, string TipoOpe, VarGeneral.VarId varId)
        //{
        //    bool aux;

        //    if (CodPais == "PEP")
        //        CodPais = "PE";

        //    var sql = "select distinct codpais, tipoope, variable1 from VariableAgrupTest where CodPais = '" + CodPais + "' and TipoOpe = '" + TipoOpe + "' ";
        //    sql += "and Variable1 = '" + Variable1 + "' ";

        //    var dt = Conexion.SqlDataTable(sql);
        //    aux = dt.Rows.Count > 0;

        //    return aux;
        //}

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

        public static string ListaItemsS(ArrayList Lista)
        {
            string aux = "(";
            for (int i = 0; i < Lista.Count; i++)
                aux += "'" + Lista[i].ToString() + "', ";
            aux = aux.Substring(0, aux.Length - 2);
            aux += ")";
            return aux;
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

        public static DataRow ObtieneUsuario(string CodUsuario)
        {
            var sql = "select EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, Pais, Password, DireccionIP, IdTipo, T.Valor as Tipo, O.Valor as Origen, CodCampaña, FecFin, FecRegistro ";
            sql += "from Usuario U, AdminPais P, AdminValor T, AdminValor O ";
            sql += "where U.CodPais = P.CodPais and U.IdTipo = T.IdAdminValor and U.IdOrigen = O.IdAdminValor and CodUsuario = '" + CodUsuario + "' ";

            var dt = Conexion.SqlDataTable(sql);
            return dt.Rows[0];
        }

        


        public static DataRow ObtieneUsuarioCorreo(string CodUsuario)
        {
            var sql = "select EmpPer, Nombres + ' ' + Apellidos as Nombres, Empresa, Telefono, Email1, P.Pais, Password, DireccionIP, FecInicio, FecFin, P1.CodTelefono, IdTipo, T.Valor as Tipo, O.Valor as Origen, ";

            sql += "U.CodCampaña, isnull(C.Pais, '') as PaisCampaña, PC.CodTelefono as CodTelefonoC, FecFin, FecRegistro ";

            sql += "from Usuario U, AdminPais P, AdminPaisN P1, AdminValor T, AdminValor O, Campaña C, AdminPaisN PC ";

            sql += "where U.CodPais = P.CodPais and U.CodPais = P1.CodPais and U.IdTipo = T.IdAdminValor and U.IdOrigen = O.IdAdminValor and U.CodCampaña = C.CodCampaña and C.CodPaisC = PC.CodPais ";

            sql += "and CodUsuario = '" + CodUsuario + "' ";

            var dt = Conexion.SqlDataTable(sql);

            return dt.Rows[0];
        }

        public static DataRow ObtieneCampaña(string codCampaña)
        {
            var sql = "select * from Campaña where CodCampaña = '" + codCampaña + "'";

            var dt = Conexion.SqlDataTable(sql);
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

        /// <summary>
        /// Comprueba que el código de campaña este registrado
        /// </summary>
        /// <param name="codCampaña"></param>
        /// <returns>Valor booleano</returns>
        public static bool ExisteCodCampaña(string codCampaña)
        {
            var flag = false;
            var sql = "select count(*) as CantCodCampaña from Campaña where CodCampaña = '" + codCampaña + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = Convert.ToInt32(row["CantCodCampaña"]) > 0;
            }

            return flag;
        }

        public static string CrearUsuarioFreeTrial(string empPer, string codUsuario, string password, string nombres,
            string apellidos, string dni, string empresa, string ruc,
            string telefono, string celular, string email1, string idActividad, string mensaje, string idConocio,
            string direccionIp, string codCampaña, string url, string referido, string gclid = "", string pais = "")
        {
            var codPais = "";
            try
            {
                var idAplicacion = ObtieneIdAdminValor("01APL", "Business");
                var idVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
                var idTipo = ObtieneIdAdminValor("03TIP", "Free Trial");
                var idOrigen = ObtieneIdAdminValor("04ORG", "Veritrade");
                var idCargo = ObtieneIdAdminValor("05CAR", "Otro");
                var idEstadoVenta = ObtieneIdAdminValor("06EVT", "Nuevo");
                var idEstadoXqNo = ObtieneIdAdminValor("07EXN", "N/A");
                //IdActividad = ObtieneIdAdminValor("08ACT", "Importación");
                //IdConocio = ObtieneIdAdminValor("09CON", "Web");

                VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
                var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);
                if (codPais == "-") codPais = "PE";

                var sql =
                    "insert into Usuario(CodUsuario, Password, EmpPer, Empresa, RUC, Nombres, Apellidos, DNI, Telefono, Celular, Email1, ";
                sql +=
                    "IdAplicacion, IdTipo, IdOrigen, IdCargo, IdEstadoVenta, IdEstadoXQNo, IdActividad, IdConocio, Mensaje, DireccionIP, ";
                sql +=
                    "CodPaisIP, CodPaisIP2, CodPais, Ubicacion, CodEstado, FecInicio, FecFin, CodSeguridad, CantUsuariosUnicos, CodCampaña, URLRegistro, URLReferido, Gclid, FecRegistro, FecActualizacion) values ";
                sql += "('" + codUsuario + "', '" + password + "', '" + empPer + "', '" + empresa + "', '" + ruc +
                       "', '" + nombres + "', '" + apellidos + "', ";
                sql += "'" + dni + "', '" + telefono + "', '" + celular + "', '" + email1 + "', ";
                sql += idAplicacion + ", " + idTipo + ", " + idOrigen + ", " + idCargo + ", " + idEstadoVenta + ", " +
                       idEstadoXqNo + ", ";
                sql += idActividad + ", " + idConocio + ", '" + mensaje + "', '" + direccionIp + "', '" + codPais +
                       "', '" + codPais + "', '" + pais + "', '" + ubicacion + "', ";
                sql += "'A', " + DateTime.Now.ToString("yyyyMMdd") + "," +
                       DateTime.Now.AddDays(5).ToString("yyyyMMdd") + ", 'Off', 1, '" + codCampaña + "', '" + url +
                       "', '" + referido + "', '" + gclid + "', getdate(), getdate())";
                Conexion.SqlExecute(sql);

                string idUsuario = BuscarIdUsuario(codUsuario, idAplicacion);

                sql = "insert into Suscripcion(IdUsuario, IdAplicacion, IdVersion, CodPais) ";
                sql += "select " + idUsuario + " as IdUsuario, " + idAplicacion + " as IdAplicacion, " + idVersion +
                       " as IdVersion, CodPais ";
                sql += "from AdminPais2 where CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";
                Conexion.SqlExecute(sql);

                sql = "CreaFavoritos";
                Conexion.SqlStoredProcedure1Param(sql, "IdUsuario", idUsuario);
                return idUsuario;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                GrabaLog("0", "", "", "0", "0", "CrearUsuarioFreeTrial", ex.Message.Replace(",", ".").Replace("'", ""));
                return null;
            }
        }

        public static string CrearUsuarioPlan(string empPer, string codUsuario, string password, string nombres,
            string apellidos, string dni, string empresa, string ruc, string telefono, string celular, string email1,
            string idActividad, string mensaje, string idConocio, string direccionIp, string codCampaña, string url,
            string referido, string direccion, decimal importe, int plan, string gclid = "", string pais = "")
        {
            var codPais = "";
            try
            {
                var idAplicacion = ObtieneIdAdminValor("01APL", "Business");
                var idVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
                var idTipo = ObtieneIdAdminValor("03TIP", "Cliente");
                var idOrigen = ObtieneIdAdminValor("04ORG", "Veritrade");
                var idCargo = ObtieneIdAdminValor("05CAR", "Otro");
                var idEstadoVenta = ObtieneIdAdminValor("06EVT", "Nuevo");
                var idEstadoXqNo = ObtieneIdAdminValor("07EXN", "N/A");

                VeritradeAdmin.Seguridad ws = new VeritradeAdmin.Seguridad();
                var ubicacion = ws.BuscaUbicacionIP2(direccionIp, ref codPais);
                if (codPais == "-") codPais = "PE";

                var sql =
                    "insert into Usuario(CodUsuario, Password, EmpPer, Empresa, RUC, Nombres, Apellidos, DNI, Telefono, Celular, Email1, Direccion, ImporteUSD, ";
                sql +=
                    "IdAplicacion, IdTipo, IdOrigen, IdCargo, IdEstadoVenta, IdEstadoXQNo, IdActividad, IdConocio, ComentAdmin, DireccionIP, ";
                sql +=
                    "CodPaisIP, CodPaisIP2, CodPais, Ubicacion, CodEstado, FecInicio, FecFin, CodSeguridad, CantUsuariosUnicos, CodCampaña, URLRegistro, URLReferido, Gclid, FecRegistro, FecActualizacion) values ";
                sql += "('" + codUsuario + "', '" + password + "', '" + empPer + "', '" + empresa + "', '" + ruc +
                       "', '" + nombres + "', '" + apellidos + "', ";
                sql += "'" + dni + "', '" + telefono + "', '" + celular + "', '" + email1 + "', '" + direccion + "', " +
                       importe + ", ";
                sql += idAplicacion + ", " + idTipo + ", " + idOrigen + ", " + idCargo + ", " + idEstadoVenta + ", " +
                       idEstadoXqNo + ", ";
                sql += idActividad + ", " + idConocio + ", '" + mensaje + "', '" + direccionIp + "', '" + codPais +
                       "', '" + codPais + "', '" + pais + "', '" + ubicacion + "', ";
                sql += "'A', " + DateTime.Now.ToString("yyyyMMdd") + "," +
                       DateTime.Now.AddDays(364).ToString("yyyyMMdd") + ", 'Off', 1, '" + codCampaña + "', '" + url +
                       "', '" + referido + "', '" + gclid + "', getdate(), getdate())";
                Conexion.SqlExecute(sql);

                string idUsuario = BuscarIdUsuario(codUsuario, idAplicacion);

                var listaPaises = new PlanesPaisSuscription().GetCodPaisesPlan(plan);
                foreach (var nombrePais in listaPaises)
                {
                    var sql2 = "INSERT INTO [Suscripcion] ([IdUsuario],[IdAplicacion],[IdVersion],[CodPais]) " +
                           "VALUES(" + idUsuario + "," + idAplicacion + "," + idVersion + ",'" + nombrePais + "')";
                    Conexion.SqlExecute(sql2);
                }

                sql = "CreaFavoritos";
                Conexion.SqlStoredProcedure1Param(sql, "IdUsuario", idUsuario);
                return idUsuario;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                GrabaLog("0", "", "", "0", "0", "CrearUsuarioFreeTrial", ex.Message.Replace(",", ".").Replace("'", ""));
                return null;
            }
        }

        public static void GrabaLog(string IdUsuario, string CodPais, string TipoOpe, string AñoMesIni, string AñoMesFin, string Pagina, string Logs)
        {
            var sql = "insert into Logs(IdUsuario, CodPais, TipoOpe, AñoMesIni, AñoMesFin, Pagina, Logs, Fecha, IdAñoMes) ";
            sql += "values(" + IdUsuario + ", '" + CodPais + "', '" + TipoOpe + "', " + AñoMesIni + ", " + AñoMesFin +
                   ", '" + Pagina + "', '" + Logs.Replace("'", "''") +
                   "', getdate(), year(getdate()) * 100 + month(getdate()))";

            Conexion.SqlExecute(sql);
        }
        public static void BuscaCodUsuario(string IdUsuario, ref string CodUsuario, ref string Password)
        {
            string sql;

            sql = "select CodUsuario, Password from Usuario where IdUsuario = " + IdUsuario;
            var dt = Conexion.SqlDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                CodUsuario = dt.Rows[0]["CodUsuario"].ToString();
                Password = dt.Rows[0]["Password"].ToString();
            }
        }

        public static void BuscaCodUsuarioEstado(string IdUsuario, ref string CodUsuario, ref string Password, ref string CodEstado)
        {
            string sql;

            sql = "select CodUsuario, Password , CodEstado from Usuario where IdUsuario = " + IdUsuario;
            var dt = Conexion.SqlDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                CodUsuario = dt.Rows[0]["CodUsuario"].ToString();
                Password = dt.Rows[0]["Password"].ToString();
                CodEstado = dt.Rows[0]["CodEstado"].ToString();
            }
        }

        public static string ObtieneIdAdminValor(string codVariable, string valor)
        {
            var idAdminValor = "";
            var sql = "select IdAdminValor from AdminValor where CodVariable = '" + codVariable + "' and Valor = '" +
                      valor + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idAdminValor = row["IdAdminValor"].ToString();
            }

            return idAdminValor;
        }

        public static string BuscarIdUsuario(string codUsuario, string idAplicacion)
        {
            var idUsuario = "";
            var sql = "select IdUsuario from Usuario where CodUsuario = '" + codUsuario + "' and IdAplicacion = " +
                      idAplicacion;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idUsuario = row["IdUsuario"].ToString();
            }

            return idUsuario;
        }

        public static string BuscarIdUsuario(string codUsuario)
        {
            var idUsuario = "";
            var sql = "select IdUsuario from Usuario where CodUsuario = '" + codUsuario + "'";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                idUsuario = row["IdUsuario"].ToString();
            }

            return idUsuario;
        }

        public static bool ValidaVisitasMes(string IdUsuario, ref int LimiteVisitas, ref int Visitas)
        {
            string IdPlan;

            IdPlan = ObtieneIdPlan(IdUsuario);

            var sql = "select LimiteVisitas from [Plan] where IdPlan = " + IdPlan;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                LimiteVisitas = Convert.ToInt32(row["LimiteVisitas"]);
            }

            sql = "select count(*) as Visitas from Historial where CodEstado is null and IdUsuario = " + IdUsuario + " ";
            sql += "and year(FecVisita) * 100 + month(FecVisita) = " + (DateTime.Today.Year * 100 + DateTime.Today.Month).ToString();

            var dt2 = Conexion.SqlDataTable(sql);
            foreach (DataRow row2 in dt2.Rows)
            {
                Visitas = Convert.ToInt32(row2["Visitas"]);
            }

            return (Visitas < LimiteVisitas);
        }

        public static string BuscaCodEstado(string CodUsuario)
        {
            string CodEstado = "";
            var sql = "select CodEstado ";
            sql += "from Usuario where CodUsuario = '" + CodUsuario + "' and IdAplicacion = 1";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                CodEstado = row["CodEstado"].ToString();
            }

            return CodEstado;
        }

        public static string ObtieneOrigen(string IdUsuario)
        {
            string Origen = "";
            var sql = "select Valor as [Origen] from Usuario U, AdminValor A where U.IdOrigen = A.IdAdminValor and IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Origen = row["Origen"].ToString();
            }

            return Origen;
        }

        public static string ObtieneIdPlan(string IdUsuario)
        {
            string IdPlan = "";
            var sql = "select IdPlan from Usuario where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                IdPlan = row["IdPlan"].ToString();
            }

            return IdPlan;
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
            string  marca = "";
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

        // Ruben 202301
        public static void BuscaURLReferido(string Origen, string IdUsuario, ref string URL1, ref string URL2, ref string URL3, ref string URL4)
        {
            string sql;

            sql = "select * from Referido where Origen = '" + Origen + "' and IdUsuario = " + IdUsuario;

            try
            {
                var dt = Conexion.SqlDataTable(sql,deadlock:true);
                
                if (dt.Rows.Count > 0)
                {
                    URL1 = dt.Rows[0]["URL1"].ToString().ToLower();
                    URL2 = dt.Rows[0]["URL2"].ToString().ToLower();
                    URL3 = dt.Rows[0]["URL3"].ToString().ToLower();
                    // Ruben 202301
                    URL4 = dt.Rows[0]["URL4"].ToString().ToLower();
                }
            }
            catch (Exception ex)
            {                                                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        public static String _GetPostJS(string URL, string inicio, string origen, string CodUsuario, string Password)
        {
            System.Text.StringBuilder strScript = new System.Text.StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("document.forms[0].method = 'post';");
            strScript.Append("document.forms[0].action = '{0}';");
            strScript.Append("document.forms[0].inicio.value = '{1}';");
            strScript.Append("document.forms[0].origen.value = '{2}';");
            strScript.Append("document.forms[0].txtCodUsuario.value = '{3}';");
            strScript.Append("document.forms[0].txtPassword.value = '{4}';");
            strScript.Append("document.forms[0].submit();");
            strScript.Append("</script>");
            return String.Format(strScript.ToString(), URL, inicio, origen, CodUsuario, Password);
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
        public static bool SessionUnica(string CodUsuario)
        {
            string sesionUnica = "";
            var sql = "select SesionUnica from Usuario where CodUsuario = '" + CodUsuario + "' ";
            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                sesionUnica = row["SesionUnica"].ToString();
            }
            return (sesionUnica == "S");
        }

        public static List<string> ObtieneUsuariosEnLinea()
        {
            var usuariosEnLinea = new List<string>();
            foreach (var sesion in ActiveSessions.Sessions)
            {
                if (sesion.Value.IdUsuario != null)
                {
                    usuariosEnLinea.Add(sesion.Value.IdUsuario);
                }
            }

            return usuariosEnLinea;
        }

        public static bool ExisteUsuarioEnLinea(string idUsuario)
        {
            bool aux = false;

            List<string> usuariosEnLinea = ObtieneUsuariosEnLinea();

            foreach (var usuario in usuariosEnLinea)
                if (idUsuario.Equals(usuario)) aux = true;

            return aux;
        }

        public static string BuscaTipoUsuario(string IdUsuario)
        {
            string TipoUsuario = "";
            string sql = "select Valor as TipoUsuario from Usuario U, AdminValor V ";
            sql += "where U.IdUsuario = " + IdUsuario + " and V.CodVariable = '03TIP' and U.IdTipo = V.IdAdminValor ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                TipoUsuario = row["TipoUsuario"].ToString();
            }
            return TipoUsuario;
        }

        public static string ObtienePlan(string IdUsuario)
        {
            string Plan = "";
            string sql = "select Valor as [Plan] from Usuario U, AdminValor A where U.IdPlan = A.IdAdminValor and IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Plan = row["Plan"].ToString();
            }

            return Plan;
        }

        public static void BuscaDatosPlanEspecial(string IdUsuario, ref string CodPais, ref string TipoOpe)
        {
            string sql = "select CodPais from Suscripcion where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                CodPais = row["CodPais"].ToString();//.Substring(0, 2);
                if (row["CodPais"].ToString().Length > 2)
                {
                    TipoOpe = row["CodPais"].ToString().Substring(2, 1);
                    if(TipoOpe == "_")
                    {
                        TipoOpe = row["CodPais"].ToString().Substring(3, 1);
                    }
                    if (TipoOpe != "I" && TipoOpe != "E" && TipoOpe != "T")
                    {
                        TipoOpe = "I";
                    }
                } 
                else TipoOpe = "I";
            }
        }

        public static bool ValidaPais(string IdUsuario, string CodPais)
        {
            int Cant = 0;

            if (CodPais.Contains("_")) CodPais = CodPais.Replace("_", "I");

            string sql = "select count(*) as Cant from Suscripcion S, BaseDatos B ";
            sql += "where (S.CodPais = B.CodPaisSuscripcion or S.CodPais = B.CodPais) ";
            sql += "and IdUsuario = " + IdUsuario + " and B.CodPais = '" + CodPais + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Cant = Convert.ToInt32(row["Cant"]);
            }
            return (Cant > 0);
        }

        public static bool ValidaPaisBusqueda(string IdUsuario, string CodPais)
        {
            int Cant = 0;
            string codPaisCorregido = CodPais.Substring(0, 2);
            if (codPaisCorregido.Contains("_")) codPaisCorregido = codPaisCorregido.Replace("_", "I");

            string sql = "select count(*) as Cant from Suscripcion S, BaseDatos B ";
            sql += "where (SUBSTRING(S.CodPais, 0, 3) = B.CodPaisSuscripcion or SUBSTRING(S.CodPais,0,3) = B.CodPais) ";
            sql += "and IdUsuario = " + IdUsuario + " and B.CodPais = '" + codPaisCorregido + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Cant = Convert.ToInt32(row["Cant"]);
            }
            return (Cant > 0);
        }

        public static bool FlagCarga(string CodPais)
        {
            string flagCarga = "";
            if (CodPais.Contains("_")) CodPais = CodPais.Replace("_", "I");

            string sql = "select FlagCarga from BaseDatos where CodPais = '" + CodPais + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flagCarga = row["FlagCarga"].ToString();
            }

            return (flagCarga == "S");
        }

        public static string ObtieneCodPaisAcceso(string IdUsuario)
        {
            string CodPais = "";

            string sql = "select top 1 B.CodPais from BaseDatos B, Suscripcion S ";
            sql += "where B.CodPais = S.CodPais and FlagCarga = 'N' and IdUsuario = " + IdUsuario + " order by 1";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                CodPais = row["CodPais"].ToString();
            }

            return CodPais;
        }

        public static int ObtieneIdOrigenUsuario(string id)
        {
            string sql = $"select idOrigen from usuario where idUsuario = {id}";
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    return Convert.ToInt32(row["idOrigen"]);
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, "Funciones.ObtieneIdOrigenUsuario");

            }
            return 0;
        }

        public static string BuscaUsuario(string IdUsuario)
        {
            string Usuario = "";
            string sql = "select Nombres + ' ' + Apellidos + ' - ' + ISNULL(Empresa,'') as Usuario from Usuario where IdUsuario = " + IdUsuario;
            DataTable dt;
            try
            {
                dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    Usuario = row["Usuario"].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                Usuario = "";
            }
            return Usuario;
        }

        public static List<string> ObtenerPaisesUsuario(int idUsuario, string idioma)
        {
            List<string> retorno = new List<string>();
            string campoDescripcion = "variableGeneral.descripcion_eng";
            if (idioma == "es")
            {
                campoDescripcion = "variableGeneral.descripcion";
            }
            string sql = $"select {campoDescripcion} from suscripcion "
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


        public static string BuscaGrupo(string idGrupo)
        {
            string grupo = "";
            string sql = "select Grupo from Grupo where IdGrupo = " + idGrupo;
            try
            {
                DataTable dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    grupo = "(GROUP) " + row["Grupo"].ToString();
                }

            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return grupo;
        }

        public static int ObtieneLimite(string idPlan, string tipoLimite)
        {
            string sql = "select " + tipoLimite + " from [Plan] where IdPlan = '" + idPlan + "'";
            int aux = 0;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    aux = Convert.ToInt32((dt.Rows[0])[tipoLimite]);
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return aux;
        }

        public static bool ObtieneFlagPlan(string idPlan, string codigo)
        {
            string sql = "select " + codigo + " from [Plan] where IdPlan = '" + idPlan + "'";
            string aux = string.Empty;
            try
            {
                var dt = Conexion.SqlDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    aux = dt.Rows[0][codigo].ToString();
                }
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }

            return aux == "S";
        }

        public static UsuarioPlan ObtieneUsuarioPlan(string idUsuario)
        {
            var _ret = new UsuarioPlan();
            string sql = "select av.CodVariable, av.Valor, p.* from dbo.[Plan] p, dbo.Usuario u, dbo.AdminValor av   where IdUsuario = {IdUsuario} and u.IdPlan=p.IdPlan and av.IdAdminValor = p.IdPlan";

            using (var db = new ConexProvider().Open)
            {
                _ret = db.Query<UsuarioPlan>(sql, new { IdUsuario = idUsuario }).FirstOrDefault();
            }
            return _ret;
        }

        public static bool EsUsuarioPrincipal(string IdUsuario)
        {
            /*string flag = "";
            
            string sql = "select FlagUsuarioPrincipal from Usuario where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = row["FlagUsuarioPrincipal"].ToString();
            }

            return (flag == "S");*/

            int flag = 0;

            string sql = @"select count(*) as Cant from Usuario UL, VeritradeAdmin.dbo.Usuario U 
                           where UL.IdUsuario = U.IdUsuarioLegacy and UL.IdTipo = 10 and UL.CodEstado = 'A' 
                            and U.CodTipoUsuario = 'PRI' and UL.IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                flag = Convert.ToInt32(row["Cant"]);
            }

            return flag > 0;

        }

        public static bool Valida2(string codUsuario, string password, ref string IdUsuario, ref string IdAplicacion, ref string CodSeguridad,
            ref int CantUsuariosUnicos)
        {
            dynamic _ret = null;
            string sql = $"select * from dbo.Usuario where CodUsuario='{codUsuario}' and Password='{password}'";
            
            using (var db = new ConexProvider().Open)
            {
                _ret = db.QueryFirstOrDefault(sql);
                if (_ret != null)
                {
                    IdUsuario = _ret.IdUsuario.ToString();
                    IdAplicacion = _ret.IdAplicacion.ToString();
                    CodSeguridad = _ret.CodSeguridad??"";
                    CantUsuariosUnicos = _ret.CantUsuariosUnicos??0;
                }
            }
            return _ret != null;
        }

        public static bool BuscaAlertasDeshabilitadasUsuario(string codUsuario)
        {
            dynamic _ret = null;
            string sql = "select alertasDeshabilitadas from dbo.Usuario where IdUsuario=@codUsuario";

            using (var db = new ConexProvider().Open)
            {
                _ret = db.QueryFirstOrDefault(sql, new { codUsuario });
                if (_ret != null && _ret.alertasDeshabilitadas != null)
                {
                    return BitConverter.ToBoolean(_ret.alertasDeshabilitadas,0);
                }
            }
            return false;
        }

        public static bool CambiarEstadoAlerta(string idUsuario, bool isAlertasDeshabilitadas)
        {
            string sql = "update dbo.Usuario set alertasDeshabilitadas = @toggle where IdUsuario=@codUsuario";

            using (var db = new ConexProvider().Open)
            {
                db.Execute(sql, new { toggle=isAlertasDeshabilitadas, codUsuario=idUsuario });
            }
            return isAlertasDeshabilitadas;
        }

        public static string GetIdUserFreeTrial()
        {
            dynamic _ret = null;
            string sql = "select top 1 IdUsuario from usuario where idtipo=8 and idplan='71' and codestado='A' order by 1 desc";

            using (var db = new ConexProvider().Open)
            {
                _ret = db.QueryFirstOrDefault(sql);
                if (_ret != null)
                {
                    return _ret.IdUsuario.ToString();
                }
            }
            return string.Empty;
        }

        public static string TruncateText(string filtro, GridRow row, Enums.TipoFiltro tf = Enums.TipoFiltro.Resumen)
        {
            int nums = 0;
            string result = string.Empty;

            switch (filtro.Trim().ToLower())
            {
                case "partida":
                    nums = (tf == Enums.TipoFiltro.Resumen ?  Variables.NumTrunResProd : Variables.NumTrunTabProd);
                    break;
                default:
                    if (tf == Enums.TipoFiltro.Tab)  {
                        if (!(new String[] { "aduanadua", "marca", "modelo", "detalleexcel" }).Contains(filtro.Trim().ToLower())) {
                            nums = Variables.NumTrunTabOthers;
                        }
                    } else
                    {
                        if ((new String[] { "importador", "proveedor" }).Contains(filtro.Trim().ToLower()))
                        {
                            nums = filtro.Trim().ToLower() == "importador" ? Variables.NumTrunResSmall : Variables.NumTrunResSmallX2;
                        }
                    }
                        
                    if (filtro.Trim().ToLower() == "detalleexcel")
                        nums = Variables.NumTextTruncateDescriptionExcel;
                    break;
            }
            if (nums > 0)
                result = row.Descripcion.Truncate(nums, withTooltip: true);
            else
                result = row.Descripcion;
            return result;
        }

        public static void SendMail(string toEmail, string subject, string body, ref string fromEmail, string[] bccAddress=null)
        {
            try
            {
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                string username = smtpSection.Network.UserName;
                string password = smtpSection.Network.Password;
                string host = smtpSection.Network.Host;
                int port = smtpSection.Network.Port;

                SmtpClient sc = new SmtpClient();
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                MailAddress _from = new MailAddress(username, ConfigurationManager.AppSettings["Alerta_EmailDisplayName"]);
                sc.Host = host;
                sc.Port = port;
                sc.EnableSsl = true;

                mail.To.Add(toEmail);

                if (bccAddress != null && bccAddress.Length > 0)
                {
                    foreach (string i in bccAddress)
                        mail.Bcc.Add(i);
                }

                mail.Subject = subject;
                mail.From = _from;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-9");
                mail.Body = body;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Send(mail);
                fromEmail = username;
            }
            catch (Exception ex)
            {
                
                
                
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                throw ex;
            }
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

        // Ruben 202310
        public static void Log(string Line)
        {
            Line = DateTime.Now.ToString() + " - " + Line;

            string LogFile = "~/App_Data2/DebugLog.txt";

            LogFile = System.Web.Hosting.HostingEnvironment.MapPath(LogFile);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(LogFile, true);

            sw.WriteLine(Line);

            sw.Close();
        }
    }
}