namespace Products.WebModel.ViewModels.TariffChange
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Products.WebModel.Resources.Common;

    public class GetCustomerEmailViewModel : BaseViewModel
    {
        public GetCustomerEmailViewModel()
        {
            DataLayer = new Dictionary<string, string>();
        }

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

        public Dictionary<string, string> DataLayer { get; set; }
    }
}