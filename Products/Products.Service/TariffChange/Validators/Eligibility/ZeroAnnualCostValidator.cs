using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class ZeroAnnualCostValidator : IAccountEligibilityValidator
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
            if (variables.ContainsKey("AnnualCost") && variables.ContainsKey("TariffEndDate") && variables.ContainsKey("TariffName"))
            {
                return new FalloutReasonResult
                {
                    FalloutReason = (!variables["TariffName"].ToString().ToLower().Contains("standard")
                                        && Convert.ToDouble(variables["AnnualCost"]) == 0
                                        && Convert.ToDateTime(variables["TariffEndDate"]) < DateTime.Today.AddDays(365))
                       ? FalloutReason.ZeroAnnualCost
                       : FalloutReason.None,
                    FalloutDescription = "Customer has less than a year left on a fixed tariff and zero annual cost."
                };
            }

            return new FalloutReasonResult { FalloutReason = FalloutReason.None };
        }
    }
}