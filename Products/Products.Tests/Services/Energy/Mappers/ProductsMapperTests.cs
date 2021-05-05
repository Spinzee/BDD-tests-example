namespace Products.Tests.Services.Energy.Mappers
{
    using System.Collections.Generic;
    using Core;
    using Model.Energy;
    using Model.Enums;
    using NUnit.Framework;
    using Service.Energy.Mappers;
    using Should;
    using Tests.Common.Helpers;
    using Tests.Energy.Helpers;
    using Tests.TariffChange.Fakes.Managers;

    [TestFixture]
    public class ProductsMapperTests
    {
        [Test]
        public static void MapProductsToCMSEnergyContentProductsMapsCorrectly()
        {
            // Arrange
            List<Product> products = FakeProductsStub.GetFakeProductList();
            List<CMSEnergyContent> cmsEnergyContents = FakeContentManagementStub.GetCMSEnergyContentList();

            // Act
            List<Product> mappedModelList = ProductsMapper.MapProductsToCMSEnergyContentProducts(products, cmsEnergyContents, new FakeTariffManager());

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(1);
            mappedModelList[0].DisplayName.ShouldEqual("Standard");
        }

        [Test]
        public static void MapProductsToCMSEnergyContentProductsReturnsEmptyListWhenNoMatches()
        {
            // Arrange
            List<Product> products = FakeProductsStub.GetFakeProductList();
            List<CMSEnergyContent> cmsEnergyContents = FakeContentManagementStub.GetCMSEnergyContentList();
            products[0].ServicePlanInvoiceDescription = "blah";

            // Act
            List<Product> mappedModelList = ProductsMapper.MapProductsToCMSEnergyContentProducts(products, cmsEnergyContents, new FakeTariffManager());

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(0);
        }

        [Test]
        public static void MapProductsToCMSEnergyContentProductsReturnsCorrectListWhenNoMatchesOnTariffNameAndTariffHasATariffGroup()
        {
            // Arrange
            List<Product> products = FakeProductsStub.GetFakeProductList();
            List<CMSEnergyContent> cmsEnergyContents = FakeContentManagementStub.GetCMSEnergyContentList();
            products[0].ServicePlanInvoiceDescription = "Non Matched";
            var tariffManager = new FakeTariffManager { TariffGroupMappings = new Dictionary<string, string> { { "ME029", TariffGroup.FixAndFibre.ToString() } } };

            // Act
            List<Product> mappedModelList = ProductsMapper.MapProductsToCMSEnergyContentProducts(products, cmsEnergyContents, tariffManager);

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(1);
            mappedModelList[0].DisplayName.ShouldEqual("Non Matched");
        }

        [TestCase(FuelType.Dual, 1)]
        [TestCase(FuelType.Electricity, 0)]
        [TestCase(FuelType.Gas, 0)]
        [TestCase(FuelType.None, 0)]
        public static void MapProductsToFilteredFixAndDriveProductsMapsCorrectly(FuelType selectedFuelType, int expectedProductCount)
        {
            // Arrange
            List<Product> products = FakeProductsStub.GetFakeFixAndDriveProductList();
            var tariffManager = new FakeTariffManager { TariffGroupMappings = new Dictionary<string, string> { { "ME123", "FixAndDrive" } } };

            // Act
            List<Product> mappedModelList = ProductsMapper.MapProductsToFilteredFixAndDriveProducts(selectedFuelType, products, tariffManager);

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(expectedProductCount);
        }
    }
}
