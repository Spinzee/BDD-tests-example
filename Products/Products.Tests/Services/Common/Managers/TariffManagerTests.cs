namespace Products.Tests.Services.Common.Managers
{
    using System.Collections.Generic;
    using Core.Configuration.Settings;
    using NUnit.Framework;
    using Service.Common.Managers;
    using Should;
    using Tests.Common.Fakes;

    [TestFixture]
    public class TariffManagerTests
    {
        [TestCase("ABC123", true)]
        [TestCase("XYZ123", false)]
        public void IsSmartReturnsCorrectValue(string servicePlanId, bool expectedResult)
        {
            // Arrange
            var fakeConfigurationSettings = new FakeConfigurationSettings
            {
                TariffManagementSettings = new TariffManagementSettings
                {
                    SmartTariffSettings = new SmartTariffSettings { ServicePlanIds = new List<string> { "ABC123" } }
                }
            };

            var tariffManager = new TariffManager(new FakeConfigManager(), fakeConfigurationSettings);

            // Act
            bool result = tariffManager.IsSmart(servicePlanId);

            //Assert
            result.ShouldEqual(expectedResult);
        }
    }
}
