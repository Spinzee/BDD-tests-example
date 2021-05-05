using Products.WebModel.Attributes;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.Resources.Common;
using Products.WebModel.ViewModels.Common;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.Broadband
{
    public class BankDetailsViewModel : BaseViewModel

    {
        public YourPriceViewModel YourPriceViewModel { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "AccountHolderAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AccountHolderRequiredError")]
        [RegularExpression(RegularExpressionConstants.AccountHolderName, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "AccountHolderRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AccountHolderTitle")]
        //[LocalisedDescription("AccountHolder")]
        public string AccountHolder { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "AccountNumberAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AccountNumberRequiredError")]
        [RegularExpression(RegularExpressionConstants.AccountNumber, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "AccountNumberRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AccountNumberTitle")]
        //[LocalisedDescription("AccountNumber")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.SortCode, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "SortCodeRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "SortCodeTitle")]
        //[LocalisedDescription("SortCode")]
        [MinLength(6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        public string SortCode { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "SortCodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        public string SortCodeSegmentOne { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "SortCodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        public string SortCodeSegmentTwo { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "SortCodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        public string SortCodeSegmentThree { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetails_Resources), Name = "AuthorisedAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AuthorisedTitle")]
        [MustBeTrue(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AuthorisedRequiredError")]
        public bool IsAuthorisedChecked { get; set; }

        public bool? IsRetry { get; set; }

        public bool IsFallout { get; set; }

        public int RetryCount { get; set; }

    }
}