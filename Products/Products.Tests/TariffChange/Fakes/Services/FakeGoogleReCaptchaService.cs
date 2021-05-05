namespace Products.Tests.TariffChange.Fakes.Services
{
    using System;
    using Models;
    using Service.Common;

    public class FakeGoogleReCaptchaService : IGoogleReCaptchaService
    {
        private readonly Exception _exception;
        private readonly FakeGoogleReCaptchaData _fakeGoogleReCaptchaData = new FakeGoogleReCaptchaData();

        public FakeGoogleReCaptchaService()
        {
        }

        public FakeGoogleReCaptchaService(FakeGoogleReCaptchaData fakeGoogleReCaptchaData)
        {
            _fakeGoogleReCaptchaData = fakeGoogleReCaptchaData;
        }

        public FakeGoogleReCaptchaService(Exception exception)
        {
            _exception = exception;
        }

        public bool ValidateReCaptcha(string reCaptchaResponse)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            return _fakeGoogleReCaptchaData.ReCaptcha.Success;
        }
    }
}