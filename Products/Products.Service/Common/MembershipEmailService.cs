using Products.Infrastructure;
using Products.Repository;
using System;

namespace Products.Service.Common
{
    public class MembershipEmailService : IMembershipEmailService
    {
        private readonly IConfigManager _configManager;
        private readonly IEmailManager _emailManager;
        private readonly IContentRepository _contentRepository;

        public MembershipEmailService(IConfigManager configManager, IEmailManager emailManager, IContentRepository contentRepository)
        {
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(emailManager == null, "emailManager is null");
            Guard.Against<ArgumentNullException>(contentRepository == null, "contentRepository is null");

            _configManager = configManager;
            _emailManager = emailManager;
            _contentRepository = contentRepository;
        }

        public void SendMembershipEmail(string emailAddress, string campaignId, string name, string address, string email, string phoneNumber, string membershipId)
        {
            var fromEmailAddress = _configManager.GetAppSetting("EmailFromAddress");

            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(fromEmailAddress), "FromEmailAddress setting is missing in web.config");

            var emailTemplate = _contentRepository.GetEmailTemplate("MembershipEmailTemplate");

            var emailBody = emailTemplate.Body.ToString()
                .Replace("[$CAMPAIGNID]", campaignId)
                .Replace("[$NAME]", name)
                .Replace("[$ADDRESS]", address)
                .Replace("[$EMAIL]", email)
                .Replace("[$PHONE]", phoneNumber)
                .Replace("[$MEMBERSHIPID]", membershipId);

            _emailManager.Send(fromEmailAddress, emailAddress, emailTemplate.Subject, emailBody, true);
        }
    }
}
