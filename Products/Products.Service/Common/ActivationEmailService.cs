using Products.Infrastructure;
using Products.Infrastructure.GuidEncryption;
using Products.Repository;
using Products.Repository.EmailTemplates;
using System;
using System.Threading.Tasks;
using Products.Model.Configuration;

namespace Products.Service.Common
{
    public class ActivationEmailService : IActivationEmailService
    {
        // TODO: move all this to EmailManager, along with the content folder etc.
        private readonly IConfigManager _configManager;
        private readonly IContentRepository _contentRepository;
        private readonly IEmailManager _emailManager;
        private readonly IGuidEncrypter _guidEncrypter;
        private IUtilityService _utilityService;

        public ActivationEmailService(IEmailManager emailManager, IContentRepository contentRepository, IGuidEncrypter guidEncrypter, IConfigManager configManager, IUtilityService utilityService)
        {
            _emailManager = emailManager;
            _contentRepository = contentRepository;
            _guidEncrypter = guidEncrypter;
            _configManager = configManager;
            _utilityService = utilityService;
        }

        public async Task SendActivationEmail(string emailAddress, Guid userId)
        {
            var mySseBaseUrl = _configManager.GetAppSetting("MySseBaseUrl");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(mySseBaseUrl), "MySseBaseUrl setting is missing in web.config.");

            var emailFromAddress = _configManager.GetAppSetting("EmailFromAddress");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(emailFromAddress), "EmailFromAddress setting is missing in web.config.");

            var cdnBaseUrl = _configManager.GetConfigSectionGroup<WebClientConfigurationSection>("webClientConfiguration");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(cdnBaseUrl.BaseUrl), "Web client configuration baseUrl setting is missing in web.config.");

            EmailTemplate emailTemplate = _contentRepository.GetEmailTemplate("ActivationEmailTemplate");
            Guard.Against<ArgumentNullException>(emailTemplate == null, "ActivationEmailTemplate is null.");


            string payload = _guidEncrypter.Encrypt(userId);
            string updateLink = $"{mySseBaseUrl}your-account/activated?id={payload}";


            await _emailManager.SendAsync(emailFromAddress, emailAddress, emailTemplate.Subject, GetActivationEmailBody(emailTemplate.Body, emailAddress, updateLink, cdnBaseUrl.BaseUrl), true);
        }

        private string GetActivationEmailBody(string emailBody, string emailAddress, string updateLink, string cdnBaseUrl)
        {
            emailBody = emailBody
                .Replace("[$Domain]", _configManager.GetAppSetting("PreLoginDomain") ?? string.Empty)
                .Replace("[$BaseUrl]", _utilityService.GetBaseUrl())
                .Replace("[$CDNBaseUrl]", cdnBaseUrl)
                .Replace("[$EmailAddress]", emailAddress)
                .Replace("[$UpdateLink]", updateLink);

            return emailBody;
        }
    }
}
