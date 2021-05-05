using Products.Model;
using System.Threading.Tasks;

namespace Products.Repository.Common
{
    public interface ICustomerAlertRepository
    {
        Task<CustomerAlertResult> IsCustomerAlertActive(string customerAlertName);
    }
}