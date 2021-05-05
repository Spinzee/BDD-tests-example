using Products.Model.HomeServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Repository.HomeServices
{
    public interface IHomeServicesSalesRepository
    {
        Task<List<int>> SaveApplication(ApplicationData applicationData);
    }
}