namespace Products.Service.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Logging;
    using Model.Configuration;
    using Products.Model.TariffChange.Customers;
    using Products.Repository.Common;
    using Products.Service.TariffChange.Mappers;
    using Repository;
    using Repository.EmailTemplates;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.ManageCustomerInformationService;
    using ServiceWrapper.PersonalProjectionService;
    using TariffChange;
    using TariffChange.Validators;
    using WebModel.Resources.TariffChange;

    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly IAnnualEnergyReviewServiceWrapper _annualEnergyReviewServiceWrapper;
        private readonly ICheckDigitValidator _checkDigitValidator;
        private readonly IEmailManager _emailManager;
        private readonly IContentRepository _contentRepository;
        private readonly IPersonalProjectionServiceWrapper _personalProjectionServiceWrapper;
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger _logger;
        private readonly IJourneyDetailsService _journeyDetailsService;
        private readonly IUtilityService _utilityService;
        private readonly IManageCustomerInformationServiceWrapper _customerInformationServiceWrapper;
        private readonly IConfigManager _configManager;

        public CustomerAccountService(IAnnualEnergyReviewServiceWrapper annualEnergyReviewServiceWrapper, ICheckDigitValidator checkDigitValidator, IEmailManager emailManager, IContentRepository contentRepository, IPersonalProjectionServiceWrapper personalProjectionServiceWrapper,
            IProfileRepository profileRepository, ILogger logger, IJourneyDetailsService journeyDetailsService, IUtilityService utilityService, IManageCustomerInformationServiceWrapper customerInformationServiceWrapper, IConfigManager configManager)
        {
            _annualEnergyReviewServiceWrapper = annualEnergyReviewServiceWrapper;
            _checkDigitValidator = checkDigitValidator;
            _emailManager = emailManager;
            _contentRepository = contentRepository;
            _personalProjectionServiceWrapper = personalProjectionServiceWrapper;
            _profileRepository = profileRepository;
            _logger = logger;
            _journeyDetailsService = journeyDetailsService;
            _utilityService = utilityService;
            _customerInformationServiceWrapper = customerInformationServiceWrapper;
            _configManager = configManager;
        }

        public CustomerAccount GetCustomerAccount(string accountNumber)
        {
            if (!_checkDigitValidator.IsValid(accountNumber))
            {
                return new CustomerAccount
                {
                    SiteDetails = new SiteDetails()
                };
            }

            return CustomerAccountModelMapper.MapCustomerAccountsResponseToCustomerAccount(_customerInformationServiceWrapper.GetCustomerAccounts(new[] { accountNumber }), accountNumber);
        }

        public List<CustomerAccount> GetCustomerAccount(IList<string> accountNumbers)
        {
            var validCustomerAccountNumbers = new string[10];
            var customerAccounts = new List<CustomerAccount>();

            int counter = 0;
            foreach (string accountNumber in accountNumbers)
            {
                if (counter <= 9)
                {
                    if (_checkDigitValidator.IsValid(accountNumber))
                    {
                        validCustomerAccountNumbers[counter] = accountNumber;
                        counter++;
                    }
                }
            }

            GetCustomerAccountsResponse response = _customerInformationServiceWrapper.GetCustomerAccounts(validCustomerAccountNumbers);

            foreach (CustomerAccountsType customerAccount in response.CustomerAccounts)
            {
                if (customerAccount.CustomerAccountStatus == CustomerAccountStatusType.Found || customerAccount.CustomerAccountStatus == CustomerAccountStatusType.MultipleServices)
                {
                    customerAccounts.Add(CustomerAccountModelMapper.MapServiceCustomerAccountToCustomerAccount(customerAccount));
                }
            }

            return customerAccounts;
        }

        public string[] ActionTariffChange(List<CustomerAccount> customerAccounts, PersonalProjectionDetails personalProjectionDetails, string emailAddress)
        {
            var aerRequestDetails = new List<aerRequestDetailsType>();

            foreach (CustomerAccount customerAccount in customerAccounts)
            {
                aerRequestDetails.Add(CreateRequestDetails(personalProjectionDetails, customerAccount, emailAddress));
            }

            var actionAerRequest = new actionAERRequest
            {
                csrUserID = "ZZ00000",
                aerRequestCollection = aerRequestDetails.ToArray()
            };
            actionAERResponse actionAerResponse = _annualEnergyReviewServiceWrapper.actionAER(actionAerRequest);

            return actionAerResponse.customerAccountCollection;
        }

        public void SendConfirmationEmail(ConfirmationEmailParameters emailParameters)
        {
            EmailTemplate emailTemplate = _contentRepository.GetEmailTemplate("ConfirmationEmailTemplate");
            string effectiveDateString = emailParameters.EffectiveDate.ToSseString();
            if (emailParameters.EffectiveDate > DateTime.Today)
            {
                effectiveDateString = emailParameters.EffectiveDate.ToSseString() + " when your current tariff ends";
            }

            var cdnBaseUrl = _configManager.GetConfigSectionGroup<WebClientConfigurationSection>("webClientConfiguration");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(cdnBaseUrl.BaseUrl), "Web client configuration baseUrl setting is missing in web.config.");

            string emailBody = emailTemplate.Body.ToString()
                .Replace("[$Domain]", ConfigurationManager.AppSettings["PreLoginDomain"] ?? string.Empty)
                .Replace("[$BaseUrl]", _utilityService.GetBaseUrl())
                .Replace("[$CDNBaseUrl]", cdnBaseUrl.BaseUrl)
                .Replace("[$Surname]", emailParameters.AccountHolderName)
                .Replace("[$Tariff]", emailParameters.TariffName)
                .Replace("[$EffectiveDate]", effectiveDateString)
                .Replace("[$RedeemBroadband]", GetRedeemBroadbandContent(emailParameters.IncludeBroadbandRedeemContent));

            _emailManager.Send(ConfigurationManager.AppSettings["EmailFromAddress"], emailParameters.EmailAddress, emailTemplate.Subject, emailBody, true);
        }

        public void AddPersonalProjection(PersonalProjectionDetails personalProjectionDetails)
        {
            var request = new SiteProjectionRequest
            {
                Site = new SiteType
                {
                    SiteId = personalProjectionDetails.SiteId.ToString(),
                    SiteConsumption = new SiteConsumptionType
                    {
                        ElectricitySpend = $"{Math.Round(personalProjectionDetails.ElectricitySpend, 2)}",
                        ElectricityConsumption = $"{(int)personalProjectionDetails.ElectricityUsage}",
                        GasSpend = $"{Math.Round(personalProjectionDetails.GasSpend, 2)}",
                        GasConsumption = $"{(int)personalProjectionDetails.GasUsage}"
                    }
                }
            };

            _personalProjectionServiceWrapper.SiteProjection(request);
        }

        public bool UpdateCustomerTariffSelection()
        {
            Customer customer = _journeyDetailsService.GetCustomer();

            try
            {
                ActionTariffChange(customer.IterableAccounts(), customer.GetPersonalProjectionDetails(), customer.EmailAddress);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured attempting to call actionAER method for customer tariff change, Accounts = {string.Join(", ", customer.CustomerAccountNumbers())}.", ex);
            }

            try
            {
                AddPersonalProjection(customer.GetPersonalProjectionDetails());

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occured attempting to call personal projection service for customer tariff change, Accounts = {string.Join(", ", customer.CustomerAccountNumbers())}.", ex);
            }

            try
            {
                SendConfirmationEmail(customer.GetConfirmationEmailParameters());
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured attempting to send confirmation email for customer tariff change, Accounts = {string.Join(", ", customer.CustomerAccountNumbers())}. {ex.Message}", ex);
            }

            return true;
        }

        public async Task<List<string>> GetUserAccountsByLoginName(string emailAddress)
        {
            return await _profileRepository.GetUserAccountsByLoginNameAsync(emailAddress);
        }

        private aerRequestDetailsType CreateRequestDetails(PersonalProjectionDetails personalProjectionDetails, CustomerAccount customerAccount, string emailAddress)
        {
            DateTime effectiveDate = customerAccount.SelectedTariff.EffectiveDate;
            bool isRenewal = effectiveDate > DateTime.Today;

            var reviewNotes = new StringBuilder(DateTime.Today.ToShortDateString());

            if (isRenewal)
            {
                reviewNotes.Append($" Renewal {effectiveDate.ToShortDateString()}");
            }

            reviewNotes.Append($", Elec spend = £{Math.Round(personalProjectionDetails.ElectricitySpend, 2)}, Elec usage = {(int)personalProjectionDetails.ElectricityUsage}, Gas spend = £{Math.Round(personalProjectionDetails.GasSpend, 2)}, Gas usage {(int)personalProjectionDetails.GasUsage}");

            reviewNotes.Append($", {emailAddress}");

            var requestDetails = new aerRequestDetailsType
            {
                customerAccountNumber = customerAccount.SiteDetails.AccountNumber,
                actionsToTake = new actionsToTakeType
                {
                    changeAccountBenefit = false,
                    changeDDStatus = false,
                    changePaperlessStatus = false,
                    changeTariff = true,
                    submitMeterReading = false,
                    tookEfficiencySurvey = false
                },
                actionDetails = new actionDetailsType
                {
                    tariffDetails = new tariffDetailsType
                    {
                        toServicePlanId = customerAccount.SelectedTariff.ServicePlanId,
                        followOnTariffFlag = false
                    },
                    outcome = new outcomeType
                    {
                        nextAerDateSpecified = true,
                        nextAerDate = DateTime.MaxValue,
                        outcomeNotes = reviewNotes.ToString()
                    }
                }
            };
            /* KWH 13 Dec 2017 - Renewals automation is not working in CS so all renewals are 
            treated as service plan change with followOnTariffFlag set to false until the issue with CS is fixed */

            if (customerAccount.PaymentDetails.IsMonthlyDirectDebit)
            {
                requestDetails.actionsToTake.changeDDStatus = true;
                requestDetails.actionDetails.paymentDetails = new paymentDetailsType
                {
                    accountInNameOf = customerAccount.PaymentDetails.BankAccountName,
                    bankAccountNumber = customerAccount.PaymentDetails.BankAccountNumber,
                    collectionDay = customerAccount.PaymentDetails.DirectDebitDay,
                    regularAmountDD = (decimal)customerAccount.PaymentDetails.DirectDebitAmount,
                    sortCodeNumber = customerAccount.PaymentDetails.BankSortCode
                };
            }

            return requestDetails;
        }

        private string GetRedeemBroadbandContent(bool includeBroadbandRedeemContent)
        {
            string redeemSection = string.Empty;
            if (includeBroadbandRedeemContent)
            {
                redeemSection = _contentRepository.GetEmailComponent("RedeemBroadband").Content.ToString();
                redeemSection = redeemSection.Replace("[$RedeemBroadbandHRef]", ConfigurationManager.AppSettings["CTCTelcoRedirectLink"]);
                redeemSection = redeemSection.Replace("[$RedeemBroadbandTitle]", Confirmation_Resources.TelcoLink);
                redeemSection = redeemSection.Replace("[$RedeemBroadbandText]", Confirmation_Resources.TelcoLink);
                redeemSection = redeemSection.Replace("[$RedeemBroadbandAlt]", Confirmation_Resources.TelcoLink);
            }

            return redeemSection;
        }
    }
}