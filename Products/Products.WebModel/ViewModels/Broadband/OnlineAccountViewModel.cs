using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.TariffChange;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Broadband
{
    public class OnlineAccountViewModel : BaseViewModel
    {
        public YourPriceViewModel YourPriceViewModel { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreatePassword_Resources), ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        [StringLength(14, ErrorMessageResourceType = typeof(CreatePassword_Resources), ErrorMessageResourceName = "PasswordLengthError", MinimumLength = 7)]
        [RegularExpression(RegularExpressionConstants.Password, ErrorMessageResourceType = typeof(CreatePassword_Resources), ErrorMessageResourceName = "PasswordRegExError")]
        [LocalisedDescription("NewPassword")]
        [Display(ResourceType = typeof(CreatePassword_Resources), Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreatePassword_Resources), ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(CreatePassword_Resources), Name = "ConfirmPassword")]
        [LocalisedDescription("ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(CreatePassword_Resources), ErrorMessageResourceName = "PasswordComparisonError")]
        public string ConfirmPassword { get; set; }

    }
}