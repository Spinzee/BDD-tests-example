namespace Products.ControllerHelpers.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Enums;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Broadband;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using Service.Broadband.Mappers;
    using Service.Energy.Mappers;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Energy;
    using Tariff = Model.Energy.Tariff;

    public class BaseEnergyControllerHelper : BaseControllerHelper
    {
        protected readonly ISessionManager SessionManager;
        protected readonly IConfigManager ConfigManager;

        private const int MaximumStepsInSegmentOneInJourney = 7;
        private const int MaximumStepsInSegmentTwoInJourney = 5;
        protected readonly WebClientData WebClientData;

        public BaseEnergyControllerHelper(
            ISessionManager sessionManager,
            IConfigManager configManager,
            WebClientData webClientData = null)
        {
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            SessionManager = sessionManager;
            ConfigManager = configManager;
            WebClientData = webClientData;
        }

        public PostcodeViewModel GetOurPricesPostcodeViewModel()
        {
            return new PostcodeViewModel
            {
                Header = OurPrices_Resources.Header,
                ParagraphText = OurPrices_Resources.Paragraph,
                SubmitButtonText = OurPrices_Resources.SubmitButtonText,
                SubmitButtonTitle = OurPrices_Resources.SubmitButtonAlt
            };
        }

        public UnableToCompleteViewModel GetUnableToCompleteViewModel(bool isEnergy)
        {
            return new UnableToCompleteViewModel
            {
                Header = Fallout_Resources.CantFindPricesHeader,
                ContactHeaderPre = Fallout_Resources.CantFindPricesContactBodyPre,
                ContactHeaderPost = Fallout_Resources.CantFindPricesContactBodyPost,
                ContactNumber = Fallout_Resources.ContactNumber2,
                ContactNumberAlt = Fallout_Resources.ContactNumber2,
                ContactNumberUrl = Fallout_Resources.ContactNumberUrl2,
                Title = isEnergy ? Fallout_Resources.CantFindPricesEnergyTitle : Fallout_Resources.CantFindPricesOurPricesTitle
            };
        }

        public bool SelectedTariffHasExtras() => GetEnergyCustomerFromSession().SelectedTariffHasExtras();

        public virtual YourPriceViewModel GetYourPriceViewModel()
        {
            var yourPriceViewModel = SessionManager.GetSessionDetails<YourPriceViewModel>(SessionKeys.EnergyYourPriceDetails);
            Guard.Against<Exception>(yourPriceViewModel == null, $"{nameof(yourPriceViewModel)} is null");
            return yourPriceViewModel;
        }

        public bool SelectedTariffHasUpgrades()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var tariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            Tariff selectedTariff = energyCustomer.SelectedTariff;

            if (tariffs.Any(t => t.IsUpgrade && t.BundlePackageType == selectedTariff.BundlePackageType))
            {
                return selectedTariff.BundlePackageType == BundlePackageType.FixAndFibre && energyCustomer.CanUpgradeToFibrePlus ||
                       selectedTariff.BundlePackageType == BundlePackageType.FixAndProtect;
            }
            return false;
        }

        public virtual DataLayerViewModel GetDataLayerViewModel()
        {
            var energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            var tariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            string affiliateCampaignCode = ConfigManager.GetAppSetting("AffiliateCampaignCode");

            var viewModel = new DataLayerViewModel
            {
                JourneyData = new Dictionary<string, string>(),
                Products = new List<Dictionary<string, string>>()
            };

            if (energyCustomer?.SelectedTariff != null)
            {
                TalkProduct selectedTalkProductProduct = energyCustomer.SelectedBroadbandProduct?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode);
                Extra selectedExtra = energyCustomer.SelectedExtras.FirstOrDefault(e => e.Type == ExtraType.ElectricalWiring);

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
                        DataLayer_Resources.MonthlySavings, energyCustomer.SelectedTariff?.BundlePackage?.MonthlySavings.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.YearlySavings, energyCustomer.SelectedTariff?.BundlePackage?.YearlySavings.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.BroadbandCost, energyCustomer.SelectedTariff?.BundlePackage?.MonthlyDiscountedCost.ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.TalkProductName, selectedTalkProductProduct?.ProductName ?? string.Empty
                    },
                    {
                        DataLayer_Resources.TalkProductCost, selectedTalkProductProduct?.GetMonthlyTalkCost().ToString("C") ?? string.Empty
                    },
                    {
                        DataLayer_Resources.ExtraName, selectedExtra?.Name ?? string.Empty
                    },
                    {
                        DataLayer_Resources.ExtraCost, selectedExtra?.BundlePrice.ToCurrency() ?? string.Empty
                    },
                    {
                        DataLayer_Resources.ExtraProductCode, selectedExtra?.ProductCode ?? string.Empty
                    }
                };

                viewModel.Products.Add(product);
            }

            string smartMeterFrequency = energyCustomer?.SmartMeterFrequency.ToDescription() ?? string.Empty;
            viewModel.JourneyData.Add(DataLayer_Resources.Bundling, energyCustomer?.IsBundlingJourney.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergyAveStandardElecKwh, energyCustomer?.Projection?.EnergyAveStandardElecKwh.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergyAveEcon7ElecKwh, energyCustomer?.Projection?.EnergyAveEcon7ElecKwh.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergyAveStandardGasKwh, energyCustomer?.Projection?.EnergyAveStandardGasKwh.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergyEconomy7DayElecKwh, energyCustomer?.Projection?.EnergyEconomy7DayElecKwh.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergyEconomy7NightElecKwh, energyCustomer?.Projection?.EnergyEconomy7NightElecKwh.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.BillingPreference, energyCustomer?.SelectedBillingPreference.ToDescription() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.FuelType, energyCustomer?.SelectedFuelType.ToDescription() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.HasMarketingConsent, energyCustomer?.ContactDetails?.MarketingConsent.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.HasSmartMeter, energyCustomer?.HasSmartMeter?.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.MeterType, energyCustomer?.SelectedElectricityMeterType.ToDescription() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.PaymentMethod, energyCustomer?.SelectedPaymentMethod.ToDescription() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.TariffCards, tariffs?.Where(t=> !t.IsUpgrade).ToList().Count.ToString() ?? "0");
            viewModel.JourneyData.Add(DataLayer_Resources.AffiliateSale, (energyCustomer?.CampaignCode == affiliateCampaignCode).ToString());
            viewModel.JourneyData.Add(DataLayer_Resources.AffiliateId, energyCustomer?.MigrateAffiliateId ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.EnergySaleId, energyCustomer?.EnergyApplicationId.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.BroadbandSaleId, energyCustomer?.BroadbandApplicationId.ToString() ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.MembershipId, energyCustomer?.MigrateMemberId ?? string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.SmartMeterFrequency, smartMeterFrequency.Equals(SmartMeterFrequency.None.ToDescription()) ? string.Empty : smartMeterFrequency);
            viewModel.JourneyData.Add(DataLayer_Resources.FocusTariff, !string.IsNullOrEmpty(energyCustomer?.ChosenProduct) ? energyCustomer.ChosenProduct : string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.CNCHasSmartMeter, energyCustomer == null ? string.Empty : energyCustomer.IsCAndCJourney() ? energyCustomer.IsSmartMeter().ToString() : string.Empty);
            viewModel.JourneyData.Add(DataLayer_Resources.SmartMeterType, string.IsNullOrEmpty(energyCustomer.GetSmartMeterType()) ? "-" : energyCustomer.GetSmartMeterType());

            return viewModel;
        }

        public virtual string GetStepCounter(string pageName)
        {
            var energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);

            int finalStepSegment1 = MaximumStepsInSegmentOneInJourney;

            if (energyCustomer == null)
            {
                return string.Empty;
            }

            bool isCAndCJourney = energyCustomer.IsCAndCJourney();

            if (isCAndCJourney) //CAndC journey Meter Details page should not be part of the journey
            {
                finalStepSegment1--;
            }

            if (!isCAndCJourney && energyCustomer.SelectedFuelType == FuelType.Gas) //Non CAndC journey Meter Details page should not be part of the journey for gas only customers
            {
                finalStepSegment1--;
            }

            if (isCAndCJourney && energyCustomer.IsPrePay() && !energyCustomer.IsSmartMeter()) //CAndC journey non smart and pre-pay customer, Smart consent page and payment page should not be part of the journey
            {
                finalStepSegment1 -= 2;
            }

            if (isCAndCJourney && !energyCustomer.IsPrePay() && !energyCustomer.IsSmartMeter()) //CAndC journey non smart and non pre-pay, customer Smart consent page should not be part of the journey
            {
                finalStepSegment1 -= 1;
            }

            int finalStepSegment2 = MaximumStepsInSegmentTwoInJourney;

            if (energyCustomer.SelectedPaymentMethod != PaymentMethod.MonthlyDirectDebit) // No DD page for Non-DD customers
            {
                finalStepSegment2--;
            }

            if (energyCustomer.ProfileExists == true) // If profile exists no password select screen
            {
                finalStepSegment2--;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (pageName)
            {
                //Segment 1
                case "EnterPostcode":
                    return string.Format(Common_Resources.StepCounter, 1, MaximumStepsInSegmentOneInJourney);
                case "SelectAddress":
                    return string.Format(Common_Resources.StepCounter, 2, MaximumStepsInSegmentOneInJourney);
                case "SelectFuel":
                    return string.Format(Common_Resources.StepCounter, 3, MaximumStepsInSegmentOneInJourney);
                case "PaymentMethod":
                    return string.Format(Common_Resources.StepCounter, 4, finalStepSegment1);
                case "SmartMeterFrequency":
                    return string.Format(Common_Resources.StepCounter, 5, finalStepSegment1);
                case "MeterType":
                    return string.Format(Common_Resources.StepCounter, finalStepSegment1 - 2, finalStepSegment1);
                case "SmartMeter":
                    return string.Format(Common_Resources.StepCounter, finalStepSegment1 - 1, finalStepSegment1);
                case "EnergyUsage":
                    return string.Format(Common_Resources.StepCounter, finalStepSegment1, finalStepSegment1);

                //Segment 2                                
                case "PersonalDetails":
                    return string.Format(Common_Resources.StepCounter, 1, finalStepSegment2);
                case "ContactDetails":
                    return string.Format(Common_Resources.StepCounter, 2, finalStepSegment2);
                case "OnlineAccount":
                    return string.Format(Common_Resources.StepCounter, 3, finalStepSegment2);
                case "BankDetails":
                    return string.Format(Common_Resources.StepCounter, finalStepSegment2 - 1, finalStepSegment2);
                case "ViewSummary":
                    return string.Format(Common_Resources.StepCounter, finalStepSegment2, finalStepSegment2);
            }

            return string.Empty;
        }

        protected void SaveEnergyCustomerInSession(EnergyCustomer customer)
        {
            SessionManager.SetSessionDetails(SessionKeys.EnergyCustomer, customer);
        }

        protected EnergyCustomer GetEnergyCustomerFromSession()
        {
            var energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
            Guard.Against<ArgumentException>(energyCustomer == null, $"{nameof(energyCustomer)} is null");
            return energyCustomer;
        }

        public bool SetSelectedTariff(string selectedTariffId, ITariffMapper tariffMapper)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Tariff selectedTariff = GetSelectedTariff(selectedTariffId);
            energyCustomer.SelectedTariff = selectedTariff;
            energyCustomer.SelectedBroadbandProductCode = energyCustomer.SelectedTariff?.BundlePackage?.ProductCode;
            energyCustomer.SelectedBroadbandProduct = null;
            energyCustomer.ResetSelectedExtras();

            bool hasSelectedTariff = selectedTariff != null;
            if (hasSelectedTariff)
            {
                YourPriceViewModel yourPrice = tariffMapper.GetYourPriceViewModel(energyCustomer, WebClientData.BaseUrl);
                SessionManager.SetSessionDetails(SessionKeys.EnergyYourPriceDetails, yourPrice);
            }

            return hasSelectedTariff;
        }

        public void SetAvailableBundleUpgrade()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var tariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            Tariff selectedTariff = energyCustomer.SelectedTariff;
            Tariff upgrade = tariffs.SingleOrDefault(t => t.IsUpgrade && t.BundlePackageType == selectedTariff.BundlePackageType);
            if (upgrade != null)
            {
                energyCustomer.AvailableBundleUpgrade = upgrade;
            }
        }

        protected void SetDefaultBroadbandProduct(BroadbandProductGroup selectedBroadbandProductGroup)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var broadbandProducts = SessionManager.GetSessionDetails<List<BroadbandProduct>>(SessionKeys.BroadbandProducts);
            energyCustomer.SelectedBroadbandProduct = GetDefaultBroadbandProduct(selectedBroadbandProductGroup, energyCustomer.SelectedBroadbandProductCode);
            energyCustomer.CanUpgradeToFibrePlus = broadbandProducts.Any(t => t.BroadbandType == BroadbandType.FibrePlus);
        }

        protected BroadbandProduct GetDefaultBroadbandProduct(BroadbandProductGroup selectedBroadbandProductGroup, string broadbandProductCode)
        {
            var broadbandProducts = SessionManager.GetSessionDetails<List<BroadbandProduct>>(SessionKeys.BroadbandProducts);
            BroadbandProduct productFound = broadbandProducts
                .Where(p => p.IsAvailable)
                .Select(p =>
                {
                    return new BroadbandProduct
                    {
                        BroadbandCode = p.BroadbandCode,
                        BroadbandType = p.BroadbandType,
                        IsAvailable = p.IsAvailable,
                        LineSpeed = p.LineSpeed,
                        ProductOrder = p.ProductOrder,
                        TalkProducts = p.TalkProducts.Where(t => t.BroadbandProductGroup == selectedBroadbandProductGroup).ToList()
                    };
                })
                .FirstOrDefault(p => p.TalkProducts.Any(t => t.ProductCode == broadbandProductCode));
            return productFound;
        }

        private Tariff GetSelectedTariff(string selectedTariffId)
        {
            var availableTariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            Guard.Against<ArgumentException>(availableTariffs == null, $"{nameof(availableTariffs)} is null");
            return availableTariffs?.FirstOrDefault(t => t.TariffId.Equals(selectedTariffId));
        }
    }
}