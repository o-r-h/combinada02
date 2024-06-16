using System.ComponentModel.DataAnnotations;

namespace Veritrade2018.Models
{
    public class FreeTrial
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_EmailUser_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_EmailAddress")]
        public string FtEmail { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Names_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Names_Required")]
        public string FtNombreCompleto { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Company_Text")]
        public string FtEmpresa { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Country_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Country_Required")]
        public string FtPais { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Phone_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_Required")]
        public string FtTelefono { get; set; }

        public string FtCodCampania { get; set; }
    }
}