namespace Products.Tests.Infrastructure
{
    using NUnit.Framework;
    using Products.Infrastructure;
    using Should;

    [TestFixture]
    public class NumberFormatterTests
    {
        [TestCase(0, "0")]
        [TestCase(1, "1st")]
        [TestCase(2, "2nd")]
        [TestCase(3, "3rd")]
        [TestCase(4, "4th")]
        [TestCase(5, "5th")]
        [TestCase(6, "6th")]
        [TestCase(7, "7th")]
        [TestCase(8, "8th")]
        [TestCase(9, "9th")]
        [TestCase(10, "10th")]
        [TestCase(11, "11th")]
        [TestCase(12, "12th")]
        [TestCase(13, "13th")]
        [TestCase(14, "14th")]
        [TestCase(15, "15th")]
        [TestCase(16, "16th")]
        [TestCase(17, "17th")]
        [TestCase(18, "18th")]
        [TestCase(19, "19th")]
        [TestCase(20, "20th")]
        [TestCase(21, "21st")]
        [TestCase(22, "22nd")]
        [TestCase(23, "23rd")]
        [TestCase(24, "24th")]
        [TestCase(25, "25th")]
        [TestCase(26, "26th")]
        [TestCase(27, "27th")]
        [TestCase(28, "28th")]
        [TestCase(29, "29th")]
        [TestCase(30, "30th")]
        [TestCase(31, "31st")]
        [TestCase(32, "32")]
        public void ToDayOfMonthOrdinalReturnsCorrectValues(int day, string ordinal)
        {
            // Arrange/Act
            string result = NumberFormatter.ToDayOfMonthOrdinal(day);

            // Assert
            result.ShouldEqual(ordinal);
        }

        [TestCase(10.55D, "10.55p")]
        [TestCase(3.29D, "3.29p")]
        [TestCase(3.3D, "3.30p")]
        [TestCase(0.99D, "0.99p")]
        [TestCase(0.50D, "0.50p")]
        [TestCase(0.01D, "0.01p")]
        [TestCase(0.005D, "0.01p")]
        [TestCase(0.004D, "0.00p")]
        [TestCase(0.003D, "0.00p")]
        [TestCase(0.002D, "0.00p")]
        [TestCase(0.001D, "0.00p")]
        [TestCase(0.00D, "0p")]
        public void ToPenceReturnsCorrectValues(double number, string expectedString)
        {
            // Arrange/Act
            string result = NumberFormatter.ToPence(number);

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase("A", "")]
        [TestCase("6", "6")]
        [TestCase("AB1", "1")]
        [TestCase("A1B2", "12")]
        [TestCase("A1B2CDE3F4G5H6I7J8K9L0", "1234567890")]
        public void ToDigitsOnlyReturnsCorrectValues(string numberText, string expectedString)
        {
            // Arrange/Act
            string result = NumberFormatter.ToDigitsOnly(numberText);

            // Assert
            result.ShouldEqual(expectedString);
        }

        [TestCase(0D, "£0", "00")]
        [TestCase(0.1D, "£0", "10")]
        [TestCase(0.99D, "£0", "99")]
        [TestCase(1.5D, "£1", "50")]
        [TestCase(21D, "£21", "00")]
        [TestCase(null, "£0", "00")]
        public void GetSplitCostArrayReturnsCorrectValues(double number, string firstString, string secondString)
        {
            // Arrange/Act
            string[] result = NumberFormatter.GetSplitCostArray(number);

            // Assert
            result[0].ShouldEqual(firstString);
            result[1].ShouldEqual(secondString);
        }
    }
}
