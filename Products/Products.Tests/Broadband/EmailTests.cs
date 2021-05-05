namespace Products.Tests.Broadband
{
    using System.Threading.Tasks;
    using System.Web;
    using Helpers;
    using Model;
    using NUnit.Framework;
    using Products.Model.Broadband;
    using Products.Tests.Common.Fakes;
    using Products.Tests.Common.Helpers;
    using Products.Web.Areas.Broadband.Controllers;
    using Products.WebModel.ViewModels.Broadband;
    using Should;
    using FakeSessionManager = Products.Tests.Broadband.Fakes.Services.FakeSessionManager;

    public class EmailTests
    {
        /*
        public async Task ShouldLogExceptionWhenMembershipEmailIsNotSent(string membership, bool shouldSendEmail, string description)
        {
            // Arrange
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "mytestsse");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "devandtest.externalmail@sse.com");

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest())
                .Build();

            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer.MembershipId = "WHSmith";

            var fakeSessionService = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails,
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };


            var fakeEmailManager = new FakeEmailManager(new Exception());
            var fakeLogger = new FakeLogger();


            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionService)
                .WithConfigManager(fakeConfigManager)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel());

            // Assert

            fakeLogger.ErrorMessage.ShouldStartWith($"Exception occured attempting to membership email for broadband customer, email: {broadbandJourneyDetails.Customer.ContactDetails.EmailAddress}");
        }
        */

        [TestCase("WHSmith", true, "Email should be sent when membershipid is present in session")]
        [TestCase(null, false, "Email should not be sent when membershipid is null")]
        public async Task ShouldSendEmailWhenMembershipIdIsPresentInSession(string membership, bool shouldSendEmail, string description)
        {
            // Arrange
            FakeConfigManager fakeConfigManager = FakeConfigManagerFactory.DefaultBroadband();
            fakeConfigManager.AddConfiguration("MySseBaseUrl", "mytestsse");
            fakeConfigManager.AddConfiguration("EmailFromAddress", "devandtest.externalmail@sse.com");

            var cookieCollection = new HttpCookieCollection { new HttpCookie("migrateMemberid", membership) };

            FakeContextManager fakeContextManager = new FakeContextManagerFactory()
                .WithHttpRequest(new FakeHttpRequest().WithWebRequest(cookieCollection))
                .Build();

            BroadbandJourneyDetails broadbandJourneyDetails = FakeBroadbandProductsData.GetCompleteBroadbandJourneyDetails();
            broadbandJourneyDetails.Customer.MembershipId = membership;

            var fakeSessionService = new FakeSessionManager
            {
                SessionObject = broadbandJourneyDetails,
                ListOfProductsSessionObject = FakeBroadbandProductsData.GetListOfAvailableProductsFromSession(),
                OpenReachSessionObject = FakeOpenReachData.GetOpenReachData()
            };

            var fakeEmailManager = new FakeEmailManager();

            var controller = new ControllerFactory()
                .WithContextManager(fakeContextManager)
                .WithSessionManager(fakeSessionService)
                .WithConfigManager(fakeConfigManager)
                .WithEmailManager(fakeEmailManager)
                .Build<SummaryController>();

            // Act
            await controller.Submit(new SummaryViewModel());

            // Assert
            if (shouldSendEmail)
            {
                fakeEmailManager.Subject.ShouldContain("Membership sign up details");
                fakeEmailManager.Body.ShouldContain(membership);
            }
            else
            {
                fakeEmailManager.Subject.ShouldNotContain("Membership sign up details");
            }
        }
    }
}
