using Products.Infrastructure;
using Products.Model.Broadband;
using Products.Repository;
using System;
using Products.Model.Configuration;

namespace Products.Service.Broadband
{
    public class ConfirmationEmailService : IConfirmationEmailService
    {
        private readonly IContentRepository _contentRepository;

        private readonly IEmailManager _emailManager;
        private readonly IConfigManager _configManager;

        public ConfirmationEmailService(IContentRepository contentRepository, IEmailManager emailManager, IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(emailManager == null, "emailManager is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(contentRepository == null, "contentRepository is null");

            _emailManager = emailManager;
            _configManager = configManager;
            _contentRepository = contentRepository;
        }

        public void SendConfirmationEmail(string baseUrl, BroadbandConfirmationEmailParameters emailParameters)
        {
            var preLoginDomain = _configManager.GetAppSetting("PreLoginDomain");
            var fromEmailAddress = _configManager.GetAppSetting("EmailFromAddress");
            var cdnBaseUrl = _configManager.GetConfigSectionGroup<WebClientConfigurationSection>("webClientConfiguration");

            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(preLoginDomain), "PreLoginDomain setting is missing in web.config");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(fromEmailAddress), "FromEmailAddress setting is missing in web.config");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(cdnBaseUrl.BaseUrl), "Web client configuration baseUrl setting is missing in web.config.");

            var emailTemplate = _contentRepository.GetEmailTemplate("BroadbandConfirmation");

            var emailBody = emailTemplate.Body.ToString()
                .Replace("[$Domain]", preLoginDomain ?? string.Empty)
                .Replace("[$BaseUrl]", baseUrl)
                .Replace("[$CDNBaseUrl]", cdnBaseUrl.BaseUrl)
                .Replace("[$TITLE]", emailParameters.Title)
                .Replace("[$SURNAME]", emailParameters.LastName)
                .Replace("[$BROADBANDPACKAGE]", emailParameters.SelectedProductTitle);

            _emailManager.Send(fromEmailAddress, emailParameters.EmailAddress, emailTemplate.Subject, emailBody, true);
        }
    }
}
