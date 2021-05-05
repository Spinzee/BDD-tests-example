namespace Products.WebModel.ViewModels.Broadband
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Common;
    using Resources.Broadband;
    using Resources.Common;

    public class SummaryViewModel : BaseViewModel
    {
        public YourPriceViewModel YourPriceViewModel { get; set; }

        public string Name { get; set; }

        public string DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string SortCode { get; set; }

        public string CliNumber { get; set; }

        public string CancellationChargesParagraph2 { get; set; }

        public string InstallationParagraph { get; set; }

        [AriaDescription(ResourceType = typeof(Summary_Resources), Name = "TermsAndConditionsAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "TermsAndConditionsCheckboxLabelText")]
        [MustBeTrue(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TermsAndConditionsCheckboxRequiredError")]
        public bool IsTermsAndConditionsChecked { get; set; }

        public bool HasCliNumber { get; set; }

        public bool HasEveningWeekend => (YourPriceViewModel?.TelName == ProductDetails_Resources.TalkCodeEveningWeekend);

        public bool HasAnytime => (YourPriceViewModel?.TelName == ProductDetails_Resources.TalkCodeAnytime);

        public bool HasAnytimePlus => (YourPriceViewModel?.TelName == ProductDetails_Resources.TalkCodeAnytimePlus);

        public List<TermsAndConditionsPdfLink> TermsAndConditionsPdfLinks { get; set; }

        public string TermsAndConditionsParagraph1 { get; set; }

        public string TermsAndConditionsParagraph2 { get; set; }

        public string TermsAndConditionsParagraph3 { get; set; }

        public string TermsAndConditionsParagraph4 { get; set; }

        public string CancellationParagraph3 { get; set; }

        public bool ShowCancellationParagraph4 { get; set; }

        public ConfirmationModalViewModel BankDetailsModal { get; set; }

        public ConfirmationModalViewModel PersonalDetailsModal { get; set; }

        public ConfirmationModalViewModel PackageDetailsModal { get; set; }

        public string Accordion7Paragraph1 { get; set; }
    }
}