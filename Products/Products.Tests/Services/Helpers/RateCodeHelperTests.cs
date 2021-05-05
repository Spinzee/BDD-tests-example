namespace Products.Tests.Services.Helpers
{
    using NUnit.Framework;
    using Service.Helpers;
    using Should;

    [TestFixture]
    public class RateCodeHelperTests
    {
        [TestCase(null, "")]
        [TestCase(0, "")]
        [TestCase(1, "Quarterly on demand")]
        [TestCase(2, "Quarterly on demand")]
        [TestCase(3, "Direct Debit")]
        [TestCase(4, "Direct Debit")]
        [TestCase(5, "Pay As You Go")]
        [TestCase(6, "Direct Debit")]
        [TestCase(7, "Direct Debit")]
        [TestCase(8, "")]
        public void GetPaymentMethodReturnsCorrectValues(int rateCode, string expectedResult)
        {
            // Arrange/Act
            string result = RateCodeHelper.GetPaymentMethod(rateCode);

            // Assert
            result.ShouldEqual(expectedResult);
        }

        [TestCase(null, "")]
        [TestCase(0, "")]
        [TestCase(1, "Quarterly on demand and Paper Bills")]
        [TestCase(2, "Quarterly on demand and Paperless Bills")]
        [TestCase(3, "Direct Debit and Paper Bills")]
        [TestCase(4, "Direct Debit and Paperless Bills")]
        [TestCase(5, "Pay as you go")]
        [TestCase(6, "Staff Only with Direct Debit and Paper Bills")]
        [TestCase(7, "Staff Only with Direct Debit and Paperless Bills")]
        [TestCase(8, "")]
        public void GetRateCodeDescriptionReturnsCorrectValues(int rateCode, string expectedResult)
        {
            // Arrange/Act
            string result = RateCodeHelper.GetRateCodeDescription(rateCode);

            // Assert
            result.ShouldEqual(expectedResult);
        }
    }
}
