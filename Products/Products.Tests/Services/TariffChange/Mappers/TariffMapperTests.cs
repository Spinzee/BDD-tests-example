namespace Products.Tests.Services.TariffChange.Mappers
{
    using System.Collections.Generic;
    using Model.Energy;
    using Model.Enums;
    using NUnit.Framework;
    using Service.TariffChange.Mappers;
    using Should;
    using Tests.Common.Helpers;
    using Tests.TariffChange.Helpers;
    using Tariff = Model.TariffChange.Tariffs.Tariff;

    [TestFixture]
    public class TariffMapperTests
    {
        [TestCase("ME001", "SingleFuelElectric", 2)]
        [TestCase("MG001", "SingleFuelGas", 2)]
        [TestCase("ME002", "DualFuel", 2)]
        public static void MapTariffsToCMSEnergyContentTariffsMapsCorrectly(string servicePlanId, string tariffName, int count)
        {
            // Arrange
            List<Tariff> tariffs = FakeTariffStub.GetFakeTariffList();
            List<CMSEnergyContent> cmsEnergyContents = FakeContentManagementStub.GetCMSEnergyContentList();

            // Act
            List<Tariff> mappedModelList = TariffMapper.MapTariffsToCMSEnergyContentTariffs(tariffs, cmsEnergyContents, servicePlanId);

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(count);
            mappedModelList[0].DisplayName.ShouldEqual(tariffName);
            mappedModelList[1].DisplayName.ShouldEqual("Standard");
        }

        [Test]
        public static void MapTariffsToCMSEnergyContentTariffsReturnsCurrentTariffOnlyWhenNoMatchesOnContent()
        {
            // Arrange
            List<Tariff> tariffs = FakeTariffStub.GetFakeTariffList();
            List<CMSEnergyContent> cmsEnergyContents = FakeContentManagementStub.GetCMSEnergyContentList();

            // Act
            List<Tariff> mappedModelList = TariffMapper.MapTariffsToCMSEnergyContentTariffs(tariffs, cmsEnergyContents, "ME043");

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(1);
            mappedModelList[0].DisplayName.ShouldEqual("Standard");
        }

        [TestCase(FuelType.Dual, 1)]
        [TestCase(FuelType.Electricity, 0)]
        [TestCase(FuelType.Gas, 0)]
        [TestCase(FuelType.None, 0)]
        public static void MapProductsToFilteredFixAndDriveProductsMapsCorrectly(FuelType selectedFuelType, int expectedProductCount)
        {
            // Arrange
            List<Tariff> tariffs = FakeTariffStub.GetFakeFixAndDriveTariffList();
            bool isDualFuel = selectedFuelType == FuelType.Dual;

            // Act
            List<Tariff> mappedModelList = TariffMapper.MapTariffsToFilteredFixAndDriveTariffs(isDualFuel, tariffs);

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(expectedProductCount);
        }
    }
}