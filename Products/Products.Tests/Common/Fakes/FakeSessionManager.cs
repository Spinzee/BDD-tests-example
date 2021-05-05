using Products.Infrastructure;
using System.Collections.Generic;

namespace Products.Tests.Common.Fakes
{
    public class FakeSessionManager : ISessionManager
    {
        public Dictionary<string, object> SessionObject { get; set; }

        public FakeSessionManager()
        {
            SessionObject = new Dictionary<string, object>();
        }

        public void ClearSession()
        {
            SessionObject = new Dictionary<string, object>();
        }

        public T GetOrDefaultSessionDetails<T>(string key) where T : new()
        {
            if (SessionObject.ContainsKey(key))
            {
                return (T)SessionObject[key];
            }

            return new T();
        }

        public T GetSessionDetails<T>(string key)
        {
            if (SessionObject.ContainsKey(key))
            {
                return (T)SessionObject[key];
            }

            return default(T);
        }

        public void RemoveSessionDetails(string key)
        {
            if (SessionObject.ContainsKey(key))
            {
                SessionObject.Remove(key);
            }
        }

        public void SetSessionDetails<T>(string key, T value)
        {
            SessionObject[key] = value;
        }
    }
}
