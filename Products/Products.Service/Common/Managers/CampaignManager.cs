using Products.Infrastructure;

namespace Products.Service.Common.Managers
{
    public class CampaignManager : ICampaignManager
    {
        private readonly IConfigManager _configManager;
        private const string SectionGroupPath = "campaignManagement";
        private const string CampaignCodesOverrideAffiliateYesSection = "campaignCodesOverrideAffiliateYes";
        private const string CampaignCodesOverrideAffiliateNoSection = "campaignCodesOverrideAffiliateNo";
        public CampaignManager(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public string GetCampaignCodesMapping(string migrateAffiliateId, string migrateCampaignId)
        {
            var masusCodeOverrideAffiliateYes = _configManager.GetValueForKeyFromSection(SectionGroupPath,
                CampaignCodesOverrideAffiliateYesSection, migrateCampaignId);
            var masusCodeOverrideAffiliateNo = _configManager.GetValueForKeyFromSection(SectionGroupPath,
                CampaignCodesOverrideAffiliateNoSection, migrateCampaignId);

            //Only check Override Flag if AffiliateId present.
            if (!string.IsNullOrEmpty(migrateAffiliateId))
            {
                if (!string.IsNullOrEmpty(masusCodeOverrideAffiliateYes))
                {
                    return masusCodeOverrideAffiliateYes;
                }

                return _configManager.GetAppSetting("AffiliateCampaignCode");
            }

            // We need to check affiliate override flag.
            if (string.IsNullOrEmpty(migrateAffiliateId)
                && !string.IsNullOrEmpty(migrateCampaignId))
            {
                if (!string.IsNullOrEmpty(masusCodeOverrideAffiliateNo))
                {
                    return masusCodeOverrideAffiliateNo;
                }

                if (!string.IsNullOrEmpty(masusCodeOverrideAffiliateYes))
                {
                    return masusCodeOverrideAffiliateYes;
                }
            }

            return _configManager.GetAppSetting("SpeculativeCode");
        }
    }
}
