using Products.Infrastructure;
using Products.Infrastructure.Logging;
using Products.Model.Constants;
using Products.Service.Common;
using Products.Service.Common.Mappers;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.ViewModels.Broadband;
using Products.WebModel.ViewModels.Common;
using System;
using System.Threading.Tasks;

namespace Products.Service.Broadband
{
    public class OnlineCreationService : IOnlineCreationService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IBroadbandJourneyService _broadbandJourneyService;
        private readonly ILogger _logger;
        private readonly IActivationEmailService _activationEmailService;
        private readonly ICustomerProfileService _profileService;

        public OnlineCreationService(ISessionManager sessionManager, ICustomerProfileService profileService, IBroadbandJourneyService broadbandJourneyService, ILogger logger, IActivationEmailService activationEmailService)
        {
            Guard.Against<ArgumentNullException>(sessionManager == null, "sessionManager is null");
            Guard.Against<ArgumentNullException>(profileService == null, "profileService is null");
            Guard.Against<ArgumentNullException>(broadbandJourneyService == null, "broadbandJourneyService is null");
            Guard.Against<ArgumentNullException>(logger == null, "logger is null");
            Guard.Against<ArgumentNullException>(activationEmailService == null, "activationEmailService is null");

            _sessionManager = sessionManager;
            _profileService = profileService;
            _broadbandJourneyService = broadbandJourneyService;
            _logger = logger;
            _activationEmailService = activationEmailService;
        }

        public OnlineAccountViewModel GetOnlineAccountViewModel()
        {
            return new OnlineAccountViewModel
            {
                YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "ContactDetails",
                    ControllerName = "CustomerDetails",
                    TitleAttributeText = Common_Resources.BackButtonAlt
                }
            };
        }

        public void SaveOnlinePassword(string password)
        {
            var broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            broadbandJourneyDetails.Password = password;
        }

        public OnlineAccountViewModel SetOnlineAccountViewModel(OnlineAccountViewModel model)
        {
            model.YourPriceViewModel = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.YourPriceDetails);

            model.BackChevronViewModel = new BackChevronViewModel
            {
                ActionName = "ContactDetails",
                ControllerName = "CustomerDetails",
                TitleAttributeText = Common_Resources.BackButtonAlt
            };

            return model;
        }

        public async Task CreateUserProfile()
        {
            var broadbandJourneyDetails = _broadbandJourneyService.GetBroadbandJourneyDetails();
            Guard.Against<ArgumentNullException>(broadbandJourneyDetails.Customer == null, "session object is null");

            //check again for concurrency
            if (!await _profileService.DoesProfileExist(broadbandJourneyDetails.Customer.ContactDetails.EmailAddress))
            {
                var userProfile = OnlineAccountMapper.GetUserProfile(broadbandJourneyDetails.Customer, broadbandJourneyDetails.Password);
                var userId = await _profileService.AddOnlineProfile(userProfile);
                broadbandJourneyDetails.Customer.UserId = userId;
            }
        }
    }
}
