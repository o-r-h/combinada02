using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Veritrade2017.Models;

namespace Veritrade2017.Helpers
{
    public static class FunctionsUser
    {
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

        public static bool SessionUnica(string CodUsuario)
        {
            var aux = true;
            var sql = "select SesionUnica from Usuario where CodUsuario = '" + CodUsuario + "' ";

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                aux = (row["SesionUnica"].ToString() == "S");
            }

            return aux;
        }

        public static bool ExisteUsuarioEnLinea(string IdUsuario)
        {
            bool aux = false;

            List<String> UsuariosEnLinea = ObtieneUsuariosEnLinea();

            foreach (var Usuario in UsuariosEnLinea)
                if (IdUsuario == Usuario.ToString()) aux = true;

            return aux;
        }

        public static List<String> ObtieneUsuariosEnLinea()
        {
            List<String> UsuariosEnLinea = new List<String>();

            //List<Hashtable> hTables = new List<Hashtable>();
            //object obj = typeof(HttpRuntime).GetProperty("CacheInternal", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
            //dynamic fieldInfo = obj.GetType().GetField("_caches", BindingFlags.NonPublic | BindingFlags.Instance);

            //If server uses "_caches" to store session info
            //if (fieldInfo != null)
            //{
            //    object[] _caches = (object[])fieldInfo.GetValue(obj);
            //    for (int i = 0; i <= _caches.Length - 1; i++)
            //    {
            //        Hashtable hTable = (Hashtable)_caches[i].GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_caches[i]);
            //        hTables.Add(hTable);
            //    }
            //}
            //If server uses "_cachesRefs" to store session info
            //else
            //{
            //    fieldInfo = obj.GetType().GetField("_cachesRefs", BindingFlags.NonPublic | BindingFlags.Instance);
            //    object[] cacheRefs = fieldInfo.GetValue(obj);
            //    for (int i = 0; i <= cacheRefs.Length - 1; i++)
            //    {
            //        var target = cacheRefs[i].GetType().GetProperty("Target").GetValue(cacheRefs[i], null);
            //        Hashtable hTable = (Hashtable)target.GetType().GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target);
            //        hTables.Add(hTable);
            //    }
            //}

            //foreach (Hashtable hTable in hTables)
            //{
            //    foreach (DictionaryEntry entry in hTable)
            //    {
            //        object o1 = entry.Value.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(entry.Value, null);
            //        if (o1.GetType().ToString() == "System.Web.SessionState.InProcSessionState")
            //        {
            //            SessionStateItemCollection sess = (SessionStateItemCollection)o1.GetType().GetField("_sessionItems", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o1);
            //            if (sess != null)
            //            {
            //                if (sess["IdUsuario"] != null)
            //                {
            //                    UsuariosEnLinea.Add(sess["IdUsuario"].ToString());
            //                }
            //            }
            //        }
            //    }
            //}

            return UsuariosEnLinea;
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

        public static string BuscaTipoUsuario(string IdUsuario)
        {
            string TipoUsuario = "";
            var sql = "select Valor as TipoUsuario from Usuario U, AdminValor V ";
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
            var Plan = "";
            var sql = "select Valor as [Plan] from Usuario U, AdminValor A where U.IdPlan = A.IdAdminValor and IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                Plan = row["Plan"].ToString();
            }

            return Plan;
        }

        public static void BuscaDatosPlanEspecial(string IdUsuario, ref string CodPais, ref string TipoOpe)
        {
            var sql = "select CodPais from Suscripcion where IdUsuario = " + IdUsuario;

            var dt = Conexion.SqlDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                CodPais = row["CodPais"].ToString().Substring(0, 2);
                if (row["CodPais"].ToString().Length > 2)
                {
                    TipoOpe = row["CodPais"].ToString().Substring(3, 1);
                }
                else
                {
                    TipoOpe = "I";
                }
            }
        }

        public static string BuscaUsuario(string IdUsuario)
        {   //Retorna el nombre completo de un usuario
            //Parametros: 
            //string IdUsuario: identificador de usuario
            var Usuario = "";
            try
            {
                var sql = "select Nombres + ' ' + Apellidos + ' - ' + Empresa as Usuario from Usuario where IdUsuario = " + IdUsuario;
                var dt = Conexion.SqlDataTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    Usuario = row["Usuario"].ToString();
                }
            }
            catch (Exception ex)
            {
                string sMensaje = ex.Message;
                Usuario = "";
            }

            return Usuario;
        }
    }
}