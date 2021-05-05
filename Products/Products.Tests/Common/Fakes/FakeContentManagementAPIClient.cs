namespace Products.Tests.Common.Fakes
{
    using System.Threading.Tasks;
    using Helpers;
    using Model.Common.CMSResponse;
    using ServiceWrapper.ContentManagementService;

    public class FakeContentManagementAPIClient : IContentManagementAPIClient
    {
        private readonly CMSResponseModel _tariffContent;

        public FakeContentManagementAPIClient()
        {
            _tariffContent = FakeContentManagementStub.GetDummyCMSResponseModelWithPdfs();
        }

        public FakeContentManagementAPIClient(CMSResponseModel tariffContent)
        {
            _tariffContent = tariffContent;
        }

        public Task<CMSResponseModel> GetTariffContent()
        {
            return Task.FromResult(_tariffContent);
        }
    }
}
