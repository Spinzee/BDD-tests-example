namespace Products.Tests.Energy.ControllerHelperTests
{
    using System;
    using Common.Fakes;
    using Common.Fakes.Services;
    using ControllerHelpers.Energy;
    using Fakes.Services;
    using NUnit.Framework;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Service.Common;
    using Service.Energy;
    using ServiceWrapper.CAndCService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.QASService;
    using Should;

    [TestFixture]
    public class QuoteControllerHelperTests
    {
        private IPostcodeCheckerService _postcodeCheckerService;
        private IQASServiceWrapper _qasServiceWrapper;
        private ILogger _logger;
        private ICAndCServiceWrapper _cAndCServiceWrapper;
        private IEnergyAlertService _energyAlertService;
        private IEnergyProductServiceWrapper _energyProductServiceWrapper;
        private ISessionManager _sessionManager;
        private IConfigManager _configManager;

        [SetUp]
        public void Initialize()
        {
            _postcodeCheckerService = new PostcodeCheckerService();
            _qasServiceWrapper = new FakeQASServiceWrapper();
            _logger = new FakeLogger();
            _cAndCServiceWrapper = new FakeCAndCServiceWrapper();
            _configManager = new FakeConfigManager();
            _energyAlertService = new EnergyAlertService(new CustomerAlertService(new FakeCustomerAlertRepository()), _configManager);
            _energyProductServiceWrapper = new FakeProductServiceWrapper();
            _sessionManager = new FakeSessionManager();
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullPostcodeCheckerServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(null, _qasServiceWrapper, _logger, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("postcodeCheckerService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullQASServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, null, _logger, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("qasServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullLoggerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, null, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("logger is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullCAndCServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, null, _energyAlertService, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("cAndCServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullEnergyAlertServiceIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, _cAndCServiceWrapper, null, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("energyAlertService is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullEnergyProductServiceWrapperIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, _cAndCServiceWrapper, _energyAlertService, null, _sessionManager, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("energyProductServiceWrapper is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullSessionManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, null, _configManager);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("sessionManager is null");
        }

        [Test]
        public void ConstructorShouldThrowArgumentExceptionWhenNullConfigManagerIsPassed()
        {
            // Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, _sessionManager, null);

            // Act/Assert
            act.ShouldThrowArgumentExceptionWithMessage("configManager is null");
        }

        [Test]
        public void ConstructorShouldCreateSuccessfullyWhenCorrectParametersArePassed()
        {
            // Arrange/Act
            var controller = new QuoteControllerHelper(_postcodeCheckerService, _qasServiceWrapper, _logger, _cAndCServiceWrapper, _energyAlertService, _energyProductServiceWrapper, _sessionManager, _configManager);

            // Assert
            controller.ShouldNotBeNull();
        }
    }
}