using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class RenewalsValidator : IAccountEligibilityValidator
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
            var falloutReason = new FalloutReasonResult();
            if (!variables.ContainsKey("TariffEndDate"))
            {
                falloutReason.FalloutReason = FalloutReason.None;
                return falloutReason;
            }
            var tariffEndDate = (DateTime)variables["TariffEndDate"];
            falloutReason.FalloutReason = tariffEndDate >= DateTime.Today && tariffEndDate <= DateTime.Today.AddDays(60)
                ? FalloutReason.Renewals
                : FalloutReason.None;
            falloutReason.FalloutDescription = "Customer tariff ends upto 60 days in the future.";
            return falloutReason;
        }
    }
}