using Model;

namespace Services
{
    public interface IProfileService
    {
        LoginStatus AttemptLogin(string email, string password);
    }
}