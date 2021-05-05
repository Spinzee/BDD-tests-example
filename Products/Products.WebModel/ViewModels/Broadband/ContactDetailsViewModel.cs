namespace Products.WebModel.ViewModels.Broadband
{
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Common;
    using Products.WebModel.Resources.Broadband;
    using Products.WebModel.Resources.Common;

    public class ContactDetailsViewModel : BaseViewModel
    {
        public ContactDetailsViewModel()
        {
            YourPriceViewModel = new YourPriceViewModel();
        }

        public YourPriceViewModel YourPriceViewModel { get; set; }

        [AriaDescription(ResourceType = typeof(ContactDetails_Resources), Name = "ContactNumberAriaDescription")]
        [RegularExpression(RegularExpressionConstants.BroadbandContactNumber, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ContactNumberRegexErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ContactNumberRequiredError")]
        [LocalisedDescription("ContactNumber")]
        [Display(ResourceType = typeof(Form_Resources), Name = "ContactNumberTitle")]
        public string ContactNumber { get; set; }

        [AriaDescription(ResourceType = typeof(Form_Resources), Name = "EmailAddressAriaDescription")]
        [RegularExpression(RegularExpressionConstants.Email, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailRequiredError")]
        [LocalisedDescription("Email")]
        [Display(ResourceType = typeof(Form_Resources), Name = "EmailTitle")]
        [StringLength(52, MinimumLength = 6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage", ErrorMessage = null)]
        [BlockedEmail]
        public string EmailAddress { get; set; }

        [AriaDescription(ResourceType = typeof(Form_Resources), Name = "ConfirmEmailAddressAriaDescription")]
        [RegularExpression(RegularExpressionConstants.Email, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ConfirmEmailRequiredError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "ConfirmEmailTitle")]
        [LocalisedDescription("Email")]
        [StringLength(52, MinimumLength = 6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage", ErrorMessage = null)]
        [Compare("EmailAddress", ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailComparisonError")]
        [BlockedEmail]
        public string ConfirmEmailAddress { get; set; }

        [AriaDescription(ResourceType = typeof(ContactDetails_Resources), Name = "MarketingConsentAriaDescription")]
        [LocalisedDescription("MarketingConsent")]
        [Display(ResourceType = typeof(Form_Resources), Name = "MarketingConsentTitle")]
        public bool IsMarketingConsentChecked { get; set; }
    }
}