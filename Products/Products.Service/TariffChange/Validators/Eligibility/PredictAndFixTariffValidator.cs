using System.Collections.Generic;
using Products.Model.Enums;
using Products.Model.TariffChange.Customers;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class PredictAndFixTariffValidator : IAccountEligibilityValidator
    {
        public FalloutReasonResult Validate(Dictionary<string, object> variables)
        {
            var tariffName = (variables.ContainsKey("TariffName") ? (variables["TariffName"]?.ToString() ?? string.Empty) : string.Empty).ToUpper();

            return new FalloutReasonResult
            {
                FalloutReason = tariffName.Contains("PREDICT AND FIX") || tariffName.Contains("PREDICT & FIX")? FalloutReason.Ineligible : FalloutReason.None,
                FalloutDescription = "Predict and fix tariff"
            };
        }

        public TariffChangeEligibilityCheckType falloutCheck => TariffChangeEligibilityCheckType.CheckAEREligibility;
    }
}
