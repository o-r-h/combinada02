using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Veritrade2018.Models
{
    public class CompraPerfil
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Email_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_EmailAddress")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_FirstName_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_FirstName_Required")]
        public string Nombres { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_LastNames_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_LastName_Required")]
        public string Apellidos { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Company_Text")]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Country_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Country_Required")]
        [Remote("IsValidCountrie", "Common")]
        public string Pais { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Phone_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_Required")]
        public string Telefono { get; set; }

        public CompraPerfil()
        {
            Email = "";
            Nombres = "";
            Apellidos = "";
            Empresa = "";
            Pais = "";
            Telefono = "";
        }
    }
}