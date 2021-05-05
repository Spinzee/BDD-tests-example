using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class MAndSTariffNameValidator : IAccountEligibilityValidator
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
            var tariffName = variables.ContainsKey("TariffName") ? variables["TariffName"]?.ToString() : string.Empty;
           
            return new FalloutReasonResult
            {
                FalloutReason = tariffName.Contains("M&S")
                ? FalloutReason.MandS
                : FalloutReason.None,
                FalloutDescription = "M&S Brand"
            };
        }
    }
}