namespace Products.Service.TariffChange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Core;
    using Common.Managers;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Mappers;
    using Model.Energy;
    using Model.Enums;
    using Products.Model.TariffChange;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using Products.Model.TariffChange.Tariffs;
    using Products.Repository.TariffChange;
    using Products.WebModel.Resources.TariffChange;
    using Products.WebModel.ViewModels.TariffChange;
    using Tariff = Model.TariffChange.Tariffs.Tariff;

    public class TariffService : ITariffService
    {
        private readonly ITariffRepository _tariffRepository;
        private readonly IConfigManager _configManager;
        private readonly IJourneyDetailsService _journeyDetailsService;
        private readonly TariffViewModelMapper _tariffViewModelMapper;
        private readonly IAvailableTariffService _availableTariffSessionService;
        private readonly ITariffManager _tariffManager;

        public TariffService(
            ITariffRepository tariffRepository,
            IConfigManager configManager,
            IJourneyDetailsService journeyDetailsService,
            TariffViewModelMapper tariffViewModelMapper,
            IAvailableTariffService availableTariffService,
            ITariffManager tariffManager)
        {
            Guard.Against<ArgumentException>(tariffRepository == null, $"{nameof(tariffRepository)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(journeyDetailsService == null, $"{nameof(journeyDetailsService)} is null");
            Guard.Against<ArgumentException>(tariffViewModelMapper == null, $"{nameof(tariffViewModelMapper)} is null");
            Guard.Against<ArgumentException>(availableTariffService == null, $"{nameof(availableTariffService)} is null");
            Guard.Against<ArgumentException>(tariffManager == null, $"{nameof(tariffManager)} is null");

            _tariffRepository = tariffRepository;
            _configManager = configManager;
            _journeyDetailsService = journeyDetailsService;
            _tariffViewModelMapper = tariffViewModelMapper;
            _availableTariffSessionService = availableTariffService;
            _tariffManager = tariffManager;
        }

        public SelectedTariffsViewModel GetCurrentSelectedTariff(string tariffName, bool isImmediateRenewal)
        {
            Customer customer = null;
            var viewModel = new SelectedTariffsViewModel();

            try
            {
                customer = _journeyDetailsService.GetCustomer();
                if (customer?.FalloutReasons == null || customer.FalloutReasons.Count > 0)
                {
                    _journeyDetailsService.ClearJourneyDetails();
                    viewModel.IsCustomerFallout = true;
                    return viewModel;
                }

                List<AvailableTariff> availableTariffs = _availableTariffSessionService.GetAvailableTariffs();

                AvailableTariff selectedTariff = availableTariffs.FirstOrDefault(t => t.Name == tariffName);

                if (selectedTariff == null)
                {
                    viewModel.IsTariffSelected = false;
                    return viewModel;
                }

                customer.CustomerSelectedTariff = TariffViewModelMapper.MapSelectedTariff(customer, selectedTariff, isImmediateRenewal);
                if (customer.HasElectricityAccount())
                {
                    customer.ElectricityAccount.SelectedTariff = TariffViewModelMapper.MapSelectedTariffDetailsForFuel(customer, selectedTariff.ElectricityDetails, isImmediateRenewal);
                }

                if (customer.HasGasAccount())
                {
                    customer.GasAccount.SelectedTariff = TariffViewModelMapper.MapSelectedTariffDetailsForFuel(customer, selectedTariff.GasDetails, isImmediateRenewal);
                }

                viewModel.ShowEmailConfirmation = _journeyDetailsService.GetCustomerJourney() == CTCJourneyType.PreLogIn;

                if (!viewModel.ShowEmailConfirmation)
                {
                    customer.EmailAddress = HttpContext.Current.User.Identity.Name;
                }

                _journeyDetailsService.SetCustomer(customer);
                _availableTariffSessionService.ClearAvailableTariffs();
                viewModel.IsTariffSelected = true;

                return viewModel;
            }
            catch (Exception ex)
            {
                string accountNumbers = customer == null ? string.Empty : string.Join(", ", customer.CustomerAccountNumbers());
                throw new Exception($"Exception occurred, Accounts = {accountNumbers}, showing Summary page.", ex);
            }
        }

        public async Task<TariffsViewModel> GetCurrentAndAvailableTariffsAsync(List<CMSEnergyContent> cmsEnergyContents)
        {
            Customer customer = null;

            try
            {
                customer = _journeyDetailsService.GetCustomer();
                CustomerAccount customerAccount = _journeyDetailsService.GetCustomerAccount();
                var viewModel = new TariffsViewModel();
                if (customer?.FalloutReasons == null || customer.FalloutReasons.Count > 0)
                {
                    _journeyDetailsService.ClearJourneyDetails();
                    viewModel.IsCustomerFallout = true;
                    return viewModel;
                }

                List<Tariff> tariffs = await GetTariffs(customer);

                if (tariffs.Count == 0)
                {
                    return viewModel;
                }

                tariffs = TariffMapper.MapTariffsToCMSEnergyContentTariffs(tariffs, cmsEnergyContents, customerAccount.CurrentTariff?.ServicePlanId);
                // ReSharper disable once PossibleNullReferenceException
                tariffs = TariffMapper.MapTariffsToFilteredFixAndDriveTariffs(customer.IsDualFuel(), tariffs);

                if (customerAccount.IsWHD)
                {
                    tariffs = await GetAvailableTariffsWithWHDDiscount(customer, tariffs);
                }

                viewModel = _tariffViewModelMapper.MapAvailableTariffs(customer, tariffs, customerAccount, cmsEnergyContents);

                viewModel = RemoveInvalidTariffs(viewModel);

                customer.CountOfTariffs = GetAvailableTariffCount(viewModel, customer).ToString();

                viewModel = _tariffViewModelMapper.MapExtendedModelProperties(viewModel);

                if (viewModel.FollowOnTariff != null)
                {
                    customer.FollowOnTariff = new SelectedTariff
                    {
                        Name = viewModel.FollowOnTariff?.Name,
                        ExitFeePerFuel = viewModel.FollowOnTariff?.ExitFeePerFuel,
                        ExitFee = viewModel.FollowOnTariff.ExitFee,
                        ProjectedMonthlyCost = viewModel.FollowOnTariff?.ProjectedMonthlyCost,
                        ProjectedAnnualCost = viewModel.FollowOnTariff?.ProjectedAnnualCost,
                        ProjectedAnnualCostValue = viewModel.FollowOnTariff.ProjectedAnnualCostValue,
                        ProjectedMonthlyCostValue = viewModel.FollowOnTariff.ProjectedMonthlyCostValue,
                        EffectiveDate = viewModel.TariffStartDate
                    };
                }

                _availableTariffSessionService.SetAvailableTariffs(viewModel.AvailableTariffs);
                _journeyDetailsService.SetCustomer(customer);
                SetProgressBar(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                string accountNumbers = customer == null ? string.Empty : string.Join(", ", customer.CustomerAccountNumbers());
                throw new Exception($"Exception occurred, Accounts = {accountNumbers}, retrieving available tariffs.", ex);
            }
        }

        private static int GetAvailableTariffCount(TariffsViewModel viewModel, Customer customer)
        {
            if (viewModel.FollowOnTariff != null)
            {
                return viewModel.AvailableTariffs.Count + 1;
            }

            TariffInformationLabel accountDetails = viewModel.CurrentTariffViewModel.ElectricityDetails ?? viewModel.CurrentTariffViewModel.GasDetails;

            if (accountDetails?.ServicePlanId == customer.GetCurrentTariff()?.ServicePlanId)
            {
                return viewModel.AvailableTariffs.Count;
            }

            return viewModel.AvailableTariffs.Count + 1;
        }

        private async Task<List<Tariff>> GetTariffs(Customer customer)
        {
            var tariffResults = new List<Tariff>();
            if (customer.ElectricityAccount != null)
            {
                tariffResults = await GetElectricityTariffs(customer.ElectricityAccount);
            }

            if (customer.GasAccount != null)
            {
                tariffResults.AddRange(await GetGasTariffs(customer.GasAccount));
            }

            List<Tariff> tariffs = tariffResults.GroupBy(t => t.DisplayName).Select(group => new Tariff
            {
                Name = group.OrderByDescending(t => t.Name).First().Name,
                Type = group.First().Type,
                IsFollowOnTariff = group.FirstOrDefault(g => g.GasDetails != null)?.IsFollowOnTariff ?? group.FirstOrDefault(e => e.ElectricityDetails != null)?.IsFollowOnTariff ?? false,
                GasDetails = group.FirstOrDefault(g => g.GasDetails != null)?.GasDetails,
                ElectricityDetails = group.FirstOrDefault(e => e.ElectricityDetails != null)?.ElectricityDetails,
                TariffGroup = _tariffManager.GetTariffGroup(group.Select(g => g.ElectricityDetails?.ServicePlanId ?? g.GasDetails?.ServicePlanId).FirstOrDefault()),
            }).ToList();
            return tariffs;
        }

        private async Task<List<Tariff>> GetGasTariffs(CustomerAccount customerAccount)
        {
            List<AvailableTariffResult> tariffResults = await _tariffRepository.GetAvailableSingleRateTariffsAsync(
                                                                                    customerAccount.SiteDetails.PostCode, 
                                                                                    customerAccount.CurrentTariff.BrandCode, 
                                                                                    customerAccount.CurrentTariff.FuelType, 
                                                                                    customerAccount.PaymentDetails.RateCode);

            var gasTariffs = new List<Tariff>();

            if (!string.IsNullOrEmpty(customerAccount.FollowOnTariffServicePlanID))
            {
                List<AvailableTariffResult> followOnTariff = tariffResults.Where(e => e.ServicePlanID == customerAccount.FollowOnTariffServicePlanID).ToList();
                gasTariffs.AddRange(tariffResults.Where(e => e.ServicePlanID != customerAccount.FollowOnTariffServicePlanID).Select(g => g.GasResult).ToList());
                if (!followOnTariff.Any())
                {
                    followOnTariff = await _tariffRepository.GetPreservedSingleRateTariffsByServicePlanIdAsync(
                                                                                customerAccount.SiteDetails.PostCode, 
                                                                                customerAccount.CurrentTariff.BrandCode, 
                                                                                customerAccount.FollowOnTariffServicePlanID, 
                                                                                customerAccount.CurrentTariff.FuelType, 
                                                                                customerAccount.PaymentDetails.RateCode);
                    tariffResults.AddRange(followOnTariff);
                }

                Tariff gasTariff = tariffResults.FirstOrDefault(e => e.ServicePlanID == customerAccount.FollowOnTariffServicePlanID)?.GasResult;
                if (gasTariff != null)
                {
                    gasTariff.IsFollowOnTariff = true;
                    gasTariffs.Add(gasTariff);
                }
            }
            else
            {
                gasTariffs.AddRange(tariffResults.Select(g => g.GasResult).ToList());
            }

            if (gasTariffs.All(e => e.GasDetails?.ServicePlanId != customerAccount.CurrentTariff.ServicePlanId))
            {
                tariffResults = await _tariffRepository.GetPreservedSingleRateTariffsByServicePlanIdAsync(
                                                                            customerAccount.SiteDetails.PostCode, 
                                                                            customerAccount.CurrentTariff.BrandCode, 
                                                                            customerAccount.CurrentTariff.ServicePlanId, 
                                                                            customerAccount.CurrentTariff.FuelType, 
                                                                            customerAccount.PaymentDetails.RateCode);

                gasTariffs.AddRange(tariffResults.Select(g => g.GasResult).ToList());
            }

            return gasTariffs;
        }

        private async Task<List<Tariff>> GetElectricityTariffs(CustomerAccount customerAccount)
        {
            List<AvailableTariffResult> tariffResults = customerAccount.SiteDetails.HasMultiRateMeter
                ? await _tariffRepository.GetAvailableMultiRateTariffsAsync(
                                                        customerAccount.SiteDetails.PostCode, 
                                                        customerAccount.CurrentTariff.BrandCode, 
                                                        GetTariffType(customerAccount.CurrentTariff.Name), 
                                                        customerAccount.PaymentDetails.RateCode)
                : await _tariffRepository.GetAvailableSingleRateTariffsAsync(
                                                        customerAccount.SiteDetails.PostCode, 
                                                        customerAccount.CurrentTariff.BrandCode, 
                                                        customerAccount.CurrentTariff.FuelType, 
                                                        customerAccount.PaymentDetails.RateCode);

            var electricityTariffs = new List<Tariff>();

            if (!string.IsNullOrEmpty(customerAccount.FollowOnTariffServicePlanID))
            {
                List<AvailableTariffResult> followOnTariff = tariffResults.Where(e => e.ServicePlanID == customerAccount.FollowOnTariffServicePlanID).ToList();
                electricityTariffs.AddRange(tariffResults.Where(e => e.ServicePlanID != customerAccount.FollowOnTariffServicePlanID).Select(e => e.ElectricityResult).ToList());

                if (!followOnTariff.Any())
                {
                    followOnTariff = customerAccount.SiteDetails.HasMultiRateMeter
                       ? await _tariffRepository.GetPreservedMultiRateTariffsByServicePlanIdAsync(
                                                        customerAccount.SiteDetails.PostCode, 
                                                        customerAccount.CurrentTariff.BrandCode, 
                                                        customerAccount.FollowOnTariffServicePlanID, 
                                                        GetTariffType(customerAccount.CurrentTariff.Name), 
                                                        customerAccount.PaymentDetails.RateCode)
                       : await _tariffRepository.GetPreservedSingleRateTariffsByServicePlanIdAsync(
                                                        customerAccount.SiteDetails.PostCode, 
                                                        customerAccount.CurrentTariff.BrandCode, 
                                                        customerAccount.FollowOnTariffServicePlanID, 
                                                        customerAccount.CurrentTariff.FuelType, 
                                                        customerAccount.PaymentDetails.RateCode);
                    tariffResults.AddRange(followOnTariff);
                }

                Tariff electricityTariff = tariffResults.FirstOrDefault(e => e.ServicePlanID == customerAccount.FollowOnTariffServicePlanID)?.ElectricityResult;
                if (electricityTariff != null)
                {
                    electricityTariff.IsFollowOnTariff = true;
                    electricityTariffs.Add(electricityTariff);
                }
            }
            else
            {
                electricityTariffs.AddRange(tariffResults.Select(e => e.ElectricityResult).ToList());
            }

            if (electricityTariffs.All(e => e.ElectricityDetails?.ServicePlanId != customerAccount.CurrentTariff.ServicePlanId))
            {
                tariffResults = customerAccount.SiteDetails.HasMultiRateMeter
                    ? await _tariffRepository.GetPreservedMultiRateTariffsByServicePlanIdAsync(
                                                                                    customerAccount.SiteDetails.PostCode, 
                                                                                    customerAccount.CurrentTariff.BrandCode, 
                                                                                    customerAccount.CurrentTariff.ServicePlanId, 
                                                                                    GetTariffType(customerAccount.CurrentTariff.Name), 
                                                                                    customerAccount.PaymentDetails.RateCode)
                    : await _tariffRepository.GetPreservedSingleRateTariffsByServicePlanIdAsync(
                                                                                    customerAccount.SiteDetails.PostCode, 
                                                                                    customerAccount.CurrentTariff.BrandCode, 
                                                                                    customerAccount.CurrentTariff.ServicePlanId, 
                                                                                    customerAccount.CurrentTariff.FuelType, 
                                                                                    customerAccount.PaymentDetails.RateCode);

                electricityTariffs.AddRange(tariffResults.Select(e => e.ElectricityResult).ToList());
            }

            return electricityTariffs;
        }

        private string GetTariffType(string tariffName)
        {
            string tariffType = string.Empty;
            string tariffTypesString = _configManager.GetAppSetting("MultiRateTariffs");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(tariffTypesString), "Missing MultiRateTariffs in Web Config File");

            if (tariffTypesString != null)
            {
                string[] tariffTypes = tariffTypesString.Split(',');
                foreach (string type in tariffTypes)
                {
                    if (tariffName.Contains(type))
                    {
                        tariffType = type;
                    }
                }
            }

            return tariffType;
        }

        private void SetProgressBar(TariffsViewModel viewModel)
        {
            CTCJourneyType customerJourneyType = _journeyDetailsService.GetCustomerJourney();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (customerJourneyType)
            {
                case CTCJourneyType.PreLogIn:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Active
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.EmailAddressSectionHeader,
                                    Status = ProgressBarStatus.Awaiting
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Awaiting
                                }
                            }
                    };
                    break;

                case CTCJourneyType.PostLogInWithMultipleSites:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SelectAddressSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Active
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Awaiting
                                }
                            }
                    };
                    break;

                case CTCJourneyType.PostLogInWithNoAccounts:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.UserDetailsSectionHeader,
                                    Status = ProgressBarStatus.Done
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Active
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Awaiting
                                }
                            }
                    };
                    break;

                case CTCJourneyType.PostLogInWithSingleSite:
                    viewModel.ProgressBarViewModel = new ProgressBarViewModel
                    {
                        Sections = new List<ProgressBarSection>
                            {
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                                    Status = ProgressBarStatus.Active
                                },
                                new ProgressBarSection
                                {
                                    Text = ProgressBar_Resource.SummarySectionHeader,
                                    Status = ProgressBarStatus.Awaiting
                                }
                            }
                    };
                    break;
            }
        }

        private TariffsViewModel RemoveInvalidTariffs(TariffsViewModel viewModel)
        {
            string codes = _configManager.GetValueForKeyFromSection("tariffManagement", "excludedTariffs", "TariffIds") ?? string.Empty;
            var filteredTariffs = new List<AvailableTariff>();
            foreach (AvailableTariff tariff in viewModel.AvailableTariffs)
            {
                bool exclude = false;

                if (viewModel.FuelType == FuelType.Gas)
                {
                    exclude = IsTariffExcluded(tariff.GasDetails, codes);
                }
                else
                {
                    if (viewModel.HasMultiRateMeter)
                    {
                        exclude = IsTariffExcluded(tariff.ElectricityDetails, codes);

                        if (!exclude)
                        {
                            exclude = IsTariffExcluded(tariff.GasDetails, codes);
                        }
                    }
                }

                if (!exclude)
                {
                    filteredTariffs.Add(tariff);
                }
            }

            viewModel.AvailableTariffs = filteredTariffs;

            return viewModel;
        }

        private static bool IsTariffExcluded(TariffInformationLabel tariff, string codes)
        {
            return tariff != null && codes.ToUpper().Contains(tariff.ServicePlanId.ToUpper());
        }

        private async Task<List<Tariff>> GetAvailableTariffsWithWHDDiscount(Customer customer, List<Tariff> tariffs)
        {
            bool directDebitDiscount = (customer.ElectricityAccount?.PaymentDetails?.HasDirectDebitDiscount ?? false) || (customer.GasAccount?.PaymentDetails?.HasDirectDebitDiscount ?? false);

            if (!directDebitDiscount)
            {
                if (customer.ElectricityAccount?.PaymentDetails != null)
                {
                    customer.ElectricityAccount.PaymentDetails.HasDirectDebitDiscount = true;
                }

                if (customer.GasAccount?.PaymentDetails != null)
                {
                    customer.GasAccount.PaymentDetails.HasDirectDebitDiscount = true;
                }

                List<Tariff> availableTariffsWithWHDDiscount = await GetTariffs(customer);
                Tariff standardTariff = availableTariffsWithWHDDiscount.First(d => d.Type == "Evergreen");
                tariffs.Replace(x1 => x1.Type == "Evergreen", standardTariff);
            }

            if (customer.ElectricityAccount?.PaymentDetails != null)
            {
                customer.ElectricityAccount.PaymentDetails.HasDirectDebitDiscount = false;
            }

            if (customer.GasAccount?.PaymentDetails != null)
            {
                customer.GasAccount.PaymentDetails.HasDirectDebitDiscount = false;
            }

            return tariffs;
        }
    }
}