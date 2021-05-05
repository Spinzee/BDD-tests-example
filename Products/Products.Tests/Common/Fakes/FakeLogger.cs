using Products.Infrastructure.Logging;
using System;

namespace Products.Tests.Common.Fakes
{
    public class FakeLogger : ILogger
    {
        public string ErrorMessage { get; set; }
        public Exception LoggedException { get; set; }

        public void Error(string message, Exception exception)
        {
            ErrorMessage = message;
            LoggedException = exception;
        }
    }
}
