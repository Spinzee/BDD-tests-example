using System;
using Infrastructure.Enums;

namespace Model
{
    public class Customer
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }
}
