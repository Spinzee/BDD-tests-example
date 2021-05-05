using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class DirectDebitCollectionDayValidator : IAccountEligibilityValidator
    {
        public TariffChangeEligibilityCheckType falloutCheck
        {
            get
            {
                return TariffChangeEligibilityCheckType.GetEnergyDataEligibility;
            }
        }


        public FalloutReasonResult Validate(Dictionary<string, object> variables)
        {
            if (variables.ContainsKey("CollectionDay"))
            {
                string CollectionDay = variables["CollectionDay"] as string;

                if (!string.IsNullOrEmpty(CollectionDay) && Convert.ToDouble(CollectionDay) > 28)
                {
                    return new FalloutReasonResult
                    {
                        FalloutReason = FalloutReason.Ineligible,
                        FalloutDescription = "Direct debit collection day is greater than 28th day of the month."
                    };
                }
            }
            return new FalloutReasonResult { FalloutReason = FalloutReason.None };
        }
    }
}
