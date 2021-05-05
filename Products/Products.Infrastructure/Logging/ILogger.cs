using System;

namespace Products.Infrastructure.Logging
{
    public interface ILogger
    {       
        void Error(string message, Exception exception);
    }
}
