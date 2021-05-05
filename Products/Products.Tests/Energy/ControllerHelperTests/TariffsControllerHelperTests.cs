namespace Products.Tests.Energy.ControllerHelperTests
{
    using System;
    using Broadband.Fakes.Services;
    using Common.Fakes;
    using ControllerHelpers;
    using ControllerHelpers.Energy;
    using Core;
    using Fakes.Services;
    using HomeServices.Fakes;
    using NUnit.Framework;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Service.Broadband.Managers;
    using Service.Common;
    using Service.Common.Managers;
    using Service.Energy;
    using Service.Energy.Mappers;
    using ServiceWrapper.BundleTariffService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.EnergyProjectionService;
    using ServiceWrapper.HomeServicesProductService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using Should;
    using TariffChange.Fakes.Managers;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;

    [TestFixture]
    public class TariffsControllerHelperTests
    {
        private IEnergyProductServiceWrapper _energyProductServiceWrapper;
        private ITariffManager _tariffManager;
        private IBundleTariffServiceWrapper _bundleTariffServiceWrapper;
        private IHomeServicesProductServiceWrapper _homeServicesProductServiceWrapper;
        private ITariffService _tariffService;
        private ILogger _logger;
        private ITariffMapper _tariffMapper;
        private IBroadbandProductsService _broadbandProductsService;
        private INewBTLineAvailabilityServiceWrapper _newBtLineAvailabilityServiceWrapper;
        private IEnergyProjectionServiceWrapper _energyProjectionServiceWrapper;
        private ISessionManager _sessionManager;
        private IConfigManager _configManager;
        private IBroadbandManager _broadbandManager;
        private WebClientData _webClientData;
        private IContentManagementControllerHelper _contentManagementControllerHelper;

        [SetUp]
        public void Initialize()
        {
            _tariffManager = new FakeTariffManager();
            _energyProductServiceWrapper = new FakeProductServiceWrapper();
            _bundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();
            _homeServicesProductServiceWrapper = new FakeHomeServicesProductServiceWrapper();
            _tariffService = new TariffService(_energyProductServiceWrapper, _tariffManager, _bundleTariffServiceWrapper, _homeServicesProductServiceWrapper);
            _logger = new FakeLogger();
            _configManager = new FakeConfigManager();
            _broadbandManager = new BroadbandManager(_configManager, new FakeConfigurationSettings());
            _tariffMapper = new TariffMapper(_tariffManager, _broadbandManager, _configManager);
            _broadbandProductsService = new BroadbandProductsService(new FakeBroadbandProductsServiceWrapper(), _broadbandManager);
            _newBtLineAvailabilityServiceWrapper = new FakeNewBTLineAvailabilityServiceWrapper();
            _energyProjectionServiceWrapper = new FakeEnergyProjectionServiceWrapper();
            _sessionManager = new FakeSessionManager();
            _webClientData = new WebClientData("http://blah");
            _contentManagementControllerHelper = new ContentManagementControllerHelper(new FakeContentManagementAPIClient(), _sessionManager  );
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullTariffServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(null, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("tariffService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullLoggerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, null, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("logger is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullTariffMapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, null, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("tariffMapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullBroadbandProductsServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, null, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("broadbandProductsService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullnewBtLineAvailabilityServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, null, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("newBtLineAvailabilityServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullEnergyProjectionServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, null, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("energyProjectionServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullEnergyProductServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, null, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("energyProductServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullSessionManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper,null, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("sessionManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullConfigManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, null, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("configManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullBroadbandManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, null, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("broadbandManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullTariffManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, null, _webClientData, _contentManagementControllerHelper);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("tariffManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullContentManagementControllerHelperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, null);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("contentManagementControllerHelper is null");
        }

        [Test]
        public void ConstructorShouldCreateSuccessfullyWhenCorrectParametersArePassed_NotNullWebClientData()
        {
            // Arrange/Act
            var controller = new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, _webClientData, _contentManagementControllerHelper);

            // Assert
            controller.ShouldNotBeNull();
        }

        [Test]
        public void ConstructorShouldCreateSuccessfullyWhenCorrectParametersArePassed_NullWebClientData()
        {
            // Arrange/Act
            var controller = new TariffsControllerHelper(_tariffService, _logger, _tariffMapper, _broadbandProductsService, _newBtLineAvailabilityServiceWrapper, _energyProjectionServiceWrapper, _energyProductServiceWrapper, _sessionManager, _configManager, _broadbandManager, _tariffManager, null, _contentManagementControllerHelper);

            // Assert
            controller.ShouldNotBeNull();
        }
    }
}

