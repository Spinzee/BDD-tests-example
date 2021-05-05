using System.Web;

namespace Products.Infrastructure
{
    public class SessionManager : ISessionManager
    {
        public T GetOrDefaultSessionDetails<T>(string key) where T : new()
        {
            var sessionObject = (T)HttpContext.Current.Session[key];

            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (sessionObject == null)
            {
                sessionObject = new T();
            }

            return sessionObject;
        }

        public T GetSessionDetails<T>(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }

        public void RemoveSessionDetails(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        public void SetSessionDetails<T>(string key, T value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void ClearSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}