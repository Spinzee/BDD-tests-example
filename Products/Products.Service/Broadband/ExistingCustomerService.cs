namespace Products.Service.Broadband
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Infrastructure;
    using Managers;
    using Model.Broadband;
    using Validators;
    using WebModel.Resources.Broadband;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;

    public class ExistingCustomerService : IExistingCustomerService
    {
        private readonly BroadbandJourneyService _broadbandJourneyService;
        private readonly IConfigManager _configManager;
        private readonly ICustomerAlertService _customerAlertService;
        private readonly IBroadbandManager _manager;

        public ExistingCustomerService(BroadbandJourneyService broadbandJourneyService,
            ICustomerAlertService customerAlertService,
            IConfigManager configManager,
            IBroadbandManager manager)
        {
            Guard.Against<ArgumentException>(broadbandJourneyService == null, $"{nameof(broadbandJourneyService)} is null");
            Guard.Against<ArgumentException>(customerAlertService == null, $"{nameof(customerAlertService)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(manager == null, $"{nameof(manager)} is null");

            _broadbandJourneyService = broadbandJourneyService;
            _customerAlertService = customerAlertService;
            _configManager = configManager;
            _manager = manager;
        }

        public void SetInformationPassedByHub(string productCode, string migrateAffiliateId, string migrateCampaignId, string membershipId)
        {
            var broadbandJourneyDetails = new BroadbandJourneyDetails
            {
                Customer =
                {
                    SelectedProductCode = productCode,
                    MigrateAffiliateId = migrateAffiliateId,
                    MigrateCampaignId = migrateCampaignId,
                    MembershipId = HubParametersValidator.ValidateMembershipId(membershipId) ? membershipId : null,
                    SelectedProductGroup = _manager.BroadbandProductGroup(productCode)
                }
            };

            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }

        public async Task<bool> IsCustomerAlert()
        {
            return await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting("BroadbandCustomerAlertName"));
        }

        public ExistingCustomerViewModel GetExistingCustomerViewModel()
        {
            return new ExistingCustomerViewModel
            {
                // TODO fix the link ... currently points to self
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = null,
                    ControllerName = null,
                    TitleAttributeText = Common_Resources.BackButtonAlt
                }
            };
        }

        public void SetExistingCustomer(bool isExistingCustomer)
        {
            BroadbandJourneyDetails broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentNullException>(broadbandJourneyDetails.Customer == null, $"{nameof(broadbandJourneyDetails.Customer)} is null");
            // ReSharper disable once PossibleNullReferenceException
            broadbandJourneyDetails.Customer.IsSSECustomer = isExistingCustomer;
            _broadbandJourneyService.SetBroadbandJourneyDetails(broadbandJourneyDetails);
        }
    }
}