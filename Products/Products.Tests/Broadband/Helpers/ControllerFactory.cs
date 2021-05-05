namespace Products.Tests.Broadband.Helpers
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Products.Repository.Broadband;
    using Products.Repository.Common;
    using Products.Tests.Broadband.Fakes.Repository;
    using Products.Tests.Broadband.Fakes.Services;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Repository;
    using ServiceWrapper.BankDetailsService;
    using ServiceWrapper.BroadbandProductsService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using StructureMap;
    using Web.DependencyResolution;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    public class ControllerFactory
    {
        private IContainer _container;
        private IConfigManager _fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
        private IContextManager _fakeContextManager = FakeContextManagerFactory.Default();
        private ILogger _fakeLogger = new FakeLogger();
        private IBankDetailsServiceWrapper _fakeBankDetailsService = new FakeBankDetailsService();
        private readonly IDatabaseHelper _fakeDatabaseHelper = new FakeDatabaseHelper();
        private IProfileRepository _fakeProfileRepository = new FakeProfileRepository();
        private IBroadbandProductsServiceWrapper _fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper();
        private IEmailManager _fakeEmailManager = new FakeEmailManager();
        private IBroadbandSalesRepository _fakeBroadbandSalesRepository = new FakeBroadbandSalesRepository();
        private ISessionManager _fakeSessionManager = new FakeSessionManager();
        private INewBTLineAvailabilityServiceWrapper _newBTLineAvailabilityServiceWrapper = new FakeNewBTLineAvailabilityServiceWrapper();
        private ICustomerAlertRepository _fakeCustomerAlertRepository = new FakeCustomerAlertRepository();

        public ControllerFactory WithNewBTLineAvailabilityServiceWrapper(INewBTLineAvailabilityServiceWrapper newBTLineAvailabilityServiceWrapper)
        {
            _newBTLineAvailabilityServiceWrapper = newBTLineAvailabilityServiceWrapper;
            return this;
        }

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

        public ControllerFactory WithBankDetailsServiceWrapper(IBankDetailsServiceWrapper bankDetailsServiceWrapper)
        {
            _fakeBankDetailsService = bankDetailsServiceWrapper;
            return this;
        }

        public ControllerFactory WithProfileRepository(IProfileRepository profileRepository)
        {
            _fakeProfileRepository = profileRepository;
            return this;
        }

        public ControllerFactory WithBroadbandProductsServiceWrapper(IBroadbandProductsServiceWrapper broadbandProductsServiceWrapper)
        {
            _fakeBroadbandProductsServiceWrapper = broadbandProductsServiceWrapper;
            return this;
        }

        public ControllerFactory WithEmailManager(IEmailManager emailManager)
        {
            _fakeEmailManager = emailManager;
            return this;
        }

        public ControllerFactory WithSalesRepository(IBroadbandSalesRepository broadbandSalesRepository)
        {
            _fakeBroadbandSalesRepository = broadbandSalesRepository;
            return this;
        }

        public ControllerFactory WithSessionManager(ISessionManager sessionManager)
        {
            _fakeSessionManager = sessionManager;
            return this;
        }

        public ControllerFactory WithCustomerAlertRepository(ICustomerAlertRepository customerAlertRepository)
        {
            _fakeCustomerAlertRepository = customerAlertRepository;
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
                _container.Configure(c => c.For<IBankDetailsServiceWrapper>().Use(_fakeBankDetailsService));
                _container.Configure(c => c.For<IDatabaseHelper>().Use(_fakeDatabaseHelper));
                _container.Configure(c => c.For<IProfileRepository>().Use(_fakeProfileRepository));
                _container.Configure(c => c.For<IBroadbandProductsServiceWrapper>().Use(_fakeBroadbandProductsServiceWrapper));
                _container.Configure(c => c.For<IEmailManager>().Use(_fakeEmailManager));
                _container.Configure(c => c.For<IBroadbandSalesRepository>().Use(_fakeBroadbandSalesRepository));
                _container.Configure(c => c.For<ISessionManager>().Use(_fakeSessionManager));
                _container.Configure(c => c.For<INewBTLineAvailabilityServiceWrapper>().Use(_newBTLineAvailabilityServiceWrapper));
                _container.Configure(c => c.For<ICustomerAlertRepository>().Use(_fakeCustomerAlertRepository));
            }

            var controller = _container.GetInstance<T>();
            controller.ControllerContext = new ControllerContext(_fakeContextManager.HttpContext, new RouteData(), controller);
            return controller;
        }
    }
}