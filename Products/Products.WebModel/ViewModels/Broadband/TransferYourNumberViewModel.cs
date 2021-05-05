using Products.WebModel.Attributes;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Broadband
{
    public class TransferYourNumberViewModel : BaseViewModel
    {
        public YourPriceViewModel YourPriceViewModel { get; set; }

        public bool KeepExistingNumber { get; set; }

        [AriaDescription(ResourceType = typeof(TransferYourNumber_Resources), Name = "LandlineAriaDescription")]
        [RequiredIf("KeepExistingNumber", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRequiredError")]
        [RegularExpression(RegularExpressionConstants.Cli, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "LandlineRequiredTitle")]
        [DataType(DataType.PhoneNumber)]
        [LocalisedDescription("PhoneNumber")]
        public string PhoneNumber { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsSSECustomerCLI { get; set; }

    }
}