namespace Products.Tests.Common.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Products.Repository.Common;

    public class FakeProfileRepository : IProfileRepository
    {
        public List<string> CustomerAccounts { get; set; }

        public bool ProfileExists { get; set; } = true;

        public int EmailSent { get; set; } = 0;

        // ReSharper disable once InconsistentNaming
        public int CallToCheckProfileDB { get; set; }

        // ReSharper disable once InconsistentNaming
        public int InsertIntoProfileDB { get; set; }

        // ReSharper disable once InconsistentNaming
        public int InsertIntoAuditDB { get; set; }

        public Exception ProfileException { get; set; }

        public Exception AuditEventException { get; set; }

        public Exception CheckProfileException { get; set; }

        public Dictionary<string, string> InsertProfileSqlParameters = new Dictionary<string, string>();

        public Dictionary<string, string> InsertAuditSqlParameters = new Dictionary<string, string>();

        public async Task<Guid> AddOnlineProfile(UserProfile userProfile)
        {
            InsertIntoProfileDB++;

            if (ProfileException != null)
            {
                throw ProfileException;
            }

            InsertProfileSqlParameters.Add("logOnName", userProfile.Email);
            InsertProfileSqlParameters.Add("password", userProfile.HashedPassword);
            InsertProfileSqlParameters.Add("title", userProfile.Title);
            InsertProfileSqlParameters.Add("firstName", userProfile.FirstName);
            InsertProfileSqlParameters.Add("lastName", userProfile.LastName);
            InsertProfileSqlParameters.Add("marketingConsent", userProfile.MarketingConsent.ToString());
            InsertProfileSqlParameters.Add("dateOfBirth", userProfile.DateOfBirth?.ToLongDateString());
            InsertProfileSqlParameters.Add("userInterest", userProfile.UserInterest.ToString());
            InsertProfileSqlParameters.Add("accountStatus", userProfile.AccountStatus.ToString());
            InsertProfileSqlParameters.Add("telephoneNumber", userProfile.TelephoneNumber);
            InsertProfileSqlParameters.Add("mobileNumber", userProfile.MobileNumber);
            InsertProfileSqlParameters.Add("forgottenPassword", userProfile.ForgottenPassword.ToString());
            InsertProfileSqlParameters.Add("signupBrand", userProfile.SignupBrand);
            InsertProfileSqlParameters.Add("dateRegistered", DateTime.Now.ToLongDateString());

            return await Task.FromResult(new Guid("fa4aa87a-bc91-4481-a29e-92609a0aefd4"));
        }

        public async Task AddAuditEvent(Guid userGuid, string email)
        {
            if (AuditEventException != null)
            {
                throw AuditEventException;
            }

            InsertIntoAuditDB++;

            InsertAuditSqlParameters.Add("userGuid", userGuid.ToString());
            InsertAuditSqlParameters.Add("email", email);

            await Task.CompletedTask;
        }

        public async Task<Guid> GetProfileIdByEmail(string emailAddress)
        {
            if (CheckProfileException != null)
            {
                throw CheckProfileException;
            }

            CallToCheckProfileDB++;

            return await Task.FromResult(ProfileExists ? new Guid("fa4aa87a-bc91-4481-a29e-92609a0aefd4") : Guid.Empty);
        }

        public async Task<List<string>> GetUserAccountsByLoginNameAsync(string logonName)
        {
            return await Task.FromResult(CustomerAccounts.ToList());
        }
    }
}
