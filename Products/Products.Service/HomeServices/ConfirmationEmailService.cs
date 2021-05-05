using Products.Infrastructure;
using Products.Model.Common;
using Products.Repository;
using System;
using System.Threading.Tasks;
using Products.Model.Configuration;

namespace Products.Service.HomeServices
{
    public class ConfirmationEmailService : IConfirmationEmailService
    {
        private readonly IConfigManager _configManager;
        private readonly IContentRepository _contentRepository;
        private readonly IEmailManager _emailManager;

        public ConfirmationEmailService(IContentRepository contentRepository, IEmailManager emailManager, IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(emailManager == null, "emailManager is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(contentRepository == null, "contentRepository is null");

            _emailManager = emailManager;
            _configManager = configManager;
            _contentRepository = contentRepository;
        }

        public async Task SendConfirmationEmail(ConfirmationEmailParameters emailParameters)
        {
            var preLoginDomain = _configManager.GetAppSetting("PreLoginDomain");
            var fromEmailAddress = _configManager.GetAppSetting("EmailFromAddress");
            var emailBaseUrl = _configManager.GetAppSetting("EmailBaseUrl");
            var cdnBaseUrl = _configManager.GetConfigSectionGroup<WebClientConfigurationSection>("webClientConfiguration");

            Guard.Against<ArgumentException>(string.IsNullOrEmpty(cdnBaseUrl.BaseUrl), "Web client configuration baseUrl setting is missing in web.config.");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(preLoginDomain), "PreLoginDomain setting is missing in web.config");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(fromEmailAddress), "FromEmailAddress setting is missing in web.config");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(emailBaseUrl), "emailBaseUrl setting is missing in web.config");

            var emailTemplate = _contentRepository.GetEmailTemplate("HomeServicesConfirmation");

            var emailBody = emailTemplate.Body.ToString()
                .Replace("[$Domain]", preLoginDomain)
                .Replace("[$BaseUrl]", emailBaseUrl)
                .Replace("[$CDNBaseUrl]", cdnBaseUrl.BaseUrl)
                .Replace("[$TITLE]", emailParameters.Title)
                .Replace("[$SURNAME]", emailParameters.LastName)
                .Replace("[$TYPE]", emailParameters.SelectedProductTitle);

            await _emailManager.SendAsync(fromEmailAddress, emailParameters.EmailAddress, emailTemplate.Subject, emailBody, true);
        }
    }
}
