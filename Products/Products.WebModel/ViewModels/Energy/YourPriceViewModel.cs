namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using Core.Enums;

    public class YourPriceViewModel
    {
        public YourPriceViewModel()
        {
            SelectedExtras = new HashSet<ExtrasItemViewModel>();
        }

        public string TariffName { get; set; }

        public string GasPerMonth { get; set; }

        public string ElectricityPerMonth { get; set; }

        public string Extra { get; set; }

        public string Discount { get; set; }

        public bool ShowDiscountText => !string.IsNullOrEmpty(Discount);

        public string ProjectedMonthlyTotalFullValue { get; set; }

        public string ProjectedMonthlyTotalPenceValue { get; set; }

        public string BundlePackagePrice { get; set; }

        public string BundlePackageOriginalPrice { get; set; }

        public bool IsBundle { get; set; }

        public bool IsBroadbandBundle { get; set; }

        public bool IsFixNProtectBundle { get; set; }

        public bool HasAnnualBundleSavings { get; set; }

        public string EnergyPerMonth { get; set; }

        public bool ShowPhonePackage { get; set; }

        public string AnnualSavingsText { get; set; }

        public IEnumerable<string> BundlePackageFeatures { get; set; }

        public BundlePackageType BundlePackageType { get; set; }

        public string BundlePackageHeaderText { get; set; }

        public string BundlePackageSubHeaderText { get; set; }

        public string NewInstallationFee { get; set; }

        public bool BroadbandApplyInstallationFee { get; set; }

        public int TotalItemsInBasket { get; set; }

        public string BasketTogglerIconFilepath { get; set; }

        public string BasketToggleIconBaseUrl { get; set; }

        public string CloseButtonImgPath { get; set; }

        public HashSet<ExtrasItemViewModel> SelectedExtras { get; set; }

        public bool ShowExtras { get; set; }

        public string BasketCssClass { get; set; }

        public PhonePackageUpgradeViewModel PhonePackageUpgradeViewModel { get; set; }
    }
}