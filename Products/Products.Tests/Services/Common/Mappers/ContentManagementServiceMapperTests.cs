namespace Products.Tests.Services.Common.Mappers
{
    using System.Collections.Generic;
    using Model.Common.CMSResponse;
    using Model.Energy;
    using NUnit.Framework;
    using Service.Mappers;
    using Should;
    using Tests.Common.Helpers;

    [TestFixture]
    public class ContentManagementServiceMapperTests
    {
        [TestCase("https://blah", "blah")]
        [TestCase("https://blah/blah.pdf", "blah.pdf")]
        [TestCase("https://blah/blah2/blah3.pdf", "blah3.pdf")]
        [TestCase("https://blah/blah2/blah3/blah4.pdf", "blah4.pdf")]
        [TestCase("https://blah/blah2/blah3/blah4/blah5.pdf", "blah5.pdf")]
        public void GetFileNameFromWebPathMapsCorrectly(string webPath, string expectedFileName)
        {
            // Arrange/Act
            string mappedModel = ContentManagementServiceMapper.GetFileNameFromWebPath(webPath);

            // Assert
            mappedModel.ShouldEqual(expectedFileName);
        }

        [Test]
        public void MapEntryToCMSEnergyContentMapsCorrectly()
        {
            // Arrange
            Entry entry = FakeContentManagementStub.GetDummyEntry();

            // Act
            CMSEnergyContent mappedModel = ContentManagementServiceMapper.MapEntryToCMSEnergyContent(entry);

            // Assert
            mappedModel.ShouldNotBeNull();
            mappedModel.TariffName.ShouldEqual("Standard tariff");
            mappedModel.TariffNameWithoutTariffWording.ShouldEqual("Standard");
            mappedModel.TagLine.ShouldEqual("Our simple energy tariff with no ties");
            mappedModel.TickUsps.ShouldNotBeNull();
            mappedModel.TickUsps.Count.ShouldEqual(1);
            mappedModel.TickUsps[0].Header.ShouldEqual("Flexible energy");
            mappedModel.TickUsps[0].Description.ShouldEqual("Energy prices may go up or down");
            mappedModel.TickUsps[0].DisplayOrder.ShouldEqual(1);
            mappedModel.PDFList.ShouldNotBeNull();
            mappedModel.PDFList.Count.ShouldEqual(1);
            mappedModel.PDFList[0].Title.ShouldEqual("My PDF Alt Text");
            mappedModel.PDFList[0].Name.ShouldEqual("My PDF Display Name");
            mappedModel.PDFList[0].Path.ShouldEqual("http://mypdfpath/mypdf.pdf");
            mappedModel.PDFList[0].Category.ShouldEqual("PDF");
        }

        [Test]
        public void MapEntryToCMSEnergyContentMapsCorrectly_NullTickUsp()
        {
            // Arrange
            Entry entry = FakeContentManagementStub.GetDummyEntry();
            entry.KeyPoints = null;

            // Act
            CMSEnergyContent mappedModel = ContentManagementServiceMapper.MapEntryToCMSEnergyContent(entry);

            // Assert
            mappedModel.ShouldNotBeNull();
            mappedModel.TariffName.ShouldEqual("Standard tariff");
            mappedModel.TariffNameWithoutTariffWording.ShouldEqual("Standard");
            mappedModel.TagLine.ShouldEqual("Our simple energy tariff with no ties");
            mappedModel.TickUsps.ShouldNotBeNull();
            mappedModel.TickUsps.Count.ShouldEqual(0);
            mappedModel.PDFList.ShouldNotBeNull();
            mappedModel.PDFList.Count.ShouldEqual(1);
            mappedModel.PDFList[0].Title.ShouldEqual("My PDF Alt Text");
            mappedModel.PDFList[0].Name.ShouldEqual("My PDF Display Name");
            mappedModel.PDFList[0].Path.ShouldEqual("http://mypdfpath/mypdf.pdf");
            mappedModel.PDFList[0].Category.ShouldEqual("PDF");
        }

        [Test]
        public void MapEntryToCMSEnergyContentMapsCorrectly_NullPdfList()
        {
            // Arrange
            Entry entry = FakeContentManagementStub.GetDummyEntry();
            entry.Documents = null;

            // Act
            CMSEnergyContent mappedModel = ContentManagementServiceMapper.MapEntryToCMSEnergyContent(entry);

            // Assert
            mappedModel.ShouldNotBeNull();
            mappedModel.TariffName.ShouldEqual("Standard tariff");
            mappedModel.TariffNameWithoutTariffWording.ShouldEqual("Standard");
            mappedModel.TagLine.ShouldEqual("Our simple energy tariff with no ties");
            mappedModel.TickUsps.ShouldNotBeNull();
            mappedModel.TickUsps.Count.ShouldEqual(1);
            mappedModel.TickUsps[0].Header.ShouldEqual("Flexible energy");
            mappedModel.TickUsps[0].Description.ShouldEqual("Energy prices may go up or down");
            mappedModel.TickUsps[0].DisplayOrder.ShouldEqual(1);
            mappedModel.PDFList.ShouldNotBeNull();
            mappedModel.PDFList.Count.ShouldEqual(0);
        }

        [Test]
        public void MapEntryListToCMSEnergyContentListMapsCorrectly()
        {
            // Arrange
            CMSResponseModel responseModel = FakeContentManagementStub.GetDummyCMSResponseModel();

            // Act
            List<CMSEnergyContent> mappedModelList = ContentManagementServiceMapper.MapEntryListToCMSEnergyContentList(responseModel.Entries);

            // Assert
            mappedModelList.ShouldNotBeNull();
            mappedModelList.Count.ShouldEqual(3);
        }
    }
}
