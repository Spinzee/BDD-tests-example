#nullable enable
using Model;

namespace Data
{
    public interface IProfileRepository
    {
        LoginStatus? GetAccountStatus(string email);
    }
}