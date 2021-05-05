namespace Products.Tests.Energy.ControllerHelperTests
{
    using System;
    using Broadband.Fakes.Repository;
    using Broadband.Fakes.Services;
    using Common.Fakes;
    using ControllerHelpers;
    using ControllerHelpers.Energy;
    using Core;
    using Fakes.Repositories;
    using HomeServices.Fakes;
    using NUnit.Framework;
    using Products.Infrastructure;
    using Products.Infrastructure.GuidEncryption;
    using Products.Infrastructure.Logging;
    using Repository;
    using Service.Broadband.Managers;
    using Service.Common;
    using Service.Common.Managers;
    using Service.Energy;
    using Service.Energy.Mappers;
    using Service.Security;
    using Should;
    using TariffChange.Fakes.Managers;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;

    [TestFixture]
    public class SignUpControllerHelperTests
    {
        private ICustomerProfileService _customerProfileService;
        private IBankValidationService _bankValidationService;
        private ISummaryService _summaryService;
        private ISessionManager _sessionManager;
        private ITariffManager _tariffManager;
        private IConfigManager _configManager;
        private ITariffMapper _tariffMapper;
        private IBroadbandProductsService _broadbandProductsService;
        private IPostcodeCheckerService _postcodeCheckerService;
        private WebClientData _webClientData;
        private IBroadbandManager _broadbandManager;
        private IContentManagementControllerHelper _contentManagementControllerHelper;

        [SetUp]
        public void Initialize()
        {
            _tariffManager = new FakeTariffManager();
            _configManager = new FakeConfigManager();
            _broadbandManager = new BroadbandManager(_configManager, new FakeConfigurationSettings());
            IContextManager contextManager = new FakeContextManager(new FakeHttpContext(new FakeHttpRequest("http:blah", "method"), new FakeHttpServerUtility(), new FakeHttpSession()));
            ICampaignManager campaignManager = new CampaignManager(_configManager);
            IUtilityService utilityService = new UtilityService(contextManager);
            IActivationEmailService activationEmailService = new ActivationEmailService(new FakeEmailManager(), new ContentRepository(), new GuidEncrypter(), _configManager, utilityService);
            ILogger logger = new FakeLogger();
            _customerProfileService = new CustomerProfileService(activationEmailService, new FakeProfileRepository(), logger, new PasswordService());
            _bankValidationService = new BankValidationService(new FakeBankDetailsService());
            IConfirmationEmailService confirmationEmailService = new ConfirmationEmailService(new ContentRepository(), new FakeEmailManager(), _configManager);
            ICryptographyService cryptographyService = new CryptographyService(_configManager);
            IEnergyApplicationDataMapper energyApplicationDataMapper = new EnergyApplicationDataMapper(cryptographyService);
            IMembershipEmailService membershipEmailService = new MembershipEmailService(_configManager, new FakeEmailManager(), new ContentRepository());
            _summaryService = new SummaryService(confirmationEmailService, logger, new FakeEnergySalesRepository(), new FakeSalesRepository(), energyApplicationDataMapper, contextManager, campaignManager, new FakeBroadbandSalesRepository(), _broadbandManager, membershipEmailService, _configManager, new FakeHomeServicesSalesRepository(), cryptographyService);
            _tariffMapper = new TariffMapper(_tariffManager, _broadbandManager, _configManager);
            _broadbandProductsService = new BroadbandProductsService(new FakeBroadbandProductsServiceWrapper(), _broadbandManager);
            _postcodeCheckerService = new PostcodeCheckerService();
            _sessionManager = new FakeSessionManager();
            _webClientData = new WebClientData("http://blah");
            _contentManagementControllerHelper = new ContentManagementControllerHelper(new FakeContentManagementAPIClient(), _sessionManager  );
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullCustomerProfileServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(null, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("customerProfileService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullBankValidationServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, null, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("bankValidationService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullSummaryServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, null, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("summaryService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullSessionManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, null, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("sessionManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullTariffManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, null, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("tariffManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullConfigManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, null, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("configManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullTariffMapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, null, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("tariffMapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullBroadbandProductsServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, null, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("broadbandProductsService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullPostcodeCheckerServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, null, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("postcodeCheckerService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullBroadbandManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, null, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("broadbandManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullContentManagementControllerHelperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, null);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("contentManagementControllerHelper is null");
        }

        [Test]
        public void ConstructorShouldCreateSuccessfullyWhenCorrectParametersArePassed_NotNullWebClientData()
        {
            // Arrange/Act
            var controller = new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, _webClientData, _broadbandManager, _contentManagementControllerHelper);

            // Assert
            controller.ShouldNotBeNull();
        }

        [Test]
        public void ConstructorShouldCreateSuccessfullyWhenCorrectParametersArePassed_NullWebClientData()
        {
            // Arrange/Act
            var controller = new SignUpControllerHelper(_customerProfileService, _bankValidationService, _summaryService, _sessionManager, _tariffManager, _configManager, _tariffMapper, _broadbandProductsService, _postcodeCheckerService, null, _broadbandManager, _contentManagementControllerHelper);

            // Assert
            controller.ShouldNotBeNull();
        }
    }
}