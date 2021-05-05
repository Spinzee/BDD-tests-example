namespace Products.WebModel.ViewModels.Energy
{
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Resources.Common;

    public class ContactDetailsViewModel : BaseViewModel
    {
        public ContactDetailsViewModel()
        {
            ShoppingBasketViewModel = new YourPriceViewModel();
        }

        [AriaDescription(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "ContactNumberAriaDescription")]
        [HelpText(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "PhoneNumberParagraph")]
        [RegularExpression(RegularExpressionConstants.BroadbandContactNumber, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PreferredPhoneNumberRegexErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "PreferredPhoneNumberRequiredError")]
        [LocalisedDescription("ContactNumber")]
        [Display(ResourceType = typeof(Form_Resources), Name = "PreferredPhoneNumberTitle")]
        [UIHint("StringWithHelpText")]
        public string ContactNumber { get; set; }

        [AriaDescription(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "EmailAddressAriaDescription")]
        [HelpText(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "EmailParagraphEnergy")]
        [RegularExpression(RegularExpressionConstants.Email, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailRequiredError")]
        [LocalisedDescription("Email")]
        [Display(ResourceType = typeof(Form_Resources), Name = "EmailTitle")]
        [StringLength(52, MinimumLength = 6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage", ErrorMessage = null)]
        [UIHint("StringWithHelpText")]
        [BlockedEmail]
        public string EmailAddress { get; set; }

        [AriaDescription(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "ConfirmEmailAddressAriaDescription")]
        [RegularExpression(RegularExpressionConstants.Email, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "ConfirmEmailRequiredError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "ConfirmEmailTitle")]
        [LocalisedDescription("Email")]
        [StringLength(52, MinimumLength = 6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailInvalidEmailErrorMessage", ErrorMessage = null)]
        [Compare("EmailAddress", ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "EmailComparisonError")]
        [BlockedEmail]
        public string ConfirmEmailAddress { get; set; }

        [AriaDescription(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "MarketingConsentAriaDescription")]
        [LocalisedDescription("MarketingConsent")]
        [Display(ResourceType = typeof(ContactDetailsCommon_Resources), Name = "CheckboxDetails")]
        public bool IsMarketingConsentChecked { get; set; }

        public YourPriceViewModel ShoppingBasketViewModel { get; set; }

        public string OkNextAccessibilityText { get; set; }
    }
}