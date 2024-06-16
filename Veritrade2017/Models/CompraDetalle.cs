using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class CompraDetalle
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "Stripe_Form_CardName")]
        [Required(ErrorMessageResourceType = typeof(ValidationResource),
            ErrorMessageResourceName = "Error_General_Required")]
        public string NombreTarjeta { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Stripe_Form_Email")]
        [Required(ErrorMessageResourceType = typeof(ValidationResource),
            ErrorMessageResourceName = "Error_General_Required")]
        public string Correo { get; set; }

        public string Telefono { get; set; }
        public string Empresa { get; set; }
        public string Pais { get; set; }

        [Required]
        public string StripeToken { get; set; }

        [Required]
        [Remote("IsValidPlan", "Common")]
        public int ChargeId { get; set; }

        public string CodCampania { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessageResourceType = typeof(ValidationResource),
            ErrorMessageResourceName = "Error_TermsConditions_Required")]
        public bool IsAgree { get; set; }
    }

    public class CompraStripeLog
    {
        public string ChargeId { get; set; }
        public string Code { get; set; }
        public string DeclineCode { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string Parameter { get; set; }
        public string CarId { get; set; }
        public string Brand { get; set; }
        public string Country { get; set; }
        public string Last4 { get; set; }
        public string Name { get; set; }

        public static void SaveLog(CompraStripeLog compraStripeLog)
        {
            var sql =
                "INSERT INTO [compras_stripe]([chargeId],[code],[declineCode],[error],[errorDescription],[errorType],[message],[parameter],[carID],[brand],[country],[last4],[name]) " +
                "VALUES ('" + compraStripeLog.ChargeId + "', '" + compraStripeLog.Code + "', '" +
                compraStripeLog.DeclineCode + "', '" +
                compraStripeLog.Error + "', '" + compraStripeLog.ErrorDescription + "', '" + compraStripeLog.ErrorType +
                "', '" +
                compraStripeLog.Message + "', '" + compraStripeLog.Parameter + "', '" + compraStripeLog.CarId + "', '" +
                compraStripeLog.Brand + "', '" + compraStripeLog.Country + "', '" + compraStripeLog.Last4 + "', '" +
                compraStripeLog.Parameter + "');";

            Conexion.SqlExecute(sql, true);
        }
    }
}