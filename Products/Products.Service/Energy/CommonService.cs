namespace Products.Service.Energy
{
    using System;
    using System.Collections.Generic;
    using Broadband.Mappers;
    using Helpers;
    using Infrastructure;
    using Model.Broadband;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Energy;
    using Tariff = Model.Energy.Tariff;

    public class CommonService : ICommonService
    {
        private readonly IConfigManager _configManager;
        private readonly ISessionManager _sessionManager;

        public CommonService(ISessionManager sessionManager, IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(sessionManager == null, "sessionManager is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");
            _sessionManager = sessionManager;
            _configManager = configManager;
        }

        public YourPriceViewModel GetYourPriceViewModel()
        {
            var yourPrice = _sessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            Guard.Against<Exception>(yourPrice == null, "Your Price Details object is null in session");

            return yourPrice;
        }

        public DataLayerViewModel GetDataLayer()
        {
            var energyCustomer = _sessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            var tariffs = _sessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            string affiliateCampaignCode = _configManager.GetAppSetting("AffiliateCampaignCode");

            var dataLayer = new DataLayerViewModel
            {
                JourneyData = new Dictionary<string, string>(),
                Products = new List<Dictionary<string, string>>()
            };

            if (energyCustomer?.SelectedTariff != null)
            {
                TalkProduct selectedTalkProductProduct = energyCustomer.SelectedBroadbandProduct?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode);

                var product = new Dictionary<string, string>
                {
                    {
                        DataLayer_Resources.MonthlyCost, energyCustomer.SelectedTariff?.GetProjectedMonthlyCost(selectedTalkProductProduct).ToString("C")
                    },
                    {
                        DataLayer_Resources.ProductName, energyCustomer.SelectedTariff?.DisplayName ?? string.Empty
                    },
                    {
                        DataLayer_Resources.GasCost, energyCustomer.SelectedTariff?.GetProjectedMonthlyGasCost()?.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.ElectricityCost, energyCustomer.SelectedTariff?.GetProjectedMonthlyElectricityCost()?.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.MonthlySavings, energyCustomer.SelectedTariff?.BroadbandPackage?.MonthlySavings.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.YearlySavings, energyCustomer.SelectedTariff?.BroadbandPackage?.YearlySavings.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.TalkProductName, selectedTalkProductProduct?.ProductName ?? string.Empty
                    },
                    {
                        DataLayer_Resources.TalkProductCost, selectedTalkProductProduct?.GetMonthlyTalkCost().ToString("C") ?? string.Empty
                    }
                };

                dataLayer.Products.Add(product);
            }

            string smartMeterFrequency = energyCustomer?.SmartMeterFrequency.ToDescription() ?? string.Empty;
            dataLayer.JourneyData.Add(DataLayer_Resources.Bundling, energyCustomer?.IsBundlingJourney.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergyAveStandardElecKwh, energyCustomer?.Projection?.EnergyAveStandardElecKwh.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergyAveEcon7ElecKwh, energyCustomer?.Projection?.EnergyAveEcon7ElecKwh.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergyAveStandardGasKwh, energyCustomer?.Projection?.EnergyAveStandardGasKwh.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergyEconomy7DayElecKwh, energyCustomer?.Projection?.EnergyEconomy7DayElecKwh.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergyEconomy7NightElecKwh, energyCustomer?.Projection?.EnergyEconomy7NightElecKwh.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.BillingPreference, energyCustomer?.SelectedBillingPreference.ToDescription() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.FuelType, energyCustomer?.SelectedFuelType.ToDescription() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.HasMarketingConsent, energyCustomer?.ContactDetails?.MarketingConsent.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.HasSmartMeter, energyCustomer?.HasSmartMeter?.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.MeterType, energyCustomer?.SelectedElectricityMeterType.ToDescription() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.PaymentMethod, energyCustomer?.SelectedPaymentMethod.ToDescription() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.TariffCards, tariffs?.Count.ToString() ?? "0");
            dataLayer.JourneyData.Add(DataLayer_Resources.AffiliateSale, (energyCustomer?.CampaignCode == affiliateCampaignCode).ToString());
            dataLayer.JourneyData.Add(DataLayer_Resources.AffiliateId, energyCustomer?.MigrateAffiliateId ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.EnergySaleId, energyCustomer?.EnergyApplicationId.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.BroadbandSaleId, energyCustomer?.BroadbandApplicationId.ToString() ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.MembershipId, energyCustomer?.MigrateMemberId ?? string.Empty);
            dataLayer.JourneyData.Add(DataLayer_Resources.SmartMeterFrequency, smartMeterFrequency.Equals(SmartMeterFrequency.None.ToDescription()) ? string.Empty : smartMeterFrequency);

            dataLayer.JourneyData.Add(DataLayer_Resources.FocusTariff, !string.IsNullOrEmpty(energyCustomer?.ChosenProduct) ? energyCustomer.ChosenProduct : string.Empty);

            dataLayer.JourneyData.Add(DataLayer_Resources.CNCHasSmartMeter, (energyCustomer == null ? string.Empty  : (energyCustomer.IsCAndCJourney() ? energyCustomer.IsSmartMeter().ToString() : string.Empty)));
            return dataLayer;
        }
    }
}