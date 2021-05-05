using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class CombinedHeatingAndPowerValidator : IAccountEligibilityValidator
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
            var cHPAccount = variables.ContainsKey("CHPAccount") ? variables["CHPAccount"]?.ToString() : "N";

            return new FalloutReasonResult
            {
                FalloutReason = cHPAccount == "N" ? FalloutReason.None : FalloutReason.Ineligible,
                FalloutDescription = "Customer has a CHP account"
            };
        }
    }
}