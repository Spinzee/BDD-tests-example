namespace Products.WebModel.ViewModels.TariffChange
{
    public class GoogleCaptchaViewModel
    {
        public bool SuppressGoogleCaptcha { get; set; }
        public string GoogleCaptchaPublicKey { get; set; }
        public bool IsValidReCaptcha { get; set; }
    }
}
