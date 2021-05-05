using System;

namespace Products.Model
{
    public class UserProfile
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool MarketingConsent { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int UserInterest { get; set; }
        public int AccountStatus { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public char ForgottenPassword { get; set; }
        public string SignupBrand { get; set; }

        public string HashedPassword { get; set; }
    }
}
