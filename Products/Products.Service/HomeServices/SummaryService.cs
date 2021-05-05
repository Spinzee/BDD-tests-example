namespace Products.Service.HomeServices
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Mappers;
    using Model.Constants;
    using Products.Model.Common;
    using Products.Model.HomeServices;
    using Products.Repository.HomeServices;
    using Products.Service.Common.Managers;
    using Products.WebModel.Resources.HomeServices;
    using ApplicationData = Model.HomeServices.ApplicationData;

    public class SummaryService : ISummaryService
    {
        private readonly ISessionManager _sessionManager;
        private readonly ILogger _logger;
        private readonly IConfigManager _configManager;
        private readonly IHomeServicesSalesRepository _homeServicesSalesRepository;
        private readonly HomeServicesApplicationDataMapper _homeServicesApplicationDataMapper;
        private readonly IConfirmationEmailService _confirmationEmailService;
        private readonly IContextManager _contextManager;
        private readonly ICampaignManager _campaignManager;
        private readonly IMembershipEmailService _membershipEmailService;

        public SummaryService(
            ISessionManager sessionManager,
            ILogger logger,
            IConfigManager configManager,
            IHomeServicesSalesRepository iHomeServicesSalesRepository,
            HomeServicesApplicationDataMapper homeServicesApplicationDataMapper,
            IConfirmationEmailService confirmationEmailService,
            IContextManager contextManager,
            ICampaignManager campaignManager,
            IMembershipEmailService membershipEmailService)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(iHomeServicesSalesRepository == null, $"{nameof(iHomeServicesSalesRepository)} is null");
            Guard.Against<ArgumentException>(homeServicesApplicationDataMapper == null, $"{nameof(homeServicesApplicationDataMapper)} is null");
            Guard.Against<ArgumentException>(confirmationEmailService == null, $"{nameof(confirmationEmailService)} is null");
            Guard.Against<ArgumentException>(contextManager == null, $"{nameof(contextManager)} is null");
            Guard.Against<ArgumentException>(campaignManager == null, $"{nameof(campaignManager)} is null");
            Guard.Against<ArgumentException>(membershipEmailService == null, $"{nameof(membershipEmailService)} is null");

            _sessionManager = sessionManager;
            _logger = logger;
            _homeServicesSalesRepository = iHomeServicesSalesRepository;
            _homeServicesApplicationDataMapper = homeServicesApplicationDataMapper;
            _configManager = configManager;
            _confirmationEmailService = confirmationEmailService;
            _campaignManager = campaignManager;
            _contextManager = contextManager;
            _membershipEmailService = membershipEmailService;
        }

        public async Task ConfirmSale()
        {
            var homeServicesCustomer = _sessionManager.GetSessionDetails<HomeServicesCustomer>(SessionKeys.HomeServicesCustomer);
            Guard.Against<Exception>(homeServicesCustomer == null, $"{nameof(homeServicesCustomer)} is null");

            Product selectedProduct = homeServicesCustomer.GetSelectedProduct();
            Guard.Against<Exception>(selectedProduct == null, $"{nameof(selectedProduct)} is null");

            string migrateCampaignid = _contextManager.GetCookieValueFromContext("migrateCampaignid");
            // ReSharper disable once PossibleNullReferenceException
            homeServicesCustomer.MigrateMemberid = _contextManager.GetCookieValueFromContext("migrateMemberid");
            homeServicesCustomer.MigrateAffiliateId = _contextManager.GetCookieValueFromContext("migrateAffiliateid");
            homeServicesCustomer.CampaignCode = _campaignManager.GetCampaignCodesMapping(homeServicesCustomer.MigrateAffiliateId, migrateCampaignid);

            ApplicationData applicationData = _homeServicesApplicationDataMapper.GetHomeServicesDataModel(homeServicesCustomer);
            homeServicesCustomer.ApplicationIds = await _homeServicesSalesRepository.SaveApplication(applicationData);

            try
            {
                var emailParameters = new ConfirmationEmailParameters
                {
                    Title = homeServicesCustomer.PersonalDetails.Title,
                    LastName = homeServicesCustomer.PersonalDetails.LastName.ToCapitalLetter(),
                    EmailAddress = homeServicesCustomer.ContactDetails.EmailAddress,
                    SelectedProductTitle = homeServicesCustomer.IsLandlord ? Summary_Resources.ConfirmationEmailLandloardText : Summary_Resources.ConfirmationEmailResidentText
                };

                await _confirmationEmailService.SendConfirmationEmail(emailParameters);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured attempting to send confirmation email for home services customer, email: {homeServicesCustomer.ContactDetails.EmailAddress}.", ex);
            }

            try
            {
                if (!string.IsNullOrEmpty(homeServicesCustomer.MigrateMemberid))
                {
                    string membershipEmailAddress = _configManager.GetAppSetting("MembershipEmailTo");
                    Guard.Against<Exception>(string.IsNullOrEmpty(membershipEmailAddress), "membership email address is not configured in config");

                    _membershipEmailService.SendMembershipEmail(membershipEmailAddress,
                            homeServicesCustomer.CampaignCode,
                            $"{homeServicesCustomer.PersonalDetails.Title} {homeServicesCustomer.PersonalDetails.FirstName} {homeServicesCustomer.PersonalDetails.LastName}",
                            homeServicesCustomer.SelectedCoverAddress.FullAddress(homeServicesCustomer.CoverPostcode),
                            homeServicesCustomer.ContactDetails.EmailAddress,
                            homeServicesCustomer.ContactDetails.ContactNumber,
                            homeServicesCustomer.MigrateMemberid);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured attempting to membership email for Home Services customer, email: {homeServicesCustomer.ContactDetails.EmailAddress}. {ex.Message}", ex);
            }
        }
    }
}
