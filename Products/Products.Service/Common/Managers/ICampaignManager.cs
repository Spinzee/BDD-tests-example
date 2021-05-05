namespace Products.Service.Common.Managers
{
    public interface ICampaignManager
    {
        string GetCampaignCodesMapping(string migrateAffiliateId, string migrateCampaignId);
    }
}
