using System.Threading.Tasks;

namespace Products.Service.Common
{
    public interface ICustomerAlertService
    {
        Task<bool> IsCustomerAlert(string customerAlertName);
    }
}