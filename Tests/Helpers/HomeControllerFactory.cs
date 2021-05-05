using Data;
using Microsoft.Extensions.DependencyInjection;
using Services;
using WebSite.Controllers;

namespace Tests.Helpers
{
    public class HomeControllerFactory
    {
        private IProfileRepository _profileRepository = new FakeProfileRepository();
        
        public HomeControllerFactory WithProfileRepository(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
            return this;
        }

        public HomeController Build()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IProfileRepository>(_profileRepository);
            serviceCollection.AddSingleton<IProductService, ProductsService>();
            serviceCollection.AddSingleton<IProfileService, ProfileService>();
            serviceCollection.AddSingleton<IPasswordService, PasswordService>();

            var service = serviceCollection.BuildServiceProvider();

            var homeController = ActivatorUtilities.CreateInstance<HomeController>(service);

            return homeController;
        }
    }
}
