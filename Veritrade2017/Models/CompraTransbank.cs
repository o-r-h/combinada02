using System;
using System.Data.SqlClient;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class CompraTransbankLog
    {
        public string Token { get; set; }
        public string AccountingDate { get; set; }
        public string BuyOrder { get; set; }
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public string AuthorizationCode { get; set; }
        public string CommerceCode { get; set; }
        public string PaymentTypeCode { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public decimal SharesAmount { get; set; }
        public int SharesNumber { get; set; }
        public bool SharesNumberSpecified { get; set; }
        public string SessionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool TransactionDateSpecified { get; set; }
        public string VCI { get; set; }

        public static void SaveLog(CompraTransbankLog compraTransbankLog)
        {
            var sql = "insert [compras_transbank] " +
                            "(Token" +
                            ",AccountingDate" +
                            ",BuyOrder" +
                            ",CardNumber" +
                            ",Amount" +
                            ",AuthorizationCode" +
                            ",CommerceCode" +
                            ",PaymentTypeCode" +
                            ",ResponseCode" +
                            ",ResponseText" +
                            ",SharesAmount" +
                            ",SharesNumber" +
                            ",SharesNumberSpecified" +
                            ",SessionId" +
                            ",TransactionDate" +
                            ",TransactionDateSpecified" +
                            ",VCI )"+
                        "values(" +
                            "'{0}'" +
                            ", '{1}'" +
                            ", '{2}'" +
                            ", '{3}'" +
                            ", {4}" +
                            ", '{5}'" +
                            ", '{6}'" +
                            ", '{7}'" +
                            ", {8}" +
                            ", '{9}'" +
                            ", {10}" +
                            ", {11}" +
                            ", {12}" +
                            ", '{13}'" +
                            ", '{14}'" +
                            ", {15}" +
                            ", '{16}'" +
                            ")";
            sql = String.Format(sql,
                compraTransbankLog.Token
            , compraTransbankLog.AccountingDate
            , compraTransbankLog.BuyOrder
            , compraTransbankLog.CardNumber
            , compraTransbankLog.Amount
            , compraTransbankLog.AuthorizationCode
            , compraTransbankLog.CommerceCode
            , compraTransbankLog.PaymentTypeCode
            , compraTransbankLog.ResponseCode
            , compraTransbankLog.ResponseText
            , compraTransbankLog.SharesAmount
            , compraTransbankLog.SharesNumber
            , compraTransbankLog.SharesNumberSpecified ? 1:0
            , compraTransbankLog.SessionId
            , compraTransbankLog.TransactionDate.ToString("yyyyMMdd hh:mm:ss")
            , compraTransbankLog.TransactionDateSpecified ? 1:0
            , compraTransbankLog.VCI);
            Conexion.SqlExecute(sql, true);
        }
    }

    public class PedidosTransbank
    {
        public int Id { get; set; }
        public string OrdenCompra { get; set; }
        public bool Aprobado { get; set; }
        public bool Procesado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public string Monto { get; set; }
        public int IdPlan { get; set; }
        public string Token { get; set; }
        public string FacturaDireccion { get; internal set; }
        public string FacturaRazonSocial { get; internal set; }
        public string FacturaRuc { get; internal set; }
        public int IdUsuario { get; internal set; }

        public static void Save(PedidosTransbank pedidoTransbank)
        {
            PedidosTransbank pedido = GetByBuyOrder(pedidoTransbank.OrdenCompra);
            string sql = "";
            if(pedido == null)
            {
                sql = "insert [pedidos_transbank] " +
                            "(Token" +
                            ", IdPlan" +
                            ", Monto" +
                            ", Empresa" +
                            ", Telefono" +
                            ", Pais" +
                            ", Email" +
                            ", Apellidos" +
                            ", Nombres" +
                            ", FechaActualizacion" +
                            ", FechaCreacion" +
                            ", Procesado" +
                            ", Aprobado" +
                            ", facturaDireccion" +
                            ", facturaRazonSocial" +
                            ", facturaRuc" +
                            ", idUsuario" +
                            ", OrdenCompra" +
                            ") " +
                        "values(" +
                            "'{0}'" +
                            ", {1}" +
                            ", '{2}'" +
                            ", '{3}'" +
                            ", '{4}'" +
                            ", '{5}'" +
                            ", '{6}'" +
                            ", '{7}'" +
                            ", '{8}'" +
                            ", '{9}'" +
                            ", '{10}'" +
                            ", {11}" +
                            ", {12}" +
                            ", '{13}'" +
                            ", '{14}'" +
                            ", '{15}'" +
                            ", '{16}'" +
                            ", '{17}'" +
                            ")";
            } else
            {
                sql = "update [pedidos_transbank] set " +
                            "Token = '{0}'" +
                            ", IdPlan = {1}" +
                            ", Monto = '{2}'" +
                            ", Empresa = '{3}'" +
                            ", Telefono = '{4}'" +
                            ", Pais = '{5}'" +
                            ", Email = '{6}'" +
                            ", Apellidos = '{7}'" +
                            ", Nombres = '{8}'" +
                            ", FechaActualizacion = '{9}'" +
                            ", FechaCreacion = '{10}'" +
                            ", Procesado = {11}" +
                            ", Aprobado = {12}" +
                            ", facturaDireccion = '{13}'" +
                            ", facturaRazonSocial = '{14}'" +
                            ", facturaRuc = '{15}'" +
                            ", idUsuario = '{16}'" +
                            "where OrdenCompra = '{17}'";
            }
        

            sql = String.Format(sql, pedidoTransbank.Token
                            , pedidoTransbank.IdPlan
                            , pedidoTransbank.Monto
                            , pedidoTransbank.Empresa
                            , pedidoTransbank.Telefono
                            , pedidoTransbank.Pais
                            , pedidoTransbank.Email
                            , pedidoTransbank.Apellidos
                            , pedidoTransbank.Nombres
                            , pedidoTransbank.FechaActualizacion.ToString("yyyyMMdd hh:mm:ss")
                            , pedidoTransbank.FechaCreacion.ToString("yyyyMMdd hh:mm:ss")
                            , pedidoTransbank.Procesado ? 1 : 0
                            , pedidoTransbank.Aprobado ? 1:0
                            , pedidoTransbank.FacturaDireccion 
                            , pedidoTransbank.FacturaRazonSocial 
                            , pedidoTransbank.FacturaRuc
                            , pedidoTransbank.IdUsuario
                            , pedidoTransbank.OrdenCompra);

            Conexion.SqlExecute(sql, true);
        }

        public static PedidosTransbank GetByBuyOrder(string buyOrder)
        {
            var sql = "Select * from pedidos_transbank where OrdenCompra = '{0}'";
            sql = String.Format(sql, buyOrder);
            var ret = Conexion.SqlDataTable(sql,true);
            if(ret.Rows.Count > 0)
            {
                PedidosTransbank pedido = new PedidosTransbank();
                pedido.Token = ret.Rows[0]["Token"].ToString();
                pedido.IdPlan = Convert.ToInt32(ret.Rows[0]["IdPlan"].ToString());
                pedido.Monto = ret.Rows[0]["Monto"].ToString();
                pedido.Empresa = ret.Rows[0]["Empresa"].ToString();
                pedido.Telefono = ret.Rows[0]["Telefono"].ToString();
                pedido.Pais = ret.Rows[0]["Pais"].ToString();
                pedido.Email = ret.Rows[0]["Email"].ToString();
                pedido.Apellidos = ret.Rows[0]["Apellidos"].ToString();
                pedido.Nombres = ret.Rows[0]["Nombres"].ToString();
                pedido.FechaActualizacion = Convert.ToDateTime(ret.Rows[0]["FechaActualizacion"].ToString());
                pedido.FechaCreacion = Convert.ToDateTime(ret.Rows[0]["FechaCreacion"].ToString());
                pedido.Procesado = Convert.ToBoolean(ret.Rows[0]["Procesado"]);
                pedido.Aprobado = Convert.ToBoolean(ret.Rows[0]["Aprobado"]);
                pedido.OrdenCompra = ret.Rows[0]["OrdenCompra"].ToString();
                pedido.FacturaDireccion = ret.Rows[0]["facturaDireccion"].ToString();
                pedido.FacturaRazonSocial= ret.Rows[0]["facturaRazonSocial"].ToString();
                pedido.FacturaRuc = ret.Rows[0]["facturaRuc"].ToString();
                pedido.IdUsuario = Convert.ToInt32(ret.Rows[0]["idUsuario"]);
                return pedido;
            }
            return null;
            
            
        }

        internal static PedidosTransbank GetByToken(string token)
        {
            var sql = "Select * from pedidos_transbank where token = '{0}'";
            sql = String.Format(sql, token);
            var ret = Conexion.SqlDataTable(sql, true);
            PedidosTransbank pedido = new PedidosTransbank();
            pedido.Token = ret.Rows[0]["Token"].ToString();
            pedido.IdPlan = Convert.ToInt32(ret.Rows[0]["IdPlan"].ToString());
            pedido.Monto = ret.Rows[0]["Monto"].ToString();
            pedido.Empresa = ret.Rows[0]["Empresa"].ToString();
            pedido.Telefono = ret.Rows[0]["Telefono"].ToString();
            pedido.Pais = ret.Rows[0]["Pais"].ToString();
            pedido.Email = ret.Rows[0]["Email"].ToString();
            pedido.Apellidos = ret.Rows[0]["Apellidos"].ToString();
            pedido.Nombres = ret.Rows[0]["Nombres"].ToString();
            pedido.FechaActualizacion = Convert.ToDateTime(ret.Rows[0]["FechaActualizacion"].ToString());
            pedido.FechaCreacion = Convert.ToDateTime(ret.Rows[0]["FechaCreacion"].ToString());
            pedido.Procesado = Convert.ToBoolean(ret.Rows[0]["Procesado"]);
            pedido.Aprobado = Convert.ToBoolean(ret.Rows[0]["Aprobado"]);
            pedido.OrdenCompra = ret.Rows[0]["OrdenCompra"].ToString();
            pedido.FacturaDireccion = ret.Rows[0]["facturaDireccion"].ToString();
            pedido.FacturaRazonSocial = ret.Rows[0]["facturaRazonSocial"].ToString();
            pedido.FacturaRuc = ret.Rows[0]["facturaRuc"].ToString();
            pedido.IdUsuario = Convert.ToInt32(ret.Rows[0]["idUsuario"]);
            return pedido;
        }

        public static string GenNewCode()
        {
            var sql = "select coalesce(max(ordenCompra),0) oc from pedidos_transbank";
            var ret = Conexion.SqlDataTable(sql, true);
            int num = Convert.ToInt32(ret.Rows[0]["oc"].ToString());
            num++;
            string newCode = "0000000000000000" + num;
            newCode = newCode.Substring(newCode.Length -12);
            return newCode;
        }
    }
}