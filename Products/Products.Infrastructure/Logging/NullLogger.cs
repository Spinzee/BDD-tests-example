using System;

namespace Products.Infrastructure.Logging
{
    public class NullLogger : ILogger
    {
        public void Error(string message, Exception exception)
        {
        }
    }
}
