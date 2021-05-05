namespace Products.Tests.Infrastructure
{
    using System;
    using Products.Infrastructure;
    using NUnit.Framework;
    using Should;

    [TestFixture]
    public class PasswordServiceTests
    {
        [Test]
        public void HashPasswordPBKDF2ShouldThrowExceptionIfPasswordIsNull()
        {
            // Arrange
            var service = new PasswordService();

            // Act
            Action act = () => service.HashPasswordPBKDF2(null);

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [TestCase("a")]
        [TestCase("ab")]
        [TestCase("abc")]
        [TestCase("abcd")]
        [TestCase("abcd1")]
        [TestCase("abcd12")]
        [TestCase("abcd123")]
        [TestCase("abcd1234")]
        [TestCase("abcde1234")]
        [TestCase("abcde12345")]
        [TestCase("abcdef12345")]
        [TestCase("abcdef123456")]
        [TestCase("abcdef1234567")]
        [TestCase("abcdefg1234567")]
        [TestCase("abcdefg12345678")]
        [TestCase("abcdefgh12345678")]
        [TestCase("abcdefgh123456789")]
        public void HashPasswordPBKDF2ShouldReturnFixedLengthHashRegardlessOfPasswordLength(string password)
        {
            // Arrange
            var service = new PasswordService();

            // Act
            string result = service.HashPasswordPBKDF2(password);

            // Assert
            result.Length.ShouldEqual(89);
        }       
    }
}
