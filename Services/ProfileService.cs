using Data;
using Infrastructure.Enums;
using Microsoft.Extensions.Configuration;
using Model;

namespace Services
{
    public class ProfileService : IProfileService
    {
        //This pattern is the best choice for required dependencies.It communicates clearly what instances should be provided.Also,
        //this is the only choice if the injected dependency is to be stored in a readonly field.

        private readonly IProfileRepository _profileRepository;
        private readonly IPasswordService _passwordService;
        
        public ProfileService(IProfileRepository profileRepository, 
                                IPasswordService passwordService)
        {
            _profileRepository = profileRepository;
            _passwordService = passwordService;
        }

        public LoginStatus AttemptLogin(string email, string password)
        {
            //Password hashing algorithms such as PBKDF2, bcrypt, and scrypt are meant for use with passwords and are purposefully slow.
            //Cryptographic hashing algorithms are fast.Fast is good in most situations, but not here.
            //password = _passwordService.HashPasswordPBKDF2(password); ;
            var loginStatus = _profileRepository.GetAccountStatus(email);

            if (loginStatus != null)
            {
                loginStatus.IsEmailValid = true;
                loginStatus.IsPasswordValid = _passwordService.AuthenticatePBKDF2(password, loginStatus.Password);
            }
            else
            {
                loginStatus = new LoginStatus {IsEmailValid = false};
            }

            return loginStatus;
        }
    }
}
