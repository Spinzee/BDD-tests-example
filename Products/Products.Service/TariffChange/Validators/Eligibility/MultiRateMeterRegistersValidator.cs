using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System;
using Products.Model.Enums;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class MultiRateMeterRegistersValidator : IAccountEligibilityValidator
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
            int count;
            var falloutReason = new FalloutReasonResult { FalloutDescription = "Customer has a multi rate meter." };
            if (int.TryParse(variables["RegisterCount"].ToString(), out count))
            {
                if (count == 1)
                {
                    falloutReason.FalloutReason = FalloutReason.None;
                    return falloutReason;
                }

                var tariffName = variables["TariffName"].ToString();
                var multiRateTariffs = ConfigurationManager.AppSettings["MultiRateTariffs"].Split(',');

                var isAllowedMultiRateTariff = multiRateTariffs.Any(acquisitionTariff => tariffName.Contains(acquisitionTariff));

                falloutReason.FalloutReason = count == 2 && isAllowedMultiRateTariff ? FalloutReason.None : FalloutReason.Ineligible;
                return falloutReason;
            }

            falloutReason.FalloutReason = FalloutReason.Ineligible;
            return falloutReason;
        }
    }
}
