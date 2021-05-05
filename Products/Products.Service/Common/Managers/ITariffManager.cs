namespace Products.Service.Common.Managers
{
    using System.Collections.Generic;
    using Core;
    using Model.Energy;
    using WebModel.ViewModels.Common;

    public interface ITariffManager
    {
        string GetTagline(string tariffName);

        List<string> GetPdfLinks(string tariffName);

        IEnumerable<TermsAndConditionsPdfLink> GetTermsAndConditionsPdfs(string tariffName, TariffGroup tariffGroup, List<CMSEnergyContent> cmsEnergyContents);

        string GetSpecialTariffCardText(TariffGroup tariffGroup);

        TariffGroup GetTariffGroup(string servicePlanId);

        IEnumerable<TariffTickUsp> GetTariffTickUsp(string servicePlanId);

        bool IsSmart(string servicePlanId);
    }
}