using Data;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rhino.Mocks.Impl.Invocation.Specifications;
using Services;
using WebSite.Controllers;
using WebSite.Models;
using Xunit;

namespace Tests
{
    public class ProductsTest
    {
        [Fact]
        public void ShouldDisplayAllServicesProductsAndStylistsAvailable()
        {
            //var controller = new HomeController(new ProfileService(new ProfileRepository(new ConfigManager()), new PasswordService()), new ProductsService() );

            //var result = controller.Products();

            //var viewResult = (ViewResult) result;
            //var model = (ProductsViewModel)(viewResult.ViewData.Model);
            //Assert.Equal("Cutting",model.Services[0].Name);
            //Assert.Equal("Colouring", model.Services[1].Name);
            //Assert.Equal("Cut & Style", model.Services[0].Products[0].Name);
            //Assert.Equal("Highlights", model.Services[1].Products[0].Name);
        }
    }
}
