using log4net;
using System;

namespace Products.Infrastructure.Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;
       
        public Log4NetLogger()
        {
            _log = LogManager.GetLogger(typeof(Log4NetLogger));
        }

        public void Error(string message, Exception exception)
        {
            _log.Error(message, exception);
        }
    }
}
