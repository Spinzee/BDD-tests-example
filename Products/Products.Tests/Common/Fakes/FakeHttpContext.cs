using System.Web;

namespace Products.Tests.Common.Fakes
{
    using System;

    public class FakeHttpContext : HttpContextBase
    {

        public FakeHttpContext(HttpRequestBase request, HttpServerUtilityBase server, HttpSessionStateBase session)
        :this(request, new FakeHttpResponse(), server,session)
        {
        }

        public FakeHttpContext(HttpRequestBase request, HttpResponseBase response, HttpServerUtilityBase server, HttpSessionStateBase session)
        {
            Request = request;
            Server = server;
            Session = session;
            Response = response;
        }        

        public override HttpRequestBase Request { get; }

        public override HttpServerUtilityBase Server { get; }

        public override HttpSessionStateBase Session { get; }

        public override HttpResponseBase Response { get; }

        public override object GetService(Type serviceType)
        {
            return null;
        }
    }
}
