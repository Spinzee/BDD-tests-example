using System;

namespace Products.Model.Common
{
    [Serializable]
    public class DirectDebitDetails
    {
        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string SortCode { get; set; }

        public string BankName { get; set; }

        public string BankAddressLine1 { get; set; }

        public string BankAddressLine2 { get; set; }

        public string BankAddressLine3 { get; set; }

        public string BankAddressLine4 { get; set; }

        public string Postcode { get; set; }

        public int DirectDebitPaymentDate { get; set; }
    }
}