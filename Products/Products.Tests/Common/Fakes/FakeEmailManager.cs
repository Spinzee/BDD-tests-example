using Products.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Tests.Common.Fakes
{
    public class FakeEmailManager : IEmailManager
    {
        // Need to do this using dictionary
        public List<string> From { get; } = new List<string>();
        public List<string> To { get; } = new List<string>();
        public List<string> Subject { get; } = new List<string>();
        public string Body { get; private set; }
        public List<bool> IsBodyHtml { get; } = new List<bool>();



        private readonly Exception _fakeException;

        public FakeEmailManager()
        {

        }

        public FakeEmailManager(Exception fakeSmtpException)
        {
            _fakeException = fakeSmtpException;
        }

        public void Send(string from, string to, string subject, string body, bool isBodyHtml)
        {
            if (_fakeException != null)
            {
                throw _fakeException;
            }

            From.Add(from);
            To.Add(to);
            Subject.Add(subject);
            Body = body;
            IsBodyHtml.Add(isBodyHtml);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml)
        {
            await Task.Delay(1);

            if (_fakeException != null)
            {
                throw _fakeException;
            }

            From.Add(from);
            To.Add(to);
            Subject.Add(subject);
            Body = body;
            IsBodyHtml.Add(isBodyHtml);
        }
    }
}
