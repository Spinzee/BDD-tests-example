using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class AtlanticBrandValidator : IAccountEligibilityValidator
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
            var brandCode = variables.ContainsKey("BrandCode") ? variables["BrandCode"]?.ToString() : string.Empty;
            return new FalloutReasonResult
            {
                FalloutReason = brandCode == "ATGA"
                   || brandCode == "ATSE"
                   || brandCode == "ATSW"
                   || brandCode == "ATHE"
                   || brandCode == "ATTE"
                ? FalloutReason.AtlanticIneligible
                : FalloutReason.None,
                FalloutDescription = "Atlantic Brand"
            };
        }
    }
}