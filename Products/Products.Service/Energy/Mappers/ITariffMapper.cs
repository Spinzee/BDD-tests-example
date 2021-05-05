namespace Products.Service.Energy.Mappers
{
    using System.Collections.Generic;
    using Core;
    using Core.Enums;
    using Products.Model.Energy;
    using Products.WebModel.ViewModels.Energy;

    public interface ITariffMapper
    {
        TariffInformationLabelViewModel GetTariffInformation(Product product, FuelCategory fuelCategory);

        YourPriceViewModel GetYourPriceViewModel(EnergyCustomer energyCustomer, string webClientBaseUrl);

        TariffsViewModel ToTariffViewModel(Tariff tariff, EnergyCustomer energyCustomer, List<CMSEnergyContent> cmsEnergyContents);

        IEnumerable<string> GetBundlePackageFeatures(BundlePackageType bundlePackageType, Tariff selectedTariff);

        string FormatBundlePackageMonthlyPrice(BundlePackage bundlePackage);

        string BundleIconFileName(BundlePackageType bundlePackageType);

        BroadbandMoreInformationViewModel GetBroadbandMoreInformationViewModel(
            string projectedBundlePackageMonthlyCost,
            string originalBundlePackageMonthlyCost,
            string bundleDisclaimerModalText,
            string projectedBundleMonthlySavings);

        HesMoreInformationViewModel GetHesMoreInformationViewModel(
            Tariff tariff,
            string originalBundleMonthlyCost,
            string projectedBundleMonthlySavings,
            string bundleProjectedYearlySavings);

        string GetMoreInformationModalId(BundlePackageType bundlePackageType);

        string GetBroadbandMoreInformationDisclaimerText1(
            string projectedBundlePackageMonthlyCost,
            string originalBundlePackageMonthlyCost,
            string bundleProjectedYearlySavings,
            string originalBundleMonthlyCost,
            string projectedBundleMonthlySavings);

        TariffGroup GetTariffGroupForProduct(Product product);
    }
}