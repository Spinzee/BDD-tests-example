using Products.Model.Broadband;
using Products.Model.Common;
using Products.WebModel.Resources.Broadband;
using Products.WebModel.ViewModels.Broadband;
using System.Collections.Generic;
using System.Configuration;

namespace Products.Service.Broadband.Mappers
{
    public static class DataLayerMapper
    {
        private static readonly string AffiliateCampaignCode = ConfigurationManager.AppSettings["AffiliateCampaignCode"];
        public static Dictionary<string, string> GetDataLayerDictionary(Customer customer, YourPriceViewModel yourPriceViewModel, ApplicationData applicationData, int applicationId)
        {
            var InstallationFee = customer.ApplyInstallationFee ? yourPriceViewModel?.InstallationFee.ToString() : "0";

            return new Dictionary<string, string>
            {
                {DataLayer_Resources.EnergyCustomerDisplayAttribute, customer.IsSSECustomer.ToString() },
                {DataLayer_Resources.ProductDisplayAttribute, customer.SelectedProductCode ?? string.Empty},
                {DataLayer_Resources.KeepYourNumberDisplayAttribute, (!string.IsNullOrEmpty(customer.CliNumber)).ToString() },
                {DataLayer_Resources.PhonePackageAmountDisplayAttribute, yourPriceViewModel?.TelCost.ToString()},
                {DataLayer_Resources.BroadbandPackageAmountDisplayAttribute, yourPriceViewModel?.BroadbandCost.ToString()},
                //{DataLayer_Resources.NonEnergySurchargeDisplayAttribute, yourPriceViewModel?.Surcharge.ToString()},
                {DataLayer_Resources.TotalMonthlyCostDisplayAttribute, yourPriceViewModel?.MonthlyCost.ToString()},
                //{DataLayer_Resources.ConnectionChargeDisplayAttribute, yourPriceViewModel?.ConnectionCharge.ToString()},
                {DataLayer_Resources.OneOffCostDisplayAttribute, yourPriceViewModel?.OneOffCost.ToString()},
                {DataLayer_Resources.FirstBillTotalDisplayAttribute, yourPriceViewModel?.FirstBillTotal.ToString()},
                {DataLayer_Resources.AffiliateSaleDisplayAttribute, (applicationData.CampaignCode == AffiliateCampaignCode).ToString()},
                {DataLayer_Resources.AffiliateIdDisplayAttribute, customer.MigrateAffiliateId},
                {DataLayer_Resources.SaleIdDisplayAttribute, applicationId.ToString()},
                {DataLayer_Resources.InstallationFeeAttribute,InstallationFee}
            };
        }
    }
}