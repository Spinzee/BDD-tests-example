using System.Web;

namespace Products.Infrastructure
{
    public interface IContextManager
    {
        HttpContextBase HttpContext { get; }

        string GetCookieValueFromContext(string cookieName);
        string GetQueryStringValueFromContext(string queryStringName);
        string GetRawUrl();
    }
}