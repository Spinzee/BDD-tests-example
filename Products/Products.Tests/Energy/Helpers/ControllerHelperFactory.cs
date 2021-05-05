namespace Products.Tests.Energy.Helpers
{
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
    using ServiceWrapper.NewBTLineAvailabilityService;
    using ServiceWrapper.QASService;
    using StructureMap;
    using TariffChange.Fakes.Managers;
    using Web.DependencyResolution;
    using FakeSessionManager = Common.Fakes.FakeSessionManager;

    public class ControllerHelperFactory
    {
        private IContainer _container;
        private readonly IBankDetailsServiceWrapper _fakeBankDetailsService = new FakeBankDetailsService();
        private readonly IBundleTariffServiceWrapper _fakeBundleTariffServiceWrapper = new FakeBundleTariffServiceWrapper();
        private readonly ICAndCServiceWrapper _fakeCAndCServiceWrapper = new FakeCAndCServiceWrapper();
        private IConfigManager _fakeConfigManager = new ConfigManager();
        private readonly IContextManager _fakeContextManager = FakeContextManagerFactory.Default();
        private readonly ICustomerAlertRepository _fakeCustomerAlertRepository = new FakeCustomerAlertRepository();

        private readonly IProfileRepository _fakeCustomerProfileRepository = new FakeProfileRepository();
        private readonly IEmailManager _fakeEmailManager = new FakeEmailManager();
        private readonly IEnergyProductServiceWrapper _fakeEnergyProductServiceWrapper = new FakeProductServiceWrapper();
        private readonly IEnergySalesRepository _fakeEnergySalesRepository = new FakeEnergySalesRepository();
        private readonly ILogger _fakeLogger = new FakeLogger();

        private readonly IEnergyProjectionServiceWrapper _fakeEnergyProjectionServiceWrapper = new FakeEnergyProjectionServiceWrapper();

        private readonly IQASServiceWrapper _fakeQASServiceWrapper = new FakeQASServiceWrapper();
        private readonly ISalesRepository _fakeSalesRepository = new FakeSalesRepository();
        private ISessionManager _fakeSessionManager = new FakeSessionManager();
        private readonly ITariffManager _fakeTariffManager = new FakeTariffManager();
        private readonly INewBTLineAvailabilityServiceWrapper _newBTLineAvailabilityServiceWrapper = new FakeNewBTLineAvailabilityServiceWrapper();
        private readonly IBroadbandProductsServiceWrapper _fakeBroadbandProductsServiceWrapper = new FakeBroadbandProductsServiceWrapper();
        private readonly IBroadbandSalesRepository _fakeBroadbandSalesRepository = new FakeBroadbandSalesRepository();
        private readonly IHomeServicesSalesRepository _fakeHomeServicesSalesRepository = new FakeHomeServicesSalesRepository();

        private IContentManagementAPIClient _fakeContentManagementAPIClient = new FakeContentManagementAPIClient();

        public ControllerHelperFactory WithSessionManager(ISessionManager sessionManager)
        {
            _fakeSessionManager = sessionManager;
            return this;
        }

        /*

        public ControllerHelperFactory WithContextManager(IContextManager contextManager)
        {
            _fakeContextManager = contextManager;
            return this;
        }

        public ControllerHelperFactory WithTariffManager(ITariffManager fakeTariffManager)
        {
            _fakeTariffManager = fakeTariffManager;
            return this;
        }

        public ControllerHelperFactory WithEmailManager(IEmailManager fakeEmailManager)
        {
            _fakeEmailManager = fakeEmailManager;
            return this;
        }

        public ControllerHelperFactory WithCustomerProfileRepository(IProfileRepository customerProfileRepository)
        {
            _fakeCustomerProfileRepository = customerProfileRepository;
            return this;
        }

        public ControllerHelperFactory WithBankDetailsServiceWrapper(IBankDetailsServiceWrapper fakeBankDetailsService)
        {
            _fakeBankDetailsService = fakeBankDetailsService;
            return this;
        }

        public ControllerHelperFactory WithEnergyProjectionServiceWrapper(
            IEnergyProjectionServiceWrapper fakEnergyProjectionServiceWrapper)
        {
            _fakEnergyProjectionServiceWrapper = fakEnergyProjectionServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithQASServiceWrapper(IQASServiceWrapper fakeQASServiceWrapper)
        {
            _fakeQASServiceWrapper = fakeQASServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithEnergyProductServiceWrapper(
            IEnergyProductServiceWrapper fakeEnergyProductServiceWrapper)
        {
            _fakeEnergyProductServiceWrapper = fakeEnergyProductServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithBundleTariffServiceWrapper(IBundleTariffServiceWrapper fakeBundleTariffServiceWrapper)
        {
            _fakeBundleTariffServiceWrapper = fakeBundleTariffServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithFakeEnergySalesRepository(IEnergySalesRepository fakeEnergySalesRepository)
        {
            _fakeEnergySalesRepository = fakeEnergySalesRepository;
            return this;
        }

        public ControllerHelperFactory WithFakeSalesRepository(ISalesRepository fakeSalesRepository)
        {
            _fakeSalesRepository = fakeSalesRepository;
            return this;
        }

        public ControllerHelperFactory WithCustomerAlertRepository(ICustomerAlertRepository customerAlertRepository)
        {
            _fakeCustomerAlertRepository = customerAlertRepository;
            return this;
        }

        public ControllerHelperFactory WithCAndCServiceWrapper(ICAndCServiceWrapper cAndCServiceWrapper)
        {
            _fakeCAndCServiceWrapper = cAndCServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithBroadbandProductsServiceWrapper(IBroadbandProductsServiceWrapper broadbandProductsServiceWrapper)
        {
            _fakeBroadbandProductsServiceWrapper = broadbandProductsServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithNewBTLineAvailabilityServiceWrapper(INewBTLineAvailabilityServiceWrapper newBTLineAvailabilityServiceWrapper)
        {
            _newBTLineAvailabilityServiceWrapper = newBTLineAvailabilityServiceWrapper;
            return this;
        }

        public ControllerHelperFactory WithBroadbandSalesRepository(IBroadbandSalesRepository broadbandSalesRepository)
        {
            _fakeBroadbandSalesRepository = broadbandSalesRepository;
            return this;
        }
        public ControllerHelperFactory WithHomeServicesSalesRepository(IHomeServicesSalesRepository homeServicesSalesRepository)
        {
            _fakeHomeServicesSalesRepository = homeServicesSalesRepository;
            return this;
        }

        */

        public ControllerHelperFactory WithConfigManager(IConfigManager fakeConfigManager)
        {
            _fakeConfigManager = fakeConfigManager;
            return this;
        }

        public ControllerHelperFactory WithContentManagementAPIClient(IContentManagementAPIClient contentManagementAPIClient)
        {
            _fakeContentManagementAPIClient = contentManagementAPIClient;
            return this;
        }

        public T Build<T>() where T : BaseControllerHelper
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
                _container.Configure(c => c.For<IEnergyProjectionServiceWrapper>().Use(_fakeEnergyProjectionServiceWrapper));
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
                _container.Configure(c => c.For<IContentManagementAPIClient>().Use(_fakeContentManagementAPIClient));
                _container.Configure(c => c.For<IHomeServicesSalesRepository>().Use(_fakeHomeServicesSalesRepository));
            }

            return  _container.GetInstance<T>();
        }
    }
}