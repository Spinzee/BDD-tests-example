using Products.Infrastructure;
using Products.Infrastructure.Logging;
using Products.Repository.Common;
using Products.Repository.HomeServices;
using Products.ServiceWrapper.BankDetailsService;
using Products.ServiceWrapper.HomeServicesProductService;
using Products.ServiceWrapper.QASService;
using Products.Tests.Common.Fakes;
using Products.Tests.Common.Fakes.Services;
using Products.Tests.Common.Helpers;
using Products.Tests.HomeServices.Fakes;
using Products.Web.DependencyResolution;
using StructureMap;
using System.Web.Mvc;
using System.Web.Routing;
using FakeSessionManager = Products.Tests.Common.Fakes.FakeSessionManager;

namespace Products.Tests.HomeServices.Helpers
{
    public class ControllerFactory
    {
        private IContainer _container;
        private IContextManager _fakeContextManager = FakeContextManagerFactory.Default();
        private ILogger _fakeLogger = new FakeLogger();
        private ISessionManager _fakeSessionManager = new FakeSessionManager();
        private IQASServiceWrapper _fakeQASServiceWrapper = new FakeQASServiceWrapper();
        private IBankDetailsServiceWrapper _fakeBankDetailsService = new FakeBankDetailsService();
        private IHomeServicesProductServiceWrapper _fakeHomeServicesProductServiceWrapper = new FakeHomeServicesProductServiceWrapper();
        private IConfigManager _fakeConfigManager = new ConfigManager();
        private IHomeServicesSalesRepository _fakHomeServicesSalesRepository = new FakeHomeServicesSalesRepository();
        private IEmailManager _fakeEmailManager = new FakeEmailManager();
        private ICustomerAlertRepository _fakeCustomerAlertRepository = new FakeCustomerAlertRepository();


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

        public ControllerFactory WithSessionManager(ISessionManager sessionManager)
        {
            _fakeSessionManager = sessionManager;
            return this;
        }

        public ControllerFactory WithQASServiceWrapper(IQASServiceWrapper fakeQASServiceWrapper)
        {
            _fakeQASServiceWrapper = fakeQASServiceWrapper;
            return this;
        }

        public ControllerFactory WithBankDetailsServiceWrapper(IBankDetailsServiceWrapper fakeBankDetailsService)
        {
            _fakeBankDetailsService = fakeBankDetailsService;
            return this;
        }

        public ControllerFactory WithHomeServicesProductServiceWrapper(IHomeServicesProductServiceWrapper fakeHomeServicesProductServiceWrapper)
        {
            _fakeHomeServicesProductServiceWrapper = fakeHomeServicesProductServiceWrapper;
            return this;
        }

        public ControllerFactory WithConfigManager(IConfigManager fakeConfigManager)
        {
            _fakeConfigManager = fakeConfigManager;
            return this;
        }

        public ControllerFactory WithFakeSalesRepository(IHomeServicesSalesRepository fakeHomeServicesSalesRepository)
        {
            _fakHomeServicesSalesRepository = fakeHomeServicesSalesRepository;
            return this;
        }

        public ControllerFactory WithEmailManager(IEmailManager fakeEmailManager)
        {
            _fakeEmailManager = fakeEmailManager;
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
                _container.Configure(c => c.For<ISessionManager>().Use(_fakeSessionManager));
                _container.Configure(c => c.For<IQASServiceWrapper>().Use(_fakeQASServiceWrapper));
                _container.Configure(c => c.For<IBankDetailsServiceWrapper>().Use(_fakeBankDetailsService));
                _container.Configure(c => c.For<IHomeServicesProductServiceWrapper>().Use(_fakeHomeServicesProductServiceWrapper));
                _container.Configure(c => c.For<IHomeServicesSalesRepository>().Use(_fakHomeServicesSalesRepository));
                _container.Configure(c => c.For<IEmailManager>().Use(_fakeEmailManager));
                _container.Configure(c => c.For<ICustomerAlertRepository>().Use(_fakeCustomerAlertRepository));
            }

            var controller = _container.GetInstance<T>();
            controller.ControllerContext = new ControllerContext(_fakeContextManager.HttpContext, new RouteData(), controller);
            return controller;
        }
    }
}
