namespace Products.Model.Broadband
{
    using System.Collections.Generic;

    public class Tariff
    {
        public string TariffName { get; set; }

        public string ProductCode { get; set; }

        public string BroadbandCode { get; set; }

        public string TalkCode { get; set; }

        public List<BroadbandPrice> Prices { get; set; }
    }
}
