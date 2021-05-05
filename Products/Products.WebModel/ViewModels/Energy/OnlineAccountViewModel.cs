using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.Energy;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Energy
{
    public class OnlineAccountViewModel : BaseViewModel
    {

        [AriaDescription(ResourceType = typeof(OnlineAccount_Resources), Name = "PasswordAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(OnlineAccount_Resources), ErrorMessageResourceName = "PasswordRequiredError")]
        [DataType(DataType.Password)]
        [RegularExpression(RegularExpressionConstants.Password, ErrorMessageResourceType = typeof(OnlineAccount_Resources), ErrorMessageResourceName = "PasswordRegexError")]
        [Display(ResourceType = typeof(OnlineAccount_Resources), Name = "Password")]
        public string Password { get; set; }

        [AriaDescription(ResourceType = typeof(OnlineAccount_Resources), Name = "ConfirmPasswordAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(OnlineAccount_Resources), ErrorMessageResourceName = "ConfirmPasswordRequiredError")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(OnlineAccount_Resources), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(OnlineAccount_Resources), ErrorMessageResourceName = "ConfirmPasswordCompareError")]
        public string ConfirmPassword { get; set; }
    }
}
