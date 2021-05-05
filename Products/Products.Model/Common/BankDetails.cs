using System;

namespace Products.Model.Common
{
    [Serializable]
    public class BankDetails
    {
        public string BankName { get; set; }
        public BankFormattedAddressType BankAddress { get; set; }
        public bool SortCodeAccountNumberValid { get; set; }
        public bool SortCodeValid { get; set; }
        public bool CorporateAccountValid { get; set; }
    }
}