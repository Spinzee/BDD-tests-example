namespace Products.Tests.TariffChange.Fakes.Models
{
    using System;
    using ServiceWrapper.GoogleReCaptchaService;

    public class FakeGoogleReCaptchaData
    {
        public FakeGoogleReCaptchaData()
        {
            ReCaptcha = new ReCaptcha { Success = true, ChallengeTimeStamp = DateTime.UtcNow };
        }

        public FakeGoogleReCaptchaData(bool success, DateTime challengeTs)
        {
            ReCaptcha = new ReCaptcha
            {
                ChallengeTimeStamp = challengeTs,
                Success = success
            };
        }

        public ReCaptcha ReCaptcha { get; set; }
    }
}