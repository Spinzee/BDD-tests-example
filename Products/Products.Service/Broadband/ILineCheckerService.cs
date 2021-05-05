namespace Products.Service.Broadband
{
    using Products.WebModel.ViewModels.Broadband;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILineCheckerService
    {
        bool IsNorthernIrelandPostcode(string postcode);

        Task<List<AddressViewModel>> GetAddresses();

        LineCheckerViewModel GetLineCheckerViewModel(string productCode);

        void SetLineCheckerDetails(LineCheckerViewModel lineCheckerViewModel);

        Task<bool?> IsProductAvailable(SelectAddressViewModel selectAddressViewModel, List<AddressViewModel> addresses);

        bool IsOpenReachFallout(int selectedAddressId, List<AddressViewModel> addresses);

        SelectAddressViewModel GetSelectAddressViewModel(List<AddressViewModel> addresses);

        void SetInformationPassedByHub(string productCode);

        Task<bool> IsCustomerAlert();

        CannotCompleteOnlineViewModel GetCannotCompleteOnlineViewModel();
    }
}