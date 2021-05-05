namespace Products.Service.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Broadband.Managers;
    using Broadband.Mappers;
    using Common;
    using Common.Managers;
    using Core;
    using Core.Enums;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Mappers;
    using Model.Broadband;
    using Model.Common;
    using Model.Energy;
    using Model.Enums;
    using Model.HomeServices;
    using Repository.Broadband;
    using Repository.Common;
    using Repository.Energy;
    using Repository.HomeServices;
    using Security;
    using ApplicationData = Model.Common.ApplicationData;
    using Common_Resources = WebModel.Resources.Energy.Common_Resources;
    using HesApplicationData = Model.HomeServices.ApplicationData;

    public class SummaryService : ISummaryService
    {
        private readonly IBroadbandManager _broadbandManager;
        private readonly IBroadbandSalesRepository _broadbandSalesRepository;
        private readonly ICampaignManager _campaignManager;
        private readonly IConfirmationEmailService _confirmationEmailService;
        private readonly IContextManager _contextManager;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEnergyApplicationDataMapper _energyApplicationDataMapper;
        private readonly IEnergySalesRepository _energySalesRepository;
        private readonly IHomeServicesSalesRepository _homeServicesSalesRepository;
        private readonly ILogger _logger;
        private readonly string _membershipEmailAddress;
        private readonly IMembershipEmailService _membershipEmailService;
        private readonly ISalesRepository _salesRepository;
        private readonly string _promoCodes;

        public SummaryService(
            IConfirmationEmailService confirmationEmailService,
            ILogger logger,
            IEnergySalesRepository energySalesRepository,
            ISalesRepository salesRepository,
            IEnergyApplicationDataMapper energyApplicationDataMapper,
            IContextManager contextManager,
            ICampaignManager campaignManager,
            IBroadbandSalesRepository broadbandSalesRepository,
            IBroadbandManager broadbandManager,
            IMembershipEmailService membershipEmailService,
            IConfigManager configManager,
            IHomeServicesSalesRepository homeServicesSalesRepository,
            ICryptographyService cryptographyService
        )
        {
            Guard.Against<ArgumentException>(confirmationEmailService == null, $"{nameof(confirmationEmailService)} is null.");
            Guard.Against<ArgumentException>(logger == null, $"{nameof(logger)} is null.");
            Guard.Against<ArgumentException>(energySalesRepository == null, $"{nameof(energySalesRepository)} is null");
            Guard.Against<ArgumentException>(salesRepository == null, $"{nameof(salesRepository)} is null");
            Guard.Against<ArgumentException>(energyApplicationDataMapper == null, $"{nameof(energyApplicationDataMapper)} is null");
            Guard.Against<ArgumentException>(contextManager == null, $"{nameof(contextManager)} is null");
            Guard.Against<ArgumentException>(campaignManager == null, $"{nameof(campaignManager)} is null");
            Guard.Against<ArgumentException>(broadbandSalesRepository == null, $"{nameof(broadbandSalesRepository)} is null");
            Guard.Against<ArgumentException>(broadbandManager == null, $"{nameof(broadbandManager)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            Guard.Against<ArgumentException>(homeServicesSalesRepository == null, $"{nameof(homeServicesSalesRepository)} is null");

            _confirmationEmailService = confirmationEmailService;
            _logger = logger;
            _energySalesRepository = energySalesRepository;
            _salesRepository = salesRepository;
            _energyApplicationDataMapper = energyApplicationDataMapper;
            _contextManager = contextManager;
            _campaignManager = campaignManager;
            _broadbandSalesRepository = broadbandSalesRepository;
            _broadbandManager = broadbandManager;
            _membershipEmailService = membershipEmailService;
            _promoCodes = configManager?.GetAppSetting("PromoCodes");
            _membershipEmailAddress = configManager?.GetAppSetting("MembershipEmailTo");
            _homeServicesSalesRepository = homeServicesSalesRepository;
            _cryptographyService = cryptographyService;
        }

        public async Task ConfirmSale(EnergyCustomer energyCustomer, OpenReachData openReachResponse = null)
        {
            Guard.Against<ArgumentException>(energyCustomer == null, $"{nameof(energyCustomer)} is null");
            // ReSharper disable once PossibleNullReferenceException
            Guard.Against<ArgumentException>(energyCustomer.SelectedTariff == null, $"{nameof(energyCustomer.SelectedTariff)} is null");
            Guard.Against<ArgumentException>(energyCustomer.PersonalDetails == null, $"{nameof(energyCustomer.PersonalDetails)} is null");
            Guard.Against<ArgumentException>(energyCustomer.IsDirectDebit() && energyCustomer.DirectDebitDetails == null, $"{nameof(energyCustomer.DirectDebitDetails)} is null");

            int baseProductId = GetBaseProductId(energyCustomer.SelectedFuelType, energyCustomer.SelectedElectricityMeterType);

            string tariffName = energyCustomer.SelectedTariff.IsBundle
                ? energyCustomer.SelectedTariff.BackendTariffName
                : energyCustomer.SelectedTariff.DisplayName;

            int subProductId = await _energySalesRepository.GetSubProductIdForFuelType(tariffName, baseProductId, energyCustomer.IsPrePay());

            if (subProductId == 0)
            {
                throw new ArgumentException($"Invalid sub product Id for - Tariff Name : {tariffName}, Base product Id : {baseProductId}, IsprePay: {energyCustomer.IsPrePay()}");
            }

            string migrateCampaignid = _contextManager.HttpContext.Request.Cookies["migrateCampaignid"]?.Value;
            energyCustomer.MigrateMemberId = _contextManager.HttpContext.Request.Cookies["migrateMemberid"]?.Value;
            energyCustomer.MigrateAffiliateId = _contextManager.HttpContext.Request.Cookies["migrateAffiliateid"]?.Value;
            energyCustomer.CampaignCode = _campaignManager.GetCampaignCodesMapping(energyCustomer.MigrateAffiliateId, migrateCampaignid);

            ApplicationData applicationData = _energyApplicationDataMapper.GetEnergyApplicationDataModel(energyCustomer, subProductId);
            energyCustomer.EnergyApplicationId = await _salesRepository.SaveApplication(applicationData);

            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                // Broadband sales
                ApplicationData broadbandApplication = UpdateApplicationDataForBroadband(applicationData, energyCustomer);
                energyCustomer.BroadbandApplicationId = await _broadbandSalesRepository.SaveApplication(broadbandApplication);
            }

            try
            {
                // ReSharper disable once PossibleInvalidOperationException
                await _salesRepository.InsertMasusReference(energyCustomer.EnergyApplicationId, energyCustomer.ContactDetails.EmailAddress, energyCustomer.UserId.Value);

                if (energyCustomer.SelectedTariff.IsBroadbandBundle())
                {
                    await _broadbandSalesRepository.InsertMasusReference(energyCustomer.BroadbandApplicationId, applicationData.Email, applicationData.UserID);
                }
            }

            catch (Exception ex)
            {
                _logger.Error($"Exception occured while inserting masus reference. {ex.Message}", ex);
            }
            
            if (energyCustomer.IsElectricalWiringSelected())
            {
                try
                {
                    HesApplicationData hesApplicationData = GetHesApplicationData(energyCustomer);
                    await _homeServicesSalesRepository.SaveApplication(hesApplicationData);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception occured while inserting Hes application. {ex.Message}", ex);
                }
            }
            
            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (openReachResponse.LineavailabilityFlags.BackOfficeFile)
                    {
                        await _broadbandSalesRepository.InsertOpenReachAudit(ApplicationAuditMapper.GetOpenReachAuditData(energyCustomer, openReachResponse));
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception occured while inserting the openreach audit record for back office file Application Id - {energyCustomer.BroadbandApplicationId}. {ex.Message}", ex);
                }

                try
                {
                    ApplicationAudit auditData = GetApplicationAuditDataForBundle(energyCustomer.BroadbandApplicationId, energyCustomer);
                    await _broadbandSalesRepository.InsertApplicationAudit(auditData);
                    await _broadbandSalesRepository.SetLastUpdated(applicationData.UserID);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Exception occured while inserting the audit record. {ex.Message}", ex);
                }
            }

            try
            {
                var emailParameters = new ConfirmationEmailParameters
                {
                    EmailAddress = energyCustomer.ContactDetails.EmailAddress,
                    FirstName = energyCustomer.PersonalDetails.FirstName.ToCapitalLetter(),
                    SelectedProductTitle = energyCustomer.SelectedTariff.DisplayName,
                    FixNProtectCoverName = string.Empty,
                    FixNFibreHeader = string.Empty
                };
                if (energyCustomer.SelectedTariff.IsHesBundle())
                {
                    emailParameters.FixNProtectCoverName = energyCustomer.SelectedTariff.IsUpgrade
                        ? Common_Resources.FixNProtectPlusCoverName
                        : Common_Resources.FixNProtectCoverName;
                }

                if (energyCustomer.SelectedTariff.IsBroadbandBundle())
                {
                    emailParameters.FixNFibreHeader = energyCustomer.SelectedTariff.IsUpgrade
                        ? Common_Resources.FixNFibrePlusHeader
                        : Common_Resources.FixNFibreHeader;
                }

                EmailTemplateName emailTemplateName = energyCustomer.SelectedTariff.IsBundle
                    ? energyCustomer.SelectedTariff.IsBroadbandBundle()
                        ? EmailTemplateName.BundleBroadbandConfirmation
                        : EmailTemplateName.BundleHomeServicesConfirmation
                    : EmailTemplateName.EnergyConfirmation;

                await _confirmationEmailService.SendConfirmationEmail(emailParameters, emailTemplateName.ToString(), energyCustomer.SelectedExtras);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    $"Exception occured attempting to send confirmation email for energy customer, email: {energyCustomer.ContactDetails.EmailAddress}.", ex);
            }

            if (energyCustomer.SelectedTariff.IsBroadbandBundle())
            {
                try
                {
                    if (!string.IsNullOrEmpty(energyCustomer.MigrateMemberId))
                    {
                        _membershipEmailService.SendMembershipEmail(_membershipEmailAddress,
                            applicationData.CampaignCode,
                            energyCustomer.PersonalDetails.FormattedName,
                            energyCustomer.FullAddress(),
                            energyCustomer.ContactDetails.EmailAddress,
                            energyCustomer.ContactDetails.ContactNumber,
                            energyCustomer.MigrateMemberId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Exception occured attempting to membership email for broadband bundle customer, email: {energyCustomer.ContactDetails.EmailAddress}.",
                        ex);
                }
            }
        }

        private HesApplicationData GetHesApplicationData(EnergyCustomer energyCustomer)
        {
            Extra selectedElectricalWiring = energyCustomer.SelectedExtras.FirstOrDefault(e => e.Type == ExtraType.ElectricalWiring);
            var hesApplicationData = new HesApplicationData
            {
                Title = _cryptographyService.EncryptHomeServicesValue(energyCustomer.PersonalDetails.Title),
                Surname = energyCustomer.PersonalDetails.LastName,
                FirstName = _cryptographyService.EncryptHomeServicesValue(energyCustomer.PersonalDetails.FirstName),
                HouseNameNumber = _cryptographyService.EncryptHomeServicesValue(energyCustomer.SelectedAddress.HouseName),
                AddressLine1 = string.IsNullOrEmpty(energyCustomer.SelectedAddress.AddressLine1)
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(energyCustomer.SelectedAddress.AddressLine1),
                AddressLine2 = string.IsNullOrEmpty(energyCustomer.SelectedAddress.AddressLine2)
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(energyCustomer.SelectedAddress.AddressLine2),
                Town = string.IsNullOrEmpty(energyCustomer.SelectedAddress.Town)
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(energyCustomer.SelectedAddress.Town),
                County = string.IsNullOrEmpty(energyCustomer.SelectedAddress.County)
                    ? null
                    : _cryptographyService.EncryptHomeServicesValue(energyCustomer.SelectedAddress.County),
                Postcode = _cryptographyService.EncryptHomeServicesValue(energyCustomer.Postcode),
                DaytimePhoneNo = _cryptographyService.EncryptHomeServicesValue(energyCustomer.ContactDetails.ContactNumber),
                EmailAddress = _cryptographyService.EncryptHomeServicesValue(energyCustomer.ContactDetails.EmailAddress),
                BankName = _cryptographyService.EncryptHomeServicesValue(energyCustomer.DirectDebitDetails.BankName),
                AccountHolder = _cryptographyService.EncryptHomeServicesValue(energyCustomer.DirectDebitDetails.AccountName),
                SortCode = _cryptographyService.EncryptHomeServicesValue(energyCustomer.DirectDebitDetails.SortCode),
                AccountNo = _cryptographyService.EncryptHomeServicesValue(energyCustomer.DirectDebitDetails.AccountNumber),
                PaymentDay = energyCustomer.DirectDebitDetails.DirectDebitPaymentDate.ToString(),
                NoMarketing = energyCustomer.ContactDetails.MarketingConsent ? "N" : "Y",
                PromoCodes = _promoCodes,
                MobilePhoneNo = null,
                AccountNumber = null,
                IsSignupWithEnergy = false,
                Affiliate = energyCustomer.CampaignCode,
                ProductData = new List<ProductData> { new ProductData { Discount = 0, InitialCost = selectedElectricalWiring?.OriginalPrice ?? 0, TotalCost = selectedElectricalWiring?.BundlePrice ?? 0, Products = selectedElectricalWiring?.Id.ToString(CultureInfo.InvariantCulture) } }
            };

            return hesApplicationData;
        }

        private static ApplicationData UpdateApplicationDataForBroadband(ApplicationData applicationData, EnergyCustomer energyCustomer)
        {
            applicationData.DayPhone = energyCustomer.CLIChoice.FinalCLI;
            applicationData.Mobile = energyCustomer.ContactDetails.ContactNumber;
            applicationData.ProductCode = energyCustomer.SelectedBroadbandProductCode;
            applicationData.LineSpeed = 0;

            return applicationData;
        }

        private static string GetLineSpeed(BroadbandProduct broadbandProduct)
        {
            var speeds = new StringBuilder();

            var fibreLineSpeed = (FibreLineSpeeds)broadbandProduct.LineSpeed;
            speeds.Append($"Fibre Max Download: {fibreLineSpeed.MaxDownload}");
            speeds.Append($", Fibre Min Download: {fibreLineSpeed.MinDownload}");
            speeds.Append($", Fibre Max Upload: {fibreLineSpeed.MaxUpload}");
            speeds.Append($", Fibre Min Upload: {fibreLineSpeed.MinUpload}");

            return speeds.ToString();
        }

        private static int GetBaseProductId(FuelType selectedFuelType, ElectricityMeterType selectedElectricityMeterType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (selectedFuelType)
            {
                case FuelType.Dual:
                    return (int)BaseProductType.DualFuel;
                case FuelType.Gas:
                    return (int)BaseProductType.GasOnly;
                case FuelType.Electricity:
                    return selectedElectricityMeterType == ElectricityMeterType.Economy7
                        ? (int)BaseProductType.Economy7
                        : (int)BaseProductType.StandardElectricOnly;
                default:
                    return 0;
            }
        }

        private ApplicationAudit GetApplicationAuditDataForBundle(int applicationId, EnergyCustomer energyCustomer)
        {
            IEnumerable<string> productsOffered =
                energyCustomer.SelectedBroadbandProduct.TalkProducts.Select(t => $"{t.ProductCode}: {t.ProductName}").ToList();
            string productsOfferedDescription = productsOffered.Any() ? string.Join(", ", productsOffered) : "";
            double? broadbandWithTalk = energyCustomer.SelectedTariff.GetProjectedMonthlyBundleCost()
                                        + energyCustomer.SelectedBroadbandProduct
                                            ?.GetSelectedTalkProduct(energyCustomer.SelectedBroadbandProductCode)
                                            ?.GetPhoneCost();
            var applicationAudit = new ApplicationAudit
            {
                ApplicationId = applicationId,
                CLIProvided = !string.IsNullOrEmpty(energyCustomer.CLIChoice.FinalCLI),
                ProductCode = energyCustomer.SelectedBroadbandProductCode,
                ProductsOfferedDescription = productsOfferedDescription,
                LineSpeedsQuoted = GetLineSpeed(energyCustomer.SelectedBroadbandProduct),
                MonthlyDDPrice = broadbandWithTalk ?? 0.0,
                ConnectionCharge = _broadbandManager.GetConnectionFee(),
                InstallationCharge = energyCustomer.ApplyInstallationFee ? _broadbandManager.GetInstallationFee() : 0.0,
                MonthlySurchargeAmount = 0.0,
                IsSSECustomer = energyCustomer.IsSSECustomerCLI
            };

            return applicationAudit;
        }
    }
}