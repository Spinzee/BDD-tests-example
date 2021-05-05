namespace Products.Tests.TariffChange.Services
{
    using System.Collections.Generic;
    using Common.Fakes;
    using Fakes.Services;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using Products.Infrastructure;
    using Products.Infrastructure.Logging;
    using Repository;
    using Repository.Common;
    using Service.Common;
    using Service.TariffChange;
    using Service.TariffChange.Validators;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.ManageCustomerInformationService;
    using ServiceWrapper.PersonalProjectionService;
    using Should;
    using ServiceStatusType = ServiceWrapper.ManageCustomerInformationService.ServiceStatusType;

    public class CustomerAccountServiceTests
    {
        [Test]
        public void ShouldReturnListOfValidFoundAccountNumbers()
        {
            // Arrange
            IAnnualEnergyReviewServiceWrapper annualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper();
            ICheckDigitValidator checkDigitValidator = new CheckDigitValidator();
            IEmailManager emailManager = new FakeEmailManager();
            IContentRepository contentRepository = new ContentRepository();
            IPersonalProjectionServiceWrapper personalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();
            IProfileRepository profileRepository = new FakeProfileRepository();
            ILogger logger = new FakeLogger();
            IJourneyDetailsService journeyDetailsService = new JourneyDetailsService(new FakeInMemoryTariffChangeSessionService());
            IUtilityService utilityService = new UtilityService(new FakeContextManager(new FakeHttpContext(new FakeHttpRequest(), new FakeHttpServerUtility(), new FakeHttpSession())));
            IManageCustomerInformationServiceWrapper fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(GetFakeCustomerAccountsResponse());
            IConfigManager fakeConfigManager = new FakeConfigManager();

            var customerAccountService = new CustomerAccountService(annualEnergyReviewServiceWrapper, checkDigitValidator,
                emailManager, contentRepository, personalProjectionServiceWrapper,
                profileRepository, logger, journeyDetailsService, utilityService,
                fakeManageCustomerInformationServiceWrapper, fakeConfigManager);

            // Assert
            var customerAccounts = new List<string>
            {
                "0239200001",
                "0239200002"
            };

            List<CustomerAccount> foundAccounts = customerAccountService.GetCustomerAccount(customerAccounts);

            // Act
            foundAccounts.Count.ShouldEqual(1);
        }

        [TestCase("Standard", "Evergreen")]
        [TestCase("Fixed", "Fixed")]
        [TestCase("blah", "Fixed")]
        [TestCase("NULL", "Evergreen")]
        public void ShouldCorrectTariffTypeForServicePlanDescription(string servicePlanDescription, string expectedTariffType)
        {
            // Arrange
            IAnnualEnergyReviewServiceWrapper annualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper();
            ICheckDigitValidator checkDigitValidator = new CheckDigitValidator();
            IEmailManager emailManager = new FakeEmailManager();
            IContentRepository contentRepository = new ContentRepository();
            IPersonalProjectionServiceWrapper personalProjectionServiceWrapper = new FakePersonalProjectionServiceWrapper();
            IProfileRepository profileRepository = new FakeProfileRepository();
            ILogger logger = new FakeLogger();
            IJourneyDetailsService journeyDetailsService = new JourneyDetailsService(new FakeInMemoryTariffChangeSessionService());
            IUtilityService utilityService = new UtilityService(new FakeContextManager(new FakeHttpContext(new FakeHttpRequest(), new FakeHttpServerUtility(), new FakeHttpSession())));
            IManageCustomerInformationServiceWrapper fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(GetFakeCustomerAccountsResponse(servicePlanDescription));
            IConfigManager fakeConfigManager = new FakeConfigManager();

            var customerAccountService = new CustomerAccountService(annualEnergyReviewServiceWrapper, checkDigitValidator,
                emailManager, contentRepository, personalProjectionServiceWrapper,
                profileRepository, logger, journeyDetailsService, utilityService,
                fakeManageCustomerInformationServiceWrapper, fakeConfigManager);

            // Assert
            var customerAccounts = new List<string>
            {
                "0239200001",
                "0239200002"
            };

            List<CustomerAccount> foundAccounts = customerAccountService.GetCustomerAccount(customerAccounts);

            // Act
            foundAccounts.Count.ShouldEqual(1);
            foundAccounts[0].CurrentTariff.TariffType.ShouldEqual(expectedTariffType);
        }

        private static GetCustomerAccountsResponse GetFakeCustomerAccountsResponse(string servicePlanDescription = "")
        {
            var response = new GetCustomerAccountsResponse
            {
                CustomerAccounts = new List<CustomerAccountsType>
                {
                    new CustomerAccountsType
                    {
                        CustomerAccountStatus = CustomerAccountStatusType.Found,
                        CustomerAccountNumber = "0239200001",

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
                                        ServicePlanDescription = servicePlanDescription,
                                        Servicetype = ServiceTypeType.electricity
                                    }
                                }
                            }
                        }
                    },
                    new CustomerAccountsType
                    {
                        CustomerAccountStatus = CustomerAccountStatusType.NotFound,
                        CustomerAccountNumber = "0239200002"
                    }
                }
            };

            if (servicePlanDescription == "NULL")
            {
                response.CustomerAccounts[0].Sites[0].Services.RemoveAt(0);
            }

            return response;
        }
    }
}