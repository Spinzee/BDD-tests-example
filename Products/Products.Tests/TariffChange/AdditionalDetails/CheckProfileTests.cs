namespace Products.Tests.TariffChange.AdditionalDetails
{
    using System;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class CheckProfileTests
    {
        [Test]
        public void DirectAccessToCheckProfileShouldRedirectToLandingPageWhenCustomerNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AdditionalDetailsController>();

            var model = new GetCustomerEmailViewModel
            {
                EmailAddress = "test@test.com"
            };

            // Act
            ActionResult result = controller.CheckProfileExists(model).Result;

            // Assert
            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldThrowExceptionWithErrorWhenCheckProfileServiceFails()
        {
            var checkProfileServiceException = new Exception("Exception occured attempting to check existence of an online profile");
            var fakeCustomerProfileRepository = new FakeProfileRepository { CheckProfileException = checkProfileServiceException };
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<AdditionalDetailsController>();

            var model = new GetCustomerEmailViewModel
            {
                EmailAddress = "anything@sse.com"
            };

            var ex = Assert.ThrowsAsync<Exception>(() => controller.CheckProfileExists(model));
            ex.ShouldEqual(checkProfileServiceException);
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Exception occured attempting to check existence of an online profile");
        }

        [TestCase("existingProfile@test.com", true, "If email exists as a profile redirect to Summary page")]
        [TestCase("nonExistingProfile@test.com", false, "If email does not exist redirect to Password page")]
        public void ProfileCheckRedirection(string emailAddress, bool exists, string description)
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var fakeCustomerProfileRepository = new FakeProfileRepository { ProfileExists = exists };

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<AdditionalDetailsController>();

            var model = new GetCustomerEmailViewModel
            {
                EmailAddress = emailAddress
            };

            TestHelper.GetResultUrlString(controller.CheckProfileExists(model).Result)
                .ShouldEqual(exists ? "TariffSummary" : "CreatePassword");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.EmailAddress.ShouldEqual(emailAddress);
        }

        [Test]
        public void DirectAccessToGetCustomerEmailShouldRedirectToLandingPageWhenCustomerNull()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AdditionalDetailsController>();

            ActionResult result = controller.GetCustomerEmail();

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldDisplayCustomerEmailViewWhenAccessedWithAValidCustomerInSession()
        {
            //Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(account);
            customer.EmailAddress = "existingProfile@test.com";
            customer.CustomerSelectedTariff = new SelectedTariff
            {
                IsFollowOnTariff = true,
                Name = "Abc Tariff",
                EffectiveDate = DateTime.Today,
                ProjectedAnnualCost = "12",
                ProjectedMonthlyCost = "1",
                ProjectedAnnualCostValue = 12,
                ProjectedMonthlyCostValue = "1",
                ExitFeePerFuel = "40",
                ExitFee = 40
            };
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var fakeCustomerProfileRepository = new FakeProfileRepository();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .Build<AdditionalDetailsController>();

            //Act
            ActionResult result = controller.GetCustomerEmail();

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<GetCustomerEmailViewModel>();
            viewModel.EmailAddress.ShouldEqual(customer.EmailAddress);
        }

        [TestCase("45678", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("my name is", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("Bill at microsoft.com", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase(null, 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("ohn@TEST.223COM", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("test1@testone.co--uk", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("test_one@testone.co-uk", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("testone@test1--com", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("10.5.26.93@test.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("test@10.5.26.93.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("test.one@testone.co.uk", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("test-one@testone.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("test_one@testone.net", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("testone@test.one.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("a@b.cd", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("test@lessthanfiftytwocharsinlengisfineforanemail.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("lessthanfiftytwocharsinlengisfineforanemail@test.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("testone+sse@testone.com", 0, "CreatePassword", "valid email address, Go to change password")]
        public void ShouldRedirectToChangePasswordIfEmailAddressIsValid(string emailAddress, int numberOfInlineErrors, string page, string description)
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var fakeCustomerProfileRepository = new FakeProfileRepository { ProfileExists = false };

            var controller = new ControllerFactory()
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AdditionalDetailsController>();

            var model = new GetCustomerEmailViewModel
            {
                EmailAddress = emailAddress,
                ConfirmEmailAddress = emailAddress
            };

            controller.ValidateViewModel(model);

            ActionResult result = controller.CheckProfileExists(model).Result;

            //Assert
            result.ShouldNotBeNull();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
            TestHelper.GetResultUrlString(result).ShouldEqual(page);
        }

        [TestCase("testone@testone.com", "testone@testone.com", 0, "CreatePassword", "valid email address, Go to change password")]
        [TestCase("ohn@TEST.COM", "different@TEST.COM", 1, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("", "", 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("ohn@TEST.COM", "", 1, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("different@TEST.COM", "ohn@TEST.COM", 1, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase("ohn@TEST.COM", null, 1, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        [TestCase(null, null, 2, "GetCustomerEmail", "Invalid email address, stay on get customer email page")]
        public void ShouldRedirectToChangePasswordIfEmailAddressAreTheSame(string emailAddress, string confirmEmailAddress, int numberOfInlineErrors, string page, string description)
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var fakeCustomerProfileRepository = new FakeProfileRepository { ProfileExists = false };

            var controller = new ControllerFactory()
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AdditionalDetailsController>();

            var model = new GetCustomerEmailViewModel
            {
                EmailAddress = emailAddress,
                ConfirmEmailAddress = confirmEmailAddress
            };

            controller.ValidateViewModel(model);

            ActionResult result = controller.CheckProfileExists(model).Result;

            //Assert
            result.ShouldNotBeNull();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
            TestHelper.GetResultUrlString(result).ShouldEqual(page);
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenSelectedTariffNull()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AdditionalDetailsController>();

            ActionResult result = controller.GetCustomerEmail();

            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }
    }
}