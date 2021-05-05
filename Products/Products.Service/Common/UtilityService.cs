using Products.Infrastructure;

namespace Products.Service.Common
{
    public class UtilityService : IUtilityService
    {
        private readonly IContextManager _contextManager;

        public UtilityService(IContextManager contextManager)
        {
            _contextManager = contextManager;
        }

        public string GetBaseUrl()
        {
            var baseUrl = string.Empty;

            var request = _contextManager.HttpContext.Request;
            if (request != null)
            {
                baseUrl = $"{request.Url?.Scheme}://{request.Url?.Authority}/";
            }

            return baseUrl;
        }
    }
}