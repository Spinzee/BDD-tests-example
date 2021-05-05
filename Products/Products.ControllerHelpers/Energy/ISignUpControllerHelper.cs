namespace Products.ControllerHelpers.Energy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Enums;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;

    public interface ISignUpControllerHelper: IStepCounterControllerHelper
    {
        PersonalDetailsViewModel GetPersonalDetailsViewModel();

        ContactDetailsViewModel GetContactDetailsViewModel();

        void SavePersonalDetailsVieModel(PersonalDetailsViewModel viewModel);

        void SetContactDetails(ContactDetailsViewModel viewModel);

        bool HasSelectedDirectDebit();

        Task<bool> DoesProfileExist();

        Task<bool> CreateOnlineProfile();

        OnlineAccountViewModel GetOnlineAccountViewModel();

        BankDetailsViewModel GetBankDetailsViewModel();

        BankDetailsViewModel GetUpdatedBankDetailsViewModel(BankDetailsViewModel viewModelToUpdate);

        bool? ValidateBankDetails(BankDetailsViewModel viewModel);

        Task ConfirmSale();

        DirectDebitMandateViewModel GetPrintMandateViewModel(ProductType productType);

        SummaryViewModel GetSummaryViewModel();

        ConfirmationViewModel ConfirmationViewModel();

        UnableToCompleteViewModel GetUnableToCompleteViewModel();

        PhonePackageViewModel GetPhonePackageViewModel();

        ExtrasViewModel GetExtrasViewModel();

        YourPriceViewModel GetYourPriceViewModel();

        BundleUpgradeViewModel GetUpgradesViewModel();

        // ReSharper disable once InconsistentNaming
        void SetPhonePackageInformation(string TalkCode, bool setToDefault);

        void UpdateCustomerKeepYourNumber(KeepYourNumberViewModel viewModel);

        void UpdateCLIFromSession(KeepYourNumberViewModel viewModel);

        Task<ConfirmAddressViewModel> GetConfirmAddressViewModel();

        void SetSelectedBTAddress(ConfirmAddressViewModel viewModel, IEnumerable<BTAddressViewModel> addresses);

        bool SelectedTariffHasExtras();
        
        void AddExtra(string productCode);

        void RemoveExtra(string productCode);

        void SetAvailableBundleUpgrade();

        void AddBundleUpgrade(string productCode);

        void RemoveBundleUpgrade(string productCode);

        void SaveOnlineAccountPassword(string password);
    }
}