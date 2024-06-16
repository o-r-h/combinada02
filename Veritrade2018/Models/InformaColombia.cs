using System;
using System.Data;
using Veritrade2018.Helpers;

namespace Veritrade2018.Models
{
    public class InformaColombia
    {
        public string ID { get; set; }
        public string Tipo { get; set; }
        public string NumeroId { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoActividad { get; set; }
        public string DescripcionActividad { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }
        public string Direccion { get; set; }
        public string Departamento { get; set; }
        public string Ciudad { get; set; }
        public string FormaJuridica { get; set; }
        public string CargoContacto { get; set; }
        public string NombreContacto { get; set; }
        public string TipoIdContacto { get; set; }
        public string IdContacto { get; set; }
        public DateTime FechaConstitucion { get; set; }
        public DateTime FechaRenovacion { get; set; }
        public string Camara { get; set; }
        public string EstadoRenovacion { get; set; }
        public DateTime FechaBalance { get; set; }
        public string TipoBalance { get; set; }
        public int DuracionBalance { get; set; }
        public string FuenteBalance { get; set; }
        public string NumeroMatricula { get; set; }
        public string FuenteMatricula { get; set; }
        public DateTime FechaEfectoMatricula { get; set; }
        public string NumIncidenciasJudiciales { get; set; }
        public int FechaEvaluacionActual { get; set; }
        public string OpinionCreditoActual { get; set; }
        public int FechaEvaluacion1 { get; set; }
        public string OpinionCredito1 { get; set; }
        public int FechaEvaluacion2 { get; set; }
        public string OpinionCredito2 { get; set; }
        public int FechaEvaluacion3 { get; set; }
        public string OpinionCredito3 { get; set; }
        public int FechaEvaluacion4 { get; set; }
        public string OpinionCredito4 { get; set; }
        public string EstadoEmpresa { get; set; }
        public string EstructuraCorporativa { get; set; }
        public string FuentesLaft { get; set; }

        
        public static InformaColombia obtenerInforme(int idEmpresa)
        {
            string sql = $"select informa_colombia.* from informa_colombia ";
            sql += $"inner join empresa_co on empresa_co.ruc = informa_colombia.numero_id ";
            sql += $"where empresa_co.idempresa = { idEmpresa}";

            var dt = Conexion.SqlDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                return new InformaColombia();
            }
            else
            {


                InformaColombia informe = null;
                foreach (DataRow row in dt.Rows)
                {
                    informe = InformaColombia.mapDataRow(row);
                    

                }
                return informe;
            }
        }

        public static InformaColombia obtenerInformePorRuc(string ruc)
        {
            string sql = $"select informa_colombia.* from informa_colombia ";
            sql += $"where informa_colombia.numero_id  = { ruc}";

            var dt = Conexion.SqlDataTable(sql);
            if (dt.Rows.Count == 0)
            {
                return new InformaColombia();
            }
            else
            {


                InformaColombia informe = null;
                foreach (DataRow row in dt.Rows)
                {
                    informe = InformaColombia.mapDataRow(row);


                }
                return informe;
            }
        }

        public static InformaColombia mapDataRow(DataRow row)
        {
            InformaColombia informe = new InformaColombia();
            informe.ID = row["ID"] != null ? Convert.ToString(row["ID"]) : null;
            informe.Tipo = row["Tipo"] != null ? Convert.ToString(row["Tipo"]) : null;
            informe.NumeroId = row["Numero_Id"] != null ? Convert.ToString(row["Numero_Id"]) : null;
            informe.RazonSocial = row["Razon_Social"] != null ? Convert.ToString(row["Razon_Social"]) : null;
            informe.CodigoActividad = row["Codigo_Actividad"] != null ? Convert.ToString(row["Codigo_Actividad"]) : null;
            informe.DescripcionActividad = row["Descripcion_Actividad"] != null ? Convert.ToString(row["Descripcion_Actividad"]) : null;
            informe.Telefono = row["Telefono"] != null ? Convert.ToString(row["Telefono"]) : null;
            informe.Email = row["Email"] != null ? Convert.ToString(row["Email"]) : null;
            informe.Web = row["Web"] != null ? Convert.ToString(row["Web"]) : null;
            informe.Direccion = row["Direccion"] != null ? Convert.ToString(row["Direccion"]) : null;
            informe.Departamento = row["Departamento"] != null ? Convert.ToString(row["Departamento"]) : null;
            informe.Ciudad = row["Ciudad"] != null ? Convert.ToString(row["Ciudad"]) : null;
            informe.FormaJuridica = row["Forma_Juridica"] != null ? Convert.ToString(row["Forma_Juridica"]) : null;
            informe.CargoContacto = row["Cargo_Contacto"] != null ? Convert.ToString(row["Cargo_Contacto"]) : null;
            informe.NombreContacto = row["Nombre_Contacto"] != null ? Convert.ToString(row["Nombre_Contacto"]) : null;
            informe.TipoIdContacto = row["Tipo_Id_Contacto"] != null ? Convert.ToString(row["Tipo_Id_Contacto"]) : null;
            informe.IdContacto = row["Id_Contacto"] != null ? Convert.ToString(row["Id_Contacto"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Constitucion"]))
            {
                informe.FechaConstitucion = Convert.ToDateTime(row["Fecha_Constitucion"]);
            }
            if (!DBNull.Value.Equals(row["Fecha_Renovacion"]))
            {
                informe.FechaRenovacion = Convert.ToDateTime(row["Fecha_Renovacion"]);
            }

            informe.Camara = row["Camara"] != null ? Convert.ToString(row["Camara"]) : null;
            informe.EstadoRenovacion = row["Estado_Renovacion"] != null ? Convert.ToString(row["Estado_Renovacion"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Balance"]))
            {
                informe.FechaBalance = Convert.ToDateTime(row["Fecha_Balance"]);
            }

            informe.TipoBalance = row["Tipo_Balance"] != null ? Convert.ToString(row["Tipo_Balance"]) : null;
            if (!DBNull.Value.Equals(row["Duracion_Balance"]))
            {
                informe.DuracionBalance = Convert.ToInt16(row["Duracion_Balance"]);
            }

            informe.FuenteBalance = row["Fuente_Balance"] != null ? Convert.ToString(row["Fuente_Balance"]) : null;
            informe.NumeroMatricula = row["Numero_Matricula"] != null ? Convert.ToString(row["Numero_Matricula"]) : null;
            informe.FuenteMatricula = row["Fuente_Matricula"] != null ? Convert.ToString(row["Fuente_Matricula"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Efecto_Matricula"]))
            {
                informe.FechaEfectoMatricula = Convert.ToDateTime(row["Fecha_Efecto_Matricula"]);
            }

            informe.NumIncidenciasJudiciales = row["Num_Incidencias_Judiciales"] != null ? Convert.ToString(row["Num_Incidencias_Judiciales"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Evaluacion_Actual"]))
            {
                informe.FechaEvaluacionActual = Convert.ToInt16(row["Fecha_Evaluacion_Actual"]);
            }

            informe.OpinionCreditoActual = row["Opinion_Credito_Actual"] != null ? Convert.ToString(row["Opinion_Credito_Actual"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Evaluacion_1"]))
            {
                informe.FechaEvaluacion1 = Convert.ToInt16(row["Fecha_Evaluacion_1"]);
            }

            informe.OpinionCredito1 = row["Opinion_Credito_1"] != null ? Convert.ToString(row["Opinion_Credito_1"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Evaluacion_2"]))
            {
                informe.FechaEvaluacion2 = Convert.ToInt16(row["Fecha_Evaluacion_2"]);
            }

            informe.OpinionCredito2 = row["Opinion_Credito_2"] != null ? Convert.ToString(row["Opinion_Credito_2"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Evaluacion_3"]))
            {
                informe.FechaEvaluacion3 = Convert.ToInt16(row["Fecha_Evaluacion_3"]);
            }

            informe.OpinionCredito3 = row["Opinion_Credito_3"] != null ? Convert.ToString(row["Opinion_Credito_3"]) : null;
            if (!DBNull.Value.Equals(row["Fecha_Evaluacion_4"]))
            {
                informe.FechaEvaluacion4 = Convert.ToInt16(row["Fecha_Evaluacion_4"]);
            }

            informe.OpinionCredito4 = row["Opinion_Credito_4"] != null ? Convert.ToString(row["Opinion_Credito_4"]) : null;
            informe.EstadoEmpresa = row["Estado_Empresa"] != null ? Convert.ToString(row["Estado_Empresa"]) : null;
            informe.EstructuraCorporativa = row["Estructura_Corporativa"] != null ? Convert.ToString(row["Estructura_Corporativa"]) : null;
            informe.FuentesLaft = row["Fuentes_Laft"] != null ? Convert.ToString(row["Fuentes_Laft"]) : null;
            return informe;

        }
    }
}