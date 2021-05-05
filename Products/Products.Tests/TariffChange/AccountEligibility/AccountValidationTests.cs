namespace Products.Tests.TariffChange.AccountEligibility
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Models;
    using Fakes.Services;
    using Helpers;
    using Model.Enums;
    using Model.TariffChange;
    using Model.TariffChange.Customers;
    using NUnit.Framework;
    using ServiceWrapper.AnnualEnergyReviewService;
    using Should;
    using Web.Areas.TariffChange.Controllers;

    public class AccountValidationTests
    {
        [TestCase(true, true, "When account has a single active energy service account then account is eligible")]
        [TestCase(false, false, "When account does not have a single active energy service account then account is ineligible")]
        public void AccountIneligibleIfNotSingleActiveEnergyServiceAccount(bool hasSingleActiveEnergyServiceAccount, bool isValid, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData();
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            customerAccount.SiteDetails.HasSingleActiveEnergyServiceAccount = hasSingleActiveEnergyServiceAccount;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("Y", false, "If payment plan special marker exists account is ineligible")]
        [TestCase("N", true, "If payment plan special marker does not exist account is eligible")]
        public void AccountIneligibleIfPaymentPlanSpecialInterestMarkerExists(string paymentPlanSpecialMarkerExists, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["PPSpecialInt"] = paymentPlanSpecialMarkerExists;
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("Y", false, "If Mixed SSE brands at same site account is ineligible")]
        [TestCase("N", true, "If single SSE brand at same site account is eligible")]
        [TestCase("", true, "No response signifies single fuel so therefore not mixed brands and is eligible.")]
        public void AccountIneligibleWhenMixedSseBrandsAtSameSite(string mixedBrandsAtSite, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["MixedBrands"] = mixedBrandsAtSite;
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("N", false, "If account is associated to multiple sites account is ineligible")]
        [TestCase("Y", true, "If account is associated to a single site account is eligible")]
        public void AccountIneligibleWhenNotAssociatedToSameSite(string isSingleSite, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["SameSite"] = isSingleSite;
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("Y", true, "If MixedBrands blank and SameSite Y then account is eligible.")]
        [TestCase("N", true, "If MixedBrands blank and SameSite N then account is eligible.")]
        public void AccountEligibleIfMixedBrandsIsBlankRegardlessOfSameSiteValue(string isSingleSite, bool isValid, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var fakeAerData = new FakeAERData();
            fakeAerData.CustomerAccountVariables["MixedBrands"] = string.Empty;
            fakeAerData.CustomerAccountVariables["SameSite"] = isSingleSite;
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });

            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility()).ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase(true, true, true, "Customer is eligible when both accounts are eligible.")]
        [TestCase(true, false, false, "Customer is ineligible when the second account is ineligible.")]
        [TestCase(false, true, false, "Customer is ineligible when the first account is ineligible.")]
        [TestCase(false, false, false, "Customer is ineligible when both accounts are ineligible.")]
        public void DualFuelCustomerEligibleIfBothAccountsAreEligible(bool isFirstAccountEligible, bool isSecondAccountEligible, bool isCustomerEligible, string description)
        {
            // Arrange
            // ReSharper disable once UseObjectOrCollectionInitializer
            var accountOne = new FakeAERData();
            var accountTwo = new FakeAERData { CustomerAccountNumber = "2222222226" };

            if (!isFirstAccountEligible)
            {
                accountOne.CustomerAccountVariables["AERPendingTriggers"] = "01";
            }

            if (!isSecondAccountEligible)
            {
                accountTwo.CustomerAccountVariables["AERPendingTriggers"] = "01";
            }

            var fakeAnnualEnergyReviewServiceWrapper =
                new FakeAnnualEnergyReviewServiceWrapper(accountOne.CustomerAccountNumber, new[] { accountOne, accountTwo });
            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "2222222226",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Same Tariff Name"
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = accountOne.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = "Same Tariff Name";
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isCustomerEligible ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isCustomerEligible)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("ATGA", false, "Atlantic should be ineligible.")]
        [TestCase("ATSE", false, "Atlantic should be ineligible.")]
        [TestCase("ATSW", false, "Atlantic should be ineligible.")]
        [TestCase("ATHE", false, "Atlantic should be ineligible.")]
        [TestCase("ATTE", false, "Atlantic should be ineligible.")]
        [TestCase("SWAL", true, "SWALEC should be eligible.")]
        public void CustomersAccountWithAtlanticBrandCodesAreIneligible(string brandCode, bool isValid, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData();
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            customerAccount.CurrentTariff.BrandCode = brandCode;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "CustomerAccountIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase(paymentMethodTypePaymentMethodCode.QC, null, true, "When no payment method, customer should be eligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.DDV, null, true, "When payment method is variable direct debit, customer should be eligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.DDB, "20", true, "When payment method is monthly direct debit, customer should be eligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.DDB, null, false, "When payment method is non monthly direct debit, customer should be ineligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.PPT, null, false, "When payment method is prepayment, customer should be ineligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.EXC, null, false, "When payment method is swipe card, PGO or DSS, customer should be ineligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.PGO, null, false, "When payment method is PGO (should never occur as should return EXC), customer should be ineligible.")]
        [TestCase(paymentMethodTypePaymentMethodCode.STO, null, false, "When payment method is standing order, customer should be ineligible.")]
        public void PaymentMethodRouting(paymentMethodTypePaymentMethodCode paymentMethod, string collectionDay, bool isValid, string description)
        {
            // Arrange
            var fakeAerData = new FakeAERData { PaymentMethodCode = paymentMethod, CollectionDay = collectionDay };
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "PaymentMethodIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        [TestCase("SOR", false, "When payment method is SOR, customer should be ineligible.")]
        [TestCase("", true, "When payment method is not SOR, customer should be eligible.")]
        public void PaymentMethodRoutingForCheckAerResponse(string paymentMethod, bool isValid, string description)
        {
            var fakeAerData = new FakeAERData { CustomerAccountVariables = { ["PaymentMethod"] = paymentMethod } };
            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(fakeAerData.CustomerAccountNumber, new[] { fakeAerData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = fakeAerData.CustomerAccountNumber;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            // Act + Assert
            TestHelper.GetResultUrlString(controller.CheckEligibility())
                .ShouldEqual(isValid ? "AvailableTariffs" : "PaymentMethodIneligible");
            if (isValid)
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.FalloutReasons.ShouldBeEmpty();
            }
            else
            {
                fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ShouldBeNull();
            }
        }

        //[Test]
        //public void IneligibleCustomersShouldClearSessionData()
        //{
        //    var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
        //    {
        //        Customer = new Customer()
        //    });

        //    var context = CreateHttpContext("user", "role");

        //    var controller = new ControllerFactory()
        //        .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
        //        .Build<AccountEligibilityController>();

        //    controller.ControllerContext = new ControllerContext(context.CurrentHandler, new RouteData(), controller);

        //    controller.ControllerContext = CreateHttpContext("user", "role");

        //    controller.CustomerAccountIneligible();

        //    fakeInMemoryTariffChangeSessionService.GetJourneyDetails().ShouldBeNull();
        //}

        [Test]
        public void DirectAccessShouldRedirectToLandingPage()
        {
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService();

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .Build<AccountEligibilityController>();

            ActionResult result = controller.CheckEligibility();

            result.ShouldNotBeNull();
            result.ShouldBeType<RedirectToRouteResult>();
            ((RedirectToRouteResult) result).RouteValues["action"].ShouldEqual("IdentifyCustomer");
            fakeInMemoryTariffChangeSessionService.JourneyDetails.ShouldBeNull();
        }

        [Test]
        public void ShouldFalloutWhenDualFuelCustomersGetFollowOnServicePlanIDForOneFuelAndNotForTheOther()
        {
            //Arrange
            var accountOne = new FakeAERData { FollowOnServicePlanId = "ME1121" };
            var accountTwo = new FakeAERData { CustomerAccountNumber = "2222222226" };


            var fakeAnnualEnergyReviewServiceWrapper =
                new FakeAnnualEnergyReviewServiceWrapper(accountOne.CustomerAccountNumber, new[] { accountOne, accountTwo });
            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "2222222226",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Same Tariff Name"
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = accountOne.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = "Same Tariff Name";
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            //Act
            ActionResult result = controller.CheckEligibility();

            //Assert
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("CustomerAccountIneligible");
            var dataLayer = controller.TempData["DataLayer"].ShouldBeType<List<FalloutReasonResult>>();
            dataLayer.Count.ShouldEqual(1);
            FalloutReasonResult falloutReasonResult = dataLayer[0];
            falloutReasonResult.FalloutReason.ShouldEqual(FalloutReason.Indeterminable);
            falloutReasonResult.FalloutDescription.ShouldEqual("Customer is dual-fuel with follow-on service plan for one fuel and not for the other.");
        }

        [Test]
        public void ShouldUpdateCustomerAccountsWithFollowOnServicePlanIDs()
        {
            //Arrange
            var accountOne = new FakeAERData { FollowOnServicePlanId = "ME1121" };
            var accountTwo = new FakeAERData { CustomerAccountNumber = "2222222226", FollowOnServicePlanId = "MG1121" };

            var fakeAnnualEnergyReviewServiceWrapper = new FakeAnnualEnergyReviewServiceWrapper(accountOne.CustomerAccountNumber, new[] { accountOne, accountTwo });
            var fakeMcisData = new FakeMCISData
            {
                CustomerAccountNumber = "2222222226",
                Postcode = "SO14 2FJ",
                ServicePlanDescription = "Same Tariff Name"
            };

            var fakeManageCustomerInformationServiceWrapper = new FakeManageCustomerInformationServiceWrapper(new[] { fakeMcisData });

            CustomerAccount customerAccount = DefaultDomainModel.ForEligibility();
            customerAccount.SiteDetails.AccountNumber = accountOne.CustomerAccountNumber;
            customerAccount.CurrentTariff.Name = "Same Tariff Name";
            customerAccount.CurrentTariff.FuelType = FuelType.Gas;
            var fakeInMemoryTariffChangeSessionService = new FakeInMemoryTariffChangeSessionService(new JourneyDetails
            {
                CustomerAccount = customerAccount
            });
            var fakeConfigManager = new FakeConfigManager();
            fakeConfigManager.AddConfiguration("AtlasAnnouncementDate", "30/04/2018 9:00");

            var controller = new ControllerFactory()
                .WithTariffChangeSessionService(fakeInMemoryTariffChangeSessionService)
                .WithAnnualEnergyReviewServiceWrapper(fakeAnnualEnergyReviewServiceWrapper)
                .WithManageCustomerInformationServiceWrapper(fakeManageCustomerInformationServiceWrapper)
                .WithConfigManager(fakeConfigManager)
                .Build<AccountEligibilityController>();

            //Act
            controller.CheckEligibility();

            //Assert
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.GasAccount.FollowOnTariffServicePlanID.ShouldEqual(accountOne.FollowOnServicePlanId);
            fakeInMemoryTariffChangeSessionService.JourneyDetails.Customer.ElectricityAccount.FollowOnTariffServicePlanID.ShouldEqual(accountTwo.FollowOnServicePlanId);
        }
    }
}