namespace Products.Service.Common
{
    public interface IGoogleReCaptchaService
    {
        bool ValidateReCaptcha(string reCaptchaResponse);
    }
}