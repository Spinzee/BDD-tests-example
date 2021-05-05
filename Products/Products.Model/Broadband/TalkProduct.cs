using System.Collections.Generic;

namespace Products.Model.Broadband
{
    using System.Linq;

    public class TalkProduct
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string TalkCode { get; set; }
        public List<BroadbandPrice> Prices { get; set; }
        public BroadbandProductGroup BroadbandProductGroup { get; set; }
    }

    public static class TalkProductExtensions
    {
        public static double GetMonthlyTalkCost(this TalkProduct talkProduct)
        {
            return talkProduct?.Prices.FirstOrDefault(p => p.FeatureCode == FeatureCodes.MonthlyTalkCharge)?.Price
                  ?? 0;
        }
    }
}
