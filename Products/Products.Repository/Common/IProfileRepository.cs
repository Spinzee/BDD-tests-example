using Products.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Repository.Common
{
    public interface IProfileRepository
    {
        Task<Guid> GetProfileIdByEmail(string emailAddress);
        Task<Guid> AddOnlineProfile(UserProfile userProfile);
        Task AddAuditEvent(Guid userGuid, string email);
        Task<List<string>> GetUserAccountsByLoginNameAsync(string logonName);
    }
}