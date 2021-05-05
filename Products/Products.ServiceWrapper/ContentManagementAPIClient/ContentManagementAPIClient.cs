namespace Products.ServiceWrapper.ContentManagementService
{
    using System.Threading.Tasks;
    using Core.Configuration.Settings;
    using Extensions;
    using Model.Common.CMSResponse;

    public class ContentManagementAPIClient : BaseAPIWrapper, IContentManagementAPIClient
    {
        public ContentManagementAPIClient(IConfigurationSettings settings): base(settings, "CMSAPI")
        {
        }

        public async Task<CMSResponseModel> GetTariffContent()
        {
            var httpClient = GetHttpClient();

            return await httpClient.GetResponseAsync<CMSResponseModel>("energy_product/entries").ConfigureAwait(false);
        }
    }
}
