namespace Products.Model.Energy
{
    using System.Collections.Generic;

    public class CMSEnergyContent
    {
        private const string TariffWording = " tariff";

        public string TariffName { get; set; }

        public string TariffNameWithoutTariffWording => TariffName.Replace(TariffWording, string.Empty);

        public string TagLine { get; set; }

        public List<TariffTickUsp> TickUsps { get; set; }

        public List<PDFContent> PDFList { get; set; }
    }
}
