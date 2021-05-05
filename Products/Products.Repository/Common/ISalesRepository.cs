namespace Products.Repository.Common
{
    using System;
    using System.Threading.Tasks;
    using Products.Model.Common;

    public interface ISalesRepository
    {
        Task<int> SaveApplication(ApplicationData application);

        Task InsertMasusReference(int applicationId, string email, Guid userId);
    }
}