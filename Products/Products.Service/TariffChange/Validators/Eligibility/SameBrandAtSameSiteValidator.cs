using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class SameBrandAtSameSiteValidator : IAccountEligibilityValidator
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
            // MixedBrands will be empty for single fuel, SameSite is implied from MCIS CustomerAccountStatus
            // When MixedBrands is 'N', SameSite must be yes as this is dual fuel
            return new FalloutReasonResult
            {
                FalloutReason = variables["MixedBrands"].ToString() == string.Empty
                   || variables["MixedBrands"].ToString() == "N" && variables["SameSite"].ToString() == "Y"
                   ? FalloutReason.None
                   : FalloutReason.Ineligible,
                FalloutDescription = "Customer has mixed brands at property."
            };
        }
    }
}