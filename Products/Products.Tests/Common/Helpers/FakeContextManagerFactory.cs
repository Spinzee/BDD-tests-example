namespace Products.Tests.Common.Helpers
{
    using System.Web;
    using Fakes;

    public class FakeContextManagerFactory
    {
        private HttpRequestBase _httpRequest = new FakeHttpRequest();
        private HttpServerUtilityBase _httpServerUtility = new FakeHttpServerUtility();
        private HttpSessionStateBase _httpSession = new FakeHttpSession();
        private readonly HttpResponseBase _httpResponse = new FakeHttpResponse();

        public static FakeContextManager Default()
        {
            return new FakeContextManagerFactory().Build();
        }

        public FakeContextManagerFactory WithHttpRequest(HttpRequestBase httpRequest)
        {
            _httpRequest = httpRequest;
            return this;
        }

        // ReSharper disable once UnusedMember.Global
        public FakeContextManagerFactory WithHttpServerUtility(HttpServerUtilityBase httpServerUtility)
        {
            _httpServerUtility = httpServerUtility;
            return this;
        }

        // ReSharper disable once UnusedMember.Global
        public FakeContextManagerFactory WithHttpSession(HttpSessionStateBase httpSession)
        {
            _httpSession = httpSession;
            return this;
        }

        public FakeContextManager Build()
        {
            return new FakeContextManager(
                new FakeHttpContext(
                    _httpRequest, 
                    _httpResponse, 
                    _httpServerUtility, 
                    _httpSession));
        }
    }
}