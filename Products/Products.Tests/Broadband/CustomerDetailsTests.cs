namespace Products.Tests.Broadband
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Fakes;
    using Common.Helpers;
    using Fakes.Services;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Should;
    using Web.Areas.Broadband.Controllers;
    using WebModel.ViewModels.Broadband;
    using WebModel.ViewModels.Common;
    using FakeSessionManager = Fakes.Services.FakeSessionManager;

    [TestFixture]
    public class CustomerDetailsControllerTests
    {
        private static IEnumerable<TestCaseData> MissingPersonalDetails()
        {
            yield return new TestCaseData(null, "", "", "", "", "");
            yield return new TestCaseData(Titles.Mr, "", "", "", "", "");
            yield return new TestCaseData(null, "Test", "", "", "", "");
            yield return new TestCaseData(null, "", "Test", "", "", "");
            yield return new TestCaseData(null, "", "", "Test", "", "");
            yield return new TestCaseData(Titles.Mr, "Test!", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test\"", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test£", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test$", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test %", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test ^", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test &", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test *", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test(", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test)", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test@", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test¬", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test`", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test{", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test}", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test[", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test]", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test =", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test *", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test /", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test +", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test ?", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test\\", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test\\|", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test;", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test:", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test#", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test~", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test <", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test >", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "greaterthanfiftytwocharsinlengthisfartoolongforafirstname", "Test", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test!", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test\"", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test£", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test$", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test %", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test ^", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test &", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test *", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test(", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test)", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test@", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test¬", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test`", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test{", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test}", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test[", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test]", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test =", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test *", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test /", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test +", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test ?", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test\\", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test\\|", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test;", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test:", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test#", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test~", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test <", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "Test >", "01", "03", "1990");
            yield return new TestCaseData(Titles.Mr, "Test", "greaterthanfiftytwocharsinlengthisfartoolongforasurname", "01", "03", "1990");
        }

        private static IEnumerable<TestCaseData> ValidPersonalDetails()
        {
            DateTime dobForNonScottishPostcodes = DateTime.Today.AddYears(-18);
            DateTime dobForScottishPostcodes = DateTime.Today.AddYears(-16);
            yield return new TestCaseData(Titles.Mr, "Test", "Test", dobForNonScottishPostcodes.Day, dobForNonScottishPostcodes.Month,
                dobForNonScottishPostcodes.Year, false, "RG1 1AA", "Non Scottish postcodes");
            yield return new TestCaseData(Titles.Mr, "Test", "Test", dobForNonScottishPostcodes.Day, dobForNonScottishPostcodes.Month,
                dobForScottishPostcodes.Year, true, "G1 1AA", "Scottish postcodes");
        }


        private static IEnumerable<TestCaseData> InvalidDateOfBirth()
        {
            yield return new TestCaseData("99", "01", "2010");
            yield return new TestCaseData("01", "99", "2010");
            yield return new TestCaseData("29", "02", "1997");
            yield return new TestCaseData("29", "02", "97");
            yield return new TestCaseData("29", "02", "1234");
        }

        private static IEnumerable<TestCaseData> UnderAgeTestData()
        {
            yield return new TestCaseData(DateTime.Today.AddYears(-18).AddDays(1), "RG1 3AB", false, "Non Scottish postcodes");
            yield return new TestCaseData(DateTime.Today.AddYears(-16).AddDays(1), "AB11 6BR", true, "Scottish postcodes");
        }

        [Test, TestCaseSource(nameof(InvalidDateOfBirth))]
        public void IfDateOfBirthIsInvalidDateRemainOnPersonalDetailsPage(string dateOfBirthDay, string dateOfBirthMonth, string dateOfBirthYear)
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG1 3AB" } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear
            };

            controller.ValidateViewModel(personalDetails);
            ActionResult result = controller.Submit(personalDetails);
            ((ViewResult) result).ViewName.ShouldEqual("~/Areas/Broadband/Views/CustomerDetails/PersonalDetails.cshtml");
        }

        [Test, TestCaseSource(nameof(MissingPersonalDetails))]
        public void IfPersonalDetailsAreInvalidRemainOnPersonalDetailsPage(
            Titles title,
            string firstName,
            string lastName,
            string dateOfBirthDay,
            string dateOfBirthMonth,
            string dateOfBirthYear
        )
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG1 3AB" } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = title,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirthDay = dateOfBirthDay,
                DateOfBirthMonth = dateOfBirthMonth,
                DateOfBirthYear = dateOfBirthYear
            };

            controller.ValidateViewModel(personalDetails);
            ActionResult result = controller.Submit(personalDetails);
            ((ViewResult) result).ViewName.ShouldEqual("~/Areas/Broadband/Views/CustomerDetails/PersonalDetails.cshtml");
        }

        [Test, TestCase(Titles.Mr, "Test", "Test", "01/02/1990")]
        public void IfSessionIsNotValidThenShowErrorPageAndLogError(
            Titles title,
            string firstName,
            string lastName,
            string dateOfBirth
        )
        {
            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = title,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth
            };

            var controller = new ControllerFactory()
                .WithSessionManager(new FakeSessionManager { SessionObject = new object() })
                .Build<CustomerDetailsController>();

            var ex = Assert.Throws<InvalidCastException>(() => controller.Submit(personalDetails));
            ex.Message.ShouldNotBeNull();
            ex.Message.ShouldEqual(@"Unable to cast object of type 'System.Object' to type 'Products.Model.Broadband.BroadbandJourneyDetails'.");
        }

        [Test, TestCaseSource(nameof(ValidPersonalDetails))]
        public void IfValidPersonalDetailsAreSuppliedProceedToContactDetailsPage(
            Titles title,
            string firstName,
            string lastName,
            int dateOfBirthDay,
            int dateOfBirthMonth,
            int dateOfBirthYear,
            bool isScottishPostcode,
            string postcode,
            string description
        )
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = postcode } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = title,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirthDay = dateOfBirthDay.ToString("00"),
                DateOfBirthMonth = dateOfBirthMonth.ToString("00"),
                DateOfBirthYear = dateOfBirthYear.ToString(),
                DateOfBirth = $"{dateOfBirthDay}/{dateOfBirthMonth}/{dateOfBirthYear}",
                IsScottishPostcode = isScottishPostcode
            };

            ActionResult result = controller.Submit(personalDetails);


            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");


            result.ShouldNotBeNull().ShouldBeType<RedirectToRouteResult>()
                .RouteValues["action"].ShouldEqual("ContactDetails");

            broadbandJourneyDetails.Customer.PersonalDetails.FirstName.ShouldEqual(personalDetails.FirstName);
            broadbandJourneyDetails.Customer.PersonalDetails.LastName.ShouldEqual(personalDetails.LastName);
            broadbandJourneyDetails.Customer.PersonalDetails.Title.ShouldEqual(personalDetails.Titles.ToString());
            broadbandJourneyDetails.Customer.PersonalDetails.DateOfBirth.ShouldEqual(
                $"{personalDetails.DateOfBirthDay}/{personalDetails.DateOfBirthMonth}/{personalDetails.DateOfBirthYear}");
        }

        [Test]
        [TestCase(true, "If SSE Customer display connection fee with strike through")]
        [TestCase(false, "If not SSE customer display connection fee")]
        public void ShouldDisplayConnectionFeeAndSurchargeWithStrikeThroughForSSECustomer(bool isSSECustomer, string description)
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = isSSECustomer, PostcodeEntered = "RG1 3AB" } }
            };
            fakeSessionManager.SetListOfProducts();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            packageController.SelectedPackage(new SelectedPackageViewModel(), "ABC");
            ActionResult result = controller.PersonalDetails();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
        }

        [Test]
        [TestCase("TransferYourNumber", "CustomerDetails")]
        [TestCase("TransferYourNumber", "CustomerDetails")]
        public void ShouldDisplayConnectionFeeAndSurchargeWithStrikeThroughForSSECustomer(string actionName, string controllerName)
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false, PostcodeEntered = "RG1 3AB" } }
            };
            fakeSessionManager.SetListOfProducts();

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            packageController.SelectedPackage(new SelectedPackageViewModel(), "ABC");
            ActionResult result = controller.PersonalDetails();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.BackChevronViewModel.ActionName.ShouldEqual(actionName);
            viewModel.BackChevronViewModel.ControllerName.ShouldEqual(controllerName);
        }

        [TestCase(true, "BB18_LR", "Line Rental Only", "£0.00", "Unlimited Broadband", "£21.00", "£21.00", "£0.00", "£21.00", "£50.00", "£5.00")]
        [TestCase(true, "FIBRE18_LR", "Line Rental Only", "£0.00", "Unlimited Fibre", "£28.00", "£28.00", "£0.00", "£28.00", "£50.00", "£5.00")]
        [TestCase(true, "FP18_LR", "Line Rental Only", "£0.00", "Unlimited Fibre Plus", "£35.00", "£35.00", "£0.00", "£35.00", "£50.00", "£5.00")]
        [TestCase(false, "BB18_LR", "Line Rental Only", "£0.00", "Unlimited Broadband", "£21.00", "£21.00", "£50.00", "£21.00", "£50.00", "£5.00")]
        [TestCase(false, "FIBRE18_LR", "Line Rental Only", "£0.00", "Unlimited Fibre", "£28.00", "£28.00", "£50.00", "£28.00", "£50.00", "£5.00")]
        [TestCase(false, "FP18_LR", "Line Rental Only", "£0.00", "Unlimited Fibre Plus", "£35.00", "£35.00", "£50.00", "£35.00", "£50.00", "£5.00")]
        [TestCase(false, "FIBRE18_LR", "Line Rental Only", "£0.00", "Unlimited Fibre", "£28.00", "£28.00", "£50.00", "£28.00", "£50.00", "£5.00")]
        [TestCase(false, "FIBRE_ANY18", "Anytime", "£7.00", "Unlimited Fibre", "£28.00", "£35.00", "£50.00", "£35.00", "£50.00", "£5.00")]
        [TestCase(false, "FIBRE_AP18", "Anytime Plus", "£12.00", "Unlimited Fibre", "£28.00", "£40.00", "£50.00", "£40.00", "£50.00", "£5.00")]
        [Test]
        public void ShouldDisplayCorrectPricesInYourPriceBoxWhenAPackageIsSelected(bool isSSECustomer, string productCode,
            string telName, string telCost, string bbName, string bbCost,
            string monthlyCost, string oneOffCost, string firstBillTotal,
            string connectionCharge, string surcharge)
        {
            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails
                {
                    Customer = new Customer { IsSSECustomer = isSSECustomer, PostcodeEntered = "RG1 3AB" }
                },
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession()
            };

            ControllerFactory controllerFactory = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionManager);

            var packageController = controllerFactory.Build<PackagesController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            packageController.SelectedPackage(new SelectedPackageViewModel(), productCode);
            ActionResult result = controller.PersonalDetails();

            var view = result.ShouldBeType<ViewResult>();
            var viewModel = view.ViewData.Model.ShouldBeType<PersonalDetailsViewModel>();
            viewModel.YourPriceViewModel.IsExistingCustomer.ShouldEqual(isSSECustomer);
            viewModel.YourPriceViewModel.TelName.ShouldEqual(telName);
            viewModel.YourPriceViewModel.TelCost.ToString("C").ShouldEqual(telCost);
            viewModel.YourPriceViewModel.BroadbandName.ShouldEqual(bbName);
            viewModel.YourPriceViewModel.BroadbandCost.ToString("C").ShouldEqual(bbCost);
            viewModel.YourPriceViewModel.MonthlyCost.ToString("C").ShouldEqual(monthlyCost);
            viewModel.YourPriceViewModel.OneOffCost.ToString("C").ShouldEqual(oneOffCost);
            viewModel.YourPriceViewModel.FirstBillTotal.ToString("C").ShouldEqual(firstBillTotal);
            viewModel.YourPriceViewModel.ConnectionCharge.ToString("C").ShouldEqual(connectionCharge);
            viewModel.YourPriceViewModel.Surcharge.ToString("C").ShouldEqual(surcharge);
        }


        [Test]
        [TestCase(null, "Step 3 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 3 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnContactDetailsPage(string cli, string stepCount, string description)
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controllerFactory = new ControllerFactory();

            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            //Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult result = controller.ContactDetails();

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [Test]
        [TestCase(null, "Step 3 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 3 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnContactDetailsPageIfModelIsNotValid(string cli, string stepCount, string description)
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controllerFactory = new ControllerFactory();

            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            var model = new ContactDetailsViewModel();

            //Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            controller.ValidateViewModel(model);
            ActionResult result = await controller.SubmitContactDetails(model);
            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [Test]
        public void ShouldDisplayCorrectStepCountOnKeepYourNumberPage()
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            //Act
            ActionResult result = controller.TransferYourNumber();

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual("Step 1 of 5");
        }

        [Test]
        public void ShouldDisplayCorrectStepCountOnKeepYourNumberPageIfModelIsNotValid()
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new TransferYourNumberViewModel
            {
                KeepExistingNumber = true
            };

            //Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.TransferYourNumber(model);

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual("Step 1 of 5");
        }

        [TestCase("0123456789", true)]
        [TestCase(null, false)]
        public async Task ShouldDisplayCorrectStepCountOnKeepYourNumberPages(string originalCliEntered, bool expectedValue)
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };

            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .WithBroadbandProductsServiceWrapper(new FakeBroadbandProductsServiceWrapper { AddressResult = AddressResult.AllAddresses })
                .WithNewBTLineAvailabilityServiceWrapper(new FakeNewBTLineAvailabilityServiceWrapper { CLI = originalCliEntered })
                .Build<LineCheckerController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO91BH"
            };

            //Act
            await controller.Submit(lineCheckerViewModel);
            ActionResult resultSelectAddressGet = await controller.SelectAddress();
            var viewResult = resultSelectAddressGet.ShouldBeType<ViewResult>();
            var viewModel = viewResult.Model.ShouldBeType<SelectAddressViewModel>();
            // ReSharper disable once PossibleNullReferenceException
            viewModel.SelectedAddressId = viewModel.Addresses.FirstOrDefault().Id;
            await controller.SelectAddress(viewModel);

            var controllerTransferNumber = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            //Act
            ActionResult resultTransferNumber = controllerTransferNumber.TransferYourNumber();

            //Assert
            resultTransferNumber.ShouldNotBeNull();
            var viewResultTransferNumber = resultTransferNumber.ShouldBeType<ViewResult>();
            var viewModelTransferNumber = viewResultTransferNumber.ViewData.Model.ShouldBeType<TransferYourNumberViewModel>();
            viewModelTransferNumber.IsSSECustomerCLI.ShouldEqual(expectedValue);
        }


        [Test]
        [TestCase(null, "Step 2 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 2 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnPersonalDetailsPage(string cli, string stepCount, string description)
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controllerFactory = new ControllerFactory();

            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            //Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult result = controller.PersonalDetails();

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }

        [Test]
        [TestCase(null, "Step 2 of 5", "no cli was provided, final step count should be 5")]
        [TestCase("12345789", "Step 2 of 5", "cli was provided, final step count should be 5")]
        public async Task ShouldDisplayCorrectStepCountOnPersonalDetailsPageIfModelIsNotValid(string cli, string stepCount, string description)
        {
            //Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controllerFactory = new ControllerFactory();

            var lineCheckerController = controllerFactory.WithSessionManager(fakeSessionManager).Build<LineCheckerController>();

            var controller = controllerFactory.Build<CustomerDetailsController>();

            var lineCheckerViewModel = new LineCheckerViewModel
            {
                PostCode = "PO9 1QH",
                PhoneNumber = cli
            };

            var model = new PersonalDetailsViewModel();

            //Act
            await lineCheckerController.Submit(lineCheckerViewModel);
            ActionResult result = controller.Submit(model);

            //Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            (viewResult.ViewBag.StepCounter as string).ShouldEqual(stepCount);
        }


        [Test]
        [TestCase(true, "01212121212")]
        [TestCase(false, null)]
        public void ShouldRedirectToPersonalDetailsIfModelIsValid(bool keepExistingNumber, string phoneNumber)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new TransferYourNumberViewModel
            {
                KeepExistingNumber = keepExistingNumber,
                PhoneNumber = phoneNumber
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.TransferYourNumber(model);

            //Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("PersonalDetails");
        }

        [Test, TestCaseSource(nameof(UnderAgeTestData))]
        public void ShouldRemainOnPersonalDetailsPageWhenCustomerIsUnderAge(DateTime dateOfBirth, string postcode, bool isScottishPostcode, string description)
        {
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = postcode } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var personalDetails = new PersonalDetailsViewModel
            {
                Titles = Titles.Dr,
                FirstName = "Test",
                LastName = "Test",
                DateOfBirthDay = dateOfBirth.Day.ToString(CultureInfo.InvariantCulture),
                DateOfBirthMonth = dateOfBirth.Month.ToString(CultureInfo.InvariantCulture),
                DateOfBirthYear = dateOfBirth.Year.ToString(CultureInfo.InvariantCulture),
                IsScottishPostcode = isScottishPostcode
            };

            ActionResult result = controller.Submit(personalDetails);
            result.ShouldNotBeNull()
                .ShouldBeType<ViewResult>()
                .ViewName.ShouldEqual("~/Areas/Broadband/Views/CustomerDetails/PersonalDetails.cshtml");
        }

        [Test]
        [TestCase(true, "Test")]
        [TestCase(true, null)]
        [TestCase(true, "12345678901234567890")]
        [TestCase(true, "07092022982")]
        [TestCase(true, "12345")]
        [TestCase(true, "03123456789")]
        [TestCase(true, "04123456789")]
        [TestCase(true, "05123456789")]
        [TestCase(true, "06123456789")]
        [TestCase(true, "07123456789")]
        [TestCase(true, "08123456789")]
        [TestCase(true, "09123456789")]
        [TestCase(true, "11123456789")]
        public void ShouldRequireAValidPhoneNumberIfCustomerSelectedKeepExistingNumber(bool keepExistingNumber, string phoneNumber)
        {
            // Arrange

            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { PostcodeEntered = "RG1 3AB" } }
            };
            var controller = new ControllerFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<CustomerDetailsController>();

            var model = new TransferYourNumberViewModel
            {
                KeepExistingNumber = keepExistingNumber,
                PhoneNumber = phoneNumber
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.TransferYourNumber(model);


            // Assert
            result.ShouldNotBeNull();
            var viewResult = result.ShouldBeType<ViewResult>();
            viewResult.ViewData.ModelState["PhoneNumber"].Errors.ShouldNotBeNull();
            viewResult.ViewData.ModelState["PhoneNumber"].Errors.Count.ShouldEqual(1);
            viewResult.ViewName.ShouldEqual("~/Areas/Broadband/Views/CustomerDetails/TransferYourNumber.cshtml");
        }

        [Test]
        [TestCase(true, "01212121212")]
        [TestCase(false, "")]
        public void ShouldSaveExistingNumberDetailsInSessionWhenModelIsValid(bool keepExistingNumber, string expectedPhoneNumber)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager
            {
                SessionObject = new BroadbandJourneyDetails { Customer = new Customer { IsSSECustomer = false } }
            };
            var controller = new ControllerFactory().WithSessionManager(fakeSessionManager).Build<CustomerDetailsController>();

            var model = new TransferYourNumberViewModel
            {
                KeepExistingNumber = keepExistingNumber,
                PhoneNumber = "01212121212"
            };

            // Act
            controller.ValidateViewModel(model);
            ActionResult result = controller.TransferYourNumber(model);
            var broadbandJourneyDetails = fakeSessionManager.GetSessionDetails<BroadbandJourneyDetails>("broadband_journey");

            //Assert
            result.ShouldNotBeNull();
            var redirectResult = result.ShouldBeType<RedirectToRouteResult>();
            redirectResult.RouteValues["action"].ShouldEqual("PersonalDetails");
            broadbandJourneyDetails.Customer.KeepExistingNumber.ShouldEqual(keepExistingNumber);
            broadbandJourneyDetails.Customer.CliNumber.ShouldEqual(expectedPhoneNumber);
        }
    }
}