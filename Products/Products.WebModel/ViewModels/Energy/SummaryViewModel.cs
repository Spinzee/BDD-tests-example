namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Attributes;
    using Common;
    using Core.Enums;
    using Model.Enums;
    using Resources.Common;
    using Resources.Energy;

    public class SummaryViewModel : BaseViewModel
    {
        public string TariffHeader { get; set; }

        public string TariffCostFullValue { get; set; }

        public string TariffCostPenceValue { get; set; }

        public ConfirmationModalViewModel TariffDetailsModal { get; set; }

        public ConfirmationModalViewModel MeterDetailsViewModel { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public TariffsViewModel SelectedTariffViewModel { get; set; }

        public bool CanChangeTariff { get; set; }

        public ConfirmationModalViewModel BankDetailsModal { get; set; }

        public string DirectDebitAccountName { get; set; }

        public string DirectDebitAccountNumber { get; set; }

        public string DirectDebitSortCode { get; set; }

        public string DirectDebitPaymentDate { get; set; }

        public FuelType FuelType { get; set; }

        public ConfirmationModalViewModel PersonalDetailsModal { get; set; }

        public string CustomerFormattedName { get; set; }

        public string DateOfBirth { get; set; }

        public string FullAddress { get; set; }

        public string ContactNumber { get; set; }

        public string EmailAddress { get; set; }

        public string ElecMeterTypeMessage { get; set; }

        public string GasMeterTypeMessage { get; set; }

        public string TillInformationHeader { get; set; }

        [AriaDescription(ResourceType = typeof(Summary_Resources), Name = "TermsAndConditionsAriaDescription")]
        [Display(ResourceType = typeof(Form_Resources), Name = "TermsAndConditionsCheckboxEnergyLabelText")]
        [MustBeTrue(ErrorMessageResourceType = typeof(Form_Resources), ErrorMessageResourceName = "TermsAndConditionsCheckboxRequiredError")]
        [UIHint("TermsAndConditions")]
        public bool IsTermsAndConditionsChecked { get; set; }

        public bool IsMeterDetailsAvailable { get; set; }

        public string DisclaimerText { get; set; }

        public string TariffTagLine { get; set; }

        public IEnumerable<TermsAndConditionsPdfLink> EnergyTermsAndConditionsPdfLinks { get; set; }

        public IEnumerable<TermsAndConditionsPdfLink> BroadbandBundleTermsAndConditionsPdfLinks { get; set; }

        public List<TermsAndConditionsPdfLink> HomeServicesBundleTermsAndConditionsPdfLinks { get; set; }

        public bool IsBroadbandBundleSelected { get; set; }

        public bool IsEnergyOnlyTariffSelected { get; set; }

        public string TermsAndConditionsText { get; set; }

        public IEnumerable<string> BundlePackageFeatures { get; set; }

        public bool DisplayExtrasSection { get; set; }

        public bool DisplayPhonePackageSection { get; set; }
        
        public string TalkPackageName { get; set; }

        public string TalkPackagePrice { get; set; }

        public string TalkPackageTagline { get; set; }

        public string SavingsPerMonthTxt { get; set; }

        public List<BaseSummaryExtra> SelectedExtras { get; set; }
        
        public TalkProductModalViewModel TalkProductModalViewModel { get; set; }

        public string DDMandateLinkText { get; set; }

        public string DDMandateLinkAltText { get; set; }

        public BundlePackageType BundlePackageType { get; set; }

        public bool IsBundle { get; set; }

        public string BundlePackageIconFileName { get; set; }

        public string BundlePackageHeaderText { get; set; }

        public string BundlePackageSubHeaderText { get; set; }

        public string MoreInformationModalId { get; set; }

        public bool IsHesBundleSelected { get; set; }

        public bool IsElectricalWiringSelected { get; set; }

        public string HesDDGuaranteeLinkText { get; set; }

        public string HesDDGuaranteeLinkAlt { get; set; }

        public SelectedExtraViewModel SelectedElectricalWiringCover { get; set; }

        public ConfirmationModalViewModel RemovePhonePackageModalViewModel { get; set; }

        public string AboutYourBundleEnergyPara1 { get; set; }

        public string AboutYourBundleEnergyPara2 { get; set; }

        public bool BroadbandApplyInstallationFee { get; set; }

        public string BroadbandInstallationFee { get; set; }

        public bool IsSmartTariff { get; set; }

        public bool IsFixAndControlTariffGroup { get; set; }
        
        public object BundleMegaModalViewModel { get; set; }

        public string BundleMegaModalName { get; set; }

        public bool IsUpgrade { get; set; }

        public string HesBundleBoilerCoverSidebarText { get; set; }

        public string FixNFibreSidebarText { get; set; }

        public string FixNProtectBreakdownCoverText { get; set; }

        public string FixNProtectAccordionHeaderText { get; set; }

        public bool IsFixAndDriveTariffGroup { get; set; }

        public string AboutYourBundlePara3 { get; set; }
    }
}