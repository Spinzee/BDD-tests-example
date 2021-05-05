using Dapper;
using Products.Infrastructure;
using Products.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Repository.Common
{
    public class ProfileRepository : IProfileRepository
    {
        private const string RegisterUserProfileProc = "RegisterUserProfile";
        private const string GetCustomerProfileProc = "GetCustomerProfileByLogOnName";
        private const string CreateAuditEventProc = "CreateAuditEvent";
        private readonly IDatabaseHelper _dbHelper;
        private readonly string _profileConnectionString;
        private readonly string _ecomAuditConnectionString;

        public ProfileRepository(IDatabaseHelper dbHelper, IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(dbHelper == null, "dbHelper is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");

            _dbHelper = dbHelper;
            _profileConnectionString = configManager.GetConnectionString("CSGatewayProfiles.DbConnection");
            _ecomAuditConnectionString = configManager.GetConnectionString("eComAudit.DbConnection");
        }

        public async Task<Guid> GetProfileIdByEmail(string emailAddress)
        {
            var profiles = await _dbHelper.ExecuteStoredProcAsync<string>(_profileConnectionString, GetCustomerProfileProc, new { logonName = emailAddress });
            return profiles.Any() ? Guid.Parse(profiles.First()) : Guid.Empty;
        }

        public async Task<Guid> AddOnlineProfile(UserProfile userProfile)
        {
            var param = new DynamicParameters();
            param.Add("@logOnName", userProfile.Email);
            param.Add("@password", userProfile.HashedPassword);
            param.Add("@title", userProfile.Title);
            param.Add("@firstName", userProfile.FirstName);
            param.Add("@lastName", userProfile.LastName);
            param.Add("@marketingConsent", userProfile.MarketingConsent);
            param.Add("@dateOfBirth", userProfile.DateOfBirth);
            param.Add("@userInterest", userProfile.UserInterest);
            param.Add("@accountStatus", userProfile.AccountStatus);
            param.Add("@telephoneNumber", userProfile.TelephoneNumber);
            param.Add("@mobileNumber", userProfile.MobileNumber);
            param.Add("@forgottenPassword", userProfile.ForgottenPassword);
            param.Add("@signupBrand", userProfile.SignupBrand);
            param.Add("@dateRegistered", DateTime.Now);

            param.Add("@userId", dbType: DbType.Guid, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteAsync(_profileConnectionString, RegisterUserProfileProc, param);
            return param.Get<Guid>("@userId");
        }

        public async Task AddAuditEvent(Guid userGuid, string email)
        {
            var param = new DynamicParameters();
            param.Add("@UserProfileGUID", userGuid.ToString("B"));
            param.Add("@AuditActionID", 1); // 1 = pending profile
            param.Add("@ActionDateTime", DateTime.Now);
            param.Add("@UserID", "Customer");
            param.Add("@UserLogonName", email);

            param.Add("@AuditEventID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _dbHelper.ExecuteAsync(_ecomAuditConnectionString, CreateAuditEventProc, param);
        }

        public async Task<List<string>> GetUserAccountsByLoginNameAsync(string logonName)
        {
            var result = await _dbHelper.ExecuteStoredProcAsync<string>(_profileConnectionString, "GetCustomerAccountNumbersByLogOnName", new
            {
                UserLoginName = logonName
            });

            return result.ToList();
        }
    }
}
