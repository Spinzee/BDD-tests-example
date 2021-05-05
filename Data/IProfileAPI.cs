using Infrastructure.Enums;
using Model;

namespace Data
{
    public interface IProfileAPI
    {
        bool CheckPassword(string email, string username);
        AccountStatus CheckStatus(string email, string username);
    }
}