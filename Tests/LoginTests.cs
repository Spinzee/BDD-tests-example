using Data;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Model;
using Xunit;
using Moq;
using Services;
using Tests.Helpers;
using WebSite.Controllers;
using WebSite.Models;

namespace Tests
{
    public class LoginTests
    {
        [Theory()]
        [InlineData(AccountStatus.Disabled, "Disabled")]
        [InlineData(AccountStatus.Locked, "Locked")]
        [InlineData(AccountStatus.AwaitingActivation, "AwaitingActivation")]
        [InlineData(AccountStatus.Active, "Products")]
        public void ShouldRedirectToCorrectPageIfEmailAndPasswordAreValid(AccountStatus status, string expectedResult)
        {
            // Arrange
            var loginStatus = new LoginStatus
            {
                Id = 0,
                Password = "FWoBU7G8xXj0/4KvAcHeYmCuefd78+YRmxOQHrdXGRbo=IMgXUbUqEHO2g1JJ2ZKBkEKVX6RwVs5O/DiKLUqPq1Y=",
                Status = status
            };

            var fakeProfileRepository = new FakeProfileRepository { LoginStatus = loginStatus };

            HomeController controller = new HomeControllerFactory()
                .WithProfileRepository(fakeProfileRepository)
                .Build();

            // Act
            var result = controller.Login(new LoginViewModel { Email = "Test@test.com", Password = "Test1234" });

            // Assert
            var actionResult = (RedirectToActionResult)result;
            Assert.Equal(expectedResult, actionResult.ActionName);
        }

        [Fact]
        public void ShouldStayOnLoginPageIfEmailDoesNotExistInDatabase()
        {
            var mockProfileRepository = new Mock<IProfileRepository>();
            mockProfileRepository.Setup(p => p.GetAccountStatus("Test")).Returns((LoginStatus?)null);

            var controller = new HomeController(new ProfileService(mockProfileRepository.Object, new PasswordService()), new ProductsService());

            var result = controller.Login(new LoginViewModel { Email = "Test", Password = "Test" });

            var actionResult = (RedirectToActionResult)result;
            Assert.Equal("Login", actionResult.ActionName);
        }

        [Fact]
        public void ShouldRemainOnLoginPageIfPasswordEnteredIsInvalid()
        {
            // Arrange 
            var loginStatus = new LoginStatus
            {
                Password = "FWoBU7G8xXj0/4KvAcHeYmCuefd78+YRmxOQHrdXGRbo=IMgXUbUqEHO2g1JJ2ZKBkEKVX6RwVs5O/DiKLUqPq1Y=",
                Id = 1,
                Status = AccountStatus.Active
            };

            var fakeProfileRepository = new FakeProfileRepository { LoginStatus = loginStatus };

            HomeController controller = new HomeControllerFactory()
                .WithProfileRepository(fakeProfileRepository)
                .Build();

            // Act
            var result = controller.Login(new LoginViewModel { Email = "test@test.com", Password = "wrongPassword" });

            // Assert
            var actionResult = (RedirectToActionResult)result;
            Assert.Equal("Login", actionResult.ActionName);
            //todo assert a log is made of invalid attempt
        }
    }
}