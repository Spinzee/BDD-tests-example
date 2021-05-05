namespace Products.Service.Helpers
{
    public static class RateCodeHelper
    {
        public static string GetPaymentMethod(int? rateCode)
        {
            switch (rateCode)
            {
                case 1:
                case 2:
                    return "Quarterly on demand";
                case 3:
                case 4:
                case 6:
                case 7:
                    return "Direct Debit";
                case 5:
                    return "Pay As You Go";
                default:
                    return string.Empty;
            }
        }

        public static string GetRateCodeDescription(int? rateCode)
        {
            switch (rateCode)
            {
                case 1: return "Quarterly on demand and Paper Bills";
                case 2: return "Quarterly on demand and Paperless Bills";
                case 3: return "Direct Debit and Paper Bills";
                case 4: return "Direct Debit and Paperless Bills";
                case 5: return "Pay as you go";
                case 6: return "Staff Only with Direct Debit and Paper Bills";
                case 7: return "Staff Only with Direct Debit and Paperless Bills";
                default:
                    return string.Empty;
            }
        }
    }
}
