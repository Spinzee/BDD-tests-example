namespace Products.WebModel.ViewModels.TariffChange
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Core;
    using Model.Enums;
    using Products.WebModel.Resources.TariffChange;

    public class TariffSummaryViewModel : BaseViewModel
    {
        [AriaDescription(ResourceType = typeof(TariffSummary_Resources), Name = "IsTermsAndConditionsCheckedLabelAlt")]
        [Display(ResourceType = typeof(TariffSummary_Resources), Name = "IsTermsAndConditionsCheckedLabelText")]
        [MustBeTrue(ErrorMessageResourceType = typeof(TariffSummary_Resources), ErrorMessageResourceName = "IsTermsAndConditionsCheckedValidationError")]
        [UIHint("TermsAndConditions")]
        public bool IsTermsAndConditionsChecked { get; set; }

        public AvailableTariff SelectedTariff { get; set; }

        public bool HasAnyDirectDebitAccount { get; set; }

        public FuelType FuelType { get; set; }

        public string DayOfPaymentEachMonth { get; set; }

        public DateTime NewTariffStartDate { get; set; }

        public string ElectricityAmount { get; set; }

        public string ElectricityAmountLabel { get; set; }

        public string ElectricityFrequency { get; set; }

        public string GasAmount { get; set; }

        public string GasAmountLabel { get; set; }

        public string GasFrequency { get; set; }

        public string GasAndElectricityFrequency { get; set; }

        public string GasAndElectricityFrequencyLabel { get; set; }

        public string Title { get; set; }

        public string Header1 { get; set; }

        public string Header2 { get; set; }

        public string SummaryDetailsClass => HasAnyDirectDebitAccount ? "col-md-6" : "col-md-12";

        public string FuelTypeIconSvg { get; set; }

        public CTCJourneyType CTCJourneyType { get; set; }

        public Dictionary<string, string> DataLayer { get; set; }
    }
}
