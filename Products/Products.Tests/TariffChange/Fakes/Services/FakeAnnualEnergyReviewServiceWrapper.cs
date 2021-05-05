namespace Products.Tests.TariffChange.Fakes.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Models;
    using ServiceWrapper.AnnualEnergyReviewService;

    public class FakeAnnualEnergyReviewServiceWrapper : IAnnualEnergyReviewServiceWrapper
    {
        private readonly string _accountNumber;
        private readonly Exception _exception;
        private readonly FakeAERData[] _fakeAerDataList;
        public bool DirectDebitStatusChanged;
        public bool PaymentMethodDetailsUpdated;
        public string ToServicePlanId;

        public FakeAnnualEnergyReviewServiceWrapper()
        {
        }

        public FakeAnnualEnergyReviewServiceWrapper(Exception exception)
        {
            _exception = exception;
        }

        public FakeAnnualEnergyReviewServiceWrapper(string accountNumber, FakeAERData[] fakeAerDataList)
        {
            _accountNumber = accountNumber;

            _fakeAerDataList = fakeAerDataList;
        }

        public checkAERResponse checkAER(string[] customerAccountNumbers)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            if (customerAccountNumbers.Length != 1)
            {
                throw new Exception("Only need single account number");
            }

            if (customerAccountNumbers[0] == _accountNumber)
            {
                return BuildCheckAerResponse();
            }

            throw new Exception("unrecognised account number " + customerAccountNumbers[0]);
        }

        public getEnergyDataResponse getEnergyData(string[] customerAccountNumbers)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            List<FakeAERData> fakeDataList = customerAccountNumbers.Select(customerAccountNumber => _fakeAerDataList.FirstOrDefault(fake => fake.CustomerAccountNumber == customerAccountNumber)).ToList();

            if (fakeDataList.Count == customerAccountNumbers.Length)
            {
                return BuildGetEnergyDataResponse(fakeDataList);
            }

            string accountNumbers = customerAccountNumbers.Aggregate("", (current, accountNumber) => current + accountNumber + ",");

            accountNumbers = accountNumbers.TrimEnd(',');
            throw new Exception("unrecognised account numbers " + accountNumbers);
        }

        public actionAERResponse actionAER(actionAERRequest request)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            ToServicePlanId = request.aerRequestCollection[0].actionDetails.tariffDetails.toServicePlanId;
            PaymentMethodDetailsUpdated = request.aerRequestCollection[0].actionDetails.paymentDetails != null;
            DirectDebitStatusChanged = request.aerRequestCollection[0].actionsToTake.changeDDStatus;

            return new actionAERResponse();
        }

        private static customerAccountVariablesType[] GetCustomerAccountVariables(Dictionary<string, string> fakeCustomerAccountVariables)
        {
            return fakeCustomerAccountVariables.Select(item => new customerAccountVariablesType { name = item.Key, value = item.Value }).ToArray();
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private static getEnergyDataResponse BuildGetEnergyDataResponse(List<FakeAERData> fakeAerDataList)
        {
            return new getEnergyDataResponse
            {
                accountsCollection = fakeAerDataList.Select(fakeAerData => new customerAccountType
                {
                    customerAccountNumber = fakeAerData.CustomerAccountNumber,
                    customerName = fakeAerData.CustomerName,
                    discountIndicators = new discountIndicatorsType { directDebitIndicator = fakeAerData.HasDirectDebitDiscount, paperlessBillingIndicator = fakeAerData.IsPaperless },
                    consumptionDetails = new consumptionDetailsType { annualUsageAmount = (decimal) fakeAerData.AnnualCost, annualUsageKiloWattHours = fakeAerData.AnnualUsageKwh.ToString(CultureInfo.InvariantCulture), consumptionRuleDescription = fakeAerData.ConsumptionRuleDescription },
                    servicePlanDetails = new servicePlanDetailsType { planEndDate = fakeAerData.EndDate, FollowOnTariffServicePlanId = fakeAerData.FollowOnServicePlanId, planId = fakeAerData.ServicePlanId },
                    paymentMethod = new paymentMethodType { paymentMethodCode = fakeAerData.PaymentMethodCode, directDebit = new paymentMethodTypeDirectDebit { collectionDay = fakeAerData.CollectionDay } }
                }).ToArray()
            };
        }

        private checkAERResponse BuildCheckAerResponse()
        {
            var checkReviewResultList = new List<checkReviewResultType>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (FakeAERData fakeAerData in _fakeAerDataList)
            {
                checkReviewResultList.Add(new checkReviewResultType
                {
                    customerAccountNumber = fakeAerData.CustomerAccountNumber,
                    variablesCollection = GetCustomerAccountVariables(fakeAerData.CustomerAccountVariables)
                });
            }

            return new checkAERResponse
            {
                reviewResultCollection = checkReviewResultList.ToArray()
            };
        }
    }
}