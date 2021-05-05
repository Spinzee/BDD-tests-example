using Products.Model.Broadband;
using Products.Model.Common;
using System;
using System.Threading.Tasks;

namespace Products.Repository.Broadband
{
    public interface IBroadbandSalesRepository
    {
        Task<int> SaveApplication(ApplicationData application);
        Task InsertApplicationAudit(ApplicationAudit auditData);
        Task InsertMasusReference(int applicationId, string email, Guid userId);
        Task SetLastUpdated(Guid userid);
        Task InsertOpenReachAudit(OpenreachAuditData auditData);
    }
}