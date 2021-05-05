using System.Web;

namespace Products.Infrastructure
{
    public class ContextManager : IContextManager
    {
        public HttpContextBase HttpContext =>
            new HttpContextWrapper(System.Web.HttpContext.Current);

        public string GetCookieValueFromContext(string cookieName)
        {
            return HttpContext.Request.Cookies[cookieName]?.Value;
        }
        public string GetQueryStringValueFromContext(string queryStringName)
        {
            return HttpContext.Request.QueryString[queryStringName];
        }
        public string GetRawUrl()
        {
            return HttpContext.Request.RawUrl;
        }

    }
}