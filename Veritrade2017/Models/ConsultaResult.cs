using System.Data;

namespace Veritrade2017.Models
{
    public class ConsultaResult
    {
        #region Resumen
        public DataTable ResumenProductosGrid { get; set; }
        public DataTable ResumenPaisOrigenGrid { get; set; }
        public DataTable ResumenImportadoresGrid { get; set; }
        public DataTable ResumenMarcasGrid { get; set; }
        public DataTable ResumenViasTransportesGrid { get; set; }
        #endregion

        #region Productos
        public DataTable ProductosGrid { get; set; }
        public DataTable ProductosChart { get; set; }
        #endregion

        #region Importadores
        public DataTable ImportadoresGrid { get; set; }
        public DataTable ImportadoresChart { get; set; }
        #endregion

        #region Exportadores
        public DataTable ExportadoresGrid { get; set; }
        public DataTable ExportadoresChart { get; set; }
        #endregion

        #region PaisOrigen
        public DataTable PaisOrigenGrid { get; set; }
        public DataTable PaisOrigenChart { get; set; }
        #endregion

        #region Vias Transportes
        public DataTable ViasTransportesGrid { get; set; }
        public DataTable ViasTransportesChart { get; set; }
        #endregion

        #region Aduanas
        public DataTable AduanasGrid { get; set; }
        public DataTable AduanaChart { get; set; }
        #endregion
        
        #region Detalle Excel
        public DataTable DetallesGrid { get; set; }
        #endregion
    }
}