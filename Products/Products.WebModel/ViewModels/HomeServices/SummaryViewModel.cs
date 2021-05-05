using System.ComponentModel.DataAnnotations;
using Products.WebModel.Attributes;
using Products.WebModel.Resources.Common;
using Products.WebModel.Resources.HomeServices;
using Products.WebModel.ViewModels.Common;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class SummaryViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(Summary_Resources), Name = "TermsAndConditionsAriaDescription")]
        [Display(ResourceType = typeof(Summary_Resources), Name = "TermsAndConditionsCheckboxHomeSerevicesLabelText")]
        [MustBeTrue(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TermsAndConditionsCheckboxRequiredError")]
        [UIHint("TermsAndConditions")]
        public bool IsTermsAndConditionsChecked { get; set; }
        public string DirectDebitAccountName { get; set; }
        public string DirectDebitAccountNumber { get; set; }
        public string DirectDebitSortCode { get; set; }
        public int? DirectDebitPaymentDay { get; set; }
        public string CustomerFormattedName { get; set; }
        public string DateOfBirth { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public bool IsLandLord { get; set; }
        public ConfirmationModalViewModel BankDetailsModal { get; set; }
        public ConfirmationModalViewModel PersonalDetailsModal { get; set; }
        public ConfirmationModalViewModel CoverDetailsModal { get; set; }
        public AccordionViewModel AccordionViewModel { get; set; }
        public CoverSummaryViewModel CoverSummaryViewModel { get; set; }
        public ConfirmationModalViewModel SupplyAddressModal { get; set; }
        public string CoverAddress { get; set; }
    }
}