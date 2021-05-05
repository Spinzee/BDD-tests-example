namespace Products.WebModel.ViewModels.Energy
{
    using System.Collections.Generic;

    public class BaseSummaryExtra
    {
        public string Name { get; set; }

        public List<string> FeatureList{ get; set; }

        public string Price { get; set; }

        public string TrashModalDataTarget { get; set; }

        public string TrashModalLinkAltText { get; set; }
    }
}
