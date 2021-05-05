namespace Products.ServiceWrapper.GoogleReCaptchaService
{
    public interface IGoogleReCaptchaServiceWrapper
    {
        bool ValidateRecaptcha(string reCaptchaResponse);
    }
}