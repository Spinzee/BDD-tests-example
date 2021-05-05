namespace Products.Model.HomeServices
{
    public class Product
    {
        public double Id { get; set; }

        public double MonthlyCost { get; set; }

        public string Description { get; set; }

        public double Excess { get; set; }

        public string FullOfferText { get; set; }

        public OfferSummary OfferSummary { get; set; }

        public string ProductCode { get; set; }

        public string UpsellOfferText { get; set; }

        public double ContractLength { get; set; }

        public string OfferSummaryText { get; set; }

        public string CoverBulletText { get; set; }
    }
}
