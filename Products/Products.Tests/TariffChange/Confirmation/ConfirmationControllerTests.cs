namespace Products.Tests.TariffChange.Confirmation
{
    using System;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Core;
    using Fakes.Services;
    using Helpers;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using Model.TariffChange.Tariffs;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.Resources.TariffChange;
    using WebModel.ViewModels.TariffChange;

    public class ConfirmationControllerTests
    {
        [Test]
        public void ShouldClearSessionDataOnConfirmationOfTariffChange()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(account);
            customer.CustomerSelectedTariff.EffectiveDate = DateTime.Now.AddDays(1);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("EmailCorporateDescriptor", "EmailCorporateDescriptor");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldBeType<ViewResult>()
                .Model.ShouldBeType<ConfirmationViewModel>()
                .Header.ShouldEqual(Confirmation_Resources.Header);

            fakeInMemoryTariffChangeSessionService.GetJourneyDetails().ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenCustomerNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("EmailCorporateDescriptor", "EmailCorporateDescriptor");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenSelectedTariffNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("EmailCorporateDescriptor", "EmailCorporateDescriptor");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenEffectiveDateMinValue()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer
                {
                    CustomerSelectedTariff = new SelectedTariff()
                }
            });

            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("EmailCorporateDescriptor", "EmailCorporateDescriptor");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithConfigManager(fakeConfigManager)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldDisplayConfirmationPageWithFollowOnContentWhenFollowOnTariffIsSelected()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(account);

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

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldBeType<ViewResult>()
                .Model.ShouldBeType<ConfirmationViewModel>()
                .Header.ShouldEqual(Confirmation_Resources.FollowOnHeader);
        }

        [TestCase(false, true, "Your tariff needs a smart meter. We’ll be in touch shortly to let you book your installation.")]
        public void ShouldDisplayConfirmationPageWithSmartContentWhenSmartIsSelected(bool isSmartCustomer, bool isSmartTariff, string expectedText)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            account.IsSmart = isSmartCustomer;
            customer.AddCustomerAccount(account);

            customer.CustomerSelectedTariff = new SelectedTariff
            {
                Name = "Abc Tariff",
                EffectiveDate = DateTime.Today,
                ProjectedAnnualCost = "12",
                ProjectedMonthlyCost = "1",
                ProjectedAnnualCostValue = 12,
                ProjectedMonthlyCostValue = "1",
                ExitFeePerFuel = "40",
                ExitFee = 40,
                IsSmartTariff = isSmartTariff
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldBeType<ViewResult>()
                .Model.ShouldBeType<ConfirmationViewModel>().BulletList[0]
                .ShouldContain(expectedText);
        }

        [TestCase(false, true, TariffGroup.FixAndProtect, true, false)]
        [TestCase(true, false, TariffGroup.FixAndProtect, false, false)]
        [TestCase(false, false, TariffGroup.FixAndProtect, false, false)]
        [TestCase(true, true, TariffGroup.FixAndProtect, false, false)]
        [TestCase(false, true, TariffGroup.FixAndFibre, false, true)]
        [TestCase(true, false, TariffGroup.FixAndFibre, false, true)]
        [TestCase(false, false, TariffGroup.FixAndFibre, false, true)]
        [TestCase(true, true, TariffGroup.FixAndFibre, false, true)]
        public void ShouldDisplayConfirmationPageWithCorrectSmartMeterMessageAndShowTelcoLink(bool isSmartCustomer, bool isSmartEligible, TariffGroup tariffGroup, bool expectedShowSmartBookingLink, bool expectedShowTelcoLink)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            account.IsSmart = isSmartCustomer;
            account.IsSmartEligible = isSmartEligible;
            customer.AddCustomerAccount(account);

            customer.CustomerSelectedTariff = new SelectedTariff
            {
                Name = "Abc Tariff",
                EffectiveDate = DateTime.Today,
                ProjectedAnnualCost = "12",
                ProjectedMonthlyCost = "1",
                ProjectedAnnualCostValue = 12,
                ProjectedMonthlyCostValue = "1",
                ExitFeePerFuel = "40",
                ExitFee = 40,
                TariffGroup = tariffGroup
            };

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer,
                CustomerAccount = account
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldBeType<ViewResult>().Model.ShouldBeType<ConfirmationViewModel>().ShowSmartBookingLink.ShouldEqual(expectedShowSmartBookingLink);
            result.ShouldBeType<ViewResult>().Model.ShouldBeType<ConfirmationViewModel>().ShowTelcoLink.ShouldEqual(expectedShowTelcoLink);
            result.ShouldBeType<ViewResult>().Model.ShouldBeType<ConfirmationViewModel>().IsSmartCustomer.ShouldEqual(isSmartCustomer);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ShouldContainCorrectValuesInDataLayer(bool isSmartEligible)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount account = DefaultDomainModel.ForSummary();
            account.IsSmartEligible = isSmartEligible;
            customer.AddCustomerAccount(account);

            customer.CustomerSelectedTariff = new SelectedTariff
            {
                IsFollowOnTariff = false,
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
                Customer = customer,
                CustomerAccount = account
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<ConfirmationController>();

            // Act
            ActionResult result = controller.ShowConfirmation();

            // Assert
            result.ShouldBeType<ViewResult>().Model.ShouldBeType<ConfirmationViewModel>().DataLayer["SmartEligible"].ShouldEqual(isSmartEligible.ToString());
        }
    }
}