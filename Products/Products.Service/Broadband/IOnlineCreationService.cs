using Products.WebModel.ViewModels.Broadband;
using System.Threading.Tasks;

namespace Products.Service.Broadband
{
    public interface IOnlineCreationService
    {
        OnlineAccountViewModel GetOnlineAccountViewModel();

        OnlineAccountViewModel SetOnlineAccountViewModel(OnlineAccountViewModel model);

        Task CreateUserProfile();

        void SaveOnlinePassword(string password);
    }
}