namespace Products.Model.TariffChange.Customers
{
    public class SiteDetails
    {
        public string AccountNumber { get; set; }
        public string Address { get; set; }
        private string _postcode;

        public string PostCode
        {
            get { return _postcode; }
            set { _postcode = NormalisePostCode(value); }
        }
        public bool Found { get; set; }
        public bool HasSingleActiveEnergyServiceAccount { get; set; }
        public bool HasMultipleServices { get; set; }
        public int MeterRegisterCount { get; set; }
        public bool HasMultiRateMeter => MeterRegisterCount > 1;
        public int SiteId { get; set; }

        public bool IsValidForPostCode(string postCode)
        {
            if (Found)
            {
                return NormalisePostCode(postCode) == PostCode;
            }
            return false;
        }

        private static string NormalisePostCode(string postcode)
        {
            return postcode.ToUpper().Replace(" ", string.Empty);
        }

        public ServiceStatusType ServiceStatusType { get; set; }
    }
}