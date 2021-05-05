namespace Products.Service.TariffChange
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.WebModel.ViewModels.TariffChange;

    public interface ITariffChangeService
    {
        void ClearJourneyDetails();

        CustomerEligibilityViewModel GetCustomerEligibilityViewModel();

        AcquisitionJourneyViewModel GetAcquisitionJourneyViewModel();

        GetCustomerEmailViewModel GetCustomerEmailViewModel();

        ConfirmationViewModel GetConfirmationViewModel();

        Task<ConfirmDetailsViewModel> GetConfirmDetailsViewModel();

        ConfirmDetailsViewModel ValidateCustomer(IdentifyCustomerViewModel viewModel);

        ConfirmAddressViewModel GetConfirmAddressViewModel();

        TariffSummaryViewModel GetTariffSummaryViewModel();

        ConfirmDetailsViewModel ValidateMultiSiteCustomer(int selectedSiteId);

        GoogleCaptchaViewModel GetGoogleCaptchaViewModel();

        GoogleCaptchaViewModel CheckGoogleCaptchaViewModel();

        IdentifyCustomerViewModel GetUnAuthenticatedIdentifyViewModel();

        void SetFollowOnAsSelectedTariff();

        Dictionary<string, string> GetDataLayer();
    }
}