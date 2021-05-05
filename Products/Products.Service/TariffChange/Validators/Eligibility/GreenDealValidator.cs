using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class GreenDealValidator : IAccountEligibilityValidator
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
            var greenDealSite = variables.ContainsKey("GreenDealSite") ? variables["GreenDealSite"]?.ToString() : "N";

            return new FalloutReasonResult
            {
                FalloutReason = greenDealSite == "N" ? FalloutReason.None : FalloutReason.Ineligible,
                FalloutDescription = "Customer has a green deal at property"
            };
        }
    }
}