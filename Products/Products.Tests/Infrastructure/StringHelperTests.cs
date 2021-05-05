namespace Products.Tests.Infrastructure
{
    using NUnit.Framework;
    using Products.Infrastructure;
    using Should;

    [TestFixture]
    public class StringHelperTests
    {
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("B1 1AA", "B1 1AA")]
        [TestCase("B11AA", "B1 1AA")]
        [TestCase("PO9 1QH", "PO9 1QH")]
        [TestCase("PO91QH", "PO9 1QH")]
        [TestCase("SW11 3RF", "SW11 3RF")]
        [TestCase("SW113RF", "SW11 3RF")]
        public void GetFormattedPostcodeReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = StringHelper.GetFormattedPostcode(input);

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("1", "1")]
        [TestCase("12", "12")]
        [TestCase("123", "123")]
        [TestCase("1234", "1234")]
        [TestCase("12345", "12345")]
        [TestCase("123456", "12-34-56")]
        public void GetFormattedSortCode1ParamReturnsCorrectValues(string input, string expectedResult)
        {
            // Arrange/Act
            string result = StringHelper.GetFormattedSortCode(input);

            // Assert
            result.ShouldEqual(expectedResult);
        }        
    }
}
