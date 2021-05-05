namespace Products.Tests.Common.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Web;

    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly IDictionary<string, string> _requestDictionary;
        private HttpCookieCollection _cookies;
        private NameValueCollection _headers;
        private NameValueCollection _queryString;
        private readonly string _rawUrl;

        // ReSharper disable once UnusedMember.Global
        public FakeHttpRequest(Uri urlReferrer) => UrlReferrer = urlReferrer;

        public FakeHttpRequest()
        {
            _requestDictionary = new Dictionary<string, string>();
            Path = "http://localhost/";
            HttpMethod = "Index";
        }

        public FakeHttpRequest(string path, string httpMethod)
        {
            Path = path;
            HttpMethod = httpMethod;
        }

        // ReSharper disable once UnusedMember.Global
        public FakeHttpRequest(string rawUrl)
        {
            _rawUrl = rawUrl;
        }

        public FakeHttpRequest WithWebRequest()
        {
            _requestDictionary["X-Requested-With"] = "foo";
            _headers = new NameValueCollection { ["X-Requested-With"] = "bar" };
            
            return this;
        }

        public FakeHttpRequest WithWebRequest(HttpCookieCollection cookieCollection)
        {
            _requestDictionary["X-Requested-With"] = "foo";
            _headers = new NameValueCollection { ["X-Requested-With"] = "bar" };
            _cookies = cookieCollection;

            return this;
        }

        public FakeHttpRequest WithWebQueryString(NameValueCollection queryStringCollection)
        {
            _queryString = queryStringCollection;
            return this;
        }

        public override string HttpMethod { get; }

        public override NameValueCollection Headers => _headers;

        public override NameValueCollection QueryString => _queryString;

        public override Uri Url => new Uri("http://localhost/");

        public override string this[string key] => _requestDictionary.ContainsKey(key) ? _requestDictionary[key] : null;

        public override string Path { get; }

        public override HttpCookieCollection Cookies => _cookies;

        public override Uri UrlReferrer { get; }

        // ReSharper disable once ConvertToAutoProperty
        public override string RawUrl => _rawUrl;

        public override string ApplicationPath => "http://localhost/enery-journey/";
    }
}
