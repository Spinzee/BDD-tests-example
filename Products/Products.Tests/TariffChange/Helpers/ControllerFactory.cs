namespace Products.Tests.TariffChange.Helpers
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common.Fakes;
    using Common.Helpers;
    using ControllerHelpers;
    using Fakes.Managers;
    using Fakes.Services;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Repository.Common;
    using Repository.TariffChange;
    using Service.Common;
    using Service.Common.Managers;
    using Service.TariffChange;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.ManageCustomerInformationService;
    using ServiceWrapper.PersonalProjectionService;
    using StructureMap;
    using Web.DependencyResolution;

    public class ControllerFactory
    {
        private IContainer _container;
        private IAnnualEnergyReviewServiceWrapper _fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper();
        private IAvailableTariffService _fakeAvailableTariffService = new FakeAvailableTariffService();
        private IConfigManager _fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
        private IContextManager _fakeContextManager = FakeContextManagerFactory.Default();
        private ICustomerAlertRepository _fakeCustomerAlertRepository = new FakeCustomerAlertRepository();
        private IProfileRepository _fakeCustomerProfileRepository = new FakeProfileRepository();
        private IEmailManager _fakeEmailManager = new FakeEmailManager();
        private IGoogleReCaptchaService _fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService();
        private ILogger _fakeLogger = new FakeLogger();
        private IManageCustomerInformationServiceWrapper _fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper();
        private IPersonalProjectionServiceWrapper _fakePersonalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();
        private ITariffChangeSessionService _fakeTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();
        private ITariffManager _fakeTariffManager = new FakeTariffManager();
        private ITariffRepository _fakeTariffRepository = new FakeTariffRepository();
        private IContentManagementControllerHelper _fakeContentManagementControllerHelper;

        public ControllerFactory WithConfigManager(IConfigManager configManager)
        {
            _fakeConfigManager = configManager;
            return this;
        }

        public ControllerFactory WithContextManager(IContextManager contextManager)
        {
            _fakeContextManager = contextManager;
            return this;
        }

        public ControllerFactory WithLogger(ILogger logger)
        {
            _fakeLogger = logger;
            return this;
        }

        public ControllerFactory WithEmailManager(IEmailManager emailManager)
        {
            _fakeEmailManager = emailManager;
            return this;
        }

        public ControllerFactory WithTariffManager(ITariffManager tariffManager)
        {
            _fakeTariffManager = tariffManager;
            return this;
        }

        public ControllerFactory WithTariffChangeSessionService(ITariffChangeSessionService tariffChangeSessionService)
        {
            _fakeTariffChangeSessionService = tariffChangeSessionService;
            return this;
        }

        public ControllerFactory WithAvailableTariffService(IAvailableTariffService availableTariffService)
        {
            _fakeAvailableTariffService = availableTariffService;
            return this;
        }

        public ControllerFactory WithAnnualEnergyReviewServiceWrapper(IAnnualEnergyReviewServiceWrapper annualEnergyReviewServiceWrapper)
        {
            _fakeAnnualEnergyReviewServiceWrapper = annualEnergyReviewServiceWrapper;
            return this;
        }

        public ControllerFactory WithManageCustomerInformationServiceWrapper(IManageCustomerInformationServiceWrapper manageCustomerInformationServiceWrapper)
        {
            _fakeManageCustomerInformationServiceWrapper = manageCustomerInformationServiceWrapper;
            return this;
        }

        public ControllerFactory WithPersonalProjectionServiceWrapper(IPersonalProjectionServiceWrapper personalProjectionServiceWrapper)
        {
            _fakePersonalProjectionServiceWrapper = personalProjectionServiceWrapper;
            return this;
        }

        public ControllerFactory WithCustomerAlertRepository(ICustomerAlertRepository customerAlertRepository)
        {
            _fakeCustomerAlertRepository = customerAlertRepository;
            return this;
        }

        public ControllerFactory WithCustomerProfileRepository(IProfileRepository customerProfileRepository)
        {
            _fakeCustomerProfileRepository = customerProfileRepository;
            return this;
        }

        public ControllerFactory WithTariffRepository(ITariffRepository tariffRepository)
        {
            _fakeTariffRepository = tariffRepository;
            return this;
        }

        public ControllerFactory WithGoogleRecaptchaService(IGoogleReCaptchaService googleReCaptchaService)
        {
            _fakeGoogleReCaptchaService = googleReCaptchaService;
            return this;
        }

        public ControllerFactory WithContentManagementControllerHelper(IContentManagementControllerHelper contentManagementControllerHelper)
        {
            _fakeContentManagementControllerHelper = contentManagementControllerHelper;
            return this;
        }

        public T Build<T>() where T : Controller
        {
            if (_container == null)
            {
                _container = IoC.Initialize(new FakeConfigurationSettings());

                _container.Configure(c => c.For<IConfigManager>().Use(_fakeConfigManager));
                _container.Configure(c => c.For<IContextManager>().Use(_fakeContextManager));
                _container.Configure(c => c.For<ILogger>().Use(_fakeLogger));
                _container.Configure(c => c.For<IEmailManager>().Use(_fakeEmailManager));
                _container.Configure(c => c.For<ITariffManager>().Use(_fakeTariffManager));
                _container.Configure(c => c.For<ITariffChangeSessionService>().Use(_fakeTariffChangeSessionService));
                _container.Configure(c => c.For<IAvailableTariffService>().Use(_fakeAvailableTariffService));
                _container.Configure(c => c.For<IAnnualEnergyReviewServiceWrapper>().Use(_fakeAnnualEnergyReviewServiceWrapper));
                _container.Configure(c => c.For<IManageCustomerInformationServiceWrapper>().Use(_fakeManageCustomerInformationServiceWrapper));
                _container.Configure(c => c.For<IPersonalProjectionServiceWrapper>().Use(_fakePersonalProjectionServiceWrapper));
                _container.Configure(c => c.For<ICustomerAlertRepository>().Use(_fakeCustomerAlertRepository));
                _container.Configure(c => c.For<IProfileRepository>().Use(_fakeCustomerProfileRepository));
                _container.Configure(c => c.For<ITariffRepository>().Use(_fakeTariffRepository));
                _container.Configure(c => c.For<IGoogleReCaptchaService>().Use(_fakeGoogleReCaptchaService));

                if (_fakeContentManagementControllerHelper != null)
                {
                    _container.Configure(c => c.For<IContentManagementControllerHelper>().Use(_fakeContentManagementControllerHelper));
                }
            }

            var controller = _container.GetInstance<T>();
            controller.ControllerContext = new ControllerContext(_fakeContextManager.HttpContext, new RouteData(), controller);
            return controller;
        }
    }
}