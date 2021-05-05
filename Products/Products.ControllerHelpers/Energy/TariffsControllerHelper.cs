namespace Products.ControllerHelpers.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Infrastructure;
    using Infrastructure.Logging;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using Service.Broadband.Managers;
    using Service.Common;
    using Service.Common.Managers;
    using Service.Energy;
    using Service.Energy.Mappers;
    using Service.QASBT;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.EnergyProjectionService;
    using ServiceWrapper.NewBTLineAvailabilityService;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
    using Tariff = Model.Energy.Tariff;

    public class TariffsControllerHelper : BaseEnergyControllerHelper, ITariffsControllerHelper
    {
        private const string BundleBannerClass = "stars";
        private const string Collapse = "collapse";
        private const string ViewAllLabel = "View all";
        private const string ViewAllShowSelector = ".available-packages-container .bundle-card-wrapper, .available-packages-container .tariff-wrapper";
        private const string MiddleTabLabel = "Energy tariffs";
        private const string BundlesLabel = "Bundles";
        private const string BundlesShowSelector = ".available-packages-container .bundle-card-wrapper";
        private const string BundlesHideSelector = ".available-packages-container .tariff-wrapper";
        private const string ActiveClass = "active";
        private const string BundleSwitchStepPartial = "_SwitchStepsBundle";
        private const string EnergySwitchStepPartial = "_SwitchStepsEnergy";
        private readonly IBroadbandProductsService _broadbandProductsService;
        private readonly IBroadbandManager _broadbandManager;
        private readonly IEnergyProductServiceWrapper _energyProductServiceWrapper;
        private readonly IEnergyProjectionServiceWrapper _energyProjectionServiceWrapper;
        private readonly ILogger _logger;
        private readonly INewBTLineAvailabilityServiceWrapper _newBtLineAvailabilityServiceWrapper;
        private readonly ITariffMapper _tariffMapper;
        private readonly ITariffService _tariffService;
        private readonly ITariffManager _tariffManager;
        private readonly IContentManagementControllerHelper _contentManagementControllerHelper;

        public TariffsControllerHelper(
            ITariffService tariffService,
            ILogger logger,
            ITariffMapper tariffMapper,
            IBroadbandProductsService broadbandProductsService,
            INewBTLineAvailabilityServiceWrapper newBtLineAvailabilityServiceWrapper,
            IEnergyProjectionServiceWrapper energyProjectionServiceWrapper,
            IEnergyProductServiceWrapper energyProductServiceWrapper,
            ISessionManager sessionManager,
            IConfigManager configManager,
            IBroadbandManager broadbandManager,
            ITariffManager tariffManager,
            WebClientData webClientData,
            IContentManagementControllerHelper contentManagementControllerHelper)
            : base(sessionManager, configManager, webClientData)
        {
            Guard.Against<ArgumentException>(tariffService == null, $"{nameof(tariffService)} is null");
            Guard.Against<ArgumentException>(logger == null, "logger is null");
            Guard.Against<ArgumentException>(tariffMapper == null, $"{nameof(tariffMapper)} is null");
            Guard.Against<ArgumentException>(broadbandProductsService == null, $"{nameof(broadbandProductsService)} is null");
            Guard.Against<ArgumentException>(newBtLineAvailabilityServiceWrapper == null, $"{nameof(newBtLineAvailabilityServiceWrapper)} is null");
            Guard.Against<ArgumentException>(broadbandManager == null, $"{nameof(broadbandManager)} is null");
            Guard.Against<ArgumentException>(energyProjectionServiceWrapper == null, $"{nameof(energyProjectionServiceWrapper)} is null");
            Guard.Against<ArgumentException>(energyProductServiceWrapper == null, $"{nameof(energyProductServiceWrapper)} is null");
            Guard.Against<ArgumentException>(tariffManager == null, $"{nameof(tariffManager)} is null");
            Guard.Against<ArgumentException>(contentManagementControllerHelper == null, $"{nameof(contentManagementControllerHelper)} is null");

            _tariffService = tariffService;
            _logger = logger;
            _tariffMapper = tariffMapper;
            _broadbandProductsService = broadbandProductsService;
            _newBtLineAvailabilityServiceWrapper = newBtLineAvailabilityServiceWrapper;
            _energyProjectionServiceWrapper = energyProjectionServiceWrapper;
            _energyProductServiceWrapper = energyProductServiceWrapper;
            _broadbandManager = broadbandManager;
            _tariffManager = tariffManager;
            _contentManagementControllerHelper = contentManagementControllerHelper;
        }

        public async Task<AvailableTariffsViewModel> GetAvailableTariffsViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            List<CMSEnergyContent> cmsEnergyContents = _contentManagementControllerHelper.GetCMSEnergyContentList();
            try
            {
                List<Product> products = await _tariffService.GetEnergyProducts(energyCustomer);

                if (products.Count == 0)
                {
                    return null;
                }

                products = energyCustomer.IsPrePay() && energyCustomer.IsSmartMeterSmets2() ? RemoveTariffGroupFromProducts(products, TariffGroup.Standard) : RemoveTariffGroupFromProducts(products, TariffGroup.StandardPayGoSMETS2);

                List<Tariff> tariffs = _tariffService.EnergyTariffs(energyCustomer, products, cmsEnergyContents)
                     .Concat(await _tariffService.BundleTariffs(energyCustomer))
                    .Select(t =>
                    {
                        t.DisplayNameSuffix =
                            t.IsBundle ? AvailableBundleTariffs_Resources.BundleTariffSuffix : AvailableBundleTariffs_Resources.EnergyTariffSuffix;
                        return t;
                    })
                    .OrderBy(t => t.GetProjectedCombinedYearlyCost())
                    .ToList();

                int totalInitialBundles = tariffs.Count(t => t.IsBundle);

                List<Tariff> unavailableBundles = tariffs.Where(t => energyCustomer.UnavailableBundles.Contains(t.TariffId)).ToList();
                tariffs = tariffs.Except(unavailableBundles).OrderBy(t => t.GetProjectedCombinedYearlyCost()).ToList();

                SessionManager.SetSessionDetails(SessionKeys.AvailableEnergyTariffs, tariffs);

                List<TariffsViewModel> allTariffsViewModels = tariffs
                    .Where(t => !t.IsUpgrade)
                    // ReSharper disable once PossibleNullReferenceException
                    .Select(t => _tariffMapper.ToTariffViewModel(t, energyCustomer, cmsEnergyContents))
                    .ToList();

                List<TariffsViewModel> energyTariffsViewModels = allTariffsViewModels
                    .Where(t => !t.IsBundle)
                    .ToList();

                List<TariffsViewModel> bundleTariffsViewModels = allTariffsViewModels
                    .Where(t => t.IsBundle)
                    .ToList();

                var viewModel = new AvailableTariffsViewModel
                {
                    Postcode = energyCustomer.Postcode,
                    HasE7Meter = energyCustomer.HasE7Meter(),
                    HeaderParagraph = GetHeaderParagraph(energyCustomer),
                    SelectedPaymentMethod = energyCustomer.SelectedPaymentMethod,
                    EnergyTariffs = energyTariffsViewModels,
                    BundleTariffs = bundleTariffsViewModels,
                    AllTariffs = allTariffsViewModels,
                    BannerClass = energyCustomer.IsBundlingJourney ? BundleBannerClass : string.Empty,
                    HeaderText = energyCustomer.IsBundlingJourney ? AvailableTariffs_Resources.Header_Bundling : AvailableTariffs_Resources.Header_Energy
                };

                viewModel.ShowBundleContent = !viewModel.HasBundleTariffs ? "hide" : string.Empty;
                viewModel.ShowEnergyContent = viewModel.HasBundleTariffs ? "hide" : string.Empty;

                if (energyCustomer.HasElectricity() &&
                    viewModel.EnergyTariffs.Any(t => string.IsNullOrEmpty(t.ProjectedElectricityYearlyCost)))
                {
                    throw new Exception("Customer has selected electricity but no electricity tariffs found.");
                }

                if (energyCustomer.HasGas() &&
                    viewModel.EnergyTariffs.Any(t => string.IsNullOrEmpty(t.ProjectedGasYearlyCost)))
                {
                    throw new Exception("Customer has selected gas but no gas tariffs found.");
                }

                if (!string.IsNullOrEmpty(energyCustomer.ChosenProduct))
                {
                    TariffsViewModel chosenTariff =
                        allTariffsViewModels.FirstOrDefault(t => t.TariffName.Equals(energyCustomer.ChosenProduct, StringComparison.OrdinalIgnoreCase));
                    viewModel.HasChosenTariff = true;
                    if (chosenTariff != null)
                    {
                        chosenTariff.IsChosenTariff = true;
                        chosenTariff.DetailsHeaderIconClass = Collapse;
                        chosenTariff.IsDataShown = true;
                        viewModel.ChosenTariff = chosenTariff;
                    }

                    viewModel.AllTariffs = allTariffsViewModels
                        .Where(t => !t.TariffName.Equals(energyCustomer.ChosenProduct, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                SetTariffDetailsHeader(allTariffsViewModels);

                bool areProductsUnavailable = unavailableBundles.Any()
                                              || totalInitialBundles == 0 && energyCustomer.IsBundlingJourney
                                              || !viewModel.IsChosenTariffAvailable && viewModel.HasChosenTariff;

                viewModel = areProductsUnavailable
                    ? SetSubHeaderTextForUnavailableProducts(unavailableBundles, viewModel, totalInitialBundles, energyCustomer)
                    : SetSubHeaderText(viewModel);

                if (viewModel.IsChosenTariffAvailable && viewModel.AllTariffs.ToList().Count > 0 || areProductsUnavailable)
                {
                    viewModel.YouMightBeInterestedInText = AvailableTariffs_Resources.ChosenTariffOtherTariffsHeader;
                }

                viewModel.LoadingModal = new ModalTitleAndBody
                {
                    Message = AvailableTariffs_Resources.BroadbandBundlePopupText
                };

                SetTabs(energyCustomer, viewModel);
                SetSwitchStep(viewModel);
                SetDataLayer(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while getting customer energy Products - {ex.Message}", ex);
            }

            return null;
        }

        public bool SetSelectedTariff(string selectedTariffId)
        {
            return SetSelectedTariff(selectedTariffId, _tariffMapper);
        }

        public void UpdateYourPriceViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            YourPriceViewModel yourPrice = _tariffMapper.GetYourPriceViewModel(energyCustomer, WebClientData.BaseUrl);
            SessionManager.SetSessionDetails(SessionKeys.EnergyYourPriceDetails, yourPrice);
        }

        public bool IsBroadbandBundleSelected(string selectedTariffId) =>
            GetSelectedTariff(selectedTariffId)?
                .BundlePackage?
                .BundlePackageType == BundlePackageType.FixAndFibre;

        public EnergyUsageViewModel GetEnergyUsageViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var viewModel = new EnergyUsageViewModel
            {
                BackChevronViewModel = GetBackChevronViewModel(energyCustomer),
                KnownEnergyUsageViewModel = new KnownEnergyUsageViewModel
                {
                    SelectedFuelType = energyCustomer.SelectedFuelType,
                    SelectedElectricityMeterType = energyCustomer.SelectedElectricityMeterType,
                    StandardElectricityUsage = (int?)energyCustomer.Projection?.EnergyAveStandardElecKwh,
                    Economy7ElectricityDayUsage = (int?)energyCustomer.Projection?.EnergyEconomy7DayElecKwh,
                    Economy7ElectricityNightUsage = (int?)energyCustomer.Projection?.EnergyEconomy7NightElecKwh,
                    StandardGasUsage = (int?)energyCustomer.Projection?.EnergyAveStandardGasKwh,
                    Frequency = energyCustomer.Projection?.Frequency ?? UsageFrequency.Annual
                },
                UnknownEnergyUsageViewModel = new UnknownEnergyUsageViewModel
                {
                    PropertyType = GetPropertyTypeButtonList(),
                    NumberOfBedrooms = GetBedroomsButtonList(),
                    NumberOfAdults = GetAdultsButtonList()
                }
            };

            return viewModel;
        }

        public EnergyUsageViewModel GetEnergyUsageViewModel(UnknownEnergyUsageViewModel viewModel)
        {
            EnergyUsageViewModel returnModel = GetEnergyUsageViewModel();

            viewModel.PropertyType = returnModel.UnknownEnergyUsageViewModel.PropertyType;
            viewModel.NumberOfBedrooms = returnModel.UnknownEnergyUsageViewModel.NumberOfBedrooms;
            viewModel.NumberOfAdults = returnModel.UnknownEnergyUsageViewModel.NumberOfAdults;

            foreach (RadioButton p in viewModel.PropertyType.Items) p.Checked = p.Value == viewModel.SelectedPropertyTypeId;

            foreach (RadioButton p in viewModel.NumberOfBedrooms.Items) p.Checked = p.Value == viewModel.SelectedNumberOfBedroomsId;

            foreach (RadioButton p in viewModel.NumberOfAdults.Items) p.Checked = p.Value == viewModel.SelectedNumberOfAdultsId;

            returnModel.UnknownEnergyUsageViewModel = viewModel;

            return returnModel;
        }

        public async Task<bool> SetUnknownUsage(UnknownEnergyUsageViewModel unknownEnergyUsageViewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            try
            {
                Projection energyProjection = await GetCustomerEnergyProjection(unknownEnergyUsageViewModel, energyCustomer.Postcode);

                energyCustomer.IsUsageKnown = false;
                energyCustomer.Projection = new Projection
                {
                    MSOA = energyProjection.MSOA,
                    LSOA = energyProjection.LSOA
                };

                if (energyCustomer.HasGas())
                {
                    energyCustomer.Projection.EnergyAveStandardGasKwh = energyProjection.EnergyAveStandardGasKwh;
                }

                if (energyCustomer.HasElectricity())
                {
                    if (energyCustomer.HasE7Meter())
                    {
                        energyCustomer.Projection.EnergyEconomy7DayElecKwh = energyProjection.EnergyEconomy7DayElecKwh;
                        energyCustomer.Projection.EnergyEconomy7NightElecKwh = energyProjection.EnergyEconomy7NightElecKwh;
                    }
                    else
                    {
                        energyCustomer.Projection.EnergyAveStandardElecKwh = energyProjection.EnergyAveStandardElecKwh;
                    }
                }

                SaveEnergyCustomerInSession(energyCustomer);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured while getting customer energy projection - {energyCustomer.Postcode}. {ex.Message}", ex);
            }

            return false;
        }

        public void SetKnownUsage(KnownEnergyUsageViewModel knownEnergyUsageViewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            energyCustomer.IsUsageKnown = true;
            energyCustomer.Projection = new Projection { Frequency = knownEnergyUsageViewModel.Frequency };
            if (energyCustomer.HasGas())
            {
                Guard.Against<ArgumentException>(knownEnergyUsageViewModel.StandardGasUsage == null, "Gas usage is null");
                // ReSharper disable once PossibleInvalidOperationException
                energyCustomer.Projection.EnergyAveStandardGasKwh = knownEnergyUsageViewModel.StandardGasUsage.Value;
            }

            if (energyCustomer.HasElectricity())
            {
                if (energyCustomer.HasE7Meter())
                {
                    Guard.Against<ArgumentException>(knownEnergyUsageViewModel.Economy7ElectricityDayUsage == null, "Electricity day usage is null");
                    Guard.Against<ArgumentException>(knownEnergyUsageViewModel.Economy7ElectricityNightUsage == null, "Electricity night usage is null");
                    // ReSharper disable once PossibleInvalidOperationException
                    energyCustomer.Projection.EnergyEconomy7DayElecKwh = knownEnergyUsageViewModel.Economy7ElectricityDayUsage.Value;
                    // ReSharper disable once PossibleInvalidOperationException
                    energyCustomer.Projection.EnergyEconomy7NightElecKwh = knownEnergyUsageViewModel.Economy7ElectricityNightUsage.Value;
                }
                else
                {
                    Guard.Against<ArgumentException>(knownEnergyUsageViewModel.StandardElectricityUsage == null, "Electricity usage is null");
                    // ReSharper disable once PossibleInvalidOperationException
                    energyCustomer.Projection.EnergyAveStandardElecKwh = knownEnergyUsageViewModel.StandardElectricityUsage.Value;
                }
            }

            SaveEnergyCustomerInSession(energyCustomer);
        }

        public void MarkBundleAsUnavailable()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            energyCustomer?.UnavailableBundles.Add(energyCustomer.SelectedTariff.TariffId);
            SessionManager.SetSessionDetails(SessionKeys.EnergyCustomer, energyCustomer);
        }

        public bool IsBroadbandPackageAvailable()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return !IsOpenReachFallout(energyCustomer.SelectedBTAddress) && energyCustomer.HasSelectedBroadbandProduct();
        }

        public async Task<bool> HasMatchingBTAddressForCustomer()
        {
            BTAddress matchedAddress = null;
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            // ReSharper disable once PossibleNullReferenceException
            List<BTAddress> openReachAddressList = await _broadbandProductsService.GetAddressesForPostcode(energyCustomer.Postcode);

            if (openReachAddressList.Any())
            {
                foreach (BTAddress openReachAddress in openReachAddressList)
                {
                    Tuple<int, int> matchResults = QASBTMatchService.ProcessQASBTMatch(openReachAddress, energyCustomer.SelectedAddress);
                    if (matchResults.Item1 == matchResults.Item2 && matchResults.Item2 > 0)
                    {
                        matchedAddress = openReachAddress;
                        break;
                    }
                }
            }

            if (matchedAddress == null)
            {
                SessionManager.SetSessionDetails(SessionKeys.BTAddressListForPostCode, openReachAddressList);
                energyCustomer.HasConfirmedNonMatchingBTAddress = true;
            }
            else
            {
                energyCustomer.SelectedBTAddress = matchedAddress;
                energyCustomer.HasConfirmedNonMatchingBTAddress = false;
            }

            return matchedAddress != null;
        }

        public async Task<OurPricesViewModel> GetOurPriceViewModel(string postcode, TariffStatus tariffStatus, FuelCategory fuelCategory)
        {
            List<Product> products = await _energyProductServiceWrapper.GetOurPricesProducts(postcode, tariffStatus.ToString(), fuelCategory.ToString());
            Guard.Against<Exception>(products == null || !products.Any(), "Our prices API returned no products.");

            return new OurPricesViewModel
            {
                Postcode = postcode,
                TariffStatus = tariffStatus,
                FuelCategory = fuelCategory,
                Products = products?
                    .GroupBy(prod => new { prod.ServicePlanInvoiceDescription, prod.ServicePlanId })
                    .OrderByDescending(p => p.Max(x => x.EffectiveDate))
                    .Select(p => new OurPriceProductViewModel
                    {
                        TariffName = p.Key.ServicePlanInvoiceDescription,
                        DisplayCheckAvailabilityLink = _tariffManager.GetTariffGroup(p.Key.ServicePlanId) != TariffGroup.FixAndProtectPlus,
                        TariffOptions = p.Select(tariff => _tariffMapper.GetTariffInformation(tariff, fuelCategory)).ToList()
                    }).ToList()
            };
        }

        public async Task SetAvailableBroadbandProduct()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            try
            {
                List<BroadbandProduct> listOfProducts =
                    await _broadbandProductsService
                        .GetAvailableBroadbandProducts(
                            energyCustomer.SelectedBTAddress,
                            energyCustomer.CLIChoice.FinalCLI);
                SessionManager.SetSessionDetails(SessionKeys.BroadbandProducts, listOfProducts);
                BroadbandProductGroup selectedBroadbandProductGroup = _broadbandManager.BroadbandProductGroup(energyCustomer.SelectedBroadbandProductCode);
                SetDefaultBroadbandProduct(selectedBroadbandProductGroup);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured in broadband product service {energyCustomer?.Postcode} {ex.Message}", ex);
            }
        }

        private void SetSwitchStep(AvailableTariffsViewModel viewModel)
        {
            if (!viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && viewModel.ChosenTariff.IsBundle)
            {
                viewModel.SwitchStepPartial = BundleSwitchStepPartial;
            }
            else if (!viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && !viewModel.ChosenTariff.IsBundle)
            {
                viewModel.SwitchStepPartial = EnergySwitchStepPartial;
            }
            else if (viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && !viewModel.ChosenTariff.IsBundle)
            {
                viewModel.SwitchStepPartial = BundleSwitchStepPartial;
            }
            else if (viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && viewModel.ChosenTariff.IsBundle)
            {
                viewModel.SwitchStepPartial = BundleSwitchStepPartial;
            }
            else if (viewModel.HasBundleTariffs && !viewModel.IsChosenTariffAvailable)
            {
                viewModel.SwitchStepPartial = BundleSwitchStepPartial;
            }
            else
            {
                viewModel.SwitchStepPartial = EnergySwitchStepPartial;
            }
        }

        private static void SetTabs(EnergyCustomer energyCustomer, AvailableTariffsViewModel viewModel)
        {
            if (!viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && viewModel.ChosenTariff.IsBundle)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.ViewAllTabName;
            }
            else if (!viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && !viewModel.ChosenTariff.IsBundle)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.EnergyTabName;
            }
            else if (viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && !viewModel.ChosenTariff.IsBundle)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.ViewAllTabName;
            }
            else if (viewModel.HasBundleTariffs && viewModel.IsChosenTariffAvailable && viewModel.ChosenTariff.IsBundle)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.BundleTabName;
            }
            else if (viewModel.HasBundleTariffs && !viewModel.IsChosenTariffAvailable && energyCustomer.IsBundlingJourney)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.BundleTabName;
            }
            else if (viewModel.HasBundleTariffs && !viewModel.IsChosenTariffAvailable && !energyCustomer.IsBundlingJourney)
            {
                viewModel.DisplayType = AvailableTariffs_Resources.ViewAllTabName;
            }
            else
            {
                viewModel.DisplayType = AvailableTariffs_Resources.EnergyTabName;
            }

            viewModel.ShowTabs = viewModel.HasBundleTariffs;
            if (viewModel.ShowTabs)
            {
                viewModel.FirstTabLabel = BundlesLabel;
                viewModel.MiddleTabLabel = MiddleTabLabel;
                viewModel.LastTabLabel = ViewAllLabel;

                viewModel.FirstTabShowTariffSelector = BundlesShowSelector;
                viewModel.FirstTabHideTariffSelector = BundlesHideSelector;
                viewModel.FirstTabActiveClass = viewModel.DisplayType == AvailableTariffs_Resources.BundleTabName ? ActiveClass : string.Empty;

                viewModel.MiddleTabShowTariffSelector = BundlesHideSelector;
                viewModel.MiddleTabHideTariffSelector = BundlesShowSelector;

                viewModel.LastTabShowTariffSelector = ViewAllShowSelector;
                viewModel.LastTabActiveClass = viewModel.DisplayType == AvailableTariffs_Resources.ViewAllTabName ? ActiveClass : string.Empty;
            }

            if (viewModel.DisplayType != AvailableTariffs_Resources.EnergyTabName)
            {
                if (viewModel.DisplayType == AvailableTariffs_Resources.BundleTabName)
                {
                    viewModel.InitialShowTariffs = viewModel.FirstTabShowTariffSelector;
                    viewModel.InitialHideTariffs = viewModel.FirstTabHideTariffSelector;
                }
                else if (viewModel.DisplayType == AvailableTariffs_Resources.ViewAllTabName)
                {
                    viewModel.InitialShowTariffs = viewModel.LastTabShowTariffSelector;
                    viewModel.InitialHideTariffs = string.Empty;
                }
            }
            else
            {
                viewModel.InitialShowTariffs = viewModel.MiddleTabShowTariffSelector;
                viewModel.InitialHideTariffs = viewModel.MiddleTabHideTariffSelector;
            }
        }

        private static void SetDataLayer(AvailableTariffsViewModel viewModel)
        {
            viewModel.DataLayer = new Dictionary<string, string>();
            if (viewModel.HasChosenTariff)
            {
                viewModel.DataLayer.Add(DataLayer_Resources.FocusTariff,
                    viewModel.IsChosenTariffAvailable ? viewModel.ChosenTariff.DisplayName : "Not Available");
            }
            else
            {
                viewModel.DataLayer.Add(DataLayer_Resources.FocusTariff, string.Empty);
            }
        }

        private static string GetHeaderParagraph(EnergyCustomer energyCustomer)
        {
            string headerText = string.Empty;
            string standardGasUsageText = string.Format(AvailableTariffs_Resources.GasUsageParaghraph1SubText,
                energyCustomer.Projection.EnergyAveStandardGasKwh);
            string standardElecUsageText = string.Format(AvailableTariffs_Resources.ElectricityUsageParaghraph1SubText,
                energyCustomer.Projection.EnergyAveStandardElecKwh);
            double? economy7ElecDayUsage = energyCustomer.Projection.EnergyEconomy7DayElecKwh;
            double? economy7ElecNightUsage = energyCustomer.Projection.EnergyEconomy7NightElecKwh;

            if (!energyCustomer.IsUsageKnown)
            {
                if (energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Economy7)
                {
                    switch (energyCustomer.SelectedFuelType)
                    {
                        case FuelType.Dual:
                            headerText =
                                $"{string.Format(AvailableTariffs_Resources.UnknownUsageE7Paragraph1, $"{standardGasUsageText}&nbsp;&nbsp;", economy7ElecDayUsage, economy7ElecNightUsage)}";
                            break;
                        case FuelType.Electricity:
                            headerText =
                                $"{string.Format(AvailableTariffs_Resources.UnknownUsageE7Paragraph1, string.Empty, economy7ElecDayUsage, economy7ElecNightUsage)}";
                            break;
                    }
                }
                else
                {
                    switch (energyCustomer.SelectedFuelType)
                    {
                        case FuelType.Dual:
                            headerText =
                                $"{string.Format(AvailableTariffs_Resources.UnknownUsageParagraph1, $"{standardGasUsageText}&nbsp;&nbsp; {standardElecUsageText}")}";
                            break;
                        case FuelType.Gas:
                            headerText = string.Format(AvailableTariffs_Resources.UnknownUsageParagraph1,
                                standardGasUsageText);
                            break;
                        case FuelType.Electricity:
                            headerText = string.Format(AvailableTariffs_Resources.UnknownUsageParagraph1,
                                standardElecUsageText);
                            break;
                    }
                }
            }
            else
            {
                headerText = AvailableTariffs_Resources.KnownUsageParagraph1;
            }

            return headerText;
        }

        private Tariff GetSelectedTariff(string selectedTariffId)
        {
            var availableTariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            Guard.Against<ArgumentException>(availableTariffs == null, $"{nameof(availableTariffs)} is null");
            return availableTariffs?.FirstOrDefault(t => t.TariffId.Equals(selectedTariffId));
        }

        private static AvailableTariffsViewModel SetSubHeaderText(AvailableTariffsViewModel viewModel)
        {
            viewModel.SubHeaderText = AvailableTariffs_Resources.TariffSubHeader;
            viewModel.SubHeaderParagraph = AvailableTariffs_Resources.TariffDisclaimer;

            if (viewModel.IsChosenTariffAvailable && viewModel.ChosenTariff.IsBundle)
            {
                viewModel.SubHeaderText = AvailableTariffs_Resources.ChosenBundleSubHeader;
                viewModel.SubHeaderParagraph = AvailableTariffs_Resources.BundleDisclaimer;
            }

            return viewModel;
        }

        private static AvailableTariffsViewModel SetSubHeaderTextForUnavailableProducts(List<Tariff> unavailableBundles, AvailableTariffsViewModel viewModel, int totalInitialBundles, EnergyCustomer energyCustomer)
        {
            if (unavailableBundles.Any() && viewModel.BundleTariffs.Any())
            {
                viewModel.SubHeaderText = AvailableTariffs_Resources.ChosenBundleUnavailableHeader;
                viewModel.SubHeaderParagraph = AvailableTariffs_Resources.ChosenBundleUnavailablePara;
            }
            else if (unavailableBundles.Any() || totalInitialBundles == 0 && energyCustomer.IsBundlingJourney)
            {
                viewModel.SubHeaderText = AvailableTariffs_Resources.AllBundlesUnavailableHeader;
                viewModel.SubHeaderParagraph = AvailableTariffs_Resources.AllBundlesUnavailablePara;
            }
            else
            {
                viewModel.SubHeaderText = AvailableTariffs_Resources.ChosenTariffNotAvailableHeader;
                viewModel.SubHeaderParagraph = AvailableTariffs_Resources.ChosenTariffNotAvailableMessage;
            }

            return viewModel;
        }

        private bool IsOpenReachFallout(BTAddress customerBTAddress)
        {
            bool fallout;
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Guard.Against<ArgumentException>(energyCustomer == null, $"{nameof(energyCustomer)} is null");
            OpenReachData openReachResponse = new OpenReachData();

            try
            {
                openReachResponse = _newBtLineAvailabilityServiceWrapper.Newbtlineavailability(
                    customerBTAddress,
                    energyCustomer?.CLIChoice.UserProvidedCLI);
            }
            catch (Exception)
            {
                openReachResponse = new OpenReachData
                {
                    LineavailabilityFlags = new LineAvailability { BackOfficeFile = true, Fallout = false, InstallLine = false },
                    LineStatus = LineStatus.NewConnection,
                    AddressLineKey = "OPENREACHFALLOUTTEMPORARY"
                };
            }

            fallout = openReachResponse.LineavailabilityFlags.Fallout;

            if (!fallout)
            {
                SessionManager.SetSessionDetails(SessionKeys.OpenReachResponse, openReachResponse);
                // ReSharper disable once PossibleNullReferenceException
                energyCustomer.CLIChoice.OpenReachProvidedCLI = null;
                energyCustomer.IsSSECustomerCLI = false;

                if (!string.IsNullOrEmpty(openReachResponse.CLI))
                {
                    energyCustomer.CLIChoice.OpenReachProvidedCLI = openReachResponse.CLI;
                    energyCustomer.IsSSECustomerCLI = true;
                }

                // ReSharper disable once PossibleNullReferenceException
                energyCustomer.ApplyInstallationFee = openReachResponse.LineavailabilityFlags.InstallLine;
                SessionManager.SetSessionDetails(SessionKeys.EnergyCustomer, energyCustomer);
            }

            return fallout;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static void SetTariffDetailsHeader(List<TariffsViewModel> allTariffsViewModels)
        {
            foreach (TariffsViewModel tariffViewModel in allTariffsViewModels)
                if (tariffViewModel.IsBundle)
                {
                    tariffViewModel.DetailsHeader = tariffViewModel.IsChosenTariff
                        ? AvailableTariffs_Resources.HideBundleDetailsHeader
                        : AvailableTariffs_Resources.ShowBundleDetailsHeader;
                }
        }

        private static BackChevronViewModel GetBackChevronViewModel(EnergyCustomer energyCustomer)
        {
            bool isCAndCJourney = energyCustomer.IsCAndCJourney();
            bool isSmartMeter = energyCustomer.IsSmartMeter();
            bool isMeterPayGo = energyCustomer.IsMeterDetailsPayGo();

            var backChevronViewModel = new BackChevronViewModel
            {
                TitleAttributeText = Resources.BackButtonAlt
            };

            if (isCAndCJourney && isSmartMeter)
            {
                backChevronViewModel.ActionName = "SmartMeterFrequency";
                backChevronViewModel.ControllerName = "Quote";
            }
            else if (isCAndCJourney && isMeterPayGo)
            {
                backChevronViewModel.ActionName = "SelectFuel";
                backChevronViewModel.ControllerName = "Quote";
            }
            else if (isCAndCJourney)
            {
                backChevronViewModel.ActionName = "PaymentMethod";
                backChevronViewModel.ControllerName = "Quote";
            }
            else
            {
                backChevronViewModel.ActionName = "SmartMeter";
                backChevronViewModel.ControllerName = "Quote";
            }

            return backChevronViewModel;
        }

        private static ButtonList GetAdultsButtonList()
        {
            var adultsButtonList = new ButtonList
            {
                Items = new List<RadioButton>(),
                SelectedValue = "UnknownEnergyUsageViewModel.SelectedNumberOfAdultsId"
            };

            adultsButtonList.Items.Add(new RadioButton
            {
                Value = "1",
                DisplayText = EnergyUsage_Resources.RadioHeadingAdults1,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionAdults1
            });
            adultsButtonList.Items.Add(new RadioButton
            {
                Value = "2",
                DisplayText = EnergyUsage_Resources.RadioHeadingAdults2,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionAdults2
            });
            adultsButtonList.Items.Add(new RadioButton
            {
                Value = "3",
                DisplayText = EnergyUsage_Resources.RadioHeadingAdults3,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionAdults3
            });
            adultsButtonList.Items.Add(new RadioButton
            {
                Value = "4",
                DisplayText = EnergyUsage_Resources.RadioHeadingAdults4,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionAdults4
            });
            adultsButtonList.Items.Add(new RadioButton
            {
                Value = "5+",
                DisplayText = EnergyUsage_Resources.RadioHeadingAdults5,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionAdults5
            });

            return adultsButtonList;
        }

        private static ButtonList GetBedroomsButtonList()
        {
            var bedroomButtonList = new ButtonList
            {
                Items = new List<RadioButton>(),
                SelectedValue = "UnknownEnergyUsageViewModel.SelectedNumberOfBedroomsId"
            };

            bedroomButtonList.Items.Add(new RadioButton
            {
                Value = "1 Bedroom",
                DisplayText = EnergyUsage_Resources.RadioHeadingBedrooms1,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionBedrooms1
            });
            bedroomButtonList.Items.Add(new RadioButton
            {
                Value = "2 Bedrooms",
                DisplayText = EnergyUsage_Resources.RadioHeadingBedrooms2,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionBedrooms2
            });
            bedroomButtonList.Items.Add(new RadioButton
            {
                Value = "3 Bedrooms",
                DisplayText = EnergyUsage_Resources.RadioHeadingBedrooms3,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionBedrooms3
            });
            bedroomButtonList.Items.Add(new RadioButton
            {
                Value = "4 Bedrooms",
                DisplayText = EnergyUsage_Resources.RadioHeadingBedrooms4,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionBedrooms4
            });
            bedroomButtonList.Items.Add(new RadioButton
            {
                Value = "5+ Bedrooms",
                DisplayText = EnergyUsage_Resources.RadioHeadingBedrooms5,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionBedrooms5
            });

            return bedroomButtonList;
        }

        private static ButtonList GetPropertyTypeButtonList()
        {
            var propertyType = new ButtonList
            {
                Items = new List<RadioButton>(),
                SelectedValue = "UnknownEnergyUsageViewModel.SelectedPropertyTypeId"
            };
            propertyType.Items.Add(new RadioButton
            {
                Value = "Mid Terrace",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType1,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType1
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "End Terrace",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType2,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType2
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "Semi-detached House",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType3,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType3
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "Detached House",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType4,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType4
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "Bungalow",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType5,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType5
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "Purpose-built Flat",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType6,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType6
            });
            propertyType.Items.Add(new RadioButton
            {
                Value = "Converted Flat",
                DisplayText = EnergyUsage_Resources.RadioHeadingPropertyType7,
                AriaDescription = EnergyUsage_Resources.RadioAriaDescriptionPropertyType7
            });

            return propertyType;
        }

        private async Task<Projection> GetCustomerEnergyProjection(UnknownEnergyUsageViewModel unknownEnergyUsageViewModel, string postCode)
        {
            string e7DaySplit = ConfigManager.GetAppSetting("Day");
            string e7NightSplit = ConfigManager.GetAppSetting("Night");

            Guard.Against<Exception>(string.IsNullOrEmpty(e7DaySplit), "e7DaySplit is not in config");
            Guard.Against<Exception>(string.IsNullOrEmpty(e7NightSplit), "e7NightSplit is not in config");

            List<EnergyMultiplier> multipliers = await _energyProjectionServiceWrapper.GetMultipliers();
            IEnumerable<EnergyMultiplier> selectedMultipliers = multipliers.Where(m => m.Value == unknownEnergyUsageViewModel.SelectedPropertyTypeId
                                                                                       || m.Value == unknownEnergyUsageViewModel.SelectedNumberOfAdultsId
                                                                                       || m.Value == unknownEnergyUsageViewModel.SelectedNumberOfBedroomsId);

            CumulativeEnergyMultiplier cumulativeMultiplier = await _energyProjectionServiceWrapper.GetCumulativeEnergyMultiplier(selectedMultipliers);
            Projection energyProjection = await _energyProjectionServiceWrapper.GetProjection(cumulativeMultiplier, postCode);

            // ReSharper disable once PossibleInvalidOperationException
            energyProjection.EnergyEconomy7DayElecKwh = Math.Round(energyProjection.EnergyAveEcon7ElecKwh.Value * (Convert.ToDouble(e7DaySplit) / 100));
            energyProjection.EnergyEconomy7NightElecKwh = energyProjection.EnergyAveEcon7ElecKwh.Value - energyProjection.EnergyEconomy7DayElecKwh;

            return energyProjection;
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private List<Product> RemoveTariffGroupFromProducts(List<Product> products, TariffGroup tariffGroupToExclude)
        {
            var filteredProducts = new List<Product>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Product product in products)
            {
                TariffGroup group = _tariffMapper.GetTariffGroupForProduct(product);
                if (group != tariffGroupToExclude)
                {
                    filteredProducts.Add(product);
                }
            }

            return filteredProducts;
        }
    }
}