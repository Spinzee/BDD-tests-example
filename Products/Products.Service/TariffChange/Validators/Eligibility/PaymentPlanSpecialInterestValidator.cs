using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class PaymentPlanSpecialInterestValidator : IAccountEligibilityValidator
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
            var paymentSpecial = variables.ContainsKey("PPSpecialInt") ? variables["PPSpecialInt"]?.ToString() : "N";

            return new FalloutReasonResult
            {
                FalloutReason = paymentSpecial == "N" ? FalloutReason.None : FalloutReason.Ineligible,
                FalloutDescription = "Customer has a special interest flag on their payment plan."
            };
        }
    }
}