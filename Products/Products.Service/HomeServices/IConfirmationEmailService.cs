using Products.Model.Common;
using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public interface IConfirmationEmailService
    {
        Task SendConfirmationEmail(ConfirmationEmailParameters emailParameters);
    }
}