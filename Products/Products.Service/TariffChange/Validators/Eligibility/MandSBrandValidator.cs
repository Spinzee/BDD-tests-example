using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using Products.Model.Enums;
using System;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class MandSBrandValidator : IAccountEligibilityValidator
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
            var brandCode = variables.ContainsKey("M&SBrand") ? variables["M&SBrand"]?.ToString() : "N";
            return new FalloutReasonResult
            {
                FalloutReason = brandCode == "N" ? FalloutReason.None : FalloutReason.MandS,
                FalloutDescription = "M&S Brand"
            };
        }
    }
}