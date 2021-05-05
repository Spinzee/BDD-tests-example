namespace Products.Service.Energy.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Broadband.Managers;
    using Broadband.Mappers;
    using Common.Managers;
    using Core;
    using Core.Enums;
    using Helpers;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Broadband;
    using Model.Energy;
    using Model.Enums;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Broadband.Extensions;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
    using Tariff = Model.Energy.Tariff;

    public class TariffMapper : ITariffMapper
    {
        private const string FuelCDNPath = "/Content/Svgs/icons/fuel/";
        private const string DuelFuelIcon = "dual-fuel-2colour.svg";
        private const string ElectricityOnlyIcon = "electricity-2colour.svg";
        private const string GasOnlyIcon = "gas-2colour.svg";
        private const string HesIcon = "hes-icon.svg";
        private const string BroadbandIcon = "broadband.svg";
        private const string Energy = "energy";
        private const string Electricity = "electricity";
        private const string Gas = "gas";
        private const string MaximumSpeedCap = "MaximumSpeedCap";
        private const string MaxDownload = "MaxDownload";
        private const string Expand = "expand";
        private const string BroadbandBundleMegaModalId = "#BroadbandBundleMegaModal";
        private const string FixNProtectBundleMegaModalId = "#FixNProtectBundleMegaModal";
        private readonly IBroadbandManager _broadbandManager;
        private readonly IConfigManager _configManager;
        private readonly ITariffManager _tariffManager;

        public TariffMapper(ITariffManager tariffManager, IBroadbandManager broadbandManager, IConfigManager configManager)
        {
            _tariffManager = tariffManager;
            _broadbandManager = broadbandManager;
            _configManager = configManager;
        }

        public TariffsViewModel ToTariffViewModel(Tariff tariff, EnergyCustomer energyCustomer, List<CMSEnergyContent> cmsEnergyContents)
        {
            string originalBundleMonthlyCost = (tariff.BundlePackage?.MonthlyOriginalCost ?? 0.0).ToCurrency();
            string projectedBundleMonthlySavings = (tariff.BundlePackage?.MonthlySavings ?? 0.00).ToCurrency();
            string projectedYearlyCost = tariff.GetProjectedCombinedYearlyCost().ToCurrency();
            string projectedYearlyEnergyCost = tariff.GetProjectedYearlyEnergyCost().ToCurrency();
            string projectedGasMonthlyCost = tariff.GetProjectedMonthlyGasCost()?.ToCurrency();
            string projectedGasYearlyCost = tariff.GetProjectedYearlyGasCost()?.ToCurrency();
            string projectedElectricityMonthlyCost = tariff.GetProjectedMonthlyElectricityCost()?.ToCurrency();
            string projectedElectricityYearlyCost = tariff.GetProjectedYearlyElectricityCost()?.ToCurrency();
            string bundleProjectedYearlySavings = tariff.BundlePackage?.YearlySavings.ToCurrency();
            string monthlySavingsPercentage = $"{tariff.BundlePackage?.MonthlySavingsPercentage ?? 0.0}%";
            string energyDescription = energyCustomer.IsDualFuel() ? Energy : energyCustomer.IsElectricityOnly() ? Electricity : Gas;

            string bundleDisclaimer1 = string.Empty, bundleDisclaimer2 = string.Empty;
            if (tariff.IsBroadbandBundle())
            {
                bundleDisclaimer1 = string.Format(AvailableBundleTariffs_Resources.BundleDisclaimer_Broadband
                    , bundleProjectedYearlySavings
                    , projectedBundleMonthlySavings
                    , monthlySavingsPercentage
                    , projectedBundleMonthlySavings
                    , bundleProjectedYearlySavings
                    , originalBundleMonthlyCost);

                var bundleDisclaimer2Builder = new StringBuilder();
                bundleDisclaimer2Builder.Append(string.Format(AvailableBundleTariffs_Resources.BundleDisclaimer_Annual, energyDescription,
                    projectedYearlyEnergyCost));
                if (energyCustomer.IsDualFuel())
                {
                    bundleDisclaimer2Builder.Append(string.Format(AvailableBundleTariffs_Resources.BundleDisclaimer_DualFuel, projectedGasYearlyCost,
                        projectedElectricityYearlyCost));
                }

                bundleDisclaimer2Builder.Append(AvailableBundleTariffs_Resources.BundleDisclaimer_DD);
                bundleDisclaimer2 = bundleDisclaimer2Builder.ToString();
            }
            else if (tariff.IsHesBundle())
            {
                bundleDisclaimer1 = string.Format(AvailableBundleTariffs_Resources.BundleDisclaimer_HES
                    , bundleProjectedYearlySavings
                    , originalBundleMonthlyCost
                    , originalBundleMonthlyCost
                    , bundleProjectedYearlySavings);

                string fuelDisclaimer = string.Empty;
                if (energyCustomer.IsDualFuel())
                {
                    fuelDisclaimer = string.Format(
                        AvailableBundleTariffs_Resources.BundleDisclaimer2_HES_Dual,
                        projectedYearlyEnergyCost,
                        projectedGasYearlyCost,
                        projectedElectricityYearlyCost);
                }
                else if (energyCustomer.IsGasOnly())
                {
                    fuelDisclaimer = string.Format(
                        AvailableBundleTariffs_Resources.BundleDisclaimer2_HES_GasOnly,
                        projectedGasYearlyCost
                    );
                }

                bundleDisclaimer2 = fuelDisclaimer + AvailableBundleTariffs_Resources.BundleDisclaimer_DD;
            }

            string energyIconName = energyCustomer.IsDualFuel()
                ? DuelFuelIcon
                : energyCustomer.IsElectricityOnly()
                    ? ElectricityOnlyIcon
                    : GasOnlyIcon;

            string bundleDisclaimerModalText = GetBroadbandMoreInformationDisclaimerText1(
                projectedBundleMonthlySavings,
                originalBundleMonthlyCost,
                bundleProjectedYearlySavings,
                originalBundleMonthlyCost,
                projectedBundleMonthlySavings);

            var tariffsViewModel = new TariffsViewModel
            {
                TariffTagLine = tariff.TariffGroup != TariffGroup.None ? _tariffManager.GetTagline(tariff.DisplayName) 
                    : GetCMSContentForATariff(cmsEnergyContents, tariff.DisplayName)?.TagLine ?? string.Empty,
                TermsAndConditionsPdfLinks = _tariffManager.GetTermsAndConditionsPdfs(tariff.DisplayName, tariff.TariffGroup, cmsEnergyContents),
                DisplayName = $"{tariff.DisplayName} {tariff.DisplayNameSuffix}",
                TariffName = tariff.DisplayName,
                ProjectedYearlyCost = projectedYearlyCost,
                ProjectedElectricityMonthlyCost = projectedElectricityMonthlyCost,
                ProjectedGasMonthlyCost = projectedGasMonthlyCost,
                ProjectedElectricityYearlyCost = tariff.ElectricityProduct?.ProjectedYearlyCost?.ToCurrency(),
                ProjectedGasYearlyCost = tariff.GasProduct?.ProjectedYearlyCost?.ToCurrency(),
                ProjectedMonthlyEnergyCost = tariff.GetProjectedMonthlyEnergyCost().ToCurrency(),
                ProjectedMonthlyCost = tariff.GetProjectedMonthlyCost().ToCurrency(),
                GasDirectDebitAmount = tariff.GetProjectedMonthlyGasCost()?.RoundUpToNearestPoundWithPoundSign(),
                ElectricityDirectDebitAmount = tariff.GetProjectedMonthlyElectricityCost()?.RoundUpToNearestPoundWithPoundSign(),
                HeatingBreakdownCoverDirectDebitAmount = tariff.BundlePackage?.MonthlyDiscountedCost.ToCurrency(),
                FuelType = tariff.GetFuelType(),
                TariffId = tariff.TariffId,
                GasTariffInformationLabel =
                    tariff.GasProduct == null ? null : GetTariffInformationLabel(tariff.GasProduct, tariff.DisplayName, true, energyCustomer),
                ElectricityTariffInformationLabel = tariff.ElectricityProduct == null
                    ? null
                    : GetTariffInformationLabel(tariff.ElectricityProduct, tariff.DisplayName, false, energyCustomer),
                CloseButtonAlt = AvailableTariffs_Resources.ModalCloseButtonAlt,
                TariffGroup = tariff.TariffGroup,
                EnergyTickUsps = tariff.EnergyTickUsps?
                    .Select(t => new TariffTickUspViewModel(t.Header, t.Description, t.DisplayOrder))
                    .OrderBy(t => t.DisplayOrder),
                BundlePackageTickUsps = tariff.BundlePackage?.TickUsps
                    .Select(t => new TariffTickUspViewModel(t.Header, t.Description, t.DisplayOrder))
                    .OrderBy(t => t.DisplayOrder),
                IsBundle = tariff.IsBundle,
                BundlePackageType = tariff.IsBundle ? tariff.BundlePackage.BundlePackageType : BundlePackageType.None,
                SubmitButtonCssClass = tariff.BundlePackage?.BundlePackageType == BundlePackageType.FixAndFibre ? "broadband-bundle-signup" : string.Empty,
                OriginalBundlePackageMonthlyCost = originalBundleMonthlyCost,
                ProjectedBundlePackageMonthlyCost = FormatBundlePackageMonthlyPrice(tariff.BundlePackage),
                ProjectedBundlePackageMonthlySavings =
                    string.Format(AvailableBundleTariffs_Resources.BroadbandMonthlySavingsTxt, projectedBundleMonthlySavings),
                BundleProjectedYearlySavings = bundleProjectedYearlySavings,
                BundleDisclaimer1Text = bundleDisclaimer1,
                BundleDisclaimer2Text = bundleDisclaimer2,
                EnergyIconPath = $"{FuelCDNPath}{energyIconName}",
                DetailsHeaderIconClass = Expand,
                SubmitFormId = $"selectedForm-{tariff.TariffId}",
                BundlePackageIconFileName = BundleIconFileName(tariff.BundlePackageType),
                MoreInformationModalId = GetMoreInformationModalId(tariff.BundlePackageType)
            };

            if (tariff.IsHesBundle())
            {
                tariffsViewModel.BundlePackageDisplayText = ProductFeatures_Resources.HesPackageDisplayText;
                tariffsViewModel.BundlePackagePriceLbl = ProductFeatures_Resources.HesBundlePackagePriceLbl;
                tariffsViewModel.UpgradeSectionHeaderText = AvailableBundleTariffs_Resources.FnPUpgradeSectionHeaderTxt;
                tariffsViewModel.UpgradeSectionBodyText = AvailableBundleTariffs_Resources.FnPUpgradeSectionBodyTxt;
            }
            else if (tariff.IsBroadbandBundle())
            {
                tariffsViewModel.BundlePackageDisplayText = ProductFeatures_Resources.BroadbandProductDisplayText;
                tariffsViewModel.BundlePackagePriceLbl = ProductFeatures_Resources.BroadbandBundlePackagePriceLbl;
                tariffsViewModel.UpgradeSectionHeaderText = AvailableBundleTariffs_Resources.FnFUpgradeSectionHeaderTxt;
                tariffsViewModel.UpgradeSectionBodyText = AvailableBundleTariffs_Resources.FnFUpgradeSectionBodyTxt;
            }

            if (tariffsViewModel.BundlePackageType == BundlePackageType.FixAndFibre)
            {
                tariffsViewModel.BroadbandMoreInformation = GetBroadbandMoreInformationViewModel(
                    tariffsViewModel.ProjectedBundlePackageMonthlyCost,
                    tariffsViewModel.OriginalBundlePackageMonthlyCost,
                    bundleDisclaimerModalText,
                    projectedBundleMonthlySavings);
            }

            if (tariffsViewModel.BundlePackageType == BundlePackageType.FixAndProtect)
            {
                tariffsViewModel.HesMoreInformation = GetHesMoreInformationViewModel(
                    tariff,
                    originalBundleMonthlyCost,
                    projectedBundleMonthlySavings,
                    bundleProjectedYearlySavings);
            }

            return tariffsViewModel;
        }

        public string GetBroadbandMoreInformationDisclaimerText1(
            string projectedBundlePackageMonthlyCost,
            string originalBundlePackageMonthlyCost,
            string bundleProjectedYearlySavings,
            string originalBundleMonthlyCost,
            string projectedBundleMonthlySavings)
            => string.Format(
                AvailableBundleTariffs_Resources.BundleDisclaimerModalText
                , projectedBundleMonthlySavings
                , bundleProjectedYearlySavings
                , projectedBundleMonthlySavings
                , projectedBundleMonthlySavings
                , bundleProjectedYearlySavings
                , originalBundleMonthlyCost
            );

        public string GetMoreInformationModalId(BundlePackageType bundlePackageType)
        {
            switch (bundlePackageType)
            {
                case BundlePackageType.FixAndFibre:
                    return BroadbandBundleMegaModalId;
                case BundlePackageType.FixAndProtect:
                    return FixNProtectBundleMegaModalId;
                // ReSharper disable once RedundantCaseLabel
                case BundlePackageType.None:
                default:
                    return string.Empty;
            }
        }

        public BroadbandMoreInformationViewModel GetBroadbandMoreInformationViewModel(
            string projectedBundlePackageMonthlyCost,
            string originalBundlePackageMonthlyCost,
            string bundleDisclaimerModalText,
            string projectedBundleMonthlySavings)
        {
            return new BroadbandMoreInformationViewModel
            {
                ProjectedBroadbandMonthlyCost = projectedBundlePackageMonthlyCost,
                OriginalBroadbandMonthlyCost = originalBundlePackageMonthlyCost,
                BundleDisclaimerModalText = bundleDisclaimerModalText,
                ProjectedBroadbandMonthlySavingsAmount = projectedBundleMonthlySavings,
                BroadbandPackageSpeed = new BroadbandPackageSpeedViewModel
                {
                    MaxDownload = _configManager.GetAppSetting(MaxDownload),
                    MaximumSpeedCap = GetSpeedCap(MaximumSpeedCap),
                    PackageDescription = AvailableBundleTariffs_Resources.FixNFibrePackageDescription,
                    HeaderText = AvailableBundleTariffs_Resources.BroadbandSpeedHeader,
                    ShowHeaderText = true
                }
            };
        }

        public HesMoreInformationViewModel GetHesMoreInformationViewModel(
            Tariff tariff,
            string originalBundleMonthlyCost,
            string projectedBundleMonthlySavings,
            string bundleProjectedYearlySavings)
        {
            var hesMoreInformation = new HesMoreInformationViewModel
            {
                ExcessAmount = tariff.BundlePackage?.HesMoreInformation?.Products?.FirstOrDefault()?.Excess.ToCurrency(),
                WhatsExcluded = tariff.BundlePackage?.HesMoreInformation?.WhatsExcluded,
                WhatsIncluded = tariff.BundlePackage?.HesMoreInformation?.WhatsIncluded,
                OriginalFixNProtectMonthlyCost = originalBundleMonthlyCost,
                ProjectedMonthlySavingsAmount = projectedBundleMonthlySavings,
                BundleDisclaimerModalText = string.Format(AvailableBundleTariffs_Resources.FnPBundleDisclaimerModalText, bundleProjectedYearlySavings),
            };
            hesMoreInformation.ExcessText = string.Format(ProductFeatures_Resources.FixNProtectExcessText, hesMoreInformation.ExcessAmount);
            return hesMoreInformation;
        }

        public IEnumerable<string> GetBundlePackageFeatures(BundlePackageType bundlePackageType, Tariff selectedTariff)
        {
            switch (bundlePackageType)
            {
                case BundlePackageType.FixAndProtect:
                    if (selectedTariff.IsUpgrade)
                    {
                        return GetFixNProtectPlusPackageFeatures();
                    }
                    return GetFixNProtectPackageFeatures();
                case BundlePackageType.FixAndFibre:
                    return GetFixNFibrePackageFeatures();
                default:
                    return new List<string>();
            }
        }

        public TariffInformationLabelViewModel GetTariffInformation(Product product, FuelCategory fuelCategory)
        {
            bool isMultiRate = fuelCategory == FuelCategory.MultiRate;
            DateTime.TryParse(product.EndOfTariffDate, out DateTime endDate);
            bool useEndDate = endDate < DateTime.Today && endDate > new DateTime(1900, 1, 1);

            return new TariffInformationLabelViewModel
            {
                ServicePlanId = product.ServicePlanId,
                Supplier = fuelCategory == FuelCategory.Gas ? Resources.SSEGasSupplier : Resources.SSEElectricitySupplier,
                TariffName = product.ServicePlanInvoiceDescription,
                TariffType = product.TariffType.ToString(),
                PaymentMethod = RateCodeHelper.GetPaymentMethod(product.RateCode),
                RateCode = product.RateCode?.ToString(),
                RateCodeDescription = RateCodeHelper.GetRateCodeDescription(product.RateCode),
                UnitRate = $"{product.UnitRate1InclVAT.ToTwoDecimalPlacesString()}{AvailableTariffs_Resources.TILUnitRate}",
                UnitRatePeak = isMultiRate ? $"{product.UnitRate1InclVAT.ToTwoDecimalPlacesString()}{AvailableTariffs_Resources.TILUnitRate}" : string.Empty,
                UnitRateOffPeak = isMultiRate ? $"{product.UnitRate2InclVAT.ToTwoDecimalPlacesString()}{AvailableTariffs_Resources.TILUnitRate}" : string.Empty,
                StandingCharge = $"{product.StandingCharge.ToTwoDecimalPlacesString()}{AvailableTariffs_Resources.TILStandingCharge}",
                FuelType = fuelCategory == FuelCategory.Gas ? FuelType.Gas : FuelType.Electricity,
                ExitFee = product.ExitFee1 > 0 ? $"{product.ExitFee1.Value.FromPenceToPound()}" : AvailableTariffs_Resources.TILNotApplicable,
                Discount = FormatText(product.RateCodeStandardDescription),
                AdditionalProducts = FormatText(product.LoyaltyBenefits),
                TariffEndsOn = useEndDate ? endDate.ToString("dd MMM yyyy") : product.EndOfTariffDateDescription,
                PriceGuaranteedUntil = useEndDate ? endDate.ToString("dd MMM yyyy") : product.PriceGuaranteeDateDescription,
                StandingChargeValue = product.StandingCharge.ToTwoDecimalPlacesString(),
                StandingChargeValueExVAT = product.StandingChargeExVAT.ToTwoDecimalPlacesString(),
                UnitRates = GetUnitRates(product, isMultiRate),
                IsBundle = product.IsBundle
            };
        }

        public string FormatBundlePackageMonthlyPrice(BundlePackage bundlePackage) =>
            Math.Abs((bundlePackage?.MonthlyDiscountedCost ?? 0.0)) <= 0.0
                ? ProductFeatures_Resources.HesBundlePackagePriceLbl
                : bundlePackage?.MonthlyDiscountedCost.ToCurrency();


        public string BundleIconFileName(BundlePackageType bundlePackageType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (bundlePackageType)
            {
                case BundlePackageType.FixAndProtect:
                    return HesIcon;
                case BundlePackageType.FixAndFibre:
                    return BroadbandIcon;
                default:
                    return string.Empty;
            }
        }

        public YourPriceViewModel GetYourPriceViewModel(EnergyCustomer energyCustomer, string webClientBaseUrl)
        {
            TalkProduct talkProduct = energyCustomer.SelectedBroadbandProduct?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode);
            Tariff tariff = energyCustomer.SelectedTariff;
            string basketToggleIconBaseUrl = $"{webClientBaseUrl}/Content/Svgs/icons";
            int totalItemsInBasket = TotalBasketItems(talkProduct, energyCustomer.SelectedExtras);
            bool showPhonePackage = (null != talkProduct) && !IsDefaultTalkProduct(talkProduct);
            var selectedExtras = new HashSet<ExtrasItemViewModel>(energyCustomer.SelectedExtras.Select(extra =>
                new ExtrasItemViewModel(extra.Name,
                    extra.BundlePrice,
                    extra.ProductCode,
                    extra.TagLine,
                    extra.BulletList1,
                    extra.BulletList2,
                    true,
                    webClientBaseUrl)));
            double basketTotal = BasketTotal(tariff, talkProduct, energyCustomer.SelectedExtras);
            var upgradeItemViewModel = new PhonePackageUpgradeViewModel(talkProduct?.TalkCode.GetTelName(), talkProduct?.GetMonthlyTalkCost() ?? 0.0, webClientBaseUrl);

            var viewModel = new YourPriceViewModel
            {
                TariffName = $"{tariff.DisplayName} {energyCustomer.SelectedTariff?.DisplayNameSuffix}",
                ProjectedMonthlyTotalFullValue = basketTotal.AmountSplitInPounds(),
                ProjectedMonthlyTotalPenceValue = basketTotal.AmountSplitInPence(),
                ElectricityPerMonth = tariff.GetProjectedMonthlyElectricityCost()?.ToString("C"),
                GasPerMonth = tariff.GetProjectedMonthlyGasCost()?.ToString("C"),
                Extra = GetExtraForYourPriceViewModel(energyCustomer),
                EnergyPerMonth = energyCustomer.SelectedTariff?.GetProjectedMonthlyEnergyCost().ToString("C"),
                IsBundle = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsBundle,
                HasAnnualBundleSavings = energyCustomer.SelectedTariff?.BundlePackage?.YearlySavings > 0,
                IsBroadbandBundle = energyCustomer.SelectedTariff.IsBroadbandBundle(),
                IsFixNProtectBundle = energyCustomer.SelectedTariff.IsHesBundle(),
                BundlePackageOriginalPrice = energyCustomer.SelectedTariff?.BundlePackage?.MonthlyOriginalCost.ToCurrency(),
                BundlePackagePrice = FormatBundlePackageMonthlyPrice(tariff.BundlePackage),
                ShowPhonePackage = showPhonePackage,
                BundlePackageType = tariff.BundlePackageType,
                BundlePackageFeatures = GetBundlePackageFeatures(tariff.BundlePackageType, tariff),
                NewInstallationFee = _broadbandManager.GetInstallationFee().ToCurrency(),
                BroadbandApplyInstallationFee = energyCustomer.SelectedTariff.IsBroadbandBundle() && energyCustomer.ApplyInstallationFee,
                TotalItemsInBasket = totalItemsInBasket,
                BasketTogglerIconFilepath = $"{basketToggleIconBaseUrl}/basket-trigger-{totalItemsInBasket}item.svg",
                BasketToggleIconBaseUrl = basketToggleIconBaseUrl,
                CloseButtonImgPath = $"{webClientBaseUrl}/Content/Svgs/close-btn.svg",
                SelectedExtras = selectedExtras,
                ShowExtras = selectedExtras.Count > 0,
                BasketCssClass = $"basket-{totalItemsInBasket}-items",
                PhonePackageUpgradeViewModel = upgradeItemViewModel
            };
            
            int? bundleContractLength =  viewModel.IsBundle ? (int?)(energyCustomer.SelectedTariff?.BundlePackage?.BundlePackageType == BundlePackageType.FixAndFibre ? 18 : 12) : null;

            viewModel.AnnualSavingsText = viewModel.IsBundle
                ? string.Format(YourPrice_Resources.AnnualSavingsText, energyCustomer.SelectedTariff?.BundlePackage?.YearlySavings.ToCurrency(), bundleContractLength.Value)
                : string.Empty;

            string totalDiscount = energyCustomer.SelectedTariff?.GetTotalDirectDebitDiscount().ToString("C");
            string gasDiscount = energyCustomer.SelectedTariff?.GetDirectDebitDiscountForGas().ToString("C");
            string electricityDiscount = energyCustomer.SelectedTariff?.GetDirectDebitDiscountForElectricity().ToString("C");

            if (viewModel.BundlePackageType == BundlePackageType.FixAndFibre)
            {
                viewModel.BundlePackageHeaderText = YourPrice_Resources.FixAndFibreHeader;
                viewModel.BundlePackageSubHeaderText = tariff.IsUpgrade ? YourPrice_Resources.FixAndFibrePlusSubHeaderText : YourPrice_Resources.FixAndFibreSubHeaderText;
            }
            else if (viewModel.BundlePackageType == BundlePackageType.FixAndProtect)
            {
                viewModel.BundlePackageHeaderText = tariff.IsUpgrade ? YourPrice_Resources.FixNProtectPlusTextHeader : YourPrice_Resources.FixNProtectTextHeader;
            }

            if (energyCustomer.IsDirectDebit() && energyCustomer.SelectedTariff.IsFixedTariff())
            {
                viewModel.Discount = energyCustomer.SelectedFuelType == FuelType.Dual
                    ? string.Format(AvailableTariffs_Resources.FixedTariffDualFuelDiscountText, Common_Resources.DualFuelDiscount, Common_Resources.SingleFuelDiscount)
                    : string.Format(AvailableTariffs_Resources.FixedTariffSingleFuelDiscountText, Common_Resources.SingleFuelDiscount);
            }

            if (!energyCustomer.IsDirectDebit() || !energyCustomer.SelectedTariff.IsStandardTariff())
            {
                return viewModel;
            }

            viewModel.Discount = energyCustomer.SelectedFuelType == FuelType.Dual
                ? string.Format(AvailableTariffs_Resources.StandardTariffDualFuelDiscountText, totalDiscount, gasDiscount, electricityDiscount)
                : string.Format(AvailableTariffs_Resources.StandardTariffSingleFuelDiscountText, totalDiscount);

            return viewModel;
        }

        public TariffGroup GetTariffGroupForProduct(Product product)
        {
            return _tariffManager.GetTariffGroup(product.ServicePlanId);
        }

        private int TotalBasketItems(TalkProduct talkProduct, IReadOnlyCollection<Extra> selectedExtras)
        {
            int total = 1;
            if (null != talkProduct && !IsDefaultTalkProduct(talkProduct))
            {
                total++;
            }

            total += selectedExtras.Count;
            return total;
        }

        private static double BasketTotal(Tariff selectedTariff, TalkProduct selectedTalkProduct, IEnumerable<Extra> selectedExtras)
        {
            double tariffTotal = selectedTariff.GetProjectedMonthlyCost(selectedTalkProduct);
            double extrasTotal = selectedExtras.Aggregate(0.0, (total, extra) => total + extra.BundlePrice);
            return (tariffTotal + extrasTotal);
        }

        public bool IsDefaultTalkProduct(TalkProduct talkProduct)
        {
            string notDisplayedBroadbandCode = _configManager.GetValueForKeyFromSection("bundleManagement", "BroadbandUpgradesNotToBeDisplayedInBasket", talkProduct?.ProductCode);
            return !(string.IsNullOrEmpty(notDisplayedBroadbandCode));
        }

        private static IEnumerable<string> GetFixNProtectPackageFeatures()
        {
            var bundlePackageFeatures = new List<string>
            {
                ProductFeatures_Resources.FixNProtectFeature1,
                ProductFeatures_Resources.FixNProtectFeature2,
                ProductFeatures_Resources.FixNProtectFeature3
            };
            return bundlePackageFeatures;
        }

        private static IEnumerable<string> GetFixNProtectPlusPackageFeatures()
        {
            var bundlePackageFeatures = new List<string>
            {
                ProductFeatures_Resources.FixNProtectFeature1,
                ProductFeatures_Resources.FixNProtectPlusFeature2,
                ProductFeatures_Resources.FixNProtectFeature3,
                ProductFeatures_Resources.FixNProtectPlusFeature4
            };
            return bundlePackageFeatures;
        }

        private static IEnumerable<string> GetFixNFibrePackageFeatures()
        {
            var packageFeatures = new List<string>
            {
                ProductFeatures_Resources.BroadbandFeature1,
                ProductFeatures_Resources.BroadbandFeature2
            };
            return packageFeatures;
        }

        private static TariffInformationLabelViewModel GetTariffInformationLabel(
            Product product,
            string tariffDisplayName,
            bool isGasProduct,
            EnergyCustomer energyCustomer)
        {
            bool isMultiRate = energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Economy7 && !isGasProduct;
            return new TariffInformationLabelViewModel
            {
                Supplier = isGasProduct ? Resources.SSEGasSupplier : Resources.SSEElectricitySupplier,
                TariffName = tariffDisplayName,
                TariffType = product.TariffType.ToString(),
                PaymentMethod = GetFormattedPaymentMethod(energyCustomer.SelectedPaymentMethod),
                UnitRate = $"{(product.UnitRate1InclVAT ?? 0).ToNumber()}{AvailableTariffs_Resources.TILUnitRate}",
                UnitRatePeak = isMultiRate ? $"{(product.UnitRate1InclVAT ?? 0).ToNumber()}{AvailableTariffs_Resources.TILUnitRate}" : string.Empty,
                UnitRateOffPeak = isMultiRate ? $"{(product.UnitRate2InclVAT ?? 0).ToNumber()}{AvailableTariffs_Resources.TILUnitRate}" : string.Empty,
                StandingCharge = $"{(product.StandingCharge ?? 0).ToNumber()}{AvailableTariffs_Resources.TILStandingCharge}",
                FuelType = energyCustomer.SelectedFuelType,
                ExitFee = product.ExitFee1 > 0 ? $"{product.ExitFee1.Value.FromPenceToPound()}" : AvailableTariffs_Resources.TILNotApplicable,
                Discount = FormatText(product.RateCodeStandardDescription),
                AdditionalProducts = FormatText(product.LoyaltyBenefits),
                TariffEndsOn = product.EndOfTariffDateDescription,
                PriceGuaranteedUntil = product.PriceGuaranteeDateDescription
            };
        }

        private static string FormatText(string textToFormat) => !string.IsNullOrEmpty(textToFormat) ? textToFormat.Replace("#163;", "£") : AvailableTariffs_Resources.TILNotApplicable;

        private string GetExtraForYourPriceViewModel(EnergyCustomer energyCustomer)
        {
            TariffGroup tariffGroup = energyCustomer.SelectedTariff?.TariffGroup ?? TariffGroup.None;
            return tariffGroup == TariffGroup.FixAndFibre ? string.Empty : _tariffManager.GetSpecialTariffCardText(tariffGroup);
        }

        private static string GetFormattedPaymentMethod(PaymentMethod energyCustomerSelectedPaymentMethod)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (energyCustomerSelectedPaymentMethod)
            {
                case PaymentMethod.MonthlyDirectDebit:
                    return "Direct Debit";
                case PaymentMethod.Quarterly:
                    return "Quarterly on demand";
                case PaymentMethod.PayAsYouGo:
                    return "Pay As You Go";
                default:
                    return "None";
            }
        }

        private static List<UnitRateViewModel> GetUnitRates(Product product, bool isMultiRate)
        {
            var unitRates = new List<UnitRateViewModel>
            {
                new UnitRateViewModel
                {
                    UnitRateExVAT = product.UnitRate1ExVAT.ToTwoDecimalPlacesString(),
                    UnitRateInclVAT = product.UnitRate1InclVAT.ToTwoDecimalPlacesString(),
                    UnitRateLabel = isMultiRate ? GetLabel(product.UnitRate1InvoiceDescription) : "Unit Rate"
                }
            };

            // ReSharper disable once InvertIf
            if (isMultiRate)
            {
                if (!string.IsNullOrEmpty(product.UnitRate2InvoiceDescription))
                {
                    unitRates.Add(new UnitRateViewModel
                    {
                        UnitRateExVAT = product.UnitRate2ExVAT.ToTwoDecimalPlacesString(),
                        UnitRateInclVAT = product.UnitRate2InclVAT.ToTwoDecimalPlacesString(),
                        UnitRateLabel = GetLabel(product.UnitRate2InvoiceDescription)
                    });
                }

                if (!string.IsNullOrEmpty(product.UnitRate3InvoiceDescription))
                {
                    unitRates.Add(new UnitRateViewModel
                    {
                        UnitRateExVAT = product.UnitRate3ExVAT.ToTwoDecimalPlacesString(),
                        UnitRateInclVAT = product.UnitRate3InclVAT.ToTwoDecimalPlacesString(),
                        UnitRateLabel = GetLabel(product.UnitRate3InvoiceDescription)
                    });
                }

                if (!string.IsNullOrEmpty(product.UnitRate4InvoiceDescription))
                {
                    unitRates.Add(new UnitRateViewModel
                    {
                        UnitRateExVAT = product.UnitRate4ExVAT.ToTwoDecimalPlacesString(),
                        UnitRateInclVAT = product.UnitRate4InclVAT.ToTwoDecimalPlacesString(),
                        UnitRateLabel = GetLabel(product.UnitRate4InvoiceDescription)
                    });
                }
            }

            return unitRates;
        }

        private static string GetLabel(string unitRateInvoiceDescription) => unitRateInvoiceDescription?.Replace(" energy", "");

        private int GetSpeedCap(string key)
        {
            string cap = _configManager.GetAppSetting(key);
            if (string.IsNullOrEmpty(cap))
            {
                return 0;
            }

            int.TryParse(cap.Substring(0, 2), out int speedCap);
            return speedCap;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public static CMSEnergyContent GetCMSContentForATariff(List<CMSEnergyContent> cmsEnergyContents, string tariffName)
        {
            return cmsEnergyContents?.FirstOrDefault(x => string.Equals(x.TariffNameWithoutTariffWording, tariffName, StringComparison.InvariantCultureIgnoreCase)) ?? new CMSEnergyContent();
        }
    }
}