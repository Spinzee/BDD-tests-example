namespace Products.Service.TariffChange.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Common.Managers;
    using Core;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Energy;
    using Model.Enums;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using Products.Model.TariffChange.Tariffs;
    using Products.WebModel.Resources.Common;
    using Products.WebModel.Resources.TariffChange;
    using Products.WebModel.ViewModels.TariffChange;
    using Tariff = Model.TariffChange.Tariffs.Tariff;

    public class TariffViewModelMapper
    {
        private readonly ITariffManager _tariffManager;
        private const string Evergreen = "evergreen";

        public TariffViewModelMapper(ITariffManager tariffManager)
        {
            _tariffManager = tariffManager;
            Guard.Against<ArgumentException>(tariffManager == null, $"{nameof(tariffManager)} is null");
        }

        public TariffsViewModel MapAvailableTariffs(Customer customer, List<Tariff> tariffs, CustomerAccount customerAccount, List<CMSEnergyContent> cmsEnergyContents)
        {
            TariffsViewModel model = GetTariffsViewModel(customer);
            CurrentTariffForFuel currentTariff = customer.ElectricityAccount?.CurrentTariff ?? customer.GasAccount?.CurrentTariff;

            foreach (Tariff tariff in tariffs)
            {
                TariffForFuel availableTariff = tariff.ElectricityDetails ?? tariff.GasDetails;

                List<TariffTickUsp> tickUSPsFromConfig = _tariffManager.GetTariffTickUsp(availableTariff?.ServicePlanId).ToList();

                if (tariff.IsFollowOnTariff)
                {
                    model.FollowOnTariff = GetAvailableTariff(customerAccount, model, tariff, cmsEnergyContents, tickUSPsFromConfig);
                    // ReSharper disable once PossibleNullReferenceException
                    int daysRemaining = (currentTariff.EndDate - DateTime.Today).Days;
                    model.NewTariffSubHeader = string.Format(AvailableTariffs_Resources.NewTariffSubHeader, model.CurrentTariffViewModel.DisplayName, daysRemaining == 1 ? "1 day" : $"{daysRemaining} days", currentTariff.EndDate.ToSseString(), daysRemaining < 10 ? "text-red" : "");
                    string newTariffStart = currentTariff.EndDate.AddDays(1).ToSseString();
                    model.TariffStartDate = currentTariff.EndDate.AddDays(1);
                    model.NewTariffParagraph1 = string.Format(AvailableTariffs_Resources.NewTariffParagraph1, model.FollowOnTariff.DisplayName, newTariffStart);
                    model.NewTariffStartMessage = string.Format(AvailableTariffs_Resources.NewTariffStartMessage, newTariffStart);
                    double exitFee = tariff.ElectricityDetails?.ExitFee ?? (tariff.GasDetails?.ExitFee ?? 0);
                    TariffGroup tariffGroup = _tariffManager.GetTariffGroup(currentTariff.ServicePlanId);
                    model.BulletText = GetBulletText(exitFee, tariffGroup);
                }
                else if (currentTariff?.ServicePlanId.RemoveSpacesAndConvertToUpper() == availableTariff?.ServicePlanId.RemoveSpacesAndConvertToUpper())
                {
                    if (customer.TariffCalculationMethod == TariffCalculationMethod.CurrentRate)
                    {
                        SetCurrentUsingCalculatedTariff(model, tariff);
                    }
                    else
                    {
                        SetCurrentUsingForecastTariff(model, tariff);
                    }

                    MapCurrentTariffLabelInformation(model, tariff);
                }
                else
                {
                    model.AvailableTariffs.Add(GetAvailableTariff(customerAccount, model, tariff, cmsEnergyContents, tickUSPsFromConfig));
                }
            }

            model.DataLayer = DataLayerMapper.GetDataLayerDictionary(customer, customerAccount);
            return model;
        }

        private static string GetBulletText(double exitFee, TariffGroup tariffGroup)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            string returnText = exitFee == 0 ? AvailableTariffs_Resources.CurrentTariffBullet1 : $"{exitFee.ToPounds()} {AvailableTariffs_Resources.EarlyExitFee}";

            if (tariffGroup == TariffGroup.FixAndControl)
            {
                returnText = AvailableTariffs_Resources.CurrentTariffBullet1FixAndControlExitFeeTariff;
            }

            return returnText;
        }

        public TariffsViewModel MapExtendedModelProperties(TariffsViewModel model)
        {
            model.AvailableTariffs.Sort();

            if (model.AvailableTariffs.Count > 0 && model.AvailableTariffs[0].ElectricityDetails != null)
            {
                model.CurrentTariffViewModel.DayOrStandardLabel = model.AvailableTariffs[0].ElectricityDetails.DayOrStandardLabel;
                model.CurrentTariffViewModel.NightOrOffPeakLabel = model.AvailableTariffs[0].ElectricityDetails.NightOrOffPeakLabel;
            }

            switch (model.FuelType)
            {
                case FuelType.Dual:
                    model.FuelTypeHeader = AvailableTariffs_Resources.HeaderDualFuel;
                    break;
                case FuelType.Electricity:
                    model.FuelTypeHeader = AvailableTariffs_Resources.HeaderElectric;
                    break;
                case FuelType.Gas:
                    model.FuelTypeHeader = AvailableTariffs_Resources.HeaderGas;
                    break;
            }

            model.ShowMultiRateMessage = model.CurrentTariffViewModel.Name.Contains("Economy 10");

            return model;
        }

        public static string LowercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);

            return new string(a);
        }

        private TariffsViewModel GetTariffsViewModel(Customer customer)
        {
            bool isGasRenewal = false, isElectricityRenewal = false;
            DateTime electricityRenewalDate = DateTime.MinValue, gasRenewalDate = DateTime.MinValue;

            if (customer.HasElectricityAccount())
            {
                isElectricityRenewal = customer.ElectricityAccount.CurrentTariff.IsRenewal;
                if (isElectricityRenewal)
                {
                    electricityRenewalDate = electricityRenewalDate == DateTime.MinValue
                        ? customer.ElectricityAccount.CurrentTariff.EndDate
                        : electricityRenewalDate;
                }
            }
            if (customer.HasGasAccount())
            {
                isGasRenewal = customer.GasAccount.CurrentTariff.IsRenewal;
                if (isGasRenewal)
                {
                    gasRenewalDate = gasRenewalDate == DateTime.MinValue
                        ? customer.GasAccount.CurrentTariff.EndDate
                        : gasRenewalDate;
                }
            }

            DateTime renewalDate = electricityRenewalDate > DateTime.Today ? electricityRenewalDate : DateTime.Today;

            renewalDate = gasRenewalDate > DateTime.Today ? gasRenewalDate : renewalDate;

            bool isImmediateRenewal = customer.CustomerSelectedTariff != null && customer.CustomerSelectedTariff.EffectiveDate == DateTime.Today;

            return new TariffsViewModel
            {
                RenewalDate = renewalDate.ToLongDateString(),
                IsRenewal = isElectricityRenewal || isGasRenewal,
                IsImmediateRenewal = isImmediateRenewal,
                FuelType = customer.GetCustomerFuelType(),
                HasMultiRateMeter = customer.ElectricityAccount?.SiteDetails?.HasMultiRateMeter ?? false,
                AvailableTariffs = new List<AvailableTariff>(),
                CurrentTariffViewModel = GetCurrentTariffViewModel(customer),
                AccordionEPPContent = GetAccordionEPPContent(customer)
            };
        }

        private static string GetAccordionEPPContent(Customer customer)
        {
            if (customer.IsCustomerInRenewalPeriod())
                return AvailableTariffs_Resources.AccordionEPPContent_Renewal;

            if (customer.TariffCalculationMethod == TariffCalculationMethod.CurrentRate)
                return AvailableTariffs_Resources.AccordionEPPContent_CurrentRate;

            return AvailableTariffs_Resources.AccordionEPPContent_Original;
        }

        private void MapCurrentTariffLabelInformation(TariffsViewModel viewModel, Tariff currentTariff)
        {
            if (currentTariff?.ElectricityDetails != null)
            {
                double tariffElectricityAnnualCost = GetElectricityAnnualCost(viewModel, currentTariff);

                viewModel.CurrentTariffViewModel.ElectricityDetails = MapElectricityTariffFuelDetails(currentTariff, tariffElectricityAnnualCost);
            }
            if (currentTariff?.GasDetails != null)
            {
                double tariffGasAnnualCost = CalculateAnnualCost(currentTariff.GasDetails.StandingChargeExclVat, currentTariff.GasDetails.UnitRate1ExclVat, viewModel.CurrentTariffViewModel.GasAnnualUsage);

                viewModel.CurrentTariffViewModel.GasDetails = MapGasTariffFuelDetails(currentTariff, tariffGasAnnualCost);
            }
        }

        private CurrentTariffViewModel GetCurrentTariffViewModel(Customer customer)
        {
            CurrentTariffForFuel currentTariff = customer.ElectricityAccount?.CurrentTariff ?? customer.GasAccount?.CurrentTariff;
            CustomerAccount customerAccount = customer.ElectricityAccount ?? customer.GasAccount;
            var currentTariffViewModel = new CurrentTariffViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                Name = currentTariff.Name,
                ExpirationMessage = currentTariff.EndDate.ToSseExpiryDateString(),
                ElectricityAnnualUsage = customer.ElectricityAccount?.CurrentTariff?.AnnualUsageKwh ?? 0,
                ElectricityAnnualCost = customer.ElectricityAccount?.CurrentTariff?.AnnualCost.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                GasAnnualUsage = customer.GasAccount?.CurrentTariff?.AnnualUsageKwh ?? 0,
                // ReSharper disable once PossibleNullReferenceException
                GasAnnualCost = customer.GasAccount?.CurrentTariff.AnnualCost.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                // ReSharper disable once PossibleNullReferenceException
                ShowWHDText = GetShowWHDText(customerAccount, customerAccount.CurrentTariff.TariffType),
                ShowFixAndControlExitFee = _tariffManager.GetTariffGroup(currentTariff.ServicePlanId) == TariffGroup.FixAndControl
            };

            if (customer.TariffCalculationMethod == TariffCalculationMethod.Original)
            {
                currentTariffViewModel.IntroParagraph1 = AvailableTariffs_Resources.StandardTariffIntro;
                currentTariffViewModel.MonthlyCostHeader = string.Format(AvailableTariffs_Resources.MonthlyCostHeader, "Average");
                currentTariffViewModel.AnnualCostHeader = string.Format(AvailableTariffs_Resources.YearlyCostHeader, "Projected");
            }
            else
            {
                int daysRemaining = (currentTariff.EndDate - DateTime.Today).Days;

                if (daysRemaining < 0)
                {
                    currentTariffViewModel.IntroParagraph1 = AvailableTariffs_Resources.StandardTariffIntro;
                }
                else
                {
                    currentTariffViewModel.IntroParagraph1 = AvailableTariffs_Resources.IntroParagraph1;
                    currentTariffViewModel.IntroParagraph2 = string.Format(AvailableTariffs_Resources.IntroParagraph2, daysRemaining == 1 ? "1 day" : $"{daysRemaining} days", currentTariff.EndDate.ToSseString());
                    currentTariffViewModel.IntroParagraph3 = string.Format(AvailableTariffs_Resources.IntroParagraph3, currentTariff.EndDate.ToSseString());
                }

                currentTariffViewModel.MonthlyCostHeader = string.Format(AvailableTariffs_Resources.MonthlyCostHeader, "Your");
                currentTariffViewModel.AnnualCostHeader = string.Format(AvailableTariffs_Resources.YearlyCostHeader, "Your");
            }

            if (customer.ElectricityAccount?.SiteDetails?.HasMultiRateMeter == true)
            {
                if (customer.ElectricityAccount?.CurrentTariff != null)
                {
                    currentTariffViewModel.ElectricityAnnualUsageDayOrStandard =
                        currentTariffViewModel.ElectricityAnnualUsage *
                        customer.ElectricityAccount.CurrentTariff.PeakPercentageOperand;

                    currentTariffViewModel.ElectricityAnnualUsageNightOrOffPeak =
                        currentTariffViewModel.ElectricityAnnualUsage *
                        customer.ElectricityAccount.CurrentTariff.OffPeakPercentageOperand;
                }
            }

            return currentTariffViewModel;
        }

        private static void SetCurrentUsingForecastTariff(TariffsViewModel model, Tariff tariff)
        {
            double.TryParse(model.CurrentTariffViewModel.ElectricityAnnualCost, out double currentElectricityAnnualCost);
            double.TryParse(model.CurrentTariffViewModel.GasAnnualCost, out double currentGasAnnualCost);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (tariff.ElectricityDetails != null && currentElectricityAnnualCost == 0)
            {
                if (model.HasMultiRateMeter)
                {
                    currentElectricityAnnualCost = CalculateAnnualCost(
                        tariff.ElectricityDetails.StandingChargeExclVat,
                        tariff.ElectricityDetails.UnitRate1ExclVat, tariff.ElectricityDetails.UnitRate2ExclVat,
                        model.CurrentTariffViewModel.ElectricityAnnualUsageDayOrStandard,
                        model.CurrentTariffViewModel.ElectricityAnnualUsageNightOrOffPeak);
                }
                else
                {
                    currentElectricityAnnualCost = CalculateAnnualCost(
                        tariff.ElectricityDetails.StandingChargeExclVat,
                        tariff.ElectricityDetails.UnitRate1ExclVat,
                        model.CurrentTariffViewModel.ElectricityAnnualUsage);
                }
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (tariff.GasDetails != null && currentGasAnnualCost == 0)
            {
                currentGasAnnualCost = CalculateAnnualCost(tariff.GasDetails.StandingChargeExclVat, tariff.GasDetails.UnitRate1ExclVat, model.CurrentTariffViewModel.GasAnnualUsage);
            }

            UpdateModelValues(model, currentElectricityAnnualCost, currentGasAnnualCost);
        }

        private static void SetCurrentUsingCalculatedTariff(TariffsViewModel model, Tariff tariff)
        {
            double currentElectricityAnnualCost = 0, currentGasAnnualCost = 0;

            if (tariff.ElectricityDetails != null)
            {
                if (model.HasMultiRateMeter)
                {
                    currentElectricityAnnualCost = CalculateAnnualCost(
                        tariff.ElectricityDetails.StandingChargeExclVat,
                        tariff.ElectricityDetails.UnitRate1ExclVat, tariff.ElectricityDetails.UnitRate2ExclVat,
                        model.CurrentTariffViewModel.ElectricityAnnualUsageDayOrStandard,
                        model.CurrentTariffViewModel.ElectricityAnnualUsageNightOrOffPeak);
                }
                else
                {
                    currentElectricityAnnualCost = CalculateAnnualCost(
                        tariff.ElectricityDetails.StandingChargeExclVat,
                        tariff.ElectricityDetails.UnitRate1ExclVat,
                        model.CurrentTariffViewModel.ElectricityAnnualUsage);
                }
            }

            if (tariff.GasDetails != null)
            {
                currentGasAnnualCost = CalculateAnnualCost(tariff.GasDetails.StandingChargeExclVat, tariff.GasDetails.UnitRate1ExclVat, model.CurrentTariffViewModel.GasAnnualUsage);
            }

            UpdateModelValues(model, currentElectricityAnnualCost, currentGasAnnualCost);
        }

        private static void UpdateModelValues(TariffsViewModel model, double electricityAnnualCost, double gasAnnualCost)
        {
            model.CurrentTariffViewModel.AnnualCost = (electricityAnnualCost + gasAnnualCost).ToPounds();
            model.CurrentTariffViewModel.MonthlyCost = ((electricityAnnualCost + gasAnnualCost) / 12).ToPounds();
            model.CurrentTariffViewModel.ElectricityAnnualCost = electricityAnnualCost.ToPounds();
            model.CurrentTariffViewModel.ElectricityMonthlyCost = (electricityAnnualCost / 12).ToPounds();
            model.CurrentTariffViewModel.GasAnnualCost = gasAnnualCost.ToPounds();
            model.CurrentTariffViewModel.GasMonthlyCost = (gasAnnualCost / 12).ToPounds();
        }

        private AvailableTariff GetAvailableTariff(CustomerAccount customerAccount, TariffsViewModel model, Tariff tariff, List<CMSEnergyContent> cmsEnergyContents, List<TariffTickUsp> tickUSPsFromConfig)
        {
            string servicePlanId = tariff.GasDetails != null ? tariff.GasDetails.ServicePlanId : tariff.ElectricityDetails.ServicePlanId;
            TariffGroup tariffGroup = _tariffManager.GetTariffGroup(servicePlanId);
            var availableTariff = new AvailableTariff
            {
                Name = tariff.Name,
                Tagline = tariffGroup != TariffGroup.None ? _tariffManager.GetTagline(tariff.DisplayName) 
                            : GetCMSContentForATariff(cmsEnergyContents, tariff.DisplayName).TagLine ?? string.Empty,
                TermsAndConditionsPdfLinks = tariffGroup != TariffGroup.None ? _tariffManager.GetPdfLinks(tariff.DisplayName) 
                    : GetCMSContentForATariff(cmsEnergyContents, tariff.DisplayName).PDFList?.Select(d => d.Path).ToList() ?? new List<string>(),
                IsFollowOnTariff = tariff.IsFollowOnTariff,
                TariffGroup = tariffGroup,
                AdditionalTariffCardText = tariffGroup != TariffGroup.None ? _tariffManager.GetSpecialTariffCardText(tariffGroup)
                                            ?? tariff.ElectricityDetails?.AdditionalProductsIncluded
                                            ?? tariff.GasDetails?.AdditionalProductsIncluded
                                            ?? string.Empty : string.Empty,
                TickUsps = GetTickUsps(tariffGroup, cmsEnergyContents, tariff.DisplayName, tickUSPsFromConfig),
                IsSmartTariff = _tariffManager.IsSmart(servicePlanId),
                ShowWHDText = GetShowWHDText(customerAccount, tariff.Type)
            };

            double exitFee = tariff.ElectricityDetails?.ExitFee ?? (double)tariff.GasDetails?.ExitFee;
            availableTariff.ExitFee = exitFee;
            // ReSharper disable once CompareOfFloatsByEqualityOperator - Only care if it's absolutely zero anyway
            availableTariff.ExitFeePerFuel = exitFee == 0
                ? availableTariff.TariffGroup == TariffGroup.FixAndFibre ? AvailableTariffs_Resources.NoEarlyExitFeeFF : AvailableTariffs_Resources.NoEarlyExitFee
                : $"{exitFee.ToPounds()} {AvailableTariffs_Resources.EarlyExitFee}";

            double tariffElectricityAnnualCost = 0;
            double tariffGasAnnualCost = 0;

            if (tariff.ElectricityDetails != null)
            {
                tariffElectricityAnnualCost = GetElectricityAnnualCost(model, tariff);

                availableTariff.ElectricityDetails = MapElectricityTariffFuelDetails(tariff, tariffElectricityAnnualCost);
            }
            if (tariff.GasDetails != null)
            {
                tariffGasAnnualCost = CalculateAnnualCost(tariff.GasDetails.StandingChargeExclVat, tariff.GasDetails.UnitRate1ExclVat, model.CurrentTariffViewModel.GasAnnualUsage);

                availableTariff.GasDetails = MapGasTariffFuelDetails(tariff, tariffGasAnnualCost);
            }
            availableTariff.ProjectedAnnualCostValue = tariffElectricityAnnualCost + tariffGasAnnualCost;
            availableTariff.ProjectedMonthlyCostValue = (availableTariff.ProjectedAnnualCostValue / 12).ToNumber();
            availableTariff.ProjectedAnnualCost = (tariffElectricityAnnualCost + tariffGasAnnualCost).ToPounds();
            availableTariff.ProjectedMonthlyCost = ((tariffElectricityAnnualCost + tariffGasAnnualCost) / 12).ToPounds();
            return availableTariff;
        }

        private static double GetElectricityAnnualCost(TariffsViewModel model, Tariff tariff)
        {
            double tariffElectricityAnnualCost;
            if (model.HasMultiRateMeter)
            {
                tariffElectricityAnnualCost = CalculateAnnualCost(
                    tariff.ElectricityDetails.StandingChargeExclVat,
                    tariff.ElectricityDetails.UnitRate1ExclVat, tariff.ElectricityDetails.UnitRate2ExclVat,
                    model.CurrentTariffViewModel.ElectricityAnnualUsageDayOrStandard,
                    model.CurrentTariffViewModel.ElectricityAnnualUsageNightOrOffPeak);
            }
            else
            {
                tariffElectricityAnnualCost = CalculateAnnualCost(
                    tariff.ElectricityDetails.StandingChargeExclVat,
                    tariff.ElectricityDetails.UnitRate1ExclVat,
                    model.CurrentTariffViewModel.ElectricityAnnualUsage);
            }

            return tariffElectricityAnnualCost;
        }

        private static TariffInformationLabel MapGasTariffFuelDetails(Tariff tariff, double tariffGasAnnualCost)
        {
            return new TariffInformationLabel
            {
                AnnualCost = tariffGasAnnualCost.ToPounds(),
                MonthlyCost = (tariffGasAnnualCost / 12).ToPounds(),
                AnnualCostValue = tariffGasAnnualCost,
                Supplier = Resources.SSEGasSupplier,
                TariffName = tariff.Name,
                TariffType = tariff.Type,
                PaymentMethod = tariff.GasDetails.PaymentMethod,
                UnitRate1 = NumberFormatter.ToPence(tariff.GasDetails.UnitRate1InclVat),
                StandingCharge = NumberFormatter.ToPence(tariff.GasDetails.StandingChargeInclVat),
                TariffEndsOn = tariff.GasDetails.TariffEndDescription,
                PriceGuaranteedUntil = tariff.GasDetails.PriceGuaranteeDescription,
                ExitFees = tariff.GasDetails.ExitFee.ToPounds(),
                DiscountsAndAdditionalCharges = tariff.GasDetails.RateCodeStandardDescription,
                AdditionalProductsAndServicesIncluded =
                                     tariff.GasDetails.AdditionalProductsIncluded ?? AvailableTariffs_Resources.NotApplicable,
                TCR = NumberFormatter.ToPence(tariff.GasDetails.TCR * 100),
                ServicePlanId = tariff.GasDetails.ServicePlanId
            };
        }

        private static TariffInformationLabel MapElectricityTariffFuelDetails(Tariff tariff, double tariffElectricityAnnualCost)
        {
            return new TariffInformationLabel
            {
                AnnualCost = tariffElectricityAnnualCost.ToPounds(),
                MonthlyCost = (tariffElectricityAnnualCost / 12).ToPounds(),
                AnnualCostValue = tariffElectricityAnnualCost,
                Supplier = Resources.SSEElectricitySupplier,
                TariffName = tariff.Name,
                TariffType = tariff.Type,
                PaymentMethod = tariff.ElectricityDetails.PaymentMethod,
                UnitRate1 = NumberFormatter.ToPence(tariff.ElectricityDetails.UnitRate1InclVat),
                UnitRate2 = NumberFormatter.ToPence(tariff.ElectricityDetails.UnitRate2InclVat),
                StandingCharge = NumberFormatter.ToPence(tariff.ElectricityDetails.StandingChargeInclVat),
                TariffEndsOn = tariff.ElectricityDetails.TariffEndDescription,
                PriceGuaranteedUntil = tariff.ElectricityDetails.PriceGuaranteeDescription,
                ExitFees = tariff.ElectricityDetails.ExitFee.ToPounds(),
                DiscountsAndAdditionalCharges = tariff.ElectricityDetails.RateCodeStandardDescription,
                AdditionalProductsAndServicesIncluded = tariff.ElectricityDetails.AdditionalProductsIncluded ?? AvailableTariffs_Resources.NotApplicable,
                TCR = NumberFormatter.ToPence(tariff.ElectricityDetails.TCR * 100),
                ServicePlanId = tariff.ElectricityDetails.ServicePlanId,
                DayOrStandardLabel = tariff.ElectricityDetails.DayOrStandardLebel,
                NightOrOffPeakLabel = tariff.ElectricityDetails.NightOrOffPeakLebel
            };
        }

        private static double CalculateAnnualCost(double standingCharge, double unitRate1, double unitRate2, double usage1, double usage2)
        {
            double annualStandingCharge = Math.Round(standingCharge * 1.05 * 365 / 100, 2);
            double annualUsageCostSplit1 = Math.Round(unitRate1 * 1.05 * usage1 / 100, 2);
            double annualUsageCostSplit2 = Math.Round(unitRate2 * 1.05 * usage2 / 100, 2);
            return annualStandingCharge + annualUsageCostSplit1 + annualUsageCostSplit2;
        }

        private static double CalculateAnnualCost(double standingCharge, double unitRate, double annualUsageKwh)
        {
            double annualStandingCharge = Math.Round(standingCharge * 1.05 * 365 / 100, 2);
            double annualUsageCost = Math.Round(unitRate * 1.05 * annualUsageKwh / 100, 2);

            return annualStandingCharge + annualUsageCost;
        }

        public static SelectedTariff MapSelectedTariff(Customer customer, AvailableTariff selectedTariff, bool isImmediateRenewal)
        {
            return new SelectedTariff
            {
                Name = selectedTariff.Name,
                Tagline = selectedTariff.Tagline,
                ExitFeePerFuel = selectedTariff.ExitFeePerFuel,
                ExitFee = selectedTariff.ExitFee,
                ProjectedAnnualCost = selectedTariff.ProjectedAnnualCost,
                ProjectedMonthlyCost = selectedTariff.ProjectedMonthlyCost,
                ProjectedAnnualCostValue = selectedTariff.ProjectedAnnualCostValue,
                ProjectedMonthlyCostValue = selectedTariff.ProjectedMonthlyCostValue,
                AdditionalProductsIncluded = selectedTariff.AdditionalTariffCardText,
                TermsAndConditionsPdfLink = selectedTariff.TermsAndConditionsPdfLinks,
                EffectiveDate = GetEffectiveDate(customer, isImmediateRenewal),
                TariffGroup = selectedTariff.TariffGroup,
                IsSmartTariff = selectedTariff.IsSmartTariff
            };
        }

        public static SelectedTariffForFuel MapSelectedTariffDetailsForFuel(Customer customer, TariffInformationLabel selectedTariff, bool isImmediateRenewal)
        {
            return new SelectedTariffForFuel
            {
                ServicePlanId = selectedTariff.ServicePlanId,
                AnnualCost = selectedTariff.AnnualCost,
                MonthlyCost = selectedTariff.MonthlyCost,
                AnnualCostValue = selectedTariff.AnnualCostValue,
                Supplier = selectedTariff.Supplier,
                TariffName = selectedTariff.TariffName,
                TariffType = selectedTariff.TariffType,
                PaymentMethod = selectedTariff.PaymentMethod,
                UnitRate1 = selectedTariff.UnitRate1,
                UnitRate2 = selectedTariff.UnitRate2,
                DayOrStandardLabel = selectedTariff.DayOrStandardLabel,
                NightOrOffPeakLabel = selectedTariff.NightOrOffPeakLabel,
                StandingCharge = selectedTariff.StandingCharge,
                TariffEndsOn = selectedTariff.TariffEndsOn,
                PriceGuaranteedUntil = selectedTariff.PriceGuaranteedUntil,
                ExitFees = selectedTariff.ExitFees,
                DiscountsAndAdditionalCharges = selectedTariff.DiscountsAndAdditionalCharges,
                AdditionalProductsAndServicesIncluded = selectedTariff.AdditionalProductsAndServicesIncluded,
                TCR = selectedTariff.TCR,
                EffectiveDate = GetEffectiveDate(customer, isImmediateRenewal)
            };
        }

        private static DateTime GetEffectiveDate(Customer customer, bool isImmediateRenewal)
        {
            DateTime effectiveDate = DateTime.MinValue;

            if (isImmediateRenewal)
            {
                effectiveDate = DateTime.Today;
            }
            else
            {
                if (customer.HasElectricityAccount())
                {
                    effectiveDate = customer.ElectricityAccount.CurrentTariff.EndDate;
                }
                else if (customer.HasGasAccount())
                {
                    effectiveDate = customer.GasAccount.CurrentTariff.EndDate;
                }
            }
            return effectiveDate;
        }

        private static bool GetShowWHDText(CustomerAccount account, string tariffType)
        {
            return account.IsWHD && tariffType.ToLower() == Evergreen;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static CMSEnergyContent GetCMSContentForATariff(List<CMSEnergyContent> cmsEnergyContents, string tariffName)
        {
            return cmsEnergyContents?.FirstOrDefault(x => string.Equals(x.TariffNameWithoutTariffWording, tariffName, StringComparison.InvariantCultureIgnoreCase)) ?? new CMSEnergyContent();
        }

        private static Dictionary<string, string> GetTickUsps(TariffGroup tariffGroup, List<CMSEnergyContent> cmsEnergyContents, string tariffName, List<TariffTickUsp> tickUSPsFromConfig)
        {
            Dictionary<string, string> dict;

            switch (tariffGroup)
            {
                case TariffGroup.None:
                    dict = MapTariffTickUspListToDictionary(GetCMSContentForATariff(cmsEnergyContents, tariffName).TickUsps) ?? new Dictionary<string, string>();
                    break;
                case TariffGroup.FixAndProtect:
                case TariffGroup.FixAndFibre:
                case TariffGroup.FixAndControl:
                case TariffGroup.FixAndDrive:
                    dict = null;
                    break;
                case TariffGroup.Standard:
                case TariffGroup.StandardPayGoSMETS2:
                    dict = MapTariffTickUspListToDictionary(tickUSPsFromConfig);
                    break;
                default:
                    dict = null;
                    break;
            }

            return dict;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static Dictionary<string, string> MapTariffTickUspListToDictionary(List<TariffTickUsp> list)
        {
            return list?.ToDictionary(tickUsp => tickUsp.Header, tickUsp => string.IsNullOrWhiteSpace(tickUsp.Description) ? string.Empty : $": {LowercaseFirst(tickUsp.Description)}");
        }
    }
}