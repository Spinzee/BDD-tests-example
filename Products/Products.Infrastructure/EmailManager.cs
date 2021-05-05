namespace Products.Infrastructure
{
    using System;
    using System.Collections.Specialized;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailManager : IEmailManager
    {
        private readonly string _corporateDescriptor;

        public EmailManager(IConfigManager configManager)
        {
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            // ReSharper disable once PossibleNullReferenceException
            _corporateDescriptor = configManager.GetAppSetting("EmailCorporateDescriptor");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(_corporateDescriptor), "Email Corporate Descriptor");
        }

        public void Send(string from, string to, string subject, string body, bool isBodyHtml)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(from), "Email from is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(to), "Email to is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(subject), "Email subject is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(body), "Email body is null or empty");

            using (var mailMessage = new MailMessage())
            {
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.Subject = subject;
                if (body != null)
                {
                    mailMessage.Body = body;
                }

                if (to != null)
                {
                    mailMessage.To.Add(to);
                }

                mailMessage.Headers.Add(new NameValueCollection { { "X-CORPDESCLABEL", _corporateDescriptor } });

                if (from != null)
                {
                    mailMessage.From = new MailAddress(from);
                }

                SendMessage(mailMessage);
            }
        }

        private static void SendMessage(MailMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Send(mailMessage);
            }
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(from), "Email from is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(to), "Email to is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(subject), "Email subject is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(body), "Email body is null or empty");

            using (var mailMessage = new MailMessage())
            {
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.Subject = subject;
                if (body != null)
                {
                    mailMessage.Body = body;
                }

                if (to != null)
                {
                    mailMessage.To.Add(to);
                }

                mailMessage.Headers.Add(new NameValueCollection { { "X-CORPDESCLABEL", _corporateDescriptor } });

                if (from != null)
                {
                    mailMessage.From = new MailAddress(from);
                }

                await SendMessageAsync(mailMessage);
            }
        }

        private static async Task SendMessageAsync(MailMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
