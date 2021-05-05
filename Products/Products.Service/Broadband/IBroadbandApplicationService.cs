using Products.WebModel.ViewModels.Broadband;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Service.Broadband
{
    public interface IBroadbandApplicationService
    {
        Task<Dictionary<string, string>> SubmitApplication();
    }
}