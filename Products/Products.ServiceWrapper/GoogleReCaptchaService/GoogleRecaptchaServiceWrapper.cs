using Newtonsoft.Json;
using Products.Infrastructure;
using System;
using System.Net;

namespace Products.ServiceWrapper.GoogleReCaptchaService
{
    public class GoogleReCaptchaServiceWrapper : IGoogleReCaptchaServiceWrapper
    {

        private readonly string _verificationLink;
        private readonly string _privateKey;


        public GoogleReCaptchaServiceWrapper(IConfigManager configManager)
        {
            _verificationLink = configManager.GetAppSetting("VerificationLink");
            _privateKey = configManager.GetAppSetting("PrivateKey");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(_verificationLink), "Missing VerificationLink entry in web.config file for Google ReCaptcha");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(_privateKey), "Missing PivateKey entry in web.config file for Google ReCaptcha");

        }

        public bool ValidateRecaptcha(string reCaptchaResponse)
        {
            using (var webClient = new WebClient())
            {
                var googleReCaptureReply = webClient.DownloadString(string.Format(_verificationLink, _privateKey, reCaptchaResponse));
                var reCaptcha = JsonConvert.DeserializeObject<ReCaptcha>(googleReCaptureReply);
                if (reCaptcha != null)
                {
                    return reCaptcha.Success;
                }
                return false;
            }
        }
    }
}
