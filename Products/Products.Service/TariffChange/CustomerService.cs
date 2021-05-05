namespace Products.Service.TariffChange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure;
    using Mappers;
    using Model.Enums;
    using Products.Model.TariffChange.Customers;
    using Products.Model.TariffChange.Enums;
    using ServiceWrapper.AnnualEnergyReviewService;
    using ServiceWrapper.ManageCustomerInformationService;
    using Validators.Eligibility;

    public class CustomerService : ICustomerService
    {
        private readonly IAnnualEnergyReviewServiceWrapper _annualEnergyReviewServiceWrapper;
        private readonly IAccountEligibilityChecker _accountEligibilityChecker;
        private readonly IJourneyDetailsService _journeyDetailsService;
        private readonly IManageCustomerInformationServiceWrapper _customerInformationServiceWrapper;
        private readonly IConfigManager _configManager;

        public CustomerService(
            IAnnualEnergyReviewServiceWrapper annualEnergyReviewServiceWrapper,
            IAccountEligibilityChecker accountEligibilityChecker,
            IJourneyDetailsService journeyDetailsService,
            IManageCustomerInformationServiceWrapper customerInformationServiceWrapper,
            IConfigManager configManager)
        {
            Guard.Against<ArgumentNullException>(annualEnergyReviewServiceWrapper == null, "annualEnergyReviewServiceWrapper is null");
            Guard.Against<ArgumentNullException>(accountEligibilityChecker == null, "accountEligibilityChecker is null");
            Guard.Against<ArgumentNullException>(journeyDetailsService == null, "journeyDetailsService is null");
            Guard.Against<ArgumentNullException>(customerInformationServiceWrapper == null, "customerInformationServiceWrapper is null");
            Guard.Against<ArgumentNullException>(configManager == null, "configManager is null");

            _annualEnergyReviewServiceWrapper = annualEnergyReviewServiceWrapper;
            _accountEligibilityChecker = accountEligibilityChecker;
            _journeyDetailsService = journeyDetailsService;
            _customerInformationServiceWrapper = customerInformationServiceWrapper;
            _configManager = configManager;
        }

        public Customer GetCustomerDetails(CustomerAccount customerAccount)
        {
            var customer = new Customer
            {
                FalloutReasons = new List<FalloutReasonResult>()
            };

            if (!customerAccount.SiteDetails.HasSingleActiveEnergyServiceAccount)
            {
                customer.FalloutReasons.Add(new FalloutReasonResult { FalloutReason = FalloutReason.Ineligible, FalloutDescription = "Doesn't have single active energy service account." });
                return customer;
            }

            customer.AddCustomerAccount(customerAccount);

            checkAERResponse checkAerResponse = _annualEnergyReviewServiceWrapper.checkAER(new[] { customerAccount.SiteDetails.AccountNumber });

            for (int index = 0; index < checkAerResponse.reviewResultCollection.Length; index++)
            {
                checkReviewResultType result = checkAerResponse.reviewResultCollection[index];

                CustomerAccount currentAccount;

                if (index == 0)
                {
                    currentAccount = customerAccount;
                }
                else
                {
                    currentAccount = CustomerAccountModelMapper.MapCustomerAccountsResponseToCustomerAccount(_customerInformationServiceWrapper.GetCustomerAccounts(new [] { result.customerAccountNumber }), result.customerAccountNumber);

                    if (!currentAccount.SiteDetails.HasSingleActiveEnergyServiceAccount)
                    {
                        customer.FalloutReasons.Add(new FalloutReasonResult { FalloutReason = FalloutReason.Ineligible, FalloutDescription = "Doesn't have single active energy service account." });
                        return customer;
                    }
                    customer.AddCustomerAccount(currentAccount);
                    if (currentAccount.CurrentTariff.DisplayName != customerAccount.CurrentTariff.DisplayName)
                    {
                        customer.FalloutReasons.Add(new FalloutReasonResult { FalloutReason = FalloutReason.Ineligible, FalloutDescription = "Dual fuel site with different tariffs on each fuel." });
                    }
                }

                Dictionary<string, object> variables = result.variablesCollection.ToDictionary<customerAccountVariablesType, string, object>(variable => variable.name, variable => variable.value);

                variables.Add("TariffName", currentAccount.CurrentTariff.Name);
                variables.Add("BrandCode", currentAccount.CurrentTariff.BrandCode);
                variables.Add("IsBillingException", currentAccount.IsBillingException);

                currentAccount.SiteDetails.MeterRegisterCount = int.Parse(variables["RegisterCount"].ToString());

                List<FalloutReasonResult> aerCheckFallouts = _accountEligibilityChecker.IsEligible(variables, TariffChangeEligibilityCheckType.CheckAEREligibility);
                if (aerCheckFallouts.Count > 0)
                {
                    customer.FalloutReasons.AddRange(aerCheckFallouts);
                    return customer;
                }

                // 3 rate meters cause GetEnergyData to throw exception
                if (currentAccount.SiteDetails.MeterRegisterCount < 3)
                {
                    getEnergyDataResponse getEnergyDataResponse = _annualEnergyReviewServiceWrapper.getEnergyData(new[] { result.customerAccountNumber });
                    customerAccountType response = getEnergyDataResponse.accountsCollection[0];

                    DateTime tariffEndDate = response.servicePlanDetails.planEndDate;
                    variables.Add("TariffEndDate", tariffEndDate);
                    variables.Add("AnnualCost", response.consumptionDetails.annualUsageAmount);
                    variables.Add("ConsumptionDetailsType", response.consumptionDetails.consumptionRuleDescription.ToString());
                    variables.Add("PaymentMethodFromGetEnergyData", response.paymentMethod.paymentMethodCode.ToString());
                    variables.Add("CollectionDay", response.paymentMethod.directDebit?.collectionDay);

                    customer.HolderName = response.customerName;
                    currentAccount.FollowOnTariffServicePlanID = response.servicePlanDetails.FollowOnTariffServicePlanId;
                    currentAccount.ConsumptionRuleDescription = response.consumptionDetails.consumptionRuleDescription.ToString();

                    currentAccount.PaymentDetails = new PaymentDetails
                    {
                        BankAccountName = response.paymentMethod.directDebit?.accountInNameOf,
                        BankAccountNumber = response.paymentMethod.directDebit?.bankAccountNumber,
                        BankSortCode = response.paymentMethod.directDebit?.sortCodeNumber,
                        DirectDebitDay = response.paymentMethod.directDebit?.collectionDay,
                        HasDirectDebitDiscount = response.discountIndicators.directDebitIndicator,
                        IsDirectDebit = response.paymentMethod.paymentMethodCode == paymentMethodTypePaymentMethodCode.DDB || response.paymentMethod.paymentMethodCode == paymentMethodTypePaymentMethodCode.DDV,
                        IsVariableDirectDebit = response.paymentMethod.paymentMethodCode == paymentMethodTypePaymentMethodCode.DDV,
                        IsMonthlyDirectDebit = !string.IsNullOrEmpty(response.paymentMethod.directDebit?.collectionDay),
                        IsPaperless = response.discountIndicators.paperlessBillingIndicator
                    };

                    currentAccount.CurrentTariff.AnnualUsageKwh = double.Parse(response.consumptionDetails.annualUsageKiloWattHours);
                    currentAccount.CurrentTariff.AnnualCost = (double)response.consumptionDetails.annualUsageAmount;
                    currentAccount.CurrentTariff.PeakPercentageOperand = (double)response.consumptionDetails.peakPercentage / 100;
                    currentAccount.CurrentTariff.EndDate = response.servicePlanDetails.planEndDate;
                    currentAccount.CurrentTariff.ServicePlanId = response.servicePlanDetails.planId;
                }
                else
                {
                    customer.FalloutReasons.Add(new FalloutReasonResult { FalloutReason = FalloutReason.Indeterminable, FalloutDescription = "Three or more meter registers at property." });
                }

                customer.FalloutReasons.AddRange(_accountEligibilityChecker.IsEligible(variables, TariffChangeEligibilityCheckType.GetEnergyDataEligibility));
            }

            ValidateCustomerWithFollowOnTariff(customer);
            customer.TariffCalculationMethod = DetermineCalculationMethod(customer);

            return customer;
        }

        private void ValidateCustomerWithFollowOnTariff(Customer customer)
        {
            if (customer.HasGasAccount() && customer.HasElectricityAccount())
            {
                if ((string.IsNullOrEmpty(customer.GasAccount.FollowOnTariffServicePlanID) && !string.IsNullOrEmpty(customer.ElectricityAccount.FollowOnTariffServicePlanID))
                    || (!string.IsNullOrEmpty(customer.GasAccount.FollowOnTariffServicePlanID) && string.IsNullOrEmpty(customer.ElectricityAccount.FollowOnTariffServicePlanID)))
                {
                    customer.FalloutReasons.Add(new FalloutReasonResult { FalloutReason = FalloutReason.Indeterminable, FalloutDescription = "Customer is dual-fuel with follow-on service plan for one fuel and not for the other." });
                }
            }
        }

        public bool CheckCustomerHasBeenSet()
        {
            if (_journeyDetailsService.GetCustomer() == null)
            {
                _journeyDetailsService.ClearJourneyDetails();
                return false;
            }

            return true;
        }

        public bool CheckEmailAddressIsNotNull()
        {
            if (_journeyDetailsService.GetCustomer()?.EmailAddress == null)
            {
                _journeyDetailsService.ClearJourneyDetails();
                return false;
            }

            return true;
        }

        private TariffCalculationMethod DetermineCalculationMethod(Customer customer)
        {
            string prevSAForecastValue = consumptionDetailsTypeConsumptionRuleDescription.previoussaforecast.ToString();
            if (customer.ElectricityAccount?.ConsumptionRuleDescription == prevSAForecastValue || customer.GasAccount?.ConsumptionRuleDescription == prevSAForecastValue)
            {
                return TariffCalculationMethod.CurrentRate;
            }

            DateTime announcementDate = DateTime.Parse(_configManager.GetAppSetting("AtlasAnnouncementDate"));

            if ((announcementDate.ToUniversalTime() - DateTime.Now.ToUniversalTime()).TotalSeconds > 0)
            {
                return TariffCalculationMethod.Original;
            }

            DateTime lastElectBillDate = customer.ElectricityAccount != null && customer.ElectricityAccount.LastBilledDate.Date != DateTime.MaxValue.Date ? customer.ElectricityAccount.LastBilledDate :  DateTime.MinValue;
            DateTime lastGasBillDate = customer.GasAccount != null && customer.GasAccount.LastBilledDate.Date != DateTime.MaxValue.Date ? customer.GasAccount.LastBilledDate : DateTime.MinValue;
            DateTime lastBilledDate = DateTime.Compare(lastElectBillDate, lastGasBillDate) >= 0 ? lastElectBillDate : lastGasBillDate;

            return lastBilledDate == DateTime.MinValue || (lastBilledDate - announcementDate).Days > 0 ? TariffCalculationMethod.Original : TariffCalculationMethod.CurrentRate;
        }
    }
}