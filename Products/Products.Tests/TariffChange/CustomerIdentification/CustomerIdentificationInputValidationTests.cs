namespace Products.Tests.TariffChange.CustomerIdentification
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using NUnit.Framework;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class CustomerIdentificationInputValidationTests
    {
        //[TestCase("123456", 1, "IdentifyCustomer", "Invalid postcode, stay on identify customer")] - this has been set as valid by ctc project
        //[TestCase("ABCDEF", 1, "IdentifyCustomer", "Invalid postcode, stay on identify customer")] - this has been set as valid by ctc project
        [TestCase("2!!&*(&(", 1, "IdentifyCustomer", "Invalid postcode, stay on identify customer")]
        [TestCase("PO9%!", 1, "IdentifyCustomer", "Invalid postcode, stay on identify customer")]
        [TestCase("PO9 1BH", 0, "ConfirmDetails", "Valid postcode, go to confirm Details")]
        [TestCase("PO91BH", 0, "ConfirmDetails", "Valid postcode, go to confirm Details")]
        [TestCase(" PO9 1BH", 0, "ConfirmDetails", "Valid postcode, go to confirm Details")]
        [TestCase("PO9 1BH ", 0, "ConfirmDetails", "Valid postcode, go to confirm Details")]
        public void ShouldGoToConfirmDetailsIfPostcodeIsValid(string postcode, int numberOfInlineErrors, string page, string description)
        {
            var viewModel = new IdentifyCustomerViewModel { AccountNumber = "1111111113", PostCode = postcode };

            var fakeDataDictionary = new Dictionary<string, string>
            {
                { "1111111113", postcode }
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeDataDictionary);
            var fakeGoogleReCaptchaData = new FakeGoogleReCaptchaData(true, DateTime.UtcNow);
            var fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService(fakeGoogleReCaptchaData);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithGoogleRecaptchaService(fakeGoogleReCaptchaService)
                .Build<CustomerIdentificationController>();

            controller.ValidateViewModel(viewModel);
            ActionResult result = controller.IdentifyCustomer(viewModel);

            result.ShouldNotBeNull();
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
            TestHelper.GetResultUrlString(result).ShouldEqual(page);
        }

        [TestCase("0000035415", 0, "ConfirmDetails", "Valid account number, go to confirm Details")]
        [TestCase("1234567890", 0, "AccountDetailsNotMatched", "Valid account number but incorrect format, go to Account Details Not Matched Page")]
        [TestCase("12345678", 1, "IdentifyCustomer", "Invalid account number, go to Identify Customer")]
        [TestCase("123456789011", 1, "IdentifyCustomer", "Invalid account number, go to Identify Customer")]
        [TestCase("adcdefghij", 1, "IdentifyCustomer", "Invalid account number, go to Identify Customer")]
        public void ShouldGoToConfirmDetailsIfAccountNumberIsValid(string accountNumber, int numberOfInlineErrors, string expectedResult, string description)
        {
            const string validPostCode = "PO20 8JF";

            var fakeCustomerAccountsResponse = new Dictionary<string, string>
            {
                { accountNumber, validPostCode }
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeCustomerAccountsResponse);

            var fakeGoogleReCaptchaData = new FakeGoogleReCaptchaData(true, DateTime.UtcNow);
            var fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService(fakeGoogleReCaptchaData);

            var controller = new ControllerFactory()
                .WithGoogleRecaptchaService(fakeGoogleReCaptchaService)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<CustomerIdentificationController>();

            var model = new IdentifyCustomerViewModel
            {
                AccountNumber = accountNumber,
                PostCode = validPostCode,
                GoogleCaptchaViewModel = new GoogleCaptchaViewModel()
            };

            controller.ValidateViewModel(model);
            ActionResult result = controller.IdentifyCustomer(model);

            result.ShouldNotBeNull();
            TestHelper.GetResultUrlString(result).ShouldEqual(expectedResult);
            controller.ModelState.Keys.Count.ShouldEqual(numberOfInlineErrors);
        }
    }
}