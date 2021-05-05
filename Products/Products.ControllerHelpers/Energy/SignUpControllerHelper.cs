namespace Products.ControllerHelpers.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Mappers;
    using Model;
    using Model.Broadband;
    using Model.Common;
    using Model.Constants;
    using Model.Energy;
    using Model.Enums;
    using Service.Broadband.Managers;
    using Service.Broadband.Mappers;
    using Service.Common;
    using Service.Common.Managers;
    using Service.Common.Mappers;
    using Service.Energy;
    using Service.Energy.Mappers;
    using WebModel.Resources.Common;
    using WebModel.Resources.Energy;
    using WebModel.ViewModels.Broadband.Extensions;
    using WebModel.ViewModels.Common;
    using WebModel.ViewModels.Energy;
    using Tariff = Model.Energy.Tariff;

    public class SignUpControllerHelper : BaseEnergyControllerHelper, ISignUpControllerHelper
    {
        private const string SummaryViewConfigGroupName = "energySummaryViewPage";
        private const string TariffSubHeaderSectionName = "tariffSubHeaders";
        private const string Energy = "energy";
        private const string EnergyAndBroadband = "energy and broadband";

        private readonly IBankValidationService _bankValidationService;
        private readonly IBroadbandProductsService _broadbandProductsService;
        private readonly ICustomerProfileService _customerProfileService;
        private readonly IPostcodeCheckerService _postcodeCheckerService;
        private readonly ISummaryService _summaryService;
        private readonly ITariffManager _tariffManager;
        private readonly ITariffMapper _tariffMapper;
        private readonly IBroadbandManager _broadbandManager;
        private readonly IContentManagementControllerHelper _contentManagementControllerHelper;

        public SignUpControllerHelper(
            ICustomerProfileService customerProfileService,
            IBankValidationService bankValidationService,
            ISummaryService summaryService,
            ISessionManager sessionManager,
            ITariffManager tariffManager,
            IConfigManager configManager,
            ITariffMapper tariffMapper,
            IBroadbandProductsService broadbandProductsService,
            IPostcodeCheckerService postcodeCheckerService,
            WebClientData webClientData, IBroadbandManager broadbandManager,
            IContentManagementControllerHelper contentManagementControllerHelper)
            : base(sessionManager, configManager, webClientData)
        {
            Guard.Against<ArgumentException>(customerProfileService == null, $"{nameof(customerProfileService)} is null");
            Guard.Against<ArgumentException>(bankValidationService == null, $"{nameof(bankValidationService)} is null");
            Guard.Against<ArgumentException>(summaryService == null, $"{nameof(summaryService)} is null");
            Guard.Against<ArgumentException>(sessionManager == null, $"{nameof(sessionManager)} is null");
            Guard.Against<ArgumentException>(tariffManager == null, $"{nameof(tariffManager)} is null");
            Guard.Against<ArgumentException>(tariffMapper == null, $"{nameof(tariffMapper)} is null");
            Guard.Against<ArgumentException>(broadbandProductsService == null, $"{nameof(broadbandProductsService)} is null");
            Guard.Against<ArgumentException>(postcodeCheckerService == null, $"{nameof(postcodeCheckerService)} is null");
            Guard.Against<ArgumentException>(broadbandManager == null, $"{nameof(broadbandManager)} is null");
            Guard.Against<ArgumentException>(contentManagementControllerHelper == null, $"{nameof(contentManagementControllerHelper)} is null");

            _customerProfileService = customerProfileService;
            _bankValidationService = bankValidationService;
            _summaryService = summaryService;
            _tariffManager = tariffManager;
            _tariffMapper = tariffMapper;
            _broadbandProductsService = broadbandProductsService;
            _postcodeCheckerService = postcodeCheckerService;
            _broadbandManager = broadbandManager;
            _contentManagementControllerHelper = contentManagementControllerHelper;
        }

        public async Task<bool> CreateOnlineProfile()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return await CreateOnlineProfile(energyCustomer, energyCustomer.OnlineAccountPassword);
        }

        public async Task<bool> DoesProfileExist()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Guid profileId = await _customerProfileService.GetProfileIdByEmail(energyCustomer.ContactDetails.EmailAddress);
            bool profileExists = profileId != Guid.Empty;
            energyCustomer.UserId = profileId;
            energyCustomer.ProfileExists = profileExists;
            return profileExists;
        }

        public ContactDetailsViewModel GetContactDetailsViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            var contactDetailsViewModel = new ContactDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "PersonalDetails",
                    ControllerName = "SignUp",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                ShoppingBasketViewModel = GetYourPriceViewModel(),
                OkNextAccessibilityText = energyCustomer.SelectedTariff.IsBundle ? Resources.BundleButtonNextSignUpAlt : Resources.ButtonNextSignUpEnergyAlt
            };

            // ReSharper disable once PossibleNullReferenceException
            if (energyCustomer.ContactDetails != null)
            {
                contactDetailsViewModel.ContactNumber = energyCustomer.ContactDetails.ContactNumber;
                contactDetailsViewModel.EmailAddress = energyCustomer.ContactDetails.EmailAddress;
                contactDetailsViewModel.ConfirmEmailAddress = energyCustomer.ContactDetails.EmailAddress;
                contactDetailsViewModel.IsMarketingConsentChecked = energyCustomer.ContactDetails.MarketingConsent;
            }

            return contactDetailsViewModel;
        }

        public OnlineAccountViewModel GetOnlineAccountViewModel()
        {
            return new OnlineAccountViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "ContactDetails",
                    ControllerName = "SignUp",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };
        }

        public PersonalDetailsViewModel GetPersonalDetailsViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            string actionName = energyCustomer.SelectedTariff.IsBundle ? "Extras" : "AvailableTariffs";
            string controllerName = energyCustomer.SelectedTariff.IsBundle ? "Signup" : "Tariffs";
            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                actionName = energyCustomer.SelectedTariff.HasExtras() ? "Extras" : "PhonePackage";
            }

            var personalDetailsViewModel = new PersonalDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    // ReSharper disable once PossibleNullReferenceException
                    ActionName = actionName,
                    ControllerName = controllerName,
                    TitleAttributeText = Resources.BackButtonAlt
                },
                // ReSharper disable once PossibleNullReferenceException
                IsScottishPostcode = _postcodeCheckerService.IsScottishPostcode(energyCustomer.Postcode),
                ShoppingBasketViewModel = GetYourPriceViewModel(),
                OkNextAccessibilityText = energyCustomer.SelectedTariff.IsBundle ? Resources.BundleButtonNextSignUpAlt : Resources.ButtonNextSignUpEnergyAlt
            };

            if (energyCustomer.PersonalDetails != null)
            {
                personalDetailsViewModel.Titles = energyCustomer.PersonalDetails.Title.ToEnum<Titles>();
                personalDetailsViewModel.FirstName = energyCustomer.PersonalDetails.FirstName;
                personalDetailsViewModel.LastName = energyCustomer.PersonalDetails.LastName;
                personalDetailsViewModel.DateOfBirth = energyCustomer.PersonalDetails.DateOfBirth;
                personalDetailsViewModel.DateOfBirthDay = energyCustomer.PersonalDetails.DateOfBirth.Split('/')[0];
                personalDetailsViewModel.DateOfBirthMonth = energyCustomer.PersonalDetails.DateOfBirth.Split('/')[1];
                personalDetailsViewModel.DateOfBirthYear = energyCustomer.PersonalDetails.DateOfBirth.Split('/')[2];
            }

            return personalDetailsViewModel;
        }

        public bool HasSelectedDirectDebit()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            return energyCustomer.IsDirectDebit();
        }

        public void SavePersonalDetailsVieModel(PersonalDetailsViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            // ReSharper disable once PossibleNullReferenceException
            energyCustomer.PersonalDetails = new PersonalDetails
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Title = viewModel.Titles.ToString(),
                DateOfBirth = viewModel.DateOfBirth
            };

            SaveEnergyCustomerInSession(energyCustomer);
        }

        public void SetContactDetails(ContactDetailsViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            // ReSharper disable once PossibleNullReferenceException
            energyCustomer.ContactDetails = new ContactDetails
            {
                ContactNumber = viewModel.ContactNumber,
                EmailAddress = viewModel.EmailAddress,
                MarketingConsent = viewModel.IsMarketingConsentChecked
            };
        }

        public void SaveOnlineAccountPassword(string password)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            energyCustomer.OnlineAccountPassword = password;
        }

        public BankDetailsViewModel GetBankDetailsViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            // ReSharper disable once PossibleNullReferenceException
            Guard.Against<ArgumentException>(energyCustomer.SelectedTariff == null, $"{nameof(energyCustomer.SelectedTariff)} is null");

            double? electricityAmount = energyCustomer.SelectedTariff.GetProjectedMonthlyElectricityCost();
            double? gasAmount = energyCustomer.SelectedTariff.GetProjectedMonthlyGasCost();
            double? bundleAmount = energyCustomer.SelectedTariff.GetProjectedMonthlyBundleCost();

            var bankDetailsViewModel = new BankDetailsViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "ContactDetails",
                    ControllerName = "SignUp",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                // ReSharper disable once PossibleNullReferenceException
                IsBroadbandBundleSelected = energyCustomer.SelectedTariff.IsBroadbandBundle(),
                IsHesBundle = energyCustomer.SelectedTariff.IsHesBundle(),
                BundleHasExtras = SelectedTariffHasExtras(),
                BundleExtras = energyCustomer.SelectedExtras,
                AmountItemList = new List<Tuple<string, string>>(),
                ExtraDetailsList = new List<Tuple<string, string>>(),
                HesBundlePackageAmount = bundleAmount?.ToCurrency(),
                HesWhyDoIPay0ModalId = "why-is-there-0-payment",
                ShoppingBasketViewModel = GetYourPriceViewModel(),
                // ReSharper disable once PossibleNullReferenceException
                IsUpgrade = energyCustomer.SelectedTariff.IsUpgrade
            };




            if (bankDetailsViewModel.BundleExtras.Count > 0)
            {
                foreach (Extra extra in bankDetailsViewModel.BundleExtras)
                    bankDetailsViewModel.ExtraDetailsList.Add(new Tuple<string, string>($"{extra.Name}: ",
                        extra.BundlePrice.ToCurrency()));
            }

            if (electricityAmount != null)
            {
                bankDetailsViewModel.AmountItemList.Add(new Tuple<string, string>(BankDetailsCommon_Resources.ElectricityLabel,
                    electricityAmount.Value.RoundUpToNearestPoundWithPoundSign()));
            }

            if (gasAmount != null)
            {
                bankDetailsViewModel.AmountItemList.Add(new Tuple<string, string>(BankDetailsCommon_Resources.GasLabel,
                    gasAmount.Value.RoundUpToNearestPoundWithPoundSign()));
            }

            if (bundleAmount != null && energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                string broadbandProductDisplayText = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade ? ProductFeatures_Resources.BroadbandPlusProductDisplayText : ProductFeatures_Resources.BroadbandProductDisplayText;

                string broadbandLabel =
                    $"{BankDetailsCommon_Resources.BroadbandLabel}{broadbandProductDisplayText}, {energyCustomer.SelectedBroadbandProduct?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode)?.TalkCode?.GetTelName()}: ";
                string broadbandDisplayAmount = (energyCustomer.SelectedTariff.GetProjectedMonthlyBundleCost()
                                                 + energyCustomer.SelectedBroadbandProduct
                                                     ?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode)
                                                     ?.GetPhoneCost())?.ToPounds();
                bankDetailsViewModel.BroadbandBundleDescription = broadbandLabel;
                bankDetailsViewModel.BroadbandBundlePackageAmount = broadbandDisplayAmount;
            }

            if (energyCustomer.SelectedTariff.IsHesBundle())
            {
                bankDetailsViewModel.HomeServicesLabel = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade
                    ? BankDetailsCommon_Resources.HomeServicesPlusLabel
                    : BankDetailsCommon_Resources.HomeServicesLabel;

                if (bankDetailsViewModel.BundleExtras.Any(x => x.Type == ExtraType.ElectricalWiring))
                {
                    bankDetailsViewModel.PDFGuaranteeLabel = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade ? BankDetailsCommon_Resources.HesPlusElecWiringPDFLinkText : BankDetailsCommon_Resources.HesElecWiringPDFLinkText;
                    bankDetailsViewModel.PDFGuaranteeLabelAlt = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade ? BankDetailsCommon_Resources.HesPlusElecWiringPDFLinkAlt : BankDetailsCommon_Resources.HesElecWiringPDFLinkAlt;
                }
                else
                {
                    bankDetailsViewModel.PDFGuaranteeLabel = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade ? BankDetailsCommon_Resources.HesPlusPDFLinkText : BankDetailsCommon_Resources.HesPDFLinkText;
                    bankDetailsViewModel.PDFGuaranteeLabelAlt = energyCustomer.SelectedTariff != null && energyCustomer.SelectedTariff.IsUpgrade ? BankDetailsCommon_Resources.HesPlusPDFLinkAlt : BankDetailsCommon_Resources.HesPDFLinkAlt;
                }
            }

            return bankDetailsViewModel;
        }

        public BankDetailsViewModel GetUpdatedBankDetailsViewModel(BankDetailsViewModel viewModelToUpdate)
        {
            BankDetailsViewModel viewModel = GetBankDetailsViewModel();
            viewModelToUpdate.BackChevronViewModel = viewModel.BackChevronViewModel;
            viewModelToUpdate.AmountItemList = viewModel.AmountItemList;
            viewModelToUpdate.ShoppingBasketViewModel = viewModel.ShoppingBasketViewModel;
            viewModelToUpdate.IsBroadbandBundleSelected = viewModel.IsBroadbandBundleSelected;
            viewModelToUpdate.BroadbandBundleDescription = viewModel.BroadbandBundleDescription;
            viewModelToUpdate.BroadbandBundlePackageAmount = viewModel.BroadbandBundlePackageAmount;
            viewModelToUpdate.IsHesBundle = viewModel.IsHesBundle;
            viewModelToUpdate.HesBundlePackageAmount = viewModel.HesBundlePackageAmount;
            viewModelToUpdate.HesWhyDoIPay0ModalId = viewModel.HesWhyDoIPay0ModalId;
            viewModelToUpdate.BundleExtras = viewModel.BundleExtras;
            viewModelToUpdate.BundleHasExtras = viewModel.BundleHasExtras;
            viewModelToUpdate.ExtraDetailsList = viewModel.ExtraDetailsList;
            viewModelToUpdate.PDFGuaranteeLabel = viewModel.PDFGuaranteeLabel;
            viewModelToUpdate.PDFGuaranteeLabelAlt = viewModel.PDFGuaranteeLabelAlt;
            viewModelToUpdate.HomeServicesLabel = viewModel.HomeServicesLabel;
            viewModelToUpdate.IsUpgrade = viewModel.IsUpgrade;

            return viewModelToUpdate;
        }

        public bool? ValidateBankDetails(BankDetailsViewModel model)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            if (energyCustomer.BankServiceRetryCount >= 3)
            {
                return false;
            }

            BankDetails bankDetails = _bankValidationService.GetBankDetails(model.SortCode, model.AccountNumber);

            if (bankDetails == null)
            {
                energyCustomer.BankServiceRetryCount++;
                return true;
            }

            energyCustomer.BankServiceRetryCount = 0;

            energyCustomer.DirectDebitDetails = new DirectDebitDetails
            {
                AccountName = model.AccountHolder,
                AccountNumber = model.AccountNumber,
                SortCode = model.SortCode,
                DirectDebitPaymentDate = int.Parse(model.DirectDebitDate),
                BankName = bankDetails.BankName,
                BankAddressLine1 = bankDetails.BankAddress.BankAddressLine1Field,
                BankAddressLine2 = bankDetails.BankAddress.BankAddressLine2Field,
                BankAddressLine3 = bankDetails.BankAddress.BankAddressLine3Field,
                Postcode = bankDetails.BankAddress.BankPostcodeField
            };

            return null;
        }

        public async Task ConfirmSale()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var openReachResponse = SessionManager.GetSessionDetails<OpenReachData>(SessionKeys.OpenReachResponse);

            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                Guard.Against<ArgumentException>(openReachResponse == null, $"{nameof(openReachResponse)} is null");
            }

            await _summaryService.ConfirmSale(energyCustomer, openReachResponse);
        }

        public DirectDebitMandateViewModel GetPrintMandateViewModel(ProductType productType)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Guard.Against<ArgumentException>(energyCustomer.IsDirectDebit() && energyCustomer.DirectDebitDetails == null,
                "Direct debit details object is null");

            return DirectDebitMapper.GetMandateViewModel(energyCustomer.DirectDebitDetails, productType);
        }

        public SummaryViewModel GetSummaryViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var availableTariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);

            Guard.Against<ArgumentException>(availableTariffs == null, $"{nameof(availableTariffs)} is null");
            Guard.Against<ArgumentException>(energyCustomer.SelectedTariff == null, "Selected tariff object is null");
            Guard.Against<ArgumentException>(energyCustomer.PersonalDetails == null, "Personal details object is null");
            Guard.Against<ArgumentException>(energyCustomer.IsDirectDebit() && energyCustomer.DirectDebitDetails == null,
                "Direct debit details object is null");
            List<CMSEnergyContent> cmsEnergyContents = _contentManagementControllerHelper.GetCMSEnergyContentList();

            Tariff tariff = energyCustomer.SelectedTariff;
            TariffsViewModel selectedTariffViewModel = _tariffMapper.ToTariffViewModel(energyCustomer.SelectedTariff, energyCustomer, cmsEnergyContents);
            string elecMessage =
                $"{energyCustomer.SelectedElectricityMeterType.ToDescription()} {(energyCustomer.IsSmartMeter() || (energyCustomer.HasSmartMeter ?? false) ? Summary_Resources.Smart : Summary_Resources.NonSmart)}";
            string gasMessage =
                $"{ElectricityMeterType.Standard.ToDescription()} {(energyCustomer.IsSmartMeter() || (energyCustomer.HasSmartMeter ?? false) ? Summary_Resources.Smart : Summary_Resources.NonSmart)}";

            string elecMeterSerial = energyCustomer.GetElectricityMeterInformation()?.MeterSerialNumber ?? string.Empty;
            string gasMeterSerial = energyCustomer.GetGasMeterInformation()?.MeterSerialNumber ?? string.Empty;

            TermsAndConditionsPdfLink[] allPdfLinks = _tariffManager.GetTermsAndConditionsPdfs(tariff.DisplayName, tariff.TariffGroup, cmsEnergyContents).ToArray();
            List<TermsAndConditionsPdfLink> energyPdfLinks = allPdfLinks.Where(p => p.IsEnergy).ToList();
            List<TermsAndConditionsPdfLink> broadbandBundlePdfLinks = allPdfLinks.Where(p => p.IsBroadbandBundle).ToList();
            List<TermsAndConditionsPdfLink> homeServicesPdfLinks = allPdfLinks.Where(pdf => pdf.IsHomeServicesBundlePdf).ToList();

            TalkProduct talkProduct = energyCustomer.SelectedBroadbandProduct?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode);
            double? talkPrice = talkProduct?.GetMonthlyTalkCost();

            string ddmProductLinkText = energyCustomer.SelectedTariff.IsBroadbandBundle() ? EnergyAndBroadband : Energy;

            string originalBundlePackageMonthlyCost = (tariff.BundlePackage?.MonthlyOriginalCost ?? 0.0).ToCurrency();
            string projectedBundlePackageMonthlySavings = (tariff.BundlePackage?.MonthlySavings ?? 0.00).ToCurrency();
            string bundlePackageProjectedYearlySavings = tariff.BundlePackage?.YearlySavings.ToCurrency();
            Extra selectedElectricalWiring = energyCustomer.SelectedExtras.FirstOrDefault(e => e.Type == ExtraType.ElectricalWiring);
            SelectedExtraViewModel selectedExtraViewModel = null;
            bool isElectricalWiringSelected = null != selectedElectricalWiring;
            if (isElectricalWiringSelected)
            {
                selectedExtraViewModel = new SelectedExtraViewModel(selectedElectricalWiring.ProductCode,
                    selectedElectricalWiring.Name,
                    selectedElectricalWiring.BundlePrice);
            }

            int? bundleContractLength = tariff.IsBundle
                ? (int?)(energyCustomer.SelectedTariff?.BundlePackage?.BundlePackageType == BundlePackageType.FixAndFibre ? 18 : 12)
                : null;
            string hesDDGuaranteeLinkText = tariff.IsUpgrade
                ? Summary_Resources.FixNProtectPlusDDGuaranteePDFLinkText
                : Summary_Resources.FixNProtectDDGuaranteePDFLinkText;
            string hesDDGuaranteeLinkAlt = tariff.IsUpgrade
                ? Summary_Resources.FixNProtectPlusDDGuaranteePDFLinkAlt
                : Summary_Resources.FixNProtectDDGuaranteePDFLinkAlt;

            if (isElectricalWiringSelected)
            {
                hesDDGuaranteeLinkText = tariff.IsUpgrade
                    ? Summary_Resources.FixNProtectPlusElectricalWiringDDGuaranteePDFLinkText
                    : Summary_Resources.FixNProtectElectricalWiringDDGuaranteePDFLinkText;
                hesDDGuaranteeLinkAlt = tariff.IsUpgrade
                    ? Summary_Resources.FixNProtectPlusElectricalWiringDDGuaranteePDFLinkAlt
                    : Summary_Resources.FixNProtectElectricalWiringDDGuaranteePDFLinkAlt;
            }

            string fixNProtectCoverName = string.Empty;
            string policyBookletName = string.Empty;            
            if (tariff.IsHesBundle())
            {
                fixNProtectCoverName = tariff.IsUpgrade ? Summary_Resources.FixNProtectPlusCoverName : Summary_Resources.FixNProtectCoverName;
                policyBookletName = tariff.IsUpgrade ? Summary_Resources.FixNProtectPlusPolicyBookletName : Summary_Resources.FixNProtectPolicyBookletName;
            }

            double projectedMonthlyCost = energyCustomer.SelectedTariff.GetProjectedMonthlyCost(talkProduct) + energyCustomer.GetTotalSelectedExtrasCost();

            var summaryViewModel = new SummaryViewModel
            {
                TariffHeader = tariff.IsBundle ? Summary_Resources.YourTariffHeaderBundle : Summary_Resources.YourTariffHeaderEnergy,
                FuelType = energyCustomer.SelectedFuelType,
                TariffCostFullValue = projectedMonthlyCost.AmountSplitInPounds(),
                TariffCostPenceValue = projectedMonthlyCost.AmountSplitInPence(),
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = energyCustomer.SelectedPaymentMethod == PaymentMethod.MonthlyDirectDebit
                        ? "BankDetails"
                        : "ContactDetails",
                    ControllerName = "SignUp",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                TariffDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "TariffDetailsModal",
                    FirstMessage = Summary_Resources.TariffDetailsModalMessage,
                    RedirectUrl = "/energy-signup/quote-details",
                    CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = "X",
                    CancelButtonText = Summary_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.TariffDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.TariffDetailsModalButtonAlt
                },
                BankDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "BankDetailsModal",
                    FirstMessage = Summary_Resources.BankDetailsModalMessage,
                    RedirectUrl = "/energy-signup/bank-details",
                    CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = "X",
                    CancelButtonText = Summary_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.BankDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.BankDetailsModalButtonAlt
                },
                PersonalDetailsModal = new ConfirmationModalViewModel
                {
                    ModalId = "PersonalDetailsModal",
                    FirstMessage = Summary_Resources.PersonalDetailsModalMessage,
                    RedirectUrl = "/energy-signup/personal-details",
                    CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = "X",
                    CancelButtonText = Summary_Resources.ModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.ModalCancelButtonAlt,
                    ButtonText = Summary_Resources.PersonalDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.PersonalDetailsModalButtonAlt
                },
                PaymentMethod = energyCustomer.SelectedPaymentMethod,
                SelectedTariffViewModel = selectedTariffViewModel,
                // ReSharper disable once PossibleNullReferenceException
                CanChangeTariff = availableTariffs.Count > 1,

                DirectDebitPaymentDate = energyCustomer.DirectDebitDetails?.DirectDebitPaymentDate != null
                    ? NumberFormatter.ToDayOfMonthOrdinal(energyCustomer.DirectDebitDetails.DirectDebitPaymentDate)
                    : string.Empty,
                DirectDebitAccountName = energyCustomer.DirectDebitDetails?.AccountName,
                DirectDebitAccountNumber = energyCustomer.DirectDebitDetails?.AccountNumber,
                DirectDebitSortCode = StringHelper.GetFormattedSortCode(energyCustomer.DirectDebitDetails?.SortCode),

                CustomerFormattedName = energyCustomer.PersonalDetails.FormattedName,
                DateOfBirth = energyCustomer.PersonalDetails.DateOfBirth,
                FullAddress = energyCustomer.FullAddress(),
                ContactNumber = energyCustomer.ContactDetails.ContactNumber,
                EmailAddress = energyCustomer.ContactDetails.EmailAddress,
                ElecMeterTypeMessage = energyCustomer.SelectedFuelType != FuelType.Gas ? elecMessage : string.Empty,
                GasMeterTypeMessage = energyCustomer.SelectedFuelType != FuelType.Electricity ? gasMessage : string.Empty,
                TillInformationHeader = GetTillInformationHeader(energyCustomer.SelectedFuelType),
                IsMeterDetailsAvailable = energyCustomer.IsCAndCJourney(),
                MeterDetailsViewModel = new ConfirmationModalViewModel
                {
                    ModalId = "WrongMeterModal",
                    FirstMessage = Summary_Resources.MeterDetailsModalMessage,
                    RedirectUrl = "/energy-signup/non-matching-meter-details",
                    CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = "X",
                    CancelButtonText = Summary_Resources.MeterDetailsModalCancelButtonText,
                    CancelButtonTextAlt = Summary_Resources.MeterDetailsModalCancelButtonAlt,
                    ButtonText = Summary_Resources.MeterDetailsModalButtonText,
                    ButtonTextAlt = Summary_Resources.MeterDetailsModalButtonAlt,
                    SecondMessage = energyCustomer.SelectedFuelType != FuelType.Gas
                        ? $"{elecMessage}{(string.IsNullOrEmpty(elecMeterSerial) ? string.Empty : " - " + elecMeterSerial)}"
                        : string.Empty,
                    ThirdMessage = energyCustomer.SelectedFuelType != FuelType.Electricity
                        ? $"{gasMessage}{(string.IsNullOrEmpty(gasMeterSerial) ? string.Empty : " - " + gasMeterSerial)}"
                        : string.Empty
                },
                TariffTagLine = tariff.TariffGroup != TariffGroup.None ? ConfigManager.GetValueForKeyFromSection(
                        SummaryViewConfigGroupName
                        , TariffSubHeaderSectionName
                        , energyCustomer.SelectedTariff.DisplayName)
                    : TariffMapper.GetCMSContentForATariff(cmsEnergyContents, tariff.DisplayName)?.TagLine ?? string.Empty,
                EnergyTermsAndConditionsPdfLinks = energyPdfLinks,
                BroadbandBundleTermsAndConditionsPdfLinks = broadbandBundlePdfLinks,
                HomeServicesBundleTermsAndConditionsPdfLinks = homeServicesPdfLinks,
                IsBroadbandBundleSelected = energyCustomer.SelectedTariff.IsBroadbandBundle(),
                IsHesBundleSelected = energyCustomer.SelectedTariff.IsHesBundle(),
                IsElectricalWiringSelected = isElectricalWiringSelected,
                SelectedElectricalWiringCover = selectedExtraViewModel,
                IsEnergyOnlyTariffSelected = !energyCustomer.SelectedTariff.IsBundle,
                TermsAndConditionsText = GetTermsAndConditionsText(energyCustomer.SelectedTariff.BundlePackageType, energyCustomer.SelectedTariff.DisplayName, fixNProtectCoverName, policyBookletName),
                BundlePackageFeatures = _tariffMapper.GetBundlePackageFeatures(tariff.BundlePackageType, tariff),
                DisplayExtrasSection = energyCustomer.HasExtrasSelected(),
                DisplayPhonePackageSection = DisplaySelectedBroadbandProduct(energyCustomer),
                TalkPackageName = talkProduct?.TalkCode.GetTelName(),
                TalkPackagePrice = talkPrice.Equals(0.00) ? Resources.LineRentalTalkPriceText : talkPrice?.ToCurrency(),
                TalkPackageTagline = ConfigManager.GetValueForKeyFromSection("energySummaryViewPage", "broadbandProductTagline", talkProduct?.ProductCode),
                SavingsPerMonthTxt = tariff.IsBundle
                    ? string.Format(Summary_Resources.SavingsPerMonthTxt, tariff.BundlePackage.MonthlySavings.ToCurrency())
                    : string.Empty,
                TalkProductModalViewModel = talkProduct != null ? AvailablePackageMapper.PopulateModalDetails(talkProduct) : null,
                DisclaimerText = tariff.IsBundle ? BundleDisclaimerText(energyCustomer) : GetEnergyDisclaimerText(energyCustomer),
                DDMandateLinkText = string.Format(Summary_Resources.DDMandateLinkText, ddmProductLinkText),
                DDMandateLinkAltText = string.Format(Summary_Resources.DDMandateLinkAltText, ddmProductLinkText),
                BundlePackageType = tariff.BundlePackageType,
                IsBundle = tariff.IsBundle,
                BundlePackageIconFileName = _tariffMapper.BundleIconFileName(tariff.BundlePackageType),
                HesDDGuaranteeLinkText = hesDDGuaranteeLinkText,
                HesDDGuaranteeLinkAlt = hesDDGuaranteeLinkAlt,
                SelectedExtras = new List<BaseSummaryExtra>(),
                AboutYourBundleEnergyPara1 =
                    tariff.IsBundle ? string.Format(Accordion_Resources.AboutYourBundleEnergyPara1, bundleContractLength.Value) : string.Empty,
                AboutYourBundleEnergyPara2 =
                    tariff.IsBundle ? string.Format(Accordion_Resources.AboutYourBundleEnergyPara2, bundleContractLength.Value) : string.Empty,
                BroadbandApplyInstallationFee = energyCustomer.SelectedTariff.IsBroadbandBundle() && energyCustomer.ApplyInstallationFee,
                BroadbandInstallationFee = _broadbandManager.GetInstallationFee().ToCurrency(),
                IsSmartTariff = tariff.IsSmartTariff,
                IsUpgrade = tariff.IsUpgrade,
                FixNProtectAccordionHeaderText = tariff.IsUpgrade
                    ? Summary_Resources.FixNProtectPlusAccordionHeader
                    : Summary_Resources.FixNProtectAccordionHeader,
                IsFixAndControlTariffGroup = tariff.TariffGroup == TariffGroup.FixAndControl,
                IsFixAndDriveTariffGroup = tariff.TariffGroup == TariffGroup.FixAndDrive
            };
            if (tariff.IsBroadbandBundle())
            {
                summaryViewModel.BundleMegaModalName = tariff.IsUpgrade ? "_UpgradeFixNFibrePlusLearnMoreModal" : "_BroadbandBundleMegaModal";
                summaryViewModel.BundleMegaModalViewModel =
                    tariff.IsUpgrade
                        ? (object)GetUpgradesViewModel()
                        : SummaryViewBroadbandMoreInformationViewModel(tariff, energyCustomer);
                summaryViewModel.FixNFibreSidebarText = tariff.IsUpgrade
                    ? Summary_Resources.FixNFibrePlusSidebarText
                    : Summary_Resources.FixNFibreSidebarText;

                summaryViewModel.AboutYourBundlePara3 = string.Format(Accordion_Resources.AboutYourBundlePara3, 
                    (tariff.IsUpgrade ? 32.00 : 28.00).ToCurrency());
            }
            else
            {
                summaryViewModel.FixNProtectBreakdownCoverText = tariff.IsUpgrade
                    ? ProductFeatures_Resources.HesPackagePlusDisplayText
                    : ProductFeatures_Resources.HesPackageDisplayText;

                summaryViewModel.HesBundleBoilerCoverSidebarText =
                    tariff.IsUpgrade
                        ? Summary_Resources.HesBundlePlusBoilerCoverSidebarText
                        : Summary_Resources.HesBundleBoilerCoverSidebarText;
                summaryViewModel.BundleMegaModalName = tariff.IsUpgrade ? "_UpgradeFixNProtectPlusLearnMoreModal" : "_FixNProtectBundleMegaModal";
                summaryViewModel.BundleMegaModalViewModel =
                    tariff.IsUpgrade
                        ? (object)GetUpgradesViewModel()
                        : _tariffMapper.GetHesMoreInformationViewModel(
                            tariff,
                            originalBundlePackageMonthlyCost,
                            projectedBundlePackageMonthlySavings,
                            bundlePackageProjectedYearlySavings);
            }

            summaryViewModel.MoreInformationModalId = tariff.IsUpgrade
                ? $"#{((BundleUpgradeViewModel)summaryViewModel.BundleMegaModalViewModel).MoreInformationModalId}"
                : _tariffMapper.GetMoreInformationModalId(tariff.BundlePackageType);

            if (energyCustomer.HasExtrasSelected())
            {
                foreach (Extra extra in energyCustomer.SelectedExtras)
                    summaryViewModel.SelectedExtras.Add(new BaseSummaryExtraWithModal
                    {
                        Name = extra.Name,
                        FeatureList = new List<string> { extra.TagLine, string.Format(ProductFeatures_Resources.ContractLengthFeature, extra.ContractLength) },
                        Price = extra.BundlePrice.ToCurrency(),
                        TrashModalDataTarget = $"#remove-extra-modal-{extra.ProductCode}",
                        TrashModalLinkAltText = Summary_Resources.RemoveExtraAlt,
                        ProductCode = extra.ProductCode,
                        SectionId = $"extra-section-{extra.ProductCode}",
                        RemoveExtraModalViewModel = new ConfirmationModalViewModel
                        {
                            ModalId = $"remove-extra-modal-{extra.ProductCode}",
                            FirstMessage = Summary_Resources.RemoveExtraPromptText,
                            RedirectUrl = "",
                            CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                            CloseButtonLabel = "X",
                            CancelButtonText = Summary_Resources.ExtraPromptNo,
                            CancelButtonTextAlt = Summary_Resources.ExtraPromptNoAlt,
                            ButtonText = Summary_Resources.UpgradePromptYes,
                            ButtonTextAlt = Summary_Resources.ExtraPromptYesAlt
                        },
                        ExtrasItemViewModel = new ExtrasItemViewModel
                        (
                            extra.Name,
                            extra.BundlePrice,
                            extra.ProductCode,
                            extra.TagLine,
                            extra.BulletList1,
                            extra.BulletList2,
                            true,
                            WebClientData.BaseUrl
                        )
                    });
            }

            if (energyCustomer.SelectedTariff.IsBundle)
            {
                summaryViewModel.RemovePhonePackageModalViewModel = new ConfirmationModalViewModel
                {
                    ModalId = "remove-upgrade-modal",
                    FirstMessage = Summary_Resources.UpgradePromptText,
                    RedirectUrl = "",
                    CloseButtonAlt = Summary_Resources.ModalCloseButtonAlt,
                    CloseButtonLabel = "X",
                    CancelButtonText = Summary_Resources.UpgradePromptNo,
                    CancelButtonTextAlt = Summary_Resources.UpgradePromptNoAlt,
                    ButtonText = Summary_Resources.UpgradePromptYes,
                    ButtonTextAlt = Summary_Resources.UpgradePromptYesAlt
                };
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (summaryViewModel.BundlePackageType == BundlePackageType.FixAndFibre)
            {
                summaryViewModel.BundlePackageHeaderText = ProductFeatures_Resources.BroadbandLabel;
                summaryViewModel.BundlePackageSubHeaderText = tariff.IsUpgrade ? YourPrice_Resources.FixAndFibrePlusSubHeaderText : YourPrice_Resources.FixAndFibreSubHeaderText;
            }
            else if (summaryViewModel.BundlePackageType == BundlePackageType.FixAndProtect)
            {
                summaryViewModel.BundlePackageHeaderText = tariff.IsUpgrade ? YourPrice_Resources.FixNProtectPlusTextHeader : YourPrice_Resources.FixNProtectTextHeader;
            }

            return summaryViewModel;
        }

        public ConfirmationViewModel ConfirmationViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            var tariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);

            Guard.Against<ArgumentException>(tariffs == null, $"{nameof(tariffs)} is null");
            Guard.Against<ArgumentException>(energyCustomer?.SelectedTariff == null, $"{nameof(energyCustomer.SelectedTariff)} is null");

            // ReSharper disable once PossibleNullReferenceException
            Tariff tariff = energyCustomer.SelectedTariff;

            var crossSellBanner = new ConfirmationCrossSellBannerViewModel();
            // ReSharper disable once PossibleNullReferenceException
            if (energyCustomer.SelectedTariff != null && tariff.IsBundle)
            {
                crossSellBanner.Header =
                    tariff.IsBroadbandBundle() ? Confirmation_Resources.CrossSellHeaderHES : Confirmation_Resources.CrossSellHeaderBroadband;
                crossSellBanner.Paragraph =
                    tariff.IsBroadbandBundle() ? Confirmation_Resources.CrossSellTextHES : Confirmation_Resources.CrossSellTextBroadband;
                crossSellBanner.DesktopImage = tariff.IsBroadbandBundle() ? "cross-sell-banner-desktop.png" : "broadband.png";
                crossSellBanner.LinkText = tariff.IsBroadbandBundle()
                    ? Confirmation_Resources.CrossSellLinkTextHES
                    : Confirmation_Resources.CrossSellLinkTextBroadband;
                crossSellBanner.LinkAltText = tariff.IsBroadbandBundle()
                    ? Confirmation_Resources.CrossSellLinkAltHES
                    : Confirmation_Resources.CrossSellLinkAltBroadband;
                crossSellBanner.LinkUrl = tariff.IsBroadbandBundle()
                    ? Confirmation_Resources.CrossSellLinkUrlHES
                    : Confirmation_Resources.CrossSellLinkUrlBroadband;
            }

            // ReSharper disable once PossibleNullReferenceException
            Tariff selectedTariff = energyCustomer.SelectedTariff;
            var confirmationViewModel = new ConfirmationViewModel
            {
                // ReSharper disable once PossibleNullReferenceException
                ProductName = energyCustomer.SelectedTariff?.DisplayName,
                DataLayer = GetDataLayerViewModel(),
                // ReSharper disable once PossibleNullReferenceException
                IsSmartMessageVisible = selectedTariff.IsSmartTariff && !(energyCustomer.IsSmartMeter() || (energyCustomer.HasSmartMeter ?? false)),
                // ReSharper disable once PossibleNullReferenceException
                IsABundle = selectedTariff.IsBundle,
                CrossSellBanner = crossSellBanner,
                ThankYouText = string.Format(Confirmation_Resources.Paragraph1, selectedTariff.DisplayName, selectedTariff.DisplayNameSuffix)
            };

            if (selectedTariff.IsBundle)
            {
                confirmationViewModel.HelpPageUrlText = Confirmation_Resources.BundleHelpPageUrlText;
                confirmationViewModel.HelpPageUrlAltText = Confirmation_Resources.BundleHelpPageUrlAlt;
                confirmationViewModel.BoxText = selectedTariff.IsBroadbandBundle()
                    ? Confirmation_Resources.BroadbandBundleBoxText
                    : Confirmation_Resources.HesBundleBoxText;
            }
            else
            {
                confirmationViewModel.HelpPageUrlText = Confirmation_Resources.EnergyHelpPageUrlText;
                confirmationViewModel.HelpPageUrlAltText = Confirmation_Resources.EnergyHelpPageUrlAlt;
                confirmationViewModel.BoxText = Confirmation_Resources.EnergyOnlyBoxText;
            }

            confirmationViewModel.WhatHappensNext = new List<string>();
            if (selectedTariff.IsBroadbandBundle())
            {
                confirmationViewModel.WhatHappensNext.Add(Confirmation_Resources.WhatHappensNext2Broadband);
            }
            else if (selectedTariff.IsHesBundle())
            {
                confirmationViewModel.WhatHappensNext.Add(Confirmation_Resources.WhatHappensNext2Hes);
                confirmationViewModel.WhatHappensNext.Add(Confirmation_Resources.WhatHappensNext3Hes);
            }
            else if (!selectedTariff.IsBundle)
            {
                confirmationViewModel.WhatHappensNext.Add(Confirmation_Resources.WhatHappensNext2Energy);
            }

            SessionManager.ClearSession();
            return confirmationViewModel;
        }

        public UnableToCompleteViewModel GetUnableToCompleteViewModel()
        {
            return new UnableToCompleteViewModel
            {
                Title = Fallout_Resources.UnableToCompleteTitle,
                Header = Fallout_Resources.UnableToCompleteHeader,
                Body = Fallout_Resources.UnableToCompleteBody,
                ContactHeaderPre = Fallout_Resources.Contact2Header,
                ContactNumber = Fallout_Resources.ContactNumber2,
                ContactNumberAlt = Fallout_Resources.ContactNumber2,
                ContactNumberUrl = Fallout_Resources.ContactNumberUrl2,
                ContactBody = Fallout_Resources.Contact2Body
            };
        }

        public PhonePackageViewModel GetPhonePackageViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            BroadbandProductGroup selectedBroadbandProductGroup = _broadbandManager.BroadbandProductGroup(energyCustomer.SelectedBroadbandProductCode);
            var viewModel = new PhonePackageViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "Upgrades",
                    ControllerName = "SignUp",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                BroadbandPackageSpeedViewModel = GetBroadbandPackageSpeedViewModel(
                    energyCustomer.SelectedBroadbandProduct,
                    energyCustomer.Postcode,
                    false,
                    true),
                SelectTalkPackageViewModel = new SelectTalkPackageViewModel
                {
                    TalkProducts = energyCustomer
                        .SelectedBroadbandProduct
                        .TalkProducts
                        .Where(tp => tp.BroadbandProductGroup == selectedBroadbandProductGroup)
                        .OrderBy(prod => prod.Prices.FirstOrDefault(p => p.FeatureCode == FeatureCodes.MonthlyTalkCharge)?.Price ?? 0)
                        .Select(tp => tp.ToTalkProduct())
                        .ToList(),
                    SelectedTalkProductCode = energyCustomer.SelectedBroadbandProductCode
                },
                KeepYourNumberViewModel = new KeepYourNumberViewModel
                {
                    // ReSharper disable once PossibleNullReferenceException
                    CLI = energyCustomer.CLIChoice.FinalCLI,
                    ShowExistingPhoneNumberParagraph = energyCustomer.IsSSECustomerCLI
                },
                ApplyInstallationFee = energyCustomer.ApplyInstallationFee,
                ShoppingBasketViewModel = GetYourPriceViewModel(),
                HowSavingsCalculatedText = energyCustomer.SelectedTariff.IsUpgrade 
                    ? Upgrades_Resources.FixNFibrePlusSavingsPara_PlusPackageAdded 
                    : Upgrades_Resources.FixNFibrePlusSavingsCalculatorPara1
            };

            viewModel.KeepYourNumberViewModel.IsReadOnly = !string.IsNullOrEmpty(viewModel.KeepYourNumberViewModel.CLI);
            viewModel.KeepYourNumberViewModel.KeepExistingNumber = !string.IsNullOrEmpty(viewModel.KeepYourNumberViewModel.CLI);
            viewModel.NewInstallationViewModel = new NewInstallationViewModel
            {
                InstallationHeader = PhonePackage_Resources.InstallationHeader,
                InstallationText = string.Format(PhonePackage_Resources.InstallationText, _broadbandManager.GetInstallationFee())
            };

            return viewModel;
        }

        public ExtrasViewModel GetExtrasViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            string backActionName;
            string backControllerName;
            if (SelectedTariffHasUpgrades())
            {
                backActionName = "Upgrades";
                backControllerName = "SignUp";
            }
            else
            {
                backActionName = energyCustomer.SelectedTariff.IsBroadbandBundle() ? "PhonePackage" : "AvailableTariffs";
                backControllerName = energyCustomer.SelectedTariff.IsBroadbandBundle() ? "SignUp" : "Tariffs";
            }

            var viewModel = new ExtrasViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = backActionName,
                    ControllerName = backControllerName,
                    TitleAttributeText = Resources.BackButtonAlt
                },
                HowSavingsCalculatedText = energyCustomer.SelectedTariff.IsUpgrade ? Upgrades_Resources.FixNProtectPlusSavingsPara_PlusPackageAdded : Upgrades_Resources.FixNProtectPlusSavingsCalculationPara1,
                ShoppingBasketViewModel = GetYourPriceViewModel(),
                Extras = energyCustomer.SelectedTariff.Extras.Select(e =>
                {
                    bool isAdded = energyCustomer.SelectedExtras.Contains(e);
                    return new ExtrasItemViewModel
                    (
                        e.Name,
                        e.BundlePrice,
                        e.ProductCode,
                        e.TagLine,
                        e.BulletList1,
                        e.BulletList2,
                        isAdded,
                        WebClientData.BaseUrl
                    );
                }).ToList()
            };
            return viewModel;
        }

        public void UpdateCLIFromSession(KeepYourNumberViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            viewModel.CLI = energyCustomer.CLIChoice.FinalCLI;
        }

        public BundleUpgradeViewModel GetUpgradesViewModel()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            string nextButtonAction = "PersonalDetails";
            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                nextButtonAction = "PhonePackage";
            }
            else if (SelectedTariffHasExtras())
            {
                nextButtonAction = "Extras";
            }

            Tariff upgrade = energyCustomer.AvailableBundleUpgrade;
            bool isAdded = energyCustomer.SelectedTariff.TariffId.Equals(upgrade.TariffId);
            return energyCustomer.SelectedTariff.IsBroadbandBundle()
                ? GetFixNFibrePlusUpgradeViewModel(energyCustomer, upgrade, isAdded, nextButtonAction)
                : GetFixNProtectPlusUpgradeViewModel(upgrade, isAdded, nextButtonAction);
        }

        private BundleUpgradeViewModel GetFixNProtectPlusUpgradeViewModel(Tariff upgrade, bool isAdded, string nextButtonAction)
        {
            var viewModel = new BundleUpgradeViewModel(
                GetYourPriceViewModel(),
                Upgrades_Resources.FixNProtectPlusModalProductName,
                Upgrades_Resources.FixNProtectPlusModalTagline,
                Upgrades_Resources.FixNProtectPlusHeader,
                Upgrades_Resources.FixNProtectPlusTagline,
                upgrade.TariffId,
                nextButtonAction,
                upgrade.BundlePackageType,
                upgrade.BundlePackage.MonthlyDiscountedCost,
                upgrade.BundlePackage.MonthlyOriginalCost,
                isAdded,
                WebClientData.BaseUrl,
                isAdded ? Upgrades_Resources.FixNProtectPlusSavingsPara_PlusPackageAdded : Upgrades_Resources.FixNProtectPlusSavingsCalculationPara1,
                whatsIncluded: upgrade.BundlePackage?.HesMoreInformation.WhatsIncluded,
                whatsExcluded: upgrade.BundlePackage?.HesMoreInformation.WhatsExcluded)
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "AvailableTariffs",
                    ControllerName = "Tariffs",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };

            return viewModel;
        }

        private BundleUpgradeViewModel GetFixNFibrePlusUpgradeViewModel(EnergyCustomer energyCustomer, Tariff upgrade, bool isAdded, string nextButtonAction)
        {
            BroadbandProductGroup selectedBroadbandProductGroup = _broadbandManager.BroadbandProductGroup(upgrade.BundlePackage.ProductCode);
            BroadbandProduct broadbandProduct = GetDefaultBroadbandProduct(selectedBroadbandProductGroup, upgrade.BundlePackage.ProductCode);
            BroadbandPackageSpeedViewModel broadbandPackageSpeedViewModel =
                GetBroadbandPackageSpeedViewModel(broadbandProduct, energyCustomer.Postcode, false, false);
            broadbandPackageSpeedViewModel.PackageDescription = AvailableBundleTariffs_Resources.FixNFibrePlusPackageDescription;

            string fibrePlusHeader = string.Format(Upgrades_Resources.FixNFibrePlusHeader, broadbandPackageSpeedViewModel.MaxDownload);
            var viewModel = new BundleUpgradeViewModel(
                GetYourPriceViewModel(),
                Upgrades_Resources.FixNFibrePlusModalProductName,
                Upgrades_Resources.FixNFibrePlusModalTagline,
                fibrePlusHeader,
                Upgrades_Resources.FixNFibrePlusTagline,
                upgrade.TariffId,
                nextButtonAction,
                upgrade.BundlePackageType,
                upgrade.BundlePackage.MonthlyDiscountedCost,
                upgrade.BundlePackage.MonthlyOriginalCost,
                isAdded,
                WebClientData.BaseUrl,
                isAdded ? Upgrades_Resources.FixNFibrePlusSavingsPara_PlusPackageAdded : Upgrades_Resources.FixNFibrePlusSavingsCalculatorPara1,
                energyCustomer.Postcode,
                broadbandPackageSpeedViewModel)
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = energyCustomer.HasSelectedBroadbandProduct() && energyCustomer.HasConfirmedNonMatchingBTAddress ?
                        "ConfirmAddress" :
                        "AvailableTariffs",
                    ControllerName = energyCustomer.HasSelectedBroadbandProduct() && energyCustomer.HasConfirmedNonMatchingBTAddress ?
                        "SignUp" :
                        "Tariffs",
                    TitleAttributeText = Resources.BackButtonAlt
                }
            };

            return viewModel;
        }

        public void SetPhonePackageInformation(string talkCode, bool setToDefault)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            List<TalkProduct> talkProducts = energyCustomer.SelectedBroadbandProduct.TalkProducts
                .OrderBy(prod => prod.Prices.FirstOrDefault(p => p.FeatureCode == FeatureCodes.MonthlyTalkCharge)?.Price ?? 0).ToList();

            energyCustomer.SelectedBroadbandProductCode = setToDefault
                ? talkProducts.FirstOrDefault()?.ProductCode
                : talkProducts.FirstOrDefault(p => p.ProductCode == talkCode)?.ProductCode;

            UpdateYourPriceViewModel(energyCustomer);
            SaveEnergyCustomerInSession(energyCustomer);
        }

        public void UpdateCustomerKeepYourNumber(KeepYourNumberViewModel viewModel)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();

            energyCustomer.CLIChoice.KeepExisting = viewModel.KeepExistingNumber;
            energyCustomer.CLIChoice.UserProvidedCLI = viewModel.CLI;

            SaveEnergyCustomerInSession(energyCustomer);
        }

        public async Task<ConfirmAddressViewModel> GetConfirmAddressViewModel()
        {
            var openReachAddressList = SessionManager.GetSessionDetails<List<BTAddress>>(SessionKeys.BTAddressListForPostCode);
            if (openReachAddressList == null)
            {
                var energyCustomer = SessionManager.GetSessionDetails<EnergyCustomer>(SessionKeys.EnergyCustomer);
                Guard.Against<ArgumentException>(energyCustomer == null, $"{nameof(energyCustomer)} is null");

                // ReSharper disable once PossibleNullReferenceException
                openReachAddressList = await _broadbandProductsService.GetAddressesForPostcode(energyCustomer.Postcode);
            }

            return new ConfirmAddressViewModel
            {
                BackChevronViewModel = new BackChevronViewModel
                {
                    ActionName = "AvailableTariffs",
                    ControllerName = "Tariffs",
                    TitleAttributeText = Resources.BackButtonAlt
                },
                Addresses = BTMapper.MapBTAddressListToBTAddressViewModelList(openReachAddressList)
            };
        }

        public void AddExtra(string productCode)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Extra selectedExtra = energyCustomer.SelectedTariff.Extras.FirstOrDefault(e => e.ProductCode == productCode);
            if (null != selectedExtra)
            {
                energyCustomer.SelectedExtras.Add(selectedExtra);
                UpdateYourPriceViewModel(energyCustomer);
            }
        }

        public void RemoveExtra(string productCode)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            energyCustomer.SelectedExtras.RemoveWhere(e => e.ProductCode.Equals(productCode));
            UpdateYourPriceViewModel(energyCustomer);
        }

        private void SetBroadbandProduct()
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                BroadbandProductGroup selectedBroadbandProductGroup = _broadbandManager.BroadbandProductGroup(energyCustomer.SelectedBroadbandProductCode);
                SetDefaultBroadbandProduct(selectedBroadbandProductGroup);
            }
        }

        public void AddBundleUpgrade(string productCode)
        {
            SetSelectedTariff(productCode, _tariffMapper);
            SetBroadbandProduct();
        }

        public void RemoveBundleUpgrade(string selectedTariffId)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            Tariff selectedTariff = energyCustomer.SelectedTariff;
            var tariffs = SessionManager.GetSessionDetails<List<Tariff>>(SessionKeys.AvailableEnergyTariffs);
            Tariff downgrade = tariffs.SingleOrDefault(t => !t.TariffId.Equals(selectedTariffId)
                                                            && !t.IsUpgrade
                                                            && t.BundlePackageType == selectedTariff.BundlePackageType);
            SetSelectedTariff(downgrade?.TariffId, _tariffMapper);
            SetAvailableBundleUpgrade();
            SetBroadbandProduct();
            UpdateYourPriceViewModel(energyCustomer);
        }

        public void SetSelectedBTAddress(ConfirmAddressViewModel viewModel, IEnumerable<BTAddressViewModel> addresses)
        {
            EnergyCustomer energyCustomer = GetEnergyCustomerFromSession();
            BTAddressViewModel selectedAddress = addresses.FirstOrDefault(x => viewModel.SelectedAddressId == x.Id);
            energyCustomer.SelectedBTAddress = BTMapper.MapBTAddressViewModelToBTAddress(selectedAddress);
        }

        private BroadbandMoreInformationViewModel SummaryViewBroadbandMoreInformationViewModel(Tariff tariff, EnergyCustomer energyCustomer)
        {
            string originalBroadbandMonthlyCost = (tariff.BundlePackage?.MonthlyOriginalCost ?? 0.0).ToCurrency();
            string projectedBroadbandMonthlySavings = (tariff.BundlePackage?.MonthlySavings ?? 0.00).ToCurrency();
            string bundleBroadbandProjectedYearlySavings = tariff.BundlePackage?.YearlySavings.ToCurrency();

            var viewModel = new BroadbandMoreInformationViewModel
            {
                ProjectedBroadbandMonthlyCost = (tariff.BundlePackage?.MonthlyDiscountedCost ?? 0.0).ToCurrency(),
                OriginalBroadbandMonthlyCost = (tariff.BundlePackage?.MonthlyOriginalCost ?? 0.0).ToCurrency(),
                ProjectedBroadbandMonthlySavingsAmount = projectedBroadbandMonthlySavings,
                BundleDisclaimerModalText = string.Format(
                    AvailableBundleTariffs_Resources.BundleDisclaimerModalText
                    , projectedBroadbandMonthlySavings
                    , bundleBroadbandProjectedYearlySavings
                    , projectedBroadbandMonthlySavings
                    , projectedBroadbandMonthlySavings
                    , bundleBroadbandProjectedYearlySavings
                    , originalBroadbandMonthlyCost
                ),
                BroadbandPackageSpeed = GetBroadbandPackageSpeedViewModel(energyCustomer.SelectedBroadbandProduct, energyCustomer.Postcode, true, true)
            };

            viewModel.BroadbandPackageSpeed.PackageDescription = AvailableBundleTariffs_Resources.FixNFibrePackageDescription;
            viewModel.BroadbandPackageSpeed.ShowPostcodeText = false;
            return viewModel;
        }

        private static string BundleDisclaimerText(EnergyCustomer energyCustomer)
        {
            Tariff tariff = energyCustomer.SelectedTariff;
            string bundleDisclaimerText = string.Empty;

            if (tariff.IsBundle && tariff.IsBroadbandBundle())
            {
                bundleDisclaimerText = GetBroadbandBundleDisclaimerText(energyCustomer);
            }
            else if (tariff.IsBundle && tariff.IsHesBundle())
            {
                bundleDisclaimerText = GetHesBundleDisclaimerText(energyCustomer);
            }

            return bundleDisclaimerText;
        }

        private static string GetEnergyDisclaimerText(EnergyCustomer energyCustomer)
        {
            Tariff tariff = energyCustomer.SelectedTariff;

            string disclaimerText = string.Empty;
            if (energyCustomer.IsDualFuel())
            {
                disclaimerText = string.Format(Summary_Resources.EnergyDisclaimerTextDual,
                    tariff.GetProjectedYearlyEnergyCost().ToCurrency(),
                    tariff.GetProjectedYearlyGasCost()?.ToCurrency(),
                    tariff.GetProjectedYearlyElectricityCost()?.ToCurrency()
                );
            }
            else if (energyCustomer.IsGasOnly())
            {
                disclaimerText = string.Format(Summary_Resources.EnergyDisclaimerTextGasOnly, tariff.GetProjectedYearlyGasCost()?.ToCurrency());
            }
            else if (energyCustomer.IsElectricityOnly())
            {
                disclaimerText = string.Format(Summary_Resources.EnergyDisclaimerTextElectricityOnly, tariff.GetProjectedYearlyElectricityCost()?.ToCurrency());
            }

            string vatDisclaimer = energyCustomer.IsDirectDebit()
                ? string.Format(Summary_Resources.VATDisclaimerDirectDebit, energyCustomer.SelectedTariff.GetTotalDirectDebitDiscount().ToString("C"))
                : Summary_Resources.VATDisclaimer;

            return $"{disclaimerText} {vatDisclaimer}";
        }

        private static string GetHesBundleDisclaimerText(EnergyCustomer energyCustomer)
        {
            Tariff tariff = energyCustomer.SelectedTariff;
            string projectedBundleMonthlySavings = (tariff.BundlePackage?.MonthlySavings ?? 0.00).ToCurrency();
            string projectedYearlyEnergyCost = tariff.GetProjectedYearlyEnergyCost().ToCurrency();
            string projectedGasYearlyCost = tariff.GetProjectedYearlyGasCost()?.ToCurrency();
            string projectedElectricityYearlyCost = tariff.GetProjectedYearlyElectricityCost()?.ToCurrency();
            string bundleProjectedYearlySavings = tariff.BundlePackage?.YearlySavings.ToCurrency();

            string disclaimer1;
            if (!energyCustomer.SelectedTariff.IsUpgrade)
            {
                disclaimer1 = string.Format(Summary_Resources.HesDisclaimerText1,
                projectedBundleMonthlySavings,
                bundleProjectedYearlySavings,
                projectedBundleMonthlySavings,
                bundleProjectedYearlySavings);
            }
            else
            {
                disclaimer1 = string.Format(Summary_Resources.HesPlusDisclaimerText1,
                    projectedBundleMonthlySavings,
                    bundleProjectedYearlySavings,
                    tariff.BundlePackage?.MonthlyOriginalCost.ToCurrency());
            }

            if (energyCustomer.IsDualFuel())
            {
                disclaimer1 += string.Format(Summary_Resources.HesDisclaimerTextDualFuel,
                    projectedYearlyEnergyCost,
                    projectedGasYearlyCost,
                    projectedElectricityYearlyCost);
            }
            else if (energyCustomer.IsGasOnly())
            {
                disclaimer1 += string.Format(Summary_Resources.HesDisclaimerTextSingleFuel, projectedGasYearlyCost);
            }

            return disclaimer1;
        }

        private BroadbandPackageSpeedViewModel GetBroadbandPackageSpeedViewModel(
            BroadbandProduct selectedBroadbandProduct,
            string postcode,
            bool showHeaderText,
            bool showPostcode)
        {
            var viewModel = new BroadbandPackageSpeedViewModel
            {
                PostCode = postcode,
                ShowUploadSpeed = true,
                MaximumSpeedCap = GetSpeedCap(),
                ShowPostcodeText = showPostcode,
                ShowSpeedRangeText = true,
                ShowHeaderText = showHeaderText,
                HeaderText = string.Format(PhonePackage_Resources.BroadbandSpeedHeaderText, postcode)
            };

            BroadbandProductExtensions.SetLineSpeedCap(selectedBroadbandProduct);
            var fibreLineSpeed = (FibreLineSpeeds)selectedBroadbandProduct.LineSpeed;

            viewModel.MaxDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed?.MaxDownload);
            viewModel.MinDownload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed?.MinDownload);
            viewModel.MaxUpload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed?.MaxUpload);
            viewModel.MinUpload = BroadbandProductExtensions.GetFormattedLineSpeeds(fibreLineSpeed?.MinUpload);
            viewModel.PackageDescription = selectedBroadbandProduct.BroadbandType.GetModalContent();

            return viewModel;
        }

        private void UpdateYourPriceViewModel(EnergyCustomer energyCustomer)
        {
            YourPriceViewModel yourPriceViewModel = _tariffMapper.GetYourPriceViewModel(energyCustomer, WebClientData.BaseUrl);
            SessionManager.SetSessionDetails(SessionKeys.EnergyYourPriceDetails, yourPriceViewModel);
        }

        private bool DisplaySelectedBroadbandProduct(EnergyCustomer energyCustomer)
        {
            string notDisplayedBroadbandCode = ConfigManager.GetValueForKeyFromSection("bundleManagement", "BroadbandUpgradesNotToBeDisplayedInBasket",
                energyCustomer.SelectedBroadbandProductCode);
            return string.IsNullOrEmpty(notDisplayedBroadbandCode) && energyCustomer.SelectedTariff.IsBroadbandBundle();
        }

        private int GetSpeedCap()
        {
            string cap = ConfigManager.GetAppSetting("FibrePlusSpeedCap");
            int.TryParse(cap.Substring(0, 2), out int speedCap);
            return speedCap;
        }

        private static string GetTillInformationHeader(FuelType fuelType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (fuelType)
            {
                case FuelType.Dual:
                    return Summary_Resources.TillInformationHeaderDual;
                case FuelType.Electricity:
                    return Summary_Resources.TillInformationHeaderElectricity;
                case FuelType.Gas:
                    return Summary_Resources.TillInformationHeaderGas;
                default:
                    return string.Empty;
            }
        }

        private static string GetTermsAndConditionsText(BundlePackageType bundlePackageType, string bundleName, string coverName, string policyBookletName)
        {
            string retVal;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (bundlePackageType)
            {
                case BundlePackageType.FixAndProtect:
                    retVal = string.Format(Form_Resources.FixAndProtectTermsAndConditionsCheckboxEnergyLabelText, bundleName, coverName, policyBookletName);
                    break;
                case BundlePackageType.FixAndFibre:
                    retVal = Form_Resources.TermsAndConditionsCheckboxBroadbandBundleLabelText;
                    break;
                default:
                    retVal = Form_Resources.TermsAndConditionsCheckboxEnergyLabelText;
                    break;
            }

            return retVal;
        }

        private static string GetBroadbandBundleDisclaimerText(EnergyCustomer energyCustomer)
        {
            const string energy = "energy";
            const string electricity = "electricity";
            const string gas = "gas";
            Tariff tariff = energyCustomer.SelectedTariff;
            string projectedBroadbandMonthlySavings = (tariff.BundlePackage?.MonthlySavings ?? 0.00).ToCurrency();
            string projectedYearlyEnergyCost = tariff.GetProjectedYearlyEnergyCost().ToCurrency();
            string projectedGasYearlyCost = tariff.GetProjectedYearlyGasCost()?.ToCurrency();
            string projectedElectricityYearlyCost = tariff.GetProjectedYearlyElectricityCost()?.ToCurrency();
            string bundleBroadbandProjectedYearlySavings = tariff.BundlePackage?.YearlySavings.ToCurrency();

            string bundleDisclaimer1;
            if (!energyCustomer.SelectedTariff.IsUpgrade)
            {
                bundleDisclaimer1 = string.Format(Summary_Resources.BundleDisclaimer_Broadband
                    , projectedBroadbandMonthlySavings
                    , bundleBroadbandProjectedYearlySavings
                    , projectedBroadbandMonthlySavings
                    , projectedBroadbandMonthlySavings
                    , bundleBroadbandProjectedYearlySavings);
            }
            else
            {
                bundleDisclaimer1 = string.Format(Summary_Resources.BundleDisclaimer_BroadbandPlus
                    , projectedBroadbandMonthlySavings
                    , bundleBroadbandProjectedYearlySavings);
            }

            string energyDescription = energyCustomer.IsDualFuel() ? energy : energyCustomer.IsElectricityOnly() ? electricity : gas;

            var bundleDisclaimer2Builder = new StringBuilder();
            bundleDisclaimer2Builder.Append(string.Format(Summary_Resources.BundleDisclaimer_Annual, energyDescription, projectedYearlyEnergyCost));
            if (energyCustomer.IsDualFuel())
            {
                bundleDisclaimer2Builder.Append(string.Format(Summary_Resources.BundleDisclaimer_DualFuel, projectedGasYearlyCost,
                    projectedElectricityYearlyCost));
            }

            return $"{bundleDisclaimer1}{bundleDisclaimer2Builder}";
        }

        private async Task<bool> CreateOnlineProfile(EnergyCustomer energyCustomer, string password)
        {
            Guard.Against<Exception>(energyCustomer?.ContactDetails == null, $"{nameof(energyCustomer.ContactDetails)} is null");
            Guard.Against<Exception>(energyCustomer?.PersonalDetails == null, $"{nameof(energyCustomer.PersonalDetails)} is null");

            // ReSharper disable once PossibleNullReferenceException
            Guid profileId = await _customerProfileService.GetProfileIdByEmail(energyCustomer?.ContactDetails.EmailAddress);
            if (profileId == Guid.Empty)
            {
                UserProfile customerProfile = OnlineAccountMapper.GetUserProfile(energyCustomer, password);
                profileId = await _customerProfileService.AddOnlineProfile(customerProfile);
                // ReSharper disable once PossibleNullReferenceException
                energyCustomer.UserId = profileId;
            }

            return profileId != Guid.Empty;
        }
    }
}