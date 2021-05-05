namespace Products.Tests.Infrastructure.Extensions
{
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;

    [TestFixture]
    public class DoubleExtensionsTests
    {
        [TestCase(0.99D, "£1")]
        [TestCase(0.50D, "£1")]
        [TestCase(0.01D, "£1")]
        [TestCase(0.005D, "£1")]
        [TestCase(0.004D, "£0")]
        [TestCase(0.003D, "£0")]
        [TestCase(0.002D, "£0")]
        [TestCase(0.001D, "£0")]
        [TestCase(0.00D, "£0")]
        public void RoundUpToNearestPoundWithPoundSignReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.RoundUpToNearestPoundWithPoundSign();

            // Assert
            result.ShouldEqual(expectedString);
        }
		
		[TestCase(0.99D, 1D)]
        [TestCase(0.50D, 1D)]
        [TestCase(0.01D, 1D)]
        [TestCase(0.005D, 1D)]
        [TestCase(0.004D, 0D)]
        [TestCase(0.003D, 0D)]
        [TestCase(0.002D, 0D)]
        [TestCase(0.001D, 0D)]
        [TestCase(0.00D, 0D)]
        public void RoundUpToNearestPoundReturnsCorrectValues(double number, double expectedResult)
        {
            // Arrange/Act
            double result = number.RoundUpToNearestPound();

            // Assert
            result.ShouldEqual(expectedResult);
        }
				
        [TestCase(10.55D, "£10.55")]
        [TestCase(3.29D, "£3.29")]
        [TestCase(3.3D, "£3.30")]
        [TestCase(0.99D, "£0.99")]
        [TestCase(0.50D, "£0.50")]
        [TestCase(0.01D, "£0.01")]
        [TestCase(0.005D, "£0.01")]
        [TestCase(0.004D, "£0.00")]
        [TestCase(0.003D, "£0.00")]
        [TestCase(0.002D, "£0.00")]
        [TestCase(0.001D, "£0.00")]
        [TestCase(0.00D, "£0")]
        public void ToPoundsReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.ToPounds();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(1234D, "1234")]
        [TestCase(12345D, "12,345")]
        [TestCase(123456D, "123,456")]
        [TestCase(1234567D, "1,234,567")]
        [TestCase(12345678D, "12,345,678")]
        [TestCase(1234.56D, "1234.56")]
        [TestCase(10.55D, "10.55")]
        [TestCase(3.29D, "3.29")]
        [TestCase(3.3D, "3.30")]
        [TestCase(0.99D, "0.99")]
        [TestCase(0.50D, "0.50")]
        [TestCase(0.01D, "0.01")]
        [TestCase(0.005D, "0.01")]
        [TestCase(0.004D, "0.00")]
        [TestCase(0.003D, "0.00")]
        [TestCase(0.002D, "0.00")]
        [TestCase(0.001D, "0.00")]
        [TestCase(0.00D, "0")]
        public void ToNumberReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.ToNumber();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(1055D, "£10.55")]
        [TestCase(329D, "£3.29")]
        [TestCase(330D, "£3.30")]
        [TestCase(99D, "£0.99")]
        [TestCase(50D, "£0.50")]
        [TestCase(1D, "£0.01")]
        [TestCase(0D, "£0")]
        public void FromPenceToPoundReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.FromPenceToPound();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(null, 0)]
        [TestCase(1D, 1)]
        [TestCase(1.1D, 1.1)]
        [TestCase(1.12D, 1.12)]
        [TestCase(1.15D, 1.15)]
        [TestCase(1.151D, 1.15)]
        [TestCase(1.155D, 1.16)]
        [TestCase(1.1511D, 1.15)]
        [TestCase(1.1551D, 1.16)]
        public void ToDecimalPointsReturnsCorrectValues(double? number, decimal expectedResult)
        {
            // Arrange/Act
            decimal result = number.ToDecimalPoints();

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase(0D, "0.00")]
        [TestCase(0.1D, "0.10")]
        [TestCase(0.99D, "0.99")]
        [TestCase(1.5D, "1.50")]
        [TestCase(21D, "21.00")]
        [TestCase(null, "0.00")]
        public void ToTwoDecimalPlacesStringReturnsCorrectValues(double? number, string expectedString)
        {
            // Arrange/Act
            string result = number.ToTwoDecimalPlacesString();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(0D, "£0")]
        [TestCase(0.1D, "£0.10")]
        [TestCase(0.99D, "£0.99")]
        [TestCase(1.5D, "£1.50")]
        [TestCase(21D, "£21")]
        [TestCase(null, "£0")]
        public void ToCurrencyPlacesStringReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.ToCurrency();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(0D, "£0")]
        [TestCase(0.1D, "£0")]
        [TestCase(0.99D, "£0")]
        [TestCase(1.5D, "£1")]
        [TestCase(21D, "£21")]
        [TestCase(null, "£0")]
        public void AmountSplitInPoundsReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.AmountSplitInPounds();

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(0D, ".00")]
        [TestCase(0.1D, ".10")]
        [TestCase(0.99D, ".99")]
        [TestCase(1.5D, ".50")]
        [TestCase(21D, ".00")]
        [TestCase(null, ".00")]
        public void AmountSplitInPenceReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = number.AmountSplitInPence();

            // Assert
            result.ShouldEqual(expectedString);
        }
    }
}
