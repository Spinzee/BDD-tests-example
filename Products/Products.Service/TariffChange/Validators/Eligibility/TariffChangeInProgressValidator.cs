using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class TariffChangeInProgressValidator : IAccountEligibilityValidator
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
            if(!variables.ContainsKey("AERPendingTriggers"))
            {
                return new FalloutReasonResult
                {
                    FalloutReason = FalloutReason.None
                };
            }

            int tariffChangesInProgress;
            if (int.TryParse(variables["AERPendingTriggers"].ToString(), out tariffChangesInProgress) && tariffChangesInProgress == 0)
            {
                return new FalloutReasonResult
                {
                    FalloutReason = FalloutReason.None
                };
            }
            return new FalloutReasonResult
            {
                FalloutReason = FalloutReason.Ineligible,
                FalloutDescription = "Customer has a pending trigger in AER."
            };
        }
    }
}