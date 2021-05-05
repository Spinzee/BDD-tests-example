using Products.WebModel.ViewModels.Broadband;
using System.Threading.Tasks;

namespace Products.Service.Broadband
{
    public interface IExistingCustomerService
    {
        ExistingCustomerViewModel GetExistingCustomerViewModel();
        void SetExistingCustomer(bool isExistingCustomer);
        void SetInformationPassedByHub(string productCode, string migrateAffiliateId, string migrateCampaignId, string membershipId);
        Task<bool> IsCustomerAlert();
    }
}