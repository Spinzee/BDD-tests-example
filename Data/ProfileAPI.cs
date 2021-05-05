using Model;
using System;
using Infrastructure.Enums;

namespace Data
{
    public class ProfileAPI : IProfileAPI
    {
        public AccountStatus CheckStatus(string email, string username)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckPassword(string email, string username)
        {
            throw new System.NotImplementedException();
        }

    }
}
