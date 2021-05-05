using System;

namespace Products.Model.TariffChange.Tariffs
{
    public class TariffChangeParameters
    {
        public string AccountNumber { get; set; }

        public string ToServicePlanId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public bool IsMonthlyDirectDebit { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankSortCode { get; set; }

        public string BankAccountName { get; set; }

        public double AdjustedDirectDebitAmount { get; set; }

        public string PaymentDay { get; set; }
    }
}
