using Products.Infrastructure;
using Products.Model;
using Products.Model.Enums;
using Products.Service.Common;
using System;
using System.Threading.Tasks;

namespace Products.Service.TariffChange
{
    public class ProfileService : IProfileService
    {
        private readonly ICustomerProfileService _profileService;
        private readonly IJourneyDetailsService _journeyDetailsService;

        public ProfileService(ICustomerProfileService profileService, IJourneyDetailsService journeyDetailsService)
        {
            Guard.Against<ArgumentNullException>(profileService == null, $"{nameof(profileService)} is null");
            Guard.Against<ArgumentNullException>(journeyDetailsService == null, $"{nameof(journeyDetailsService)} is null");
            _profileService = profileService;
            _journeyDetailsService = journeyDetailsService;
        }


        public async Task<bool?> CheckProfileExists(string emailAddress)
        {
            var customer = _journeyDetailsService.GetCustomer();
            if (customer == null)
            {
                _journeyDetailsService.ClearJourneyDetails();
                return null;
            }

            customer.EmailAddress = emailAddress;
            _journeyDetailsService.SetCustomer(customer);

            var profileId = await _profileService.GetProfileIdByEmail(emailAddress);
            return profileId != Guid.Empty;
        }

        public async Task<bool> CreateOnlineProfile(string password)
        {
            var email = _journeyDetailsService.GetCustomer()?.EmailAddress;

            var customerProfile = new UserProfile
            {
                Password = password,
                Email = email,
                AccountStatus = (int)AccountStatus.AwaitingActivation,
                MarketingConsent = false,
                DateOfBirth = null,
                FirstName = string.Empty,
                LastName = string.Empty,
                ForgottenPassword = '0',
                SignupBrand = ((int)Site.SSE).ToString()
            };

            var profileId = await _profileService.AddOnlineProfile(customerProfile);
            return profileId != Guid.Empty;
        }
    }
}