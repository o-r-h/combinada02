using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class Compras
    {
        public int IdUsuario { get; set; }
        public string NombrePlan { get; set; }
        public int Monto { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Empresa { get; set; }
        public string Telefono { get; set; }
        public string Pais { get; set; }
        public string FacturaRuc { get; set; }
        public string FacturaRazonSocial { get; set; }
        public string FacturaDireccion { get; set; }
        public int Pasarela { get; set; }

        public Compras()
        {
            IdUsuario = 0;
            NombrePlan = "";
            Monto = 0;
            Nombre = "";
            Correo = "";
            Empresa = "";
            Telefono = "";
            Pais = "";
            FacturaRuc = "";
            FacturaRazonSocial = "";
            FacturaDireccion = "";
            Pasarela = 0;
        }

        public static void InsertCompra(Compras compras)
        {
            var sql =
                "INSERT INTO [dbo].[compras] ([idUsuario],[nombrePlan],[monto],[nombre],[correo],[empresa],[telefono],[pais]," +
                "[facturaRuc],[facturaRazonSocial],[facturaDireccion],[pasarela]) VALUES (" + compras.IdUsuario +
                ", '" + compras.NombrePlan + "', " + compras.Monto + ", '" + compras.Nombre + "', '" + compras.Correo +
                "', '" + compras.Empresa + "', '" + compras.Telefono + "', '" + compras.Pais + "', '" +
                compras.FacturaRuc + "', '" + compras.FacturaRazonSocial + "', '" + compras.FacturaDireccion + "', " +
                compras.Pasarela + ")";
            Conexion.SqlExecute(sql, true);
        }
    }
}