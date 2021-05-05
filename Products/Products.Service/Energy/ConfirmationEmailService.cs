namespace Products.Service.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Infrastructure;
    using Model.Common;
    using Model.Configuration;
    using Model.Energy;
    using Repository;
    using Repository.EmailTemplates;

    public class ConfirmationEmailService : IConfirmationEmailService
    {
        private readonly IConfigManager _configManager;
        private readonly IContentRepository _contentRepository;
        private readonly IEmailManager _emailManager;

        public ConfirmationEmailService(IContentRepository contentRepository, IEmailManager emailManager, IConfigManager configManager)
        {
            Guard.Against<ArgumentException>(emailManager == null, $"{nameof(emailManager)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(contentRepository == null, $"{nameof(contentRepository)} is null");

            _emailManager = emailManager;
            _configManager = configManager;
            _contentRepository = contentRepository;
        }

        public async Task SendConfirmationEmail(ConfirmationEmailParameters emailParameters, string emailTemplateName, HashSet<Extra> selectedExtras)
        {
            string preLoginDomain = _configManager.GetAppSetting("PreLoginDomain");
            string fromEmailAddress = _configManager.GetAppSetting("EmailFromAddress");
            string emailBaseUrl = _configManager.GetAppSetting("EmailBaseUrl");
            var cdnBaseUrl = _configManager.GetConfigSectionGroup<WebClientConfigurationSection>("webClientConfiguration");

            Guard.Against<ArgumentException>(string.IsNullOrEmpty(cdnBaseUrl.BaseUrl), "Web client configuration baseUrl setting is missing in web.config.");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(preLoginDomain), "PreLoginDomain setting is missing in web.config");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(fromEmailAddress), "FromEmailAddress setting is missing in web.config");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(emailBaseUrl), "emailBaseUrl setting is missing in web.config");

            EmailTemplate emailTemplate = _contentRepository.GetEmailTemplate(emailTemplateName);

            string emailBody = emailTemplate.Body.ToString()
                .Replace("[$Domain]", preLoginDomain)
                .Replace("[$BaseUrl]", emailBaseUrl)
                .Replace("[$CDNBaseUrl]", cdnBaseUrl.BaseUrl)
                .Replace("[$FIRSTNAME]", emailParameters.FirstName)
                .Replace("[$SELECTEDPRODUCTTITLE]", emailParameters.SelectedProductTitle)
                .Replace("[$PhoneNo]", emailParameters.PhoneNumber)
                .Replace("[$COVER_NAME]", emailParameters.FixNProtectCoverName)
                .Replace("[$COVER_NAME_TEXT]",  emailParameters.FixNProtectCoverName.ToLowerInvariant())
                .Replace("[$FIXNFIBREHEADER]", emailParameters.FixNFibreHeader)
                .Replace("[$ExtrasSection]", GetSelectedExtraContent(selectedExtras, cdnBaseUrl.BaseUrl));

            await _emailManager.SendAsync(fromEmailAddress, emailParameters.EmailAddress, emailTemplate.Subject, emailBody, true);
        }

        private string GetSelectedExtraContent(HashSet<Extra> selectedExtras, string cdnBaseUrl)
        {
            string extrasSection = string.Empty;
            if (selectedExtras.Count > 0)
            {
                extrasSection = _contentRepository.GetEmailComponent("ExtrasSection").Content.ToString();
                string extrasContent = string.Empty;
                foreach (Extra extra in selectedExtras)
                {
                    switch (extra.Type)
                    {
                        case ExtraType.ElectricalWiring:
                            extrasContent = string.Concat(extrasContent, _contentRepository.GetEmailComponent(extra.Type.ToString()).Content.ToString());
                            break;
                    }
                }

                extrasSection = extrasSection.Replace("[$Extras]", extrasContent);
                extrasSection = extrasSection.Replace("[$CDNBaseUrl]", cdnBaseUrl);
            }

            return extrasSection;
        }
    }
}