namespace Products.ControllerHelpers.HomeServices
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Enums;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.HomeServices;

    public interface IHomeServicesControllerHelper
    {
        ConfirmationViewModel ConfirmationViewModel();

        BankDetailsViewModel GetBankDetailsViewModel();

        ContactDetailsViewModel GetContactDetailsViewModel();

        CoverDetailsViewModel GetCoverDetailsViewModel();

        PostcodeViewModel GetEnterPostcodeViewModel(HomeServicesCustomerType customerType, AddressTypes addressType);

        PersonalDetailsViewModel GetPersonalDetailsViewModel();

        Task<SelectAddressViewModel> GetSelectAddressViewModel(AddressTypes addressType);

        void SavePersonalDetailsViewModel(PersonalDetailsViewModel personalDetailsViewModel);

        void SavePostcode(PostcodeViewModel viewModel);

        void SetContactDetails(ContactDetailsViewModel contactDetailsViewModel);

        void SetManualAddress(SelectAddressViewModel model);

        Task<bool> SetSelectedAddressByMoniker(SelectAddressViewModel model);

        void ProcessBankDetails(BankDetailsViewModel model);

        SummaryViewModel GetSummaryViewModel();

        bool IsNorthernIrelandPostcode(string postcode);

        DirectDebitMandateViewModel GetPrintMandateViewModel();

        Task<bool?> IsProductAvailable(PostcodeViewModel model);

        void SaveProductCode(string productCode);

        YourCoverBasketViewModel GetYourCoverBasket();

        bool UpdateExcessProductCode(string productCode);

        CoverDetailsHeaderViewModel GetYourCoverHeader();

        YourCoverBasketViewModel GetYourCoverBasketAjax();

        bool UpdateExtraProductCode(string productCode);

        Task ConfirmSale();

        bool IsLandlord();

        Task<bool> IsCustomerAlert();

        Dictionary<string, string> GetDataLayer();

        string GetStepCounter(string actionName);
    }
}
