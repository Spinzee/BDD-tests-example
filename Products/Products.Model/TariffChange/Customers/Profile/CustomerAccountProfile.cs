using Products.Model.Enums;
using System;

namespace Products.Model.TariffChange.Customers.Profile
{
    [Serializable]
    public sealed class CustomerAccountProfile
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public StatementDeliveryMethod StatementDeliveryMethod { get; set; }
        public string Postcode { get; set; }
        public Guid UserId { get; set; }
        public AccountType AccountType { get; set; }
        public Site Site { get; set; }
        public string CsExport { get; set; }
        public string OriginalBrand { get; set; }
        public string Loyalty { get; set; }
        public bool OneVueActive { get; set; }
        public Guid UniqueId { get; set; }
        public bool IsInException { get; set; }
        public string CallLineIndicator { get; set; }
    }
}