namespace Products.Infrastructure
{
    public interface ISessionManager
    {
        void SetSessionDetails<T>(string key, T value);

        T GetSessionDetails<T>(string key);

        void RemoveSessionDetails(string key);

        void ClearSession();

        T GetOrDefaultSessionDetails<T>(string key) where T : new();
    }
}
