namespace Products.Tests.Common.Fakes
{
    using System.Collections.Generic;
    using System.Web;

    public class FakeHttpResponse : HttpResponseBase
    {
        public Dictionary<string, string> AddedHeaders = new Dictionary<string, string>();

        //private readonly string _expectedUrl;

        //public FakeHttpResponse(string expectedUrl = "")
        //{
        //    _expectedUrl = expectedUrl;
        //}

        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        public override void AddHeader(string name, string value)
        {
            AddedHeaders.Add(name, value);
        }
    }
}