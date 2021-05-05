using Products.Infrastructure;
using Products.ServiceWrapper.GoogleReCaptchaService;
using System;

namespace Products.Service.Common
{
    public class GoogleReCaptchaService : IGoogleReCaptchaService
    {
        private readonly IConfigManager _configManager;
        private readonly IGoogleReCaptchaServiceWrapper _googleReCaptchaServiceWrapper;


        public GoogleReCaptchaService(IConfigManager configManager, IGoogleReCaptchaServiceWrapper googleReCaptchaServiceWrapper)
        {
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            Guard.Against<ArgumentNullException>(googleReCaptchaServiceWrapper == null, "googleReCaptchaServiceWrapper is null");

            _configManager = configManager;
            _googleReCaptchaServiceWrapper = googleReCaptchaServiceWrapper;
        }

        public bool ValidateReCaptcha(string reCaptchaResponse)
        {
            var supressGoogleReCaptcha = _configManager.GetAppSetting("SupressGoogleReCaptcha");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(supressGoogleReCaptcha), "Missing SupressGoogleReCaptcha entry in web.config file for Google ReCaptcha");

            bool suppresed;

            Guard.Against<ArgumentException>(!bool.TryParse(supressGoogleReCaptcha, out suppresed),
                "SupressGoogleReCaptcha entry in web.config file for Google ReCaptcha should be true or false");

            if (suppresed)
                return true;

            return _googleReCaptchaServiceWrapper.ValidateRecaptcha(reCaptchaResponse);
        }
    }
}
