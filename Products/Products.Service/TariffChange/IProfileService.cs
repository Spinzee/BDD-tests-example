using System.Threading.Tasks;

namespace Products.Service.TariffChange
{
    public interface IProfileService
    {
        Task<bool?> CheckProfileExists(string emailAddress);
        Task<bool> CreateOnlineProfile(string password);
    }
}