using System;
using System.IO;
using System.Web;

namespace Products.Tests.Common.Fakes
{
    public class FakeHttpServerUtility : HttpServerUtilityBase
    {
        public string BasePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        public override string MapPath(string path)
        {
            return Path.Combine(BasePath, path.TrimStart('/').Replace("~/", string.Empty).Replace('/', '\\'));
        }
    }
}
