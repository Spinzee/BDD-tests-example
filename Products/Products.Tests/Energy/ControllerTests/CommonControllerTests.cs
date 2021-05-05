namespace Products.Tests.Energy.ControllerTests
{
    using System.Web.Mvc;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Constants;
    using Products.Tests.Common.Fakes;
    using Products.Web.Areas.Energy.Controllers;
    using Products.WebModel.ViewModels.Energy;
    using Should;

    public class CommonControllerTests
    {
        [Test]
        public void ShouldReturnYourPriceView()
        {
            // Arrange
            var fakeConfigManager = new FakeConfigManager();
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SetSessionDetails(SessionKeys.EnergyYourPriceDetails, new YourPriceViewModel());
            fakeConfigManager.AddConfiguration("EnergyLinkBackToHubURL", "TestUrl");
            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithSessionManager(fakeSessionManager)
                .Build<CommonController>();

            // Act
            PartialViewResult result = controller.YourPriceDetails();

            // Assert
            result.ShouldNotBeNull()
                .ShouldBeType<PartialViewResult>()
                .Model.ShouldBeType<YourPriceViewModel>();
        }
    }
}
