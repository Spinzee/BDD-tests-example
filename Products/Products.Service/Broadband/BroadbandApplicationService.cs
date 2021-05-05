namespace Products.Service.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Mappers;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using Repository.Broadband;
    using Validators;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Broadband.Extensions;

    public class BroadbandApplicationService : IBroadbandApplicationService
    {
        private readonly IApplicationDataMapper _applicationDataMapper;
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly IBroadbandSalesRepository _broadbandSalesRepository;
        private readonly IConfirmationEmailService _confirmationEmailService;
        private readonly IContextManager _contextManager;
        private readonly string _emailBaseUrl;
        private readonly ILogger _logger;
        private readonly string _membershipEmailAddress;
        private readonly IMembershipEmailService _membershipEmailService;
        private readonly ISessionManager _sessionManager;

        public BroadbandApplicationService(IBroadbandJourneyService broadbandJourneyService,
            ISessionManager sessionManager,
            IConfirmationEmailService confirmationEmailService,
            IBroadbandSalesRepository broadbandSalesRepository,
            IApplicationDataMapper applicationDataMapper,
            ILogger logger,
            IConfigManager configManager,
            IMembershipEmailService membershipEmailService,
            IContextManager contextManager)
        {
            Guard.Against<ArgumentNullException>(broadbandJourneyService == null, "broadbandJourneyService is null");
            Guard.Against<ArgumentNullException>(confirmationEmailService == null, "confirmationEmailService is null");
            Guard.Against<ArgumentNullException>(broadbandSalesRepository == null, "salesRepository is null");
            Guard.Against<ArgumentNullException>(applicationDataMapper == null, "applicationDataMapper is null");
            Guard.Against<ArgumentNullException>(logger == null, "logger is null");
            Guard.Against<ArgumentNullException>(sessionManager == null, "sessionManager is null");
            Guard.Against<ArgumentNullException>(membershipEmailService == null, "membershipEmailService is null");
            Guard.Against<ArgumentNullException>(contextManager == null, $"{nameof(contextManager)} is null");

            _broadbandJourneyService = broadbandJourneyService;
            _confirmationEmailService = confirmationEmailService;
            _broadbandSalesRepository = broadbandSalesRepository;
            _applicationDataMapper = applicationDataMapper;
            _membershipEmailService = membershipEmailService;
            _logger = logger;
            _sessionManager = sessionManager;
            _emailBaseUrl = configManager.GetAppSetting("EmailBaseUrl");
            _membershipEmailAddress = configManager.GetAppSetting("MembershipEmailTo");
            _contextManager = contextManager;
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_emailBaseUrl), "EmailBaseUrl is empty in web.config");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_membershipEmailAddress), "MembershipEmailTo is empty in web.config");
        }

        public async Task<Dictionary<string, string>> SubmitApplication()
        {
            Customer customer = _broadbandJourneyService.GetBroadbandJourneyDetails().Customer;
            Guard.Against<ArgumentException>(customer == null, $"{nameof(customer)} is null");
            var openReachResponse = _sessionManager.GetSessionDetails<OpenReachData>(SessionKeys.OpenReachResponse);
            var yourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails);

            string migrateCampaignid = _contextManager.HttpContext.Request.Cookies["migrateCampaignid"]?.Value ?? string.Empty;
            string migrateMemberid = _contextManager.HttpContext.Request.Cookies["migrateMemberid"]?.Value ?? string.Empty;
            string migrateAffiliateid = _contextManager.HttpContext.Request.Cookies["migrateAffiliateid"]?.Value ?? string.Empty;

            // ReSharper disable once PossibleNullReferenceException
            customer.MigrateAffiliateId = migrateAffiliateid;
            customer.MigrateCampaignId = migrateCampaignid;
            customer.MembershipId = HubParametersValidator.ValidateMembershipId(migrateMemberid) ? migrateMemberid : null;

            ApplicationData applicationData = _applicationDataMapper.GetApplicationData(customer);
            int applicationId = await _broadbandSalesRepository.SaveApplication(applicationData);
            customer.ApplicationId = applicationId;

            try
            {
                await _broadbandSalesRepository.InsertMasusReference(applicationId, applicationData.Email, applicationData.UserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occured while inserting masus reference.", ex);
            }

            try
            {
                if (openReachResponse.LineavailabilityFlags.BackOfficeFile)
                {
                    await _broadbandSalesRepository.InsertOpenReachAudit(ApplicationAuditMapper.GetOpenReachAuditData(applicationId, customer, openReachResponse));
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while inserting the openreach audit record for back office file Application Id - {applicationId}. {ex.Message}", ex);
            }

            try
            {
                var products = _sessionManager.GetSessionDetails<List<BroadbandProduct>>(SessionKeys.BroadbandProducts);
                ApplicationAudit auditData = ApplicationAuditMapper.GetApplicationAuditData(applicationId, customer, products, yourPriceViewModel);
                await _broadbandSalesRepository.InsertApplicationAudit(auditData);
                await _broadbandSalesRepository.SetLastUpdated(applicationData.UserID);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while inserting the audit record. {ex.Message}", ex);
            }

            try
            {
                var emailParameters = new BroadbandConfirmationEmailParameters
                {
                    Title = customer.PersonalDetails.Title,
                    LastName = customer.PersonalDetails.LastName.ToCapitalLetter(),
                    SelectedProductTitle = customer.SelectedProduct.BroadbandType.GetTitle(customer.SelectedProduct.GetSelectedTalkProduct(customer.SelectedProductCode).BroadbandProductGroup),
                    EmailAddress = customer.ContactDetails.EmailAddress
                };
                _confirmationEmailService.SendConfirmationEmail(_emailBaseUrl, emailParameters);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured attempting to send confirmation email for broadband customer, email: {customer.ContactDetails.EmailAddress}.", ex);
            }

            try
            {
                if (!string.IsNullOrEmpty(customer.MembershipId))
                {
                    _membershipEmailService.SendMembershipEmail(_membershipEmailAddress,
                        applicationData.CampaignCode,
                        customer.PersonalDetails.FormattedName,
                        $"{customer.SelectedAddress.FormattedAddress} {customer.SelectedAddress.Postcode}",
                        customer.ContactDetails.EmailAddress,
                        customer.ContactDetails.ContactNumber,
                        customer.MembershipId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured attempting to membership email for broadband customer, email: {customer.ContactDetails.EmailAddress}.", ex);
            }

            return DataLayerMapper.GetDataLayerDictionary(customer, yourPriceViewModel, applicationData, applicationId);
        }
    }
}