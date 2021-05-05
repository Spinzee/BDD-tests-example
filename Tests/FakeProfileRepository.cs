using Data;
using Model;

namespace Tests
{
    public class FakeProfileRepository : IProfileRepository
    {
        public LoginStatus? LoginStatus { get; set; }

        public LoginStatus? GetAccountStatus(string email)
        {
            return LoginStatus;
        }
    }
}