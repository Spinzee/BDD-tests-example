using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.HomeServices;
using Products.WebModel.ViewModels.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class BankDetailsViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(BankDetailsCommon_Resources), Name = "AccountHolderAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AccountHolderRequiredError")]
        [RegularExpression(RegularExpressionConstants.AccountHolderName, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "AccountHolderRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AccountHolderTitle")]
        public string AccountHolder { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetailsCommon_Resources), Name = "AccountNumberAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AccountNumberRequiredError")]
        [RegularExpression(RegularExpressionConstants.AccountNumber, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "AccountNumberRegExError")]
        [MaxLength(8, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AccountNumberRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AccountNumberTitle")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRequiredError")]
        [RegularExpression(RegularExpressionConstants.SortCode, ErrorMessageResourceType = typeof(Form_Resources),
            ErrorMessageResourceName = "SortCodeRegExError")]
        [Display(ResourceType = typeof(Form_Resources), Name = "SortCodeTitle")]
        [MinLength(6, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        public string SortCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [UIHint("MultipleInputElement")]
        public string SortCodeSegmentOne { get; set; }

        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [UIHint("MultipleInputElement")]
        public string SortCodeSegmentTwo { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetailsCommon_Resources), Name = "SortCodeAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [MinLength(2, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "SortCodeRegExError")]
        [UIHint("MultipleInputElement")]
        public string SortCodeSegmentThree { get; set; }

        [AriaDescription(ResourceType = typeof(BankDetailsCommon_Resources), Name = "AuthorisedAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "AuthorisedTitle")]
        [MustBeTrue(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "AuthorisedRequiredError")]
        public bool IsAuthorisedChecked { get; set; }

        [AriaDescription(ResourceType = typeof(Form_Resources), Name = "DirectDebitPaymentDateAriaDescription")]
        [Required(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DirectDebitPaymentDateRequiredError")]
        [Range(1, 28, ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "DirectDebitPaymentDateRegexError")]
        [UIHint("MultipleInputElement")]
        public string DirectDebitDate { get; set; }

        public List<ProductsDataViewModel> ProductData { get; set; }
        public bool IsRetryExceeded { get; set; }
        public bool BankDetailsIsValid { get; set; }
        public bool IsRetry { get; set; }
    }

    public class ProductsDataViewModel
    {
        public string Amount { get; set; }
        public string ProductName { get; set; }
    }
}
