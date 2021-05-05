using Products.Model.Enums;
using Products.WebModel.ViewModels.HomeServices;
using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public interface IAddressService
    {
        Task<SelectAddressViewModel> GetSelectAddressViewModel(AddressTypes addressType);
        void SetManualAddress(SelectAddressViewModel model);
        Task<bool> SetSelectedAddressByMoniker(SelectAddressViewModel model);
    }
}
