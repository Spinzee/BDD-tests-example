namespace Products.Tests.TariffChange.CustomerIdentification
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Service.Common;
    using ServiceWrapper.ManageCustomerInformationService;
    using Should;
    using Web.Areas.TariffChange.Controllers;
    using WebModel.ViewModels.TariffChange;

    public class CustomerIdentificationControllerTests
    {
        [Test]
        public void ShouldThrowExceptionIfCustomerAlertServicesFails()
        {
            var fakeCustomerAlertRepository = new FakeCustomerAlertRepository(new Exception());

            var controller = new ControllerFactory()
                .WithCustomerAlertRepository(fakeCustomerAlertRepository)
                .Build<CustomerIdentificationController>();

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => controller.IdentifyCustomer());
            ex.Message.ShouldNotBeNull();
        }

        [TestCase(true, false, null, null, "If CTC customer alert DB record is present and no start and end date, redirect to CTC unavailable")]
        [TestCase(false, true, null, null, "If no CTC customer alert is present, start the CTC journey")]
        [TestCase(true, false, -60, 60, "If alert present, and time is within start and end time, start CTC journey")]
        [TestCase(true, true, 1, 60, "If alert present, and start time is after time, redirect to CTC unavailable")]
        [TestCase(true, true, -60, -1, "If alert present, and end time is before time, redirect to CTC unavailable")]
        [TestCase(true, true, 1380, 1500, "If alert present, and alert date is after the date, redirect to CTC unavailable")]
        [TestCase(true, true, -1500, -1380, "If alert present, and alert date is before the date, redirect to CTC unavailable")]
        public void CustomerAlertRedirection(bool isRecordPresent, bool ctcAvailable, int? startTimeOffset, int? endTimeOffset, string description)
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            DateTime currentDateTime = DateTime.Now;

            CustomerAlertResult customerAlertResult = null;

            if (isRecordPresent)
            {
                if (startTimeOffset != null && endTimeOffset != null)
                {
                    customerAlertResult = new CustomerAlertResult
                    {
                        StartTime = currentDateTime.AddMinutes(startTimeOffset.Value),
                        EndTime = currentDateTime.AddMinutes(endTimeOffset.Value)
                    };
                }
                else
                {
                    customerAlertResult = new CustomerAlertResult
                    {
                        StartTime = null,
                        EndTime = null
                    };
                }
            }

            var fakeCustomerAlertRepository = new FakeCustomerAlertRepository(new List<CustomerAlertResult> { customerAlertResult });

            var controller = new ControllerFactory()
                .WithCustomerAlertRepository(fakeCustomerAlertRepository)
                .WithContextManager(fakeContextManager)
                .Build<CustomerIdentificationController>();

            // Act
            ActionResult result = controller.IdentifyCustomer().Result;

            // Assert
            TestHelper.GetResultUrlString(result).ShouldEqual(ctcAvailable ? "IdentifyCustomer" : "CtcUnavailable");
            fakeCustomerAlertRepository.IsCustomerAlertMethodCount.ShouldEqual(1);
            fakeCustomerAlertRepository.AlertName.ShouldEqual("CustomerAlertName");
        }

        [Test]
        public void ShouldRemainOnIdentifyCustomerViewOnValidationError()
        {
            var controller = new ControllerFactory().Build<CustomerIdentificationController>();

            controller.ModelState.AddModelError(string.Empty, string.Empty);

            // Act
            ActionResult result = controller.IdentifyCustomer(new IdentifyCustomerViewModel());

            result.ShouldNotBeNull();
            result.ShouldBeType<ViewResult>();
            ((ViewResult) result).ViewName.ShouldEqual("IdentifyCustomer");
        }

        private static IEnumerable<TestCaseData> AccountNumberAndPostcodeCombinations()
        {
            yield return new TestCaseData("1111111113", "SO14 1FJ", true, "Account and postcode exist");
            yield return new TestCaseData("1111111113", "so141fj", true, "Account and lowercase postcode (without space) exist");
            yield return new TestCaseData("4444444442", "SO14 1FJ", false, "Account number does not exist");
            yield return new TestCaseData("1111111113", "SO14 2FJ", false, "Account number exists but postcode does not match");
            yield return new TestCaseData("1111111113", "SO14", false, "Account number exists but partial postcode does not match");
            yield return new TestCaseData("3333333333", "SO14 3FJ", false, "Invalid check digit on account number is invalid");
        }

        [Test, TestCaseSource(nameof(AccountNumberAndPostcodeCombinations)), Description("Validation Of Account Number And Postcode Redirection")]
        public void ValidationOfAccountNumberAndPostcodeRedirection(string accountNumber, string postcode, bool expectedResult, string description)
        {
            // Arrange
            var fakeDataDictionary = new Dictionary<string, string>
            {
                { "1111111113", "SO14 1FJ" },
                { "2222222226", "SO14 2FJ" },
                { "3333333339", "SO14 3FJ" }
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

            // Act and Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer(new IdentifyCustomerViewModel { AccountNumber = accountNumber, PostCode = postcode })).ShouldEqual(expectedResult ? "ConfirmDetails" : "AccountDetailsNotMatched");
            if (expectedResult)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.CustomerAccount.SiteDetails?.AccountNumber.ShouldEqual(accountNumber);
            }
        }

        [Test, Description("Should Redirect to Ineligible Customer fallout when customer account is associated to multiple services")]
        public void CustomerWithMultipleServicesAssociatedToAccountIsIneligible()
        {
            // Arrange
            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "2222222226",
                Postcode = "SO14 2FJ",
                CustomerAccountStatus = CustomerAccountStatusType.MultipleServices
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });
            var fakeGoogleReCaptchaData = new FakeGoogleReCaptchaData(true, DateTime.UtcNow);
            var fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService(fakeGoogleReCaptchaData);
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithGoogleRecaptchaService(fakeGoogleReCaptchaService)
                .Build<CustomerIdentificationController>();

            // Act and Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer(new IdentifyCustomerViewModel
                {
                    AccountNumber = fakeMcisData.CustomerAccountNumber,
                    PostCode = fakeMcisData.Postcode
                }))
                .ShouldEqual("CustomerAccountIneligible");
        }

        [Test, Description("Should Throw Exception With Error When Manage Customer Information Service Fails")]
        public void ShouldThrowExceptionWithErrorWhenManageCustomerInformationServiceFails()
        {
            // Arrange
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new Exception());

            var controller = new ControllerFactory()
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<CustomerIdentificationController>();

            // Act & Assert
            var model = new IdentifyCustomerViewModel { AccountNumber = "1111111113", PostCode = "SO14 1FJ" };
            var ex = Assert.Throws<Exception>(() => controller.IdentifyCustomer(model));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual($"Exception occured, Account = {model.AccountNumber}, Postcode = {model.PostCode}, attempting to validate for customer tariff change, customer identification.");
        }

        [Test]
        public void ShouldThrowExceptionWhenGoogleReCaptchaServicesFails()
        {
            // Arrange
            var captchaException = new Exception("Google recaptcha service failed.");
            var fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService(captchaException);

            var controller = new ControllerFactory()
                .WithGoogleRecaptchaService(fakeGoogleReCaptchaService)
                .Build<CustomerIdentificationController>();

            // Act & Assert
            var model = new IdentifyCustomerViewModel { AccountNumber = "1111111113", PostCode = "SO14 1FJ" };
            var ex = Assert.Throws<Exception>(() => controller.IdentifyCustomer(model));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual("Google recaptcha service failed.");
        }

        [TestCase("1111111113", "SO14 1FJ", true, "If Google ReCapture Success is true redirect to ConfirmDetails")]
        [TestCase("1111111113", "SO14 1FJ", false, "If Google ReCapture Success is false redirect to IdentifyCustomer")]
        public void GoogleReCaptureValidationRedirection(string accountNumber, string postcode, bool success, string description)
        {
            // Arrange
            var fakeDataDictionary = new Dictionary<string, string>
            {
                { "1111111113", "SO14 1FJ" },
                { "2222222226", "SO14 2FJ" },
                { "3333333339", "SO14 3FJ" }
            };
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeDataDictionary);

            var fakeGoogleReCaptchaData = new FakeGoogleReCaptchaData(success, DateTime.UtcNow);
            var fakeGoogleReCaptchaService = new FakeGoogleReCaptchaService(fakeGoogleReCaptchaData);

            var controller = new ControllerFactory()
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithGoogleRecaptchaService(fakeGoogleReCaptchaService)
                .Build<CustomerIdentificationController>();

            // Act and Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer(new IdentifyCustomerViewModel { AccountNumber = accountNumber, PostCode = postcode })).ShouldEqual(success ? "ConfirmDetails" : "IdentifyCustomer");
        }

        [TestCase(CustomerAccountStatusType.MultipleServices, "CustomerAccountIneligible")]
        [TestCase(CustomerAccountStatusType.Found, "ConfirmDetails")]
        public void ShouldRedirectCustomerWithMultiServicesToAccountIneligible(CustomerAccountStatusType customerAccountStatusType, string expectedResult)
        {
            // Arrange
            var fakeCustomerAccountsResponse = new GetCustomerAccountsResponse
            {
                CustomerAccounts = new List<CustomerAccountsType>
                {
                    new CustomerAccountsType
                    {
                        CustomerAccountStatus = customerAccountStatusType,
                        CustomerAccountNumber = "12345",

                        Sites = new List<SitesType>
                        {
                            new SitesType
                            {
                                SiteAddress = new AddressType
                                {
                                    AddressID = "111111",
                                    PostCode = "BN15 0QW"
                                },
                                Services = new List<ServiceType>
                                {
                                    new ServiceType
                                    {
                                        ServiceStatus = ServiceStatusType.Active,
                                        ServicePlanDescription = "",
                                        Servicetype = ServiceTypeType.electricity
                                    }
                                }
                            }
                        }
                    }
                }
            };

            HttpContext.Current = CreateHttpContext("user", "role");

            var customerAccounts = new List<string>
            {
                "1111"
            };

            var fakeCustomerAccountRepository = new FakeProfileRepository { CustomerAccounts = customerAccounts };
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeCustomerAccountsResponse);

            var controller = new ControllerFactory()
                .WithCustomerProfileRepository(fakeCustomerAccountRepository)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<CustomerIdentificationController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer().Result).ShouldEqual(expectedResult);
        }

        [Test]
        public void ShouldRedirectCustomerToPreLoginJourneyIfNoAccounts()
        {
            // Arrange
            var fakeCustomerAccountsResponse = new GetCustomerAccountsResponse
            {
                CustomerAccounts = new List<CustomerAccountsType>()
            };

            HttpContext.Current = CreateHttpContext("user", "role");

            var customerAccounts = new List<string>
            {
                "1111"
            };

            var fakeCustomerAccountRepository = new FakeProfileRepository { CustomerAccounts = customerAccounts };
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeCustomerAccountsResponse);

            var controller = new ControllerFactory()
                .WithCustomerProfileRepository(fakeCustomerAccountRepository)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<CustomerIdentificationController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer().Result).ShouldEqual("IdentifyCustomer");
        }

        [TestCase("11111", "22222", "ConfirmDetails")]
        [TestCase("11111", "11111", "ConfirmDetails")]
        public void ShouldRedirectCustomerWithMultipleSitesToCorrectResult(string site1AddressId, string site2AddressId, string expectedResult)
        {
            // Arrange
            var fakeCustomerAccountsResponse = new GetCustomerAccountsResponse
            {
                CustomerAccounts = new List<CustomerAccountsType>
                {
                    new CustomerAccountsType
                    {
                        CustomerAccountNumber = "12345",
                        Sites = new List<SitesType>
                        {
                            new SitesType
                            {
                                SiteAddress = new AddressType
                                {
                                    AddressID = site1AddressId,
                                    PostCode = "BN15 0QW"
                                },
                                Services = new List<ServiceType>
                                {
                                    new ServiceType
                                    {
                                        ServiceStatus = ServiceStatusType.Active,
                                        ServicePlanDescription = "",
                                        Servicetype = ServiceTypeType.electricity
                                    }
                                }
                            }
                        }
                    },
                    new CustomerAccountsType
                    {
                        CustomerAccountNumber = "98765",
                        Sites = new List<SitesType>
                        {
                            new SitesType
                            {
                                SiteAddress = new AddressType
                                {
                                    AddressID = site2AddressId,
                                    PostCode = "BN15 0QW"
                                },
                                Services = new List<ServiceType>
                                {
                                    new ServiceType
                                    {
                                        ServiceStatus = ServiceStatusType.Active,
                                        ServicePlanDescription = "",
                                        Servicetype = ServiceTypeType.gas
                                    }
                                }
                            }
                        }
                    }
                }
            };

            HttpContext.Current = CreateHttpContext("user", "role");

            var customerAccounts = new List<string>
            {
                "1111",
                "3333"
            };

            var fakeCustomerAccountRepository = new FakeProfileRepository { CustomerAccounts = customerAccounts };
            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(fakeCustomerAccountsResponse);

            var controller = new ControllerFactory()
                .WithCustomerProfileRepository(fakeCustomerAccountRepository)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .Build<CustomerIdentificationController>();

            // Act/Assert
            TestHelper.GetResultUrlString(controller.IdentifyCustomer().Result).ShouldEqual(expectedResult);
        }

        [Test]
        public void DirectAccessToConfirmDetailsShouldRedirectToLandingPage()
        {
            // Arrange
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<CustomerIdentificationController>();

            // Act
            ActionResult result = controller.ConfirmDetails();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        protected static HttpContext CreateHttpContext(string userName, string role)
        {
            var stringWriter = new StringWriter();

            var request = new HttpRequest(string.Empty, "http://a/a.aspx", string.Empty);
            var response = new HttpResponse(stringWriter);
            var context = new HttpContext(request, response)
                { User = new MyAcountsPrincipal(new GenericIdentity(userName), new[] { role }, Guid.NewGuid()) };

            return context;
        }
    }
}