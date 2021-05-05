using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class SingleMeterRegisterValidator : IAccountEligibilityValidator
    {
        public TariffChangeEligibilityCheckType falloutCheck
        {
            get
            {
                return TariffChangeEligibilityCheckType.CheckAEREligibility;
            }
        }

        public FalloutReasonResult Validate(Dictionary<string, object> variables)
        {
            int count;
            var falloutReason = new FalloutReasonResult { FalloutDescription = "Customer meter has more than one register." };
            if (variables.ContainsKey("RegisterCount") && int.TryParse(variables["RegisterCount"].ToString(), out count))
            {
                falloutReason.FalloutReason = count == 1 ? FalloutReason.None : FalloutReason.Ineligible;
                return falloutReason;
            }
            falloutReason.FalloutReason = FalloutReason.Ineligible;
            return falloutReason;
        }
    }
}