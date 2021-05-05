/*
using NUnit.Framework;
using Products.Tests.Broadband.Helpers;
using Products.Web.Areas.Broadband.Controllers;
using Should;
using System.Web.Mvc;
using FakeSessionManager = Products.Tests.Broadband.Fakes.Services.FakeSessionManager;

namespace Products.Tests.Broadband
{
    [TestFixture]
    public class ExistingCustomerTests
    {
        //[Ignore("")]
        //[Test]

        public void CustomersShouldBeRedirectedToLineCheckerPageWhenTheyUseOldExistingCustomerLink()
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<ExistingCustomerController>();

            // Act
            var resultExistingCustomer = controller.ExistingCustomer();

            // Assert
            resultExistingCustomer.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult)resultExistingCustomer).RouteValues["action"].ShouldEqual("LineChecker");
        }

    }
}
*/
