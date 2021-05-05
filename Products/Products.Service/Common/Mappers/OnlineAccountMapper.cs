using Products.Model;
using Products.Model.Broadband;
using Products.Model.Energy;
using Products.Model.Enums;

namespace Products.Service.Common.Mappers
{
    public static class OnlineAccountMapper
    {
        public static UserProfile GetUserProfile(Customer customer, string password)
        {
            return new UserProfile
            {
                Title = customer.PersonalDetails.Title,
                FirstName = customer.PersonalDetails.FirstName,
                LastName = customer.PersonalDetails.LastName,
                DateOfBirth = System.DateTime.Parse(customer.PersonalDetails.DateOfBirth),
                TelephoneNumber = customer.ContactDetails.ContactNumber,
                MobileNumber = customer.ContactDetails.ContactNumber,
                Email = customer.ContactDetails.EmailAddress,
                MarketingConsent = customer.ContactDetails.MarketingConsent,
                Password = password,
                AccountStatus = (int)AccountStatus.AwaitingActivation,
                ForgottenPassword = '0',
                SignupBrand = ((int)Site.SSE).ToString(),
                UserInterest = 0
            };
        }

        public static UserProfile GetUserProfile(EnergyCustomer customer, string password)
        {
            return new UserProfile
            {
                Title = customer.PersonalDetails.Title,
                FirstName = customer.PersonalDetails.FirstName,
                LastName = customer.PersonalDetails.LastName,
                DateOfBirth = System.DateTime.Parse(customer.PersonalDetails.DateOfBirth),
                TelephoneNumber = customer.ContactDetails.ContactNumber,
                MobileNumber = customer.ContactDetails.ContactNumber,
                Email = customer.ContactDetails.EmailAddress,
                MarketingConsent = customer.ContactDetails.MarketingConsent,
                Password = password,
                AccountStatus = (int)AccountStatus.AwaitingActivation,
                ForgottenPassword = '0',
                SignupBrand = ((int)Site.SSE).ToString(),
                UserInterest = 0
            };
        }
    }
}
