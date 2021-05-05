namespace Products.Tests.HomeServices
{
    using System.Collections.Generic;
    using Model.Constants;
    using NUnit.Framework;
    using Products.Model.HomeServices;
    using Products.Service.HomeServices;
    using Products.Tests.Common.Fakes;
    using Should;

    public class StepCounterTests
    {
        [TestCaseSource(nameof(GetTestCasesForStepCounter))]
        public void ShouldDisplayCorrectStepCounterForFirstSection(HomeServicesCustomer customer, string pageName, string expectedStepCounter)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.HomeServicesCustomer, customer);
            var stepCounterService = new StepCounterService(fakeSessionManager);

            // Act
            string stepCounter = stepCounterService.GetStepCounter(pageName);

            // Assert
            stepCounter.ShouldEqual(expectedStepCounter);
        }

        private static IEnumerable<TestCaseData> GetTestCasesForStepCounter()
        {
            yield return new TestCaseData(new HomeServicesCustomer(), "Postcode", "Step 1 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false } , "CoverDetails", "Step 2 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false }, "PersonalDetails", "Step 3 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false }, "SelectAddress", "Step 4 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false }, "ContactDetails", "Step 5 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false }, "BankDetails", "Step 6 of 7");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = false }, "Summary", "Step 7 of 7");

            yield return new TestCaseData(new HomeServicesCustomer(), "LandlordPostcode", "Step 1 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "CoverDetails", "Step 2 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "PersonalDetails", "Step 3 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "SelectAddress", "Step 4 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "LandlordBillingPostcode", "Step 5 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "SelectBillingAddress", "Step 6 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "ContactDetails", "Step 7 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "BankDetails", "Step 8 of 9");
            yield return new TestCaseData(new HomeServicesCustomer { IsLandlord = true }, "Summary", "Step 9 of 9");
        }
    }
}
