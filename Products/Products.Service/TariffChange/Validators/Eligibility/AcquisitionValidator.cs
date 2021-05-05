using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class AcquisitionValidator : IAccountEligibilityValidator
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

            var acquisitionTariffs = ConfigurationManager.AppSettings["AcquisitionTariffs"].Split(',');
            return new FalloutReasonResult
            {
                FalloutReason = acquisitionTariffs.Any(acquisitionTariff => tariffName.Contains(acquisitionTariff))
                ? FalloutReason.Acquisition
                : FalloutReason.None,
                FalloutDescription = "Customer tariff is on the list of acquisition tariffs"
            };
        }
    }
}