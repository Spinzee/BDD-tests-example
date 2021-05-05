namespace Products.Tests.Common.Fakes
{
    using System.Web;
    using Products.Infrastructure;

    public class FakeContextManager : IContextManager
    {
        public HttpContextBase HttpContext { get; }

        public FakeContextManager(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }

        public string GetCookieValueFromContext(string cookieName)
        {
            switch (cookieName)
            {
                case "migrateMemberid":
                    return "123456";
                case "migrateAffiliateid":
                    return "affiliateCode1";
                case "migrateCampaignid":
                    return "1410789843095";
                default:
                    return string.Empty;
            }
        }

        public string GetQueryStringValueFromContext(string queryStringName)
        {
            return queryStringName;
        }

        public string GetRawUrl()
        {
            return string.Empty;
        }
    }
}
