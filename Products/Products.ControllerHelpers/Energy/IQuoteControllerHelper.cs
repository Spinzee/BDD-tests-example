namespace Products.ControllerHelpers.Energy
{
    using System.Threading.Tasks;
    using Products.WebModel.ViewModels.Energy;

    public interface IQuoteControllerHelper : IStepCounterControllerHelper
    {
        void SaveCustomer(PostcodeViewModel viewModel);

        void SetShowCliInSession(bool showCli);

        void SetBundlingJourney(bool isBundlingJourney);

        bool IsNorthernIrelandPostcode(string postcode);

        PostcodeViewModel GetEnergyPostcodeViewModel();

        Task<bool?> IsValidPostCode(string postcode);

        Task<bool> IsOurPricesCustomerAlert();

        Task<bool> IsEnergyCustomerAlert();

        UnableToCompleteViewModel GetUnableToCompleteViewModel(bool isEnergy);

        PostcodeViewModel GetOurPricesPostcodeViewModel();

        SelectFuelViewModel GetSelectFuelViewModel();

        SelectFuelViewModel SetSelectedFuel(SelectFuelViewModel viewModel);

        SelectPaymentMethodViewModel GetSelectPaymentMethodViewModel();

        void SaveSelectedPaymentMethod(SelectPaymentMethodViewModel viewModel);

        bool ShowSmartMeterTypeQuestionInNonCAndCJourney();

        bool ShowSmartFrequency();

        bool ShowUsageScreenForCAndCJourneyNonSmartCustomer();

        void SetBillingPreference();

        SelectMeterTypeViewModel GetSelectMeterTypeViewModel();

        SelectSmartMeterViewModel GetSelectSmartMeterViewModel();

        SmartMeterFrequencyViewModel GetSmartMeterFrequencyViewModel();

        void SetSelectedMeterType(SelectMeterTypeViewModel viewModel);

        void SetHasSmartMeter(SelectSmartMeterViewModel viewModel);

        bool IsAMeterTypeFallout(SelectMeterTypeViewModel viewModel);

        void SaveSelectedSmartMeterFrequency(SmartMeterFrequencyViewModel viewModel);

        Task<SelectAddressViewModel> GetSelectAddressViewModel();

        Task<bool> SetSelectedAddress(string addressMoniker, string addressPickListEntry);

        void SetManualAddress(SelectAddressViewModel model);

        Task<bool> ProcessAddress();

        void ResetCustomer();

        void SaveDetailsFromHub(ExistingCustomerViewModel viewModel);
    }
}
