using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.TariffChange;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.TariffChange
{
    public class IdentifyCustomerViewModel
    {
        public IdentifyCustomerViewModel()
        {
            GoogleCaptchaViewModel = new GoogleCaptchaViewModel();
        }

        [Required(ErrorMessageResourceType = typeof(IdentifyCustomer_Resources), ErrorMessageResourceName = "AccountNumberRequiredError")]
        [RegularExpression(RegularExpressionConstants.CustomerAccountNumber, ErrorMessageResourceType = typeof(IdentifyCustomer_Resources), ErrorMessageResourceName = "AccountNumberRegexErrorMessage")]
        [Display(ResourceType = typeof(IdentifyCustomer_Resources), Name = "AccountNumberTitle")]
        [LocalisedDescription("AccountNumber")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(IdentifyCustomer_Resources), ErrorMessageResourceName = "PostcodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.AddProductPostcode, ErrorMessageResourceType = typeof(IdentifyCustomer_Resources), ErrorMessageResourceName = "PostCodeRegexErrorMessage")]
        [Display(ResourceType = typeof(IdentifyCustomer_Resources), Name = "PostcodeTitle")]
        [DataType(DataType.PostalCode)]
        [LocalisedDescription("PostCode")]
        public string PostCode { get; set; }

        public GoogleCaptchaViewModel GoogleCaptchaViewModel { get; set; }

        public string LoginRedirectUrl { get; set; }
    }
}
