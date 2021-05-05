namespace Products.WebModel.ViewModels.Energy
{
    using System.Text;

    public class BTAddressViewModel
    {
        public int Id { get; set; }

        public string SubPremises { get; set; }

        public string PremiseName { get; set; }

        public string ThoroughfareNumber { get; set; }

        public string ThoroughfareName { get; set; }

        public string Locality { get; set; }

        public string PostTown { get; set; }

        public string Postcode { get; set; }

        public string DistrictId { get; set; }

        public string ParentUPRN { get; set; }

        public string UPRN { get; set; }

        public string FormattedAddressLine1 => GetFormattedAddress();

        public string FormattedAddress => $"{FormattedAddressLine1},{Locality} {PostTown}";

        public string GetFormattedHouseName()
        {
            var formattedAddress = new StringBuilder();

            if (!string.IsNullOrEmpty(SubPremises))
            {
                formattedAddress.Append(SubPremises);
            }

            if (!string.IsNullOrEmpty(PremiseName))
            {
                formattedAddress.Append(PremiseName);
            }

            return formattedAddress.ToString().TrimEnd(',');
        }

        private string GetFormattedAddress()
        {
            var formattedAddress = new StringBuilder();

            if (!string.IsNullOrEmpty(SubPremises))
            {
                formattedAddress.Append(SubPremises + ", ");
            }

            if (!string.IsNullOrEmpty(PremiseName))
            {
                formattedAddress.Append(PremiseName + ", ");
            }

            if (!string.IsNullOrEmpty(ThoroughfareNumber))
            {
                formattedAddress.Append(ThoroughfareNumber + " ");
            }

            if (!string.IsNullOrEmpty(ThoroughfareName))
            {
                formattedAddress.Append(ThoroughfareName);
            }

            return formattedAddress.ToString().TrimEnd(',');
        }
    }
}