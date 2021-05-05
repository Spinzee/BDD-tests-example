namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using Common;
    using Core;
    using Core.Enums;
    using Model.Enums;

    public class TariffsViewModel
    {
        private const string TariffPartialName = "_AvailableTariff";
        private const string BundlePartialName = "_AvailableBundle";
        public string ProjectedElectricityMonthlyCost { get; set; }

        public string ProjectedGasMonthlyCost { get; set; }

        public string ProjectedBundlePackageMonthlyCost { get; set; }

        public string OriginalBundlePackageMonthlyCost { get; set; }

        public string ProjectedBundlePackageMonthlySavings { get; set; }

        public string BundleProjectedYearlySavings { get; set; }

        public string ProjectedElectricityYearlyCost { get; set; }

        public string ProjectedMonthlyEnergyCost { get; set; }

        public string ProjectedGasYearlyCost { get; set; }

        public string GasDirectDebitAmount { get; set; }

        public string ElectricityDirectDebitAmount { get; set; }

        public string HeatingBreakdownCoverDirectDebitAmount { get; set; }

        public string ProjectedMonthlyCost { get; set; }

        public string ProjectedYearlyCost { get; set; }

        public string TariffId { get; set; }

        public string DisplayName { get; set; }

        public string TariffName { get; set; }

        public string TariffTagLine { get; set; }

        public IEnumerable<TermsAndConditionsPdfLink> TermsAndConditionsPdfLinks { get; set; }

        public FuelType FuelType { get; set; }

        public TariffInformationLabelViewModel GasTariffInformationLabel { get; set; }

        public TariffInformationLabelViewModel ElectricityTariffInformationLabel { get; set; }

        public string CloseButtonAlt { get; set; }

        public TariffGroup TariffGroup { get; set; }

        public IEnumerable<TariffTickUspViewModel> EnergyTickUsps { get; set; }

        public IEnumerable<TariffTickUspViewModel> BundlePackageTickUsps { get; set; }

        public bool IsBundle { get; set; }

        public string SubmitButtonCssClass { get; set; }

        public string BundleDisclaimer1Text { get; set; }

        public string BundleDisclaimer2Text { get; set; }

        public bool HasGas => FuelType == FuelType.Dual || FuelType == FuelType.Gas;

        public bool HasElectric => FuelType == FuelType.Dual || FuelType == FuelType.Electricity;

        public string EnergyIconPath { get; set; }

        public bool IsChosenTariff { get; set; }

        public string DetailsHeader { get; set; }

        public string DetailsHeaderIconClass { get; set; }

        public bool IsDataShown { get; set; }

        public BroadbandMoreInformationViewModel BroadbandMoreInformation { get; set; }

        public HesMoreInformationViewModel HesMoreInformation { get; set; }

        public string PartialName => IsBundle ? BundlePartialName : TariffPartialName;

        public string SubmitFormId { get; set; }

        public BundlePackageType BundlePackageType { get; set; }

        public string MoreInformationModalId { get; set; }

        public string BundlePackageIconFileName { get; set; }

        public string BundlePackageDisplayText { get; set; }

        public string BundlePackagePriceLbl { get; set; }

        public string UpgradeSectionHeaderText { get; set; }

        public string UpgradeSectionBodyText { get; set; }
    }
}