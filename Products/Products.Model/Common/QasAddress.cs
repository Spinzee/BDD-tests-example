namespace Products.Model.Common
{
    using System;
    using System.Text.RegularExpressions;

    [Serializable]
    public class QasAddress
    {
        public string MPAN { get; set; } // Electricity Meter Number

        public string MPRN { get; set; } // Gas Meter Number

        public string HouseName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Moniker { get; set; }

        public string PicklistEntry { get; set; }
    }

    public static class CustomerAddressExtensions
    {
        public static string GetHouseNumber(this QasAddress address)
        {
            Match matchGroup = GetRegexMatch(address.HouseName);
            if (matchGroup.Success)
            {
                return matchGroup.Groups[0].Value.Trim();
            }

            return null;
        }
        public static string GetHouseName(this QasAddress address)
        {
            Match matchGroup = GetRegexMatch(address.HouseName);
            if (matchGroup.Success)
            {
                string returnAddress = address.HouseName.Substring(matchGroup.Groups[0].Index + matchGroup.Groups[0].Length).Trim();
                return !string.IsNullOrEmpty(returnAddress) ? returnAddress : null;
            }

            return address.HouseName;
        }

        public static string FullAddress(this QasAddress customerAddress, string postcode)
        {
            if (customerAddress == null)
                return string.Empty;

            var sb = new System.Text.StringBuilder($"{customerAddress.HouseName},");
            sb.Append($" {customerAddress.AddressLine1},");

            if (!string.IsNullOrEmpty(customerAddress.AddressLine2))
                sb.Append($" {customerAddress.AddressLine2},");

            sb.Append($" {customerAddress.Town},");

            if (!string.IsNullOrEmpty(customerAddress.County))
                sb.Append($" {customerAddress.County},");

            sb.Append($" {postcode}");
            return sb.ToString();
        }

        private static Match GetRegexMatch(string houseName)
        {
            return Regex.Match(houseName, @"^\d+[A-z]*/*[\d]*[A-z]*");
        }
    }
}
