using System;

namespace Products.Model.Common
{
    [Serializable]
    public class BankFormattedAddressType
    {
        public string BankAddressLine1Field { get; set; }
        public string BankAddressLine2Field { get; set; }
        public string BankAddressLine3Field { get; set; }
        public string BankAddressLine4Field { get; set; }
        public string BankPostcodeField { get; set; }
    }
}