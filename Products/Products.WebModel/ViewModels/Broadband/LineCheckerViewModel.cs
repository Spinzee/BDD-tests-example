using Products.WebModel.Attributes;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Broadband
{
    public class LineCheckerViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(LineChecker_Resources), Name = "PostcodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostcodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.Postcode, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PostCodeRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "PostcodeTitle")]
        [DataType(DataType.PostalCode)]
        [LocalisedDescription("PostCode")]
        public string PostCode { get; set; }

        [AriaDescription(ResourceType = typeof(LineChecker_Resources), Name = "LandlineAriaDescription")]
        [RegularExpression(RegularExpressionConstants.Cli, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "LandlineTitle")]
        [DataType(DataType.PhoneNumber)]
        [LocalisedDescription("PhoneNumber")]
        public string PhoneNumber { get; set; }

        public string ProductCode { get; set; }
    }
}