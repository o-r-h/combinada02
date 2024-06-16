using System;
using System.Data.SqlClient;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class CompraPayMe
    {
        public string AcquirerId { get; set; }
        public string IdCommerce { get; set; }
        public string PurchaseAmount { get; set; }
        public string PurchaseCurrencyCode { get; set; }
        public string PurchaseOperationNumber { get; set; }
        public string PurchaseVerification { get; set; }
        public string IdEntCommerce { get; set; }
        public string CodCardHolderCommerce { get; set; }
        public string Names { get; set; }
        public string LastNames { get; set; }
        public string Mail { get; set; }
        public string Lenguaje { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string UserCodePayme { get; set; }
    }

    public class CompraPayMeLog
    {
        public string AuthorizationResult { get; set; }
        public string AuthorizationCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Bin { get; set; }
        public string Brand { get; set; }
        public string PaymentReferenceCode { get; set; }
        public string PurchaseVerification { get; set; }
        public string CardType { get; set; }
        public string BankName { get; set; }
        public string PurchaseOperationNumber { get; set; }

        public static void SaveLog(CompraPayMeLog compraPayMeLog)
        {
            var sql = "INSERT INTO [compras_payme] ([authorizationResult],[authorizationCode]" +
                      ",[errorCode],[errorMessage],[bin]" +
                      ",[brand],[paymentReferenceCode],[purchaseVerification]" +
                      ",[cardType],[bankName],[purchaseOperationNumber]) " +
                      "VALUES ('" + compraPayMeLog.AuthorizationResult + "','" + compraPayMeLog.AuthorizationCode +
                      "','" +
                      compraPayMeLog.ErrorCode + "','" + compraPayMeLog.ErrorMessage + "','" + compraPayMeLog.Bin +
                      "','" +
                      compraPayMeLog.Brand + "','" + compraPayMeLog.PaymentReferenceCode + "','" +
                      compraPayMeLog.PurchaseVerification + "','" +
                      compraPayMeLog.CardType + "','" + compraPayMeLog.BankName + "','" +
                      compraPayMeLog.PurchaseOperationNumber + "');";

            Conexion.SqlExecute(sql, true);
        }
    }

    public class PedidosPayMe
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public bool Estado { get; set; }
        public bool Procesado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public static string GetLastCode()
        {
            try
            {
                var sql =
                    "SELECT TOP 1 codigo as codigo FROM pedidos_payme WHERE estado = 0 AND procesado = 0 ORDER BY codigo ASC";
                var resultado = Conexion.SqlReturn(sql, true);

                if (DBNull.Value.Equals(resultado) || resultado == null)
                {
                    var sqlSearch =
                        "SELECT MAX(codigo) as codigo FROM pedidos_payme WHERE estado = 1 ORDER BY codigo ASC";
                    var resultqlSearch = Conexion.SqlReturn(sqlSearch, true);
                    int numero;
                    if (!DBNull.Value.Equals(resultqlSearch))
                    {
                         numero = Convert.ToInt32(resultqlSearch) + 1;
                    }
                    else
                    {
                        numero = 1;
                    }
                    var cadena = "00000" + numero;
                    var longitud = numero.ToString().Length;

                    cadena = cadena.Substring(longitud - 1, 6);
                    NewCode(cadena);

                    return cadena;
                }
                return resultado.ToString();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void NewCode(string code)
        {
            try
            {
                var sql = "INSERT INTO [pedidos_payme] (codigo, estado, procesado) VALUES('" + code + "', 1, 0)";
                Conexion.SqlExecute(sql, true);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void LeaveCode(string code)
        {
            try
            {
                var sql = "UPDATE [pedidos_payme] " +
                          "SET estado = 0, fecha_actualizacion = getdate() " +
                          "WHERE [codigo] = '" + code + "'";

                Conexion.SqlExecute(sql, true);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SetCode(string code)
        {
            try
            {
                var sql = "UPDATE [pedidos_payme] " +
                          "SET estado = 1, fecha_actualizacion = getdate() " +
                          "WHERE [codigo] = '" + code + "'";

                Conexion.SqlExecute(sql, true);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void EndCode(string code)
        {
            try
            {
                var sql = "UPDATE [pedidos_payme] " +
                          "SET procesado = 1, fecha_actualizacion = getdate() " +
                          "WHERE [codigo] = '" + code + "'";

                Conexion.SqlExecute(sql, true);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool ExistCode(string code)
        {
            try
            {
                var sql = "SELECT * FROM pedidos_payme WHERE codigo = '" + code + "'";
                var dt = Conexion.SqlDataTable(sql, true);

                return dt.Rows.Count > 0;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string GetOtherCode(string lastCode)
        {
            try
            {
                if (ExistCode(lastCode))
                {
                    var sql = "UPDATE [pedidos_payme] " +
                              "SET estado = 1, procesado = 1, fecha_actualizacion = getdate() " +
                              "WHERE [codigo] = '" + lastCode + "'";
                    Conexion.SqlExecute(sql, true);
                }
                else
                {
                    var sqlInsert = "INSERT INTO [pedidos_payme] (codigo, estado, procesado) VALUES('" + lastCode +
                                    "', 1, 1)";
                    Conexion.SqlExecute(sqlInsert, true);
                }

                return GetLastCode();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}