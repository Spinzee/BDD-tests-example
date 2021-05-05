namespace Products.Tests.Energy.Helpers
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Broadband.Fakes.Repository;
    using Broadband.Fakes.Services;
    using Common.Fakes;
    using Common.Fakes.Services;
    using Common.Helpers;
    using ControllerHelpers;
    using Fakes.Repositories;
    using Fakes.Services;
    using HomeServices.Fakes;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Repository.Broadband;
    using Repository.Common;
    using Repository.Energy;
    using Repository.HomeServices;
    using Service.Common.Managers;
    using ServiceWrapper.BankDetailsService;
    using ServiceWrapper.BroadbandProductsService;
    using ServiceWrapper.BundleTariffService;
    using ServiceWrapper.CAndCService;
    using ServiceWrapper.ContentManagementService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.EnergyProjectionService;
    using ServiceWrapper.HomeServicesProductService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using ServiceWrapper.QASService;
    using StructureMap;
    using TariffChange.Fakes.Managers;
    using Web.DependencyResolution;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;

    public class ControllerFactory
    {
        private readonly ILogger _fakeLogger = new FakeLogger();
        private IContainer _container;
        private IBankDetailsServiceWrapper _fakeBankDetailsService = new FakeBankDetailsService();
        private IBroadbandProductsServiceWrapper _fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper();
        private IBroadbandSalesRepository _fakeBroadbandSalesRepository = new FakeBroadbandSalesRepository();
        private IBundleTariffServiceWrapper _fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();
        private ICAndCServiceWrapper _fakeCAndCServiceWrapper = new FakeCAndCServiceWrapper();
        private IConfigManager _fakeConfigManager = new ConfigManager();
        private IContextManager _fakeContextManager = FakeContextManagerFactory.Default();
        private ICustomerAlertRepository _fakeCustomerAlertRepository = new FakeCustomerAlertRepository();        
        private readonly IHomeServicesProductServiceWrapper _fakeHomeServicesProductServiceWrapper = new FakeHomeServicesProductServiceWrapper();

        private IProfileRepository _fakeCustomerProfileRepository = new FakeProfileRepository();
        private IEmailManager _fakeEmailManager = new FakeEmailManager();
        private IEnergyProductServiceWrapper _fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
        private IEnergySalesRepository _fakeEnergySalesRepository = new FakeEnergySalesRepository();
        private IHomeServicesSalesRepository _fakeHomeServicesSalesRepository = new FakeHomeServicesSalesRepository();

        private IEnergyProjectionServiceWrapper _fakEnergyProjectionServiceWrapper = new FakeEnergyProjectionServiceWrapper();

        private IQASServiceWrapper _fakeQASServiceWrapper = new FakeQASServiceWrapper();
        private ISalesRepository _fakeSalesRepository = new FakeSalesRepository();
        private ISessionManager _fakeSessionManager = new FakeSessionManager();
        private ITariffManager _fakeTariffManager = new FakeTariffManager();
        private INewBTLineAvailabilityServiceWrapper _newBTLineAvailabilityServiceWrapper = new FakeNewBTLineAvailabilityServiceWrapper();

        private readonly IContentManagementAPIClient _fakeContentManagementAPIClient = new FakeContentManagementAPIClient();
        private IContentManagementControllerHelper _fakeContentManagementControllerHelper;

        public ControllerFactory WithContextManager(IContextManager contextManager)
        {
            _fakeContextManager = contextManager;
            return this;
        }

        public ControllerFactory WithTariffManager(ITariffManager fakeTariffManager)
        {
            _fakeTariffManager = fakeTariffManager;
            return this;
        }

        public ControllerFactory WithEmailManager(IEmailManager fakeEmailManager)
        {
            _fakeEmailManager = fakeEmailManager;
            return this;
        }

        public ControllerFactory WithSessionManager(ISessionManager sessionManager)
        {
            _fakeSessionManager = sessionManager;
            return this;
        }

        public ControllerFactory WithCustomerProfileRepository(IProfileRepository customerProfileRepository)
        {
            _fakeCustomerProfileRepository = customerProfileRepository;
            return this;
        }

        public ControllerFactory WithBankDetailsServiceWrapper(IBankDetailsServiceWrapper fakeBankDetailsService)
        {
            _fakeBankDetailsService = fakeBankDetailsService;
            return this;
        }

        public ControllerFactory WithEnergyProjectionServiceWrapper(
            IEnergyProjectionServiceWrapper fakEnergyProjectionServiceWrapper)
        {
            _fakEnergyProjectionServiceWrapper = fakEnergyProjectionServiceWrapper;
            return this;
        }

        public ControllerFactory WithQASServiceWrapper(IQASServiceWrapper fakeQASServiceWrapper)
        {
            _fakeQASServiceWrapper = fakeQASServiceWrapper;
            return this;
        }

        public ControllerFactory WithEnergyProductServiceWrapper(
            IEnergyProductServiceWrapper fakeEnergyProductServiceWrapper)
        {
            _fakeEnergyProductServiceWrapper = fakeEnergyProductServiceWrapper;
            return this;
        }

        public ControllerFactory WithBundleTariffServiceWrapper(IBundleTariffServiceWrapper fakeBundleTariffServiceWrapper)
        {
            _fakeBundleTariffServiceWrapper = fakeBundleTariffServiceWrapper;
            return this;
        }

        public ControllerFactory WithFakeEnergySalesRepository(IEnergySalesRepository fakeEnergySalesRepository)
        {
            _fakeEnergySalesRepository = fakeEnergySalesRepository;
            return this;
        }

        public ControllerFactory WithFakeSalesRepository(ISalesRepository fakeSalesRepository)
        {
            _fakeSalesRepository = fakeSalesRepository;
            return this;
        }

        public ControllerFactory WithConfigManager(IConfigManager fakeConfigManager)
        {
            _fakeConfigManager = fakeConfigManager;
            return this;
        }

        public ControllerFactory WithCustomerAlertRepository(ICustomerAlertRepository customerAlertRepository)
        {
            _fakeCustomerAlertRepository = customerAlertRepository;
            return this;
        }

        public ControllerFactory WithCAndCServiceWrapper(ICAndCServiceWrapper cAndCServiceWrapper)
        {
            _fakeCAndCServiceWrapper = cAndCServiceWrapper;
            return this;
        }

        public ControllerFactory WithBroadbandProductsServiceWrapper(IBroadbandProductsServiceWrapper broadbandProductsServiceWrapper)
        {
            _fakeBroadbandProductsServiceWrapper = broadbandProductsServiceWrapper;
            return this;
        }

        public ControllerFactory WithNewBTLineAvailabilityServiceWrapper(INewBTLineAvailabilityServiceWrapper newBTLineAvailabilityServiceWrapper)
        {
            _newBTLineAvailabilityServiceWrapper = newBTLineAvailabilityServiceWrapper;
            return this;
        }

        public ControllerFactory WithBroadbandSalesRepository(IBroadbandSalesRepository broadbandSalesRepository)
        {
            _fakeBroadbandSalesRepository = broadbandSalesRepository;
            return this;
        }

        public ControllerFactory WithHomeServicesSalesRepository(IHomeServicesSalesRepository homeServicesSalesRepository)
        {
            _fakeHomeServicesSalesRepository = homeServicesSalesRepository;
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
                _container.Configure(c => c.For<ITariffManager>().Use(_fakeTariffManager));
                _container.Configure(c => c.For<ILogger>().Use(_fakeLogger));
                _container.Configure(c => c.For<ISessionManager>().Use(_fakeSessionManager));
                _container.Configure(c => c.For<IProfileRepository>().Use(_fakeCustomerProfileRepository));
                _container.Configure(c => c.For<IBankDetailsServiceWrapper>().Use(_fakeBankDetailsService));
                _container.Configure(c => c.For<IEnergyProjectionServiceWrapper>().Use(_fakEnergyProjectionServiceWrapper));
                _container.Configure(c => c.For<IQASServiceWrapper>().Use(_fakeQASServiceWrapper));
                _container.Configure(c => c.For<IEnergyProductServiceWrapper>().Use(_fakeEnergyProductServiceWrapper));
                _container.Configure(c => c.For<IEmailManager>().Use(_fakeEmailManager));
                _container.Configure(c => c.For<IEnergySalesRepository>().Use(_fakeEnergySalesRepository));
                _container.Configure(c => c.For<ISalesRepository>().Use(_fakeSalesRepository));
                _container.Configure(c => c.For<ICustomerAlertRepository>().Use(_fakeCustomerAlertRepository));
                _container.Configure(c => c.For<ICAndCServiceWrapper>().Use(_fakeCAndCServiceWrapper));
                _container.Configure(c => c.For<IBundleTariffServiceWrapper>().Use(_fakeBundleTariffServiceWrapper));
                _container.Configure(c => c.For<INewBTLineAvailabilityServiceWrapper>().Use(_newBTLineAvailabilityServiceWrapper));
                _container.Configure(c => c.For<IBroadbandProductsServiceWrapper>().Use(_fakeBroadbandProductsServiceWrapper));
                _container.Configure(c => c.For<IBroadbandSalesRepository>().Use(_fakeBroadbandSalesRepository));
                _container.Configure(c => c.For<IHomeServicesProductServiceWrapper>().Use(_fakeHomeServicesProductServiceWrapper));
                _container.Configure(c => c.For<IHomeServicesSalesRepository>().Use(_fakeHomeServicesSalesRepository));
                _container.Configure(c => c.For<IContentManagementAPIClient>().Use(_fakeContentManagementAPIClient));

                if (_fakeContentManagementControllerHelper != null)
                {
                    _container.Configure(c => c.For<IContentManagementControllerHelper>().Use(_fakeContentManagementControllerHelper));
                }
            }

            var controller = _container.GetInstance<T>();
            controller.ControllerContext = new ControllerContext(_fakeContextManager.HttpContext, new RouteData(), controller);

            var routes = new RouteCollection();
            routes.MapRoute("quote-details", "{controller}/{action}/{id}", new
            {
                controller = "Home",
                action = "Index",
                id = UrlParameter.Optional
            });

            controller.Url = new UrlHelper(new RequestContext(controller.HttpContext, new RouteData()), routes);

            return controller;
        }
    }
}