namespace Products.Tests.Infrastructure.Extensions
{
    using System.Collections.Generic;
    using Core;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;

    [TestFixture]
    public class GenericExtensionsTests
    {
        [Test]
        public void GenericListReplaceAtStartReturnsCorrectList()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            //Act
            list.Replace(x1 => x1 == 1, 4);

            // Assert
            list.Count.ShouldEqual(3);
            list[0].ShouldEqual(4);
            list[1].ShouldEqual(2);
            list[2].ShouldEqual(3);
        }

        [Test]
        public void GenericListReplaceAtMiddleReturnsCorrectList()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            //Act
            list.Replace(x1 => x1 == 2, 4);

            // Assert
            list.Count.ShouldEqual(3);
            list[0].ShouldEqual(1);
            list[1].ShouldEqual(4);
            list[2].ShouldEqual(3);
        }

        [Test]
        public void GenericListReplaceAtEndReturnsCorrectList()
        {
            // Arrange
            var list = new List<int> { 1, 2, 3 };

            //Act
            list.Replace(x1 => x1 == 3, 4);

            // Assert
            list.Count.ShouldEqual(3);
            list[0].ShouldEqual(1);
            list[1].ShouldEqual(2);
            list[2].ShouldEqual(4);
        }

        [Test]
        public void ToEnumWithValidValueReturnsCorrectEnumValue()
        {
            // Arrange/Act
            const string enumValue = "ADSL";
            var result = enumValue.ToEnum<BroadbandType>();

            // Assert
            result.ShouldEqual(BroadbandType.ADSL);
        }
    }
}
