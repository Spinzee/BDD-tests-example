namespace Products.Tests.TariffChange.Fakes.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Model.Energy;
    using Products.Infrastructure.Extensions;
    using Service.Common.Managers;
    using WebModel.ViewModels.Common;

    public class FakeTariffManager : ITariffManager
    {
        public FakeTariffManager()
        {
            TaglineMappings = new Dictionary<string, string>();
            PdfLinkMappings = new Dictionary<string, string>();
            SarTranslationMappings = new Dictionary<string, string>();
            TariffGroupMappings = new Dictionary<string, string>();
            SmartTariffGroupMappings = new Dictionary<string, string>();
        }

        public Dictionary<string, string> TaglineMappings { get; set; }

        public Dictionary<string, string> PdfLinkMappings { get; set; }

        public Dictionary<string, string> SarTranslationMappings { get; set; }

        public Dictionary<string, string> TariffGroupMappings { get; set; }

        public Dictionary<string, string> SmartTariffGroupMappings { get; set; }

        public string GetTagline(string tariffName)
        {
            if (TaglineMappings != null && TaglineMappings.ContainsKey(tariffName))
            {
                return TaglineMappings[tariffName];
            }

            return null;
        }

        public List<string> GetPdfLinks(string tariffName)
        {
            if (PdfLinkMappings != null && PdfLinkMappings.ContainsKey(tariffName))
            {
                return PdfLinkMappings[tariffName].Split('|').ToList();
            }

            return null;
        }

        public IEnumerable<TermsAndConditionsPdfLink> GetTermsAndConditionsPdfs(string tariffName, TariffGroup tariffGroup, List<CMSEnergyContent> cmsEnergyContents)
        {
            return new List<TermsAndConditionsPdfLink>();
        }

        public string GetSpecialTariffCardText(TariffGroup tariffGroup)
        {
            return string.Empty;
        }

        public TariffGroup GetTariffGroup(string servicePlanId)
        {
            string group = TariffGroupMappings.ContainsKey(servicePlanId) ? TariffGroupMappings[servicePlanId] : string.Empty;
            return string.IsNullOrEmpty(group) ? TariffGroup.None : group.ToEnum<TariffGroup>();
        }

        public IEnumerable<TariffTickUsp> GetTariffTickUsp(string servicePlanId)
        {
            return new List<TariffTickUsp>();
        }

        public bool IsSmart(string servicePlanId)
        {
            string group = SmartTariffGroupMappings.ContainsKey(servicePlanId) ? TariffGroupMappings[servicePlanId] : string.Empty;
            return group.Equals("Smart");
        }
    }
}