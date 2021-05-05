namespace Products.Tests.Energy.ControllerTests
{
    using System.Collections.Generic;
    using ControllerHelpers.Energy;
    using Helpers;
    using NUnit.Framework;
    using Products.Model.Constants;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Products.Tests.Common.Fakes;
    using Should;

    public class StepCounterControllerTests
    {
        [TestCaseSource(nameof(GetTestCasesForStepCounter))]
        public void ShouldDisplayCorrectStepCounterForFirstSection(EnergyCustomer customer, string pageName, string expectedStepCounter)
        {
            // Arrange
            var fakeSessionManager = new FakeSessionManager();
            fakeSessionManager.SessionObject.Add(SessionKeys.EnergyCustomer, customer);

            var controllerHelper = new ControllerHelperFactory()
                .WithSessionManager(fakeSessionManager)
                .Build<SignUpControllerHelper>();

            // Act
            string stepCounter = controllerHelper.GetStepCounter(pageName);

            //Assert
            stepCounter.ShouldEqual(expectedStepCounter);
        }

        private static IEnumerable<TestCaseData> GetTestCasesForStepCounter()
        {
            // Group 1
            yield return new TestCaseData(new EnergyCustomer(), "EnterPostcode", "Step 1 of 7");
            yield return new TestCaseData(new EnergyCustomer(), "SelectAddress", "Step 2 of 7");
            yield return new TestCaseData(new EnergyCustomer(), "SelectFuel", "Step 3 of 7");
            yield return new TestCaseData(new EnergyCustomer(), "PaymentMethod", "Step 4 of 7");
            yield return new TestCaseData(new EnergyCustomer { SelectedFuelType = FuelType.Electricity }, "MeterType", "Step 5 of 7");
            yield return new TestCaseData(new EnergyCustomer { SelectedFuelType = FuelType.Electricity }, "SmartMeter", "Step 6 of 7");
            yield return new TestCaseData(new EnergyCustomer { SelectedFuelType = FuelType.Electricity }, "EnergyUsage", "Step 7 of 7");
            yield return new TestCaseData(new EnergyCustomer { SelectedFuelType = FuelType.Gas }, "SmartMeter", "Step 5 of 6");
            yield return new TestCaseData(new EnergyCustomer { SelectedFuelType = FuelType.Gas }, "EnergyUsage", "Step 6 of 6");

            // C AND C Journey
            yield return new TestCaseData(new EnergyCustomer(), "EnterPostcode", "Step 1 of 7");
            yield return new TestCaseData(new EnergyCustomer(), "SelectAddress", "Step 2 of 7");
            yield return new TestCaseData(new EnergyCustomer(), "SelectFuel", "Step 3 of 7");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoNonSmart() }, "SelectFuel", "Step 3 of 7");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoNonSmart(), SelectedPaymentMethod = PaymentMethod.PayAsYouGo }, "EnergyUsage", "Step 4 of 4");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1() }, "SmartMeterFrequency", "Step 5 of 6");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1(), SelectedFuelType = FuelType.Gas }, "PaymentMethod", "Step 4 of 6");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1(), SelectedFuelType = FuelType.Gas }, "SmartMeterFrequency", "Step 5 of 6");
            yield return new TestCaseData(new EnergyCustomer { MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1(), SelectedFuelType = FuelType.Gas }, "EnergyUsage", "Step 6 of 6");

            // Group 2
            // DD            
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit }, "PersonalDetails", "Step 1 of 5");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit }, "ContactDetails", "Step 2 of 5");
            // DD + Profile Doesn't Exist
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit }, "OnlineAccount", "Step 3 of 5");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit }, "BankDetails", "Step 4 of 5");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit }, "ViewSummary", "Step 5 of 5");
            // DD + Profile Exists
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                ProfileExists = true
            }, "BankDetails", "Step 3 of 4");
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
                ProfileExists = true
            }, "ViewSummary", "Step 4 of 4");

            // QC            
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.Quarterly }, "PersonalDetails", "Step 1 of 4");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.Quarterly }, "ContactDetails", "Step 2 of 4");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.Quarterly }, "OnlineAccount", "Step 3 of 4");
            // QC + Profile Doesn't Exist
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.Quarterly,
                ProfileExists = false
            }, "OnlineAccount", "Step 3 of 4");
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.Quarterly,
                ProfileExists = false
            }, "ViewSummary", "Step 4 of 4");
            // QC + Profile Exists
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.Quarterly,
                ProfileExists = true
            }, "ViewSummary", "Step 3 of 3");

            // Pre-pay
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.PayAsYouGo }, "PersonalDetails", "Step 1 of 4");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.PayAsYouGo }, "ContactDetails", "Step 2 of 4");
            yield return new TestCaseData(new EnergyCustomer { SelectedPaymentMethod = PaymentMethod.PayAsYouGo }, "OnlineAccount", "Step 3 of 4");
            // Pre-pay + Profile Doesn't Exist
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                ProfileExists = false
            }, "OnlineAccount", "Step 3 of 4");
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                ProfileExists = false
            }, "ViewSummary", "Step 4 of 4");
            // Pre-pay + Profile Exists
            yield return new TestCaseData(new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.PayAsYouGo,
                ProfileExists = true
            }, "ViewSummary", "Step 3 of 3");
        }
    }
}
