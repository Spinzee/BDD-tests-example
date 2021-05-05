namespace Products.Tests.Energy
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common.Fakes;
    using Core;
    using Core.Enums;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Common;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Should;
    using Web.Areas.Energy.Controllers;
    using Web.Attributes;
    using WebModel.ViewModels.Energy;

    public class EnergyCheckSessionAttributeOnControllerTests
    {
        [TestCaseSource(nameof(DirectPageAccessSessionTestCases))]
        public void AllHttpGetControllerActionsShouldExecuteEnergyCheckSessionAttributeAndRedirectToHubPageWhenSessionDataIsMissing(string requestPath, EnergyCustomer energyCustomer, YourPriceViewModel yourPriceViewModel)
        {
            // Arrange
            var fakeContextManager =
                new FakeContextManager(new FakeHttpContext(new FakeHttpRequest(requestPath, "GET"),
                    new FakeHttpServerUtility(), new FakeHttpSession()));

            fakeContextManager.HttpContext.Session.Add(SessionKeys.EnergyCustomer, energyCustomer);
            fakeContextManager.HttpContext.Session.Add(SessionKeys.EnergyYourPriceDetails, yourPriceViewModel);

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .Build<QuoteController>();

            var controllerContext = new ControllerContext(fakeContextManager.HttpContext, new RouteData(), controller);

            var actionExecutingContext = new ActionExecutingContext(controllerContext,
                new FakeActionDescriptor(string.Empty), new Dictionary<string, object>());
            var energyCheckSessionAttribute = new EnergyCheckSessionAttribute();

            // Act
            energyCheckSessionAttribute.OnActionExecuting(actionExecutingContext);

            // Assert
            actionExecutingContext.Result.ShouldNotBeNull();
            var redirectResult = actionExecutingContext.Result.ShouldBeType<RedirectResult>();
            redirectResult.Url.ShouldEqual(ConfigurationManager.AppSettings["EnergyLinkBackToHubURL"]);
        }

        private static IEnumerable<TestCaseData> DirectPageAccessSessionTestCases()
        {
            yield return new TestCaseData("/energy-signup/payment-method", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/meter-type", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/meter-type", new EnergyCustomer { SelectedBillingPreference = BillingPreference.None }, null);
            yield return new TestCaseData("/energy-signup/smart-meter", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/energy-usage", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/quote-details", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/quote-details", new EnergyCustomer { Projection = null }, null);

            yield return new TestCaseData("/energy-signup/confirm-address", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/confirm-address", new EnergyCustomer { SelectedTariff = new Tariff() }, null);
            yield return new TestCaseData("/energy-signup/confirm-address", new EnergyCustomer
            {
                SelectedTariff = new Tariff
                {
                    BundlePackage = new BundlePackage("100", 20, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>())
                }
            }, null);

            yield return new TestCaseData("/energy-signup/phone-package", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/phone-package", new EnergyCustomer { SelectedTariff = new Tariff() }, null);
            yield return new TestCaseData("/energy-signup/phone-package", new EnergyCustomer
            {
                SelectedTariff = new Tariff
                {
                    BundlePackage = new BundlePackage("100", 20, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>())
                }
            }, new YourPriceViewModel());

            yield return new TestCaseData("/energy-signup/select-address", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy-signup/online-account", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/online-account", new EnergyCustomer { SelectedTariff = new Tariff() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/personal-details", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/contact-details", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/bank-details", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/confirmation", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/order-summary", new EnergyCustomer(), new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/order-summary", new EnergyCustomer { SelectedTariff = new Tariff() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/order-summary",
                new EnergyCustomer { SelectedTariff = new Tariff(), PersonalDetails = new PersonalDetails() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/order-summary",
                new EnergyCustomer { SelectedTariff = new Tariff(), PersonalDetails = new PersonalDetails(), ContactDetails = new ContactDetails() }, null);
            yield return new TestCaseData("/energy-signup/order-summary",
                new EnergyCustomer
                {
                    SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit, SelectedTariff = new Tariff(), PersonalDetails = new PersonalDetails(),
                    ContactDetails = new ContactDetails()
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy/common/yourpricedetails", new EnergyCustomer(), null);
            yield return new TestCaseData("/energy/common/yourpricedetails", new EnergyCustomer(), new YourPriceViewModel());

            yield return new TestCaseData("/energy-signup/contact-details", new EnergyCustomer { SelectedTariff = new Tariff() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/bank-details", new EnergyCustomer { SelectedTariff = new Tariff() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/confirmation", new EnergyCustomer { SelectedTariff = new Tariff() }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/online-account",
                new EnergyCustomer { SelectedTariff = new Tariff(), ContactDetails = new ContactDetails(), ProfileExists = true }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/bank-details",
                new EnergyCustomer
                {
                    SelectedPaymentMethod = PaymentMethod.PayAsYouGo, SelectedTariff = new Tariff(), PersonalDetails = new PersonalDetails(),
                    ContactDetails = new ContactDetails()
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/meter-type", new EnergyCustomer { SelectedFuelType = FuelType.Gas }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/energy-usage", new EnergyCustomer { SelectedFuelType = FuelType.Gas, HasSmartMeter = null },
                new YourPriceViewModel());

            //CAndC Journey Test cases
            yield return new TestCaseData("/energy-signup/payment-method",
                new EnergyCustomer { SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoNonSmart() },
                new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/meter-type",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(),
                    SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/smart-meter",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(),
                    SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit, SelectedElectricityMeterType = ElectricityMeterType.Standard
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/smart-meter-frequency",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = null, SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                    SelectedElectricityMeterType = ElectricityMeterType.Standard
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/smart-meter-frequency",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(),
                    SelectedPaymentMethod = PaymentMethod.None, SelectedElectricityMeterType = ElectricityMeterType.Standard
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/smart-meter-frequency",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(),
                    SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit, SelectedElectricityMeterType = ElectricityMeterType.Standard
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/energy-usage",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = null, SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                    SelectedElectricityMeterType = ElectricityMeterType.Standard, HasSmartMeter = null
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/energy-usage",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Dual, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart(),
                    SelectedPaymentMethod = PaymentMethod.None, SelectedElectricityMeterType = ElectricityMeterType.Standard
                }, new YourPriceViewModel());
            yield return new TestCaseData("/energy-signup/energy-usage",
                new EnergyCustomer
                {
                    SelectedFuelType = FuelType.Electricity, MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2(),
                    SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit, SelectedElectricityMeterType = ElectricityMeterType.Standard,
                    SmartMeterFrequency = SmartMeterFrequency.None
                }, new YourPriceViewModel());
        }
    }
}