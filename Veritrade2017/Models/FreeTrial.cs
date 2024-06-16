using System.ComponentModel.DataAnnotations;

namespace Veritrade2017.Models
{
    public class FreeTrial
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_EmailUser_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Email_EmailAddress")]
        public string FtEmail { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Names_Text")]
        //[Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Names_Required")]
        public string FtNombreCompleto { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_FirstName_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_FirstName_Required")]
        public string FtNombres { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_LastNames_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_LastName_Required")]
        public string FtApellidos{ get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Company_Text")]
        public string FtEmpresa { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Country_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Country_Required")]
        public string FtPais { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Field_Phone_Text")]
        [Required(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_Required")]
        [Phone(ErrorMessageResourceType = typeof(Resources.ValidationResource), ErrorMessageResourceName = "Error_Phone_Valid")]
        public string FtTelefono { get; set; }

        public string FtCodCampania { get; set; }
    }
}