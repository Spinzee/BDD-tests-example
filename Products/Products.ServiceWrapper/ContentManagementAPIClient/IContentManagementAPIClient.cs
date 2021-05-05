namespace Products.ServiceWrapper.ContentManagementService
{
    using System.Threading.Tasks;
    using Model.Common.CMSResponse;

    public interface IContentManagementAPIClient
    {
        Task<CMSResponseModel> GetTariffContent();
    }
}
