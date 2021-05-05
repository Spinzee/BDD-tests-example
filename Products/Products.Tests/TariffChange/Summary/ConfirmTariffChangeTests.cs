namespace Products.Tests.TariffChange.Summary
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class ConfirmTariffChangeTests
    {
        [Test]
        public async Task AddOnlineProfileShouldAddTheProfileAndRedirectToTariffSummary()
        {
            // Arrange            
            const string email = "a@eeeee.com";
            const bool expectedEmailMarketing = false;

            Customer customer = DefaultDomainModel.CustomerForSummary();
            customer.EmailAddress = email;
            customer.Password = "P455w0rd#12345";
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var fakeEmailManager = new FakeEmailManager(new Exception());
            var fakeLogger = new FakeLogger();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var fakeCustomerProfileRepository = new FakeProfileRepository() { ProfileExists = false };
            var fakePersonalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();
            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .WithLogger(fakeLogger)
                .WithEmailManager(fakeEmailManager)
                .WithPersonalProjectionServiceWrapper(fakePersonalProjectionServiceWrapper)
                .Build<SummaryController>();

            var model = new TariffSummaryViewModel { IsTermsAndConditionsChecked = true };
            // Act
            ActionResult result = await controller.TariffSummary(model);

            // Assert
            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ShowConfirmation");
            fakeCustomerProfileRepository.InsertProfileSqlParameters["logOnName"].ShouldEqual(email);
            fakeCustomerProfileRepository.InsertProfileSqlParameters["password"].Length.ShouldEqual(89);
            fakeCustomerProfileRepository.InsertProfileSqlParameters["accountStatus"].ShouldEqual(((int)AccountStatus.AwaitingActivation).ToString());
            fakeCustomerProfileRepository.InsertProfileSqlParameters["marketingConsent"].ShouldEqual(expectedEmailMarketing.ToString());
        }

        [Test]
        public async Task ShouldNotCreateOnlineProfileWhenPasswordIsNullEmpty()
        {
            // Arrange            
            const string email = "a@eeeee.com";            

            Customer customer = DefaultDomainModel.CustomerForSummary();
            customer.EmailAddress = email;
            
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var fakeEmailManager = new FakeEmailManager(new Exception());
            var fakeLogger = new FakeLogger();
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultTariffChange();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "aa");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "aa");

            var fakeCustomerProfileRepository = new FakeProfileRepository() { ProfileExists = false };
            var fakePersonalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();
            var controller = new ControllerFactory()
                .WithConfigManager(fakeConfigManager)
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithCustomerProfileRepository(fakeCustomerProfileRepository)
                .WithLogger(fakeLogger)
                .WithEmailManager(fakeEmailManager)
                .WithPersonalProjectionServiceWrapper(fakePersonalProjectionServiceWrapper)
                .Build<SummaryController>();

            var model = new TariffSummaryViewModel { IsTermsAndConditionsChecked = true };
            // Act
            ActionResult result = await controller.TariffSummary(model);

            // Assert
            result.ShouldNotBeNull();
            var routeResult = result.ShouldBeType<RedirectToRouteResult>();
            routeResult.RouteValues["action"].ShouldEqual("ShowConfirmation");
            fakeCustomerProfileRepository.InsertAuditSqlParameters.Count.ShouldEqual(0);            
        }

        [Test]
        public async Task ShouldNotRedirectToShowConfirmationWhenCheckBoxNotTicked()
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });
            var model = new TariffSummaryViewModel { IsTermsAndConditionsChecked = false };

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            controller.ModelState.AddModelError("IsConsentChecked", new Exception("IsConsentChecked must be checked"));

            // Act
            ActionResult result = await controller.TariffSummary(model);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            ((ViewResult) result).ViewName.ShouldEqual("TariffSummary");
        }

        [Test]
        public async Task EligibleCustomerShouldBeAbleToCompleteTariffChange()
        {
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            ActionResult result =await controller.TariffSummary(new TariffSummaryViewModel());

            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("ShowConfirmation");
        }

        [TestCase(144.01, "£13")]
        [TestCase(144, "£12")]
        [TestCase(143.99, "£12")]
        public void NewDirectDebitAmountShouldBeRoundedUpToWholeNumber(double projectedAnnualCost, string expectedMonthlyDirectDebit)
        {
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customerAccount.SelectedTariff.AnnualCostValue = projectedAnnualCost;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            ActionResult result = controller.TariffSummary();

            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            var view = (ViewResult) result;
            view.Model.ShouldBeType<TariffSummaryViewModel>();
            var model = (TariffSummaryViewModel) view.Model;
            model.ElectricityAmount.ShouldEqual(expectedMonthlyDirectDebit);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void SingleFuelCustomerShouldSeeDirectDebitDetailsIfDirectDebit(bool isDirectDebit, bool shouldSeeDetails)
        {
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customerAccount.PaymentDetails.IsDirectDebit = isDirectDebit;
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            ActionResult result = controller.TariffSummary();

            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            var view = (ViewResult) result;
            view.Model.ShouldBeType<TariffSummaryViewModel>();
            var model = (TariffSummaryViewModel) view.Model;
            model.HasAnyDirectDebitAccount.ShouldEqual(shouldSeeDetails);
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void DualFuelCustomerShouldSeeDirectDebitDetailsIfAtLeastOneFuelIsDirectDebit(bool firstFuelIsDd, bool secondFuelIsDd, bool shouldSeeDetails)
        {
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount gasAccount = DefaultDomainModel.ForSummary();
            gasAccount.PaymentDetails.IsDirectDebit = firstFuelIsDd;
            gasAccount.CurrentTariff.FuelType = FuelType.Gas;
            CustomerAccount electricAccount = DefaultDomainModel.ForSummary();
            electricAccount.PaymentDetails.IsDirectDebit = secondFuelIsDd;
            customer.AddCustomerAccount(electricAccount);
            customer.AddCustomerAccount(gasAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            ActionResult result = controller.TariffSummary();

            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            var view = (ViewResult) result;
            view.Model.ShouldBeType<TariffSummaryViewModel>();
            var model = (TariffSummaryViewModel) view.Model;
            model.HasAnyDirectDebitAccount.ShouldEqual(shouldSeeDetails);
        }

        [TestCase(FuelType.Dual)]
        [TestCase(FuelType.Electricity)]
        [TestCase(FuelType.Gas)]
        public void CustomerShouldSeeCorrectFuelType(FuelType fuelType)
        {
            Customer customer = DefaultDomainModel.CustomerForSummary();
            if (fuelType == FuelType.Dual)
            {
                CustomerAccount electricAccount = DefaultDomainModel.ForSummary();
                CustomerAccount gasAccount = DefaultDomainModel.ForSummary();
                gasAccount.CurrentTariff.FuelType = FuelType.Gas;
                customer.AddCustomerAccount(electricAccount);
                customer.AddCustomerAccount(gasAccount);
            }
            else
            {
                CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
                customerAccount.CurrentTariff.FuelType = fuelType;
                customer.AddCustomerAccount(customerAccount);
            }

            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            ActionResult result = controller.TariffSummary();

            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>()
                .Model.ShouldBeType<TariffSummaryViewModel>()
                .FuelType.ShouldEqual(fuelType);
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, false, false)]
        public async Task OnlyMonthlyDirectDebitCustomersShouldSendPaymentDetailsInActionAer(bool isDirectDebit, bool isMonthlyDirectDebit, bool shouldProvidePaymentDetails)
        {
            // Arrange
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customerAccount.PaymentDetails.IsDirectDebit = isDirectDebit;
            customerAccount.PaymentDetails.IsMonthlyDirectDebit = isMonthlyDirectDebit;
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("ShowConfirmation");
            fakeAnnualEnergyReviewServiceWrapper.PaymentMethodDetailsUpdated.ShouldEqual(shouldProvidePaymentDetails);
            fakeAnnualEnergyReviewServiceWrapper.DirectDebitStatusChanged.ShouldEqual(shouldProvidePaymentDetails);
        }

        [Test]
        public async Task AddPersonalProjectionShouldAddTheProjectionDetails()
        {
            // Arrange
            var expectedPersonalProjectionDetails = new PersonalProjectionDetails
            {
                ElectricitySpend = 400.00,
                ElectricityUsage = 1000,
                GasSpend = 0.00,
                GasUsage = 0,
                SiteId = 12345
            };
            Customer customer = DefaultDomainModel.CustomerForSummary();
            CustomerAccount customerAccount = DefaultDomainModel.ForSummary();
            customer.AddCustomerAccount(customerAccount);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = customer
            });

            var fakePersonalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithPersonalProjectionServiceWrapper(fakePersonalProjectionServiceWrapper)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("ShowConfirmation");
            fakePersonalProjectionServiceWrapper.LastProjectionDetails.ElectricitySpend.ShouldEqual(expectedPersonalProjectionDetails.ElectricitySpend);
            fakePersonalProjectionServiceWrapper.LastProjectionDetails.ElectricityUsage.ShouldEqual(expectedPersonalProjectionDetails.ElectricityUsage);
            fakePersonalProjectionServiceWrapper.LastProjectionDetails.GasSpend.ShouldEqual(expectedPersonalProjectionDetails.GasSpend);
            fakePersonalProjectionServiceWrapper.LastProjectionDetails.GasUsage.ShouldEqual(expectedPersonalProjectionDetails.GasUsage);
            fakePersonalProjectionServiceWrapper.LastProjectionDetails.SiteId.ShouldEqual(expectedPersonalProjectionDetails.SiteId);
        }

        [Test]
        public void DirectAccessToGetShouldRedirectToLandingPageWhenCustomerNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            // Act
            ActionResult result = controller.TariffSummary();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public async Task DirectAccessToPostShouldRedirectToLandingPageWhenCustomerNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            // Act
            ActionResult result = await controller.TariffSummary(new TariffSummaryViewModel());

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void DirectAccessShouldRedirectToLandingPageWhenEmailNull()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                Customer = new Customer()
            });

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<SummaryController>();

            // Act
            ActionResult result = controller.TariffSummary();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }
    }
}