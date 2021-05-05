namespace Products.ServiceWrapper.PersonalProjectionService
{
    public interface IPersonalProjectionServiceWrapper
    {
        SiteProjectionResponse SiteProjection(SiteProjectionRequest request);
    }

    public class PersonalProjectionServiceWrapper : IPersonalProjectionServiceWrapper
    {
        private readonly IPersonalProjectionServiceClientFactory _personalProjectionServiceClientFactory;

        public PersonalProjectionServiceWrapper(IPersonalProjectionServiceClientFactory personalProjectionServiceClientFactory)
        {
            _personalProjectionServiceClientFactory = personalProjectionServiceClientFactory;
        }

        public SiteProjectionResponse SiteProjection(SiteProjectionRequest request)
        {
            SiteProjectionResponse response;

            using (var client = _personalProjectionServiceClientFactory.Create())
            {
                request.messageHeader = _personalProjectionServiceClientFactory.CreateMessageHeader();

                response = client.SiteProjection(request);
            }

            return response;
        }
    }
}