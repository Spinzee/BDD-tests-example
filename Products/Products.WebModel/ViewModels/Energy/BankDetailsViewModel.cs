namespace Products.WebModel.ViewModels.Energy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Model.Energy;
    using Products.WebModel.Resources.Common;

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
        [Display(ResourceType = typeof(BankDetailsCommon_Resources), Name = "DDPaymentDateAlt")]
        [UIHint("MultipleInputElement")]
        public string DirectDebitDate { get; set; }

        public List<Tuple<string, string>> AmountItemList { get; set; }

        public bool IsRetry { get; set; }

        public bool IsBroadbandBundleSelected { get; set; }

        public bool IsHesBundle { get; set; }

        public HashSet<Extra> BundleExtras { get; set; }

        public string BroadbandBundleDescription { get; set; }

        public string BroadbandBundlePackageAmount { get; set; }

        public string HesBundlePackageAmount { get; set; }

        public string HesWhyDoIPay0ModalId { get; set; }

        public YourPriceViewModel ShoppingBasketViewModel { get; set; }

        public List<Tuple<string, string>> ExtraDetailsList { get; set; }

        public bool BundleHasExtras { get; set; }

        public string PDFGuaranteeLabel { get; set; }

        public string PDFGuaranteeLabelAlt { get; set; }

        public string HomeServicesLabel { get; set; }

        public bool IsUpgrade { get; set; }
    }
}