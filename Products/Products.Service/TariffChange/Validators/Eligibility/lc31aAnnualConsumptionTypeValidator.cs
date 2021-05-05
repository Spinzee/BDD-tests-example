using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class Lc31AAnnualConsumptionTypeValidator : IAccountEligibilityValidator
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
            if (variables.ContainsKey("ConsumptionDetailsType") && variables.ContainsKey("LastBillSendDays")
                && variables["ConsumptionDetailsType"].ToString().Contains("eac_or_aq")
                    && Convert.ToDouble(variables["LastBillSendDays"]) > 0)
            {
                return new FalloutReasonResult
                {
                    FalloutReason = FalloutReason.Ineligible,
                    FalloutDescription = "Annual Consumption Details Type not lc31a for existing customer."
                };
            }

            return new FalloutReasonResult { FalloutReason = FalloutReason.None };
        }
    }
}
