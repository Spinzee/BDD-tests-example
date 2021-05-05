namespace Products.WebModel.ViewModels.Common
{
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Resources.Common;

    public class KeepYourNumberViewModel : BaseViewModel
    {
        public bool KeepExistingNumber { get; set; }

        [AriaDescription(ResourceType = typeof(KeepYourNumber_Resources), Name = "LandlineAriaDescription")]
        [RequiredIf("KeepExistingNumber", true, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRequiredError")]
        [RegularExpression(RegularExpressionConstants.Cli, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "LandlineRegexErrorMessage")]
        [Display(ResourceType = typeof(Form_Resources), Name = "LandlineRequiredTitle")]
        [DataType(DataType.PhoneNumber)]
        [LocalisedDescription("PhoneNumber")]
        public string CLI { get; set; }

        public bool IsReadOnly { get; set; }

        public bool ShowExistingPhoneNumberParagraph { get; set; }
    }
}