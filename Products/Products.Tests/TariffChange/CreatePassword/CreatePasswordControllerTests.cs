namespace Products.Tests.TariffChange.CreatePassword
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Products.Model.TariffChange.Tariffs;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class CreatePasswordControllerTests
    {       
        [Test]
        public void AddOnlineProfileShouldAddTheProfileAndRedirectToTariffSummary()
        {
            // Arrange
            var viewModel = new CreatePasswordViewModel
            {
                Password = "P455w0rd#12345"
            };

            const string email = "existingprofile@sse.com";
            var customer = new Customer { EmailAddress = email };
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakeCustomerProfileRepository = new FakeProfileRepository();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<CreatePasswordController>();

            // Act
            ActionResult result = controller.CreateProfile(viewModel);

            // Assert
            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("TariffSummary");
            customer.Password.ShouldEqual("P455w0rd#12345");            
        }

        [Test]
        public void DirectAccessToCreatePasswordShouldRedirectToLandingPageWhenCustomerNull()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<CreatePasswordController>();

            ActionResult result = controller.CreatePassword();

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessToCreatePasswordShouldRedirectToLandingPageWhenEmailNull()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<CreatePasswordController>();

            ActionResult result = controller.CreatePassword();

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessToShowSummaryShouldRedirectToLandingPageWhenCustomerNull()
        {
            var viewModel = new CreatePasswordViewModel
            {
                Password = "password"
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails());

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<CreatePasswordController>();

            ActionResult result = controller.CreateProfile(viewModel);

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessToShowSummaryShouldRedirectToLandingPageWhenEmailNull()
        {
            var viewModel = new CreatePasswordViewModel
            {
                Password = "password"
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<CreatePasswordController>();

            ActionResult result = controller.CreateProfile(viewModel);

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [TestCase("password", "password", 0, "TariffSummary", "Valid email address, Go to Tariff Summary")]
        [TestCase(null, "password", 2, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("password", null, 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase(null, null, 2, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("", "password", 2, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("password", "", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [Test]
        public void ShouldRedirectToTariffSummaryPageIfEmailAddressesAreTheSame(string password, string confirmPassword, int numberOfInlineErrors, string page, string description)
        {
            var model = new CreatePasswordViewModel
            {
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var fakeCustomerProfileRepository = new FakeProfileRepository();

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer
                {
                    EmailAddress = "test@test.com"
                }
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<CreatePasswordController>();

            controller.ValidateViewModel(model);

            ActionResult result = controller.CreateProfile(model);

            result.ShouldNotBeNull();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
            TestHelper.GetResultUrlString(result).ShouldEqual(page);
        }

        [TestCase("password", 0, "TariffSummary", "Valid email address, Go to Tariff Summary")]
        [TestCase("password#~@", 0, "TariffSummary", "Valid email address, Go to Tariff Summary")]
        [TestCase(null, 2, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("", 2, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("123456", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("greaterthanfourteen", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("password%", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("password!", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        [TestCase("password_1", 1, "CreatePassword", "Invalid email address, stay on get create password page")]
        public void ShouldRedirectToTariffSummaryIfPasswordIsValid(string password, int numberOfInlineErrors, string page, string description)
        {
            var model = new CreatePasswordViewModel
            {
                Password = password,
                ConfirmPassword = password
            };

            var fakeCustomerProfileRepository = new FakeProfileRepository();

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer
                {
                    EmailAddress = "test@test.com"
                }
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<CreatePasswordController>();

            controller.ValidateViewModel(model);

            ActionResult result = controller.CreateProfile(model);

            result.ShouldNotBeNull();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
            TestHelper.GetResultUrlString(result).ShouldEqual(page);
        }
    }
}