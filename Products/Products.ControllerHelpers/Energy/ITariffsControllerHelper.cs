namespace Products.ControllerHelpers.Energy
{
    using Core;
    using System.Threading.Tasks;
    using WebModel.ViewModels.Energy;

    public interface ITariffsControllerHelper : IStepCounterControllerHelper
    {
        Task<AvailableTariffsViewModel> GetAvailableTariffsViewModel();

        bool SetSelectedTariff(string selectedTariffId);

        EnergyUsageViewModel GetEnergyUsageViewModel();

        EnergyUsageViewModel GetEnergyUsageViewModel(UnknownEnergyUsageViewModel viewModel);

        Task<bool> SetUnknownUsage(UnknownEnergyUsageViewModel unknownEnergyUsageViewModel);

        void SetKnownUsage(KnownEnergyUsageViewModel unknownEnergyUsageViewModel);

        bool IsBroadbandBundleSelected(string selectedTariffId);

        Task<bool> HasMatchingBTAddressForCustomer();

        void MarkBundleAsUnavailable();

        bool IsBroadbandPackageAvailable();

        void UpdateYourPriceViewModel();

        bool SelectedTariffHasExtras();

        bool SelectedTariffHasUpgrades();

        Task SetAvailableBroadbandProduct();

        void SetAvailableBundleUpgrade();
        

        Task<OurPricesViewModel> GetOurPriceViewModel(string postcode, TariffStatus tariffStatus, FuelCategory fuelCategory);
    }
}