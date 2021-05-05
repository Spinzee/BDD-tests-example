using Products.Model;
using System;
using System.Threading.Tasks;

namespace Products.Service.Common
{
    public interface ICustomerProfileService
    {
        Task<Guid> AddOnlineProfile(UserProfile userProfile);
        Task<bool> DoesProfileExist(string emailAddress);
        Task<Guid> GetProfileIdByEmail(string emailAddress);
    }
}