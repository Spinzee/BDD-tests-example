using Products.Infrastructure;
using Products.Infrastructure.Logging;
using Products.Model;
using Products.Repository.Common;
using System;
using System.Threading.Tasks;

namespace Products.Service.Common
{
    public class CustomerProfileService : ICustomerProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IActivationEmailService _activationEmailService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger _logger;

        public CustomerProfileService(IActivationEmailService activationEmailService,
            IProfileRepository profileRepository,
            ILogger logger,
            IPasswordService passwordService)
        {
            Guard.Against<ArgumentNullException>(activationEmailService == null, $"{nameof(activationEmailService)} is null");
            Guard.Against<ArgumentNullException>(profileRepository == null, $"{nameof(profileRepository)} is null");
            Guard.Against<ArgumentNullException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentNullException>(passwordService == null, $"{nameof(passwordService)} is null");

            _profileRepository = profileRepository;
            _activationEmailService = activationEmailService;
            _logger = logger;
            _passwordService = passwordService;
        }

        public async Task<bool> DoesProfileExist(string emailAddress)
        {
            var profileData = await _profileRepository.GetProfileIdByEmail(emailAddress);
            return profileData != Guid.Empty;
        }

        public async Task<Guid> GetProfileIdByEmail(string emailAddress)
        {
            return await _profileRepository.GetProfileIdByEmail(emailAddress);
        }

        public async Task<Guid> AddOnlineProfile(UserProfile userProfile)
        {
            Guard.Against<ArgumentNullException>(userProfile == null, $"{nameof(userProfile)} is null");
            Guid userId;

            try
            {
                userProfile.HashedPassword = _passwordService.HashPasswordPBKDF2(userProfile.Password);
                userId = await _profileRepository.AddOnlineProfile(userProfile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured, Email: {userProfile.Email}, attempting to create an online profile", ex);
            }

            try
            {
                await _profileRepository.AddAuditEvent(userId, userProfile.Email);
            }
            catch (Exception e)
            {
                _logger.Error("Exception occured while inserting profile audit.", e);
            }

            try
            {
                await _activationEmailService.SendActivationEmail(userProfile.Email, userId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured, Email: {userProfile.Email}, attempting to send activation email {ex.Message}", ex);
            }

            return userId;
        }
    }
}
