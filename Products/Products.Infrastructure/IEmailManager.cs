namespace Products.Infrastructure
{
    using System.Threading.Tasks;

    public interface IEmailManager
    {
        void Send(string from, string to, string subject, string body, bool isBodyHtml);
        Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml);
    }
}
