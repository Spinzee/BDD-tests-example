using System.Text;

namespace Products.Model.TariffChange.Customers
{
    public class PaymentDetails
    {
        private string _bankAccountName;

        public bool HasDirectDebitDiscount { get; set; }
        public bool IsDirectDebit { get; set; }
        public bool IsMonthlyDirectDebit { get; set; }
        public bool IsVariableDirectDebit { get; set; }
        public bool IsPaperless { get; set; }
        public string DirectDebitDay { get; set; }
        public double DirectDebitAmount { get; set; }
        public string BankAccountName
        {
            get
            {
                StringBuilder parsed = new StringBuilder();
                foreach (char c in _bankAccountName)
                {
                    int asciiCode = c;
                    if (asciiCode >= 65 && asciiCode <= 90
                        || asciiCode >= 97 && asciiCode <= 122
                        || asciiCode == 32
                        || asciiCode >= 48 && asciiCode <= 57
                        || asciiCode == 38)
                    {
                        parsed.Append(c);
                    }
                }

                return parsed.ToString();
            }
            set
            {
                _bankAccountName = value;
            }
        }

        public string BankAccountNumber { get; set; }
        public string BankSortCode { get; set; }


        public int RateCode =>
            HasDirectDebitDiscount
                ? IsPaperless ? (int)RateCodeEnum.DirectDebitAndPaperless : (int)RateCodeEnum.DirectDebitAndPaper
                : IsPaperless ? (int)RateCodeEnum.QuarterlyAndPaperless : (int)RateCodeEnum.QuarterlyAndPaper;

        private enum RateCodeEnum
        {
            QuarterlyAndPaper = 1,
            QuarterlyAndPaperless = 2,
            DirectDebitAndPaper = 3,
            DirectDebitAndPaperless = 4
        }
    }
}