namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;
    using System.Linq;

    public class DataLayerViewModel
    {
        public Dictionary<string, string> JourneyData { get; set; }

        public List<Dictionary<string, string>> Products { get; set; }

        public string JourneyDataToJson()
        {
            IEnumerable<string> rows = JourneyData.Select(data => $"'{data.Key}':'{data.Value}'");
            return string.Join(",", rows);
        }

        public string ProductsToJson()
        {
            IEnumerable<string> rows = Products.Select(product => { return "{" + string.Join(",", product.Select(p => $"'{p.Key}':'{p.Value}'")) + "}"; });
            return string.Join(",", rows);
        }
    }
}