using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public class BillNotInExceptionValidator : IAccountEligibilityValidator
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
            if (variables.ContainsKey("IsBillingException")
                && variables.ContainsKey("CAException")
                && variables.ContainsKey("LastBillSendDays")
                && variables.ContainsKey("ConsumptionDetailsType"))
            {
                if(Convert.ToBoolean(variables["IsBillingException"]) || variables["CAException"].ToString() == "Y")
                {
                    if (Convert.ToDouble(variables["LastBillSendDays"]) > 0 || !variables["ConsumptionDetailsType"].ToString().Contains("eac_or_aq"))
                    {
                        return new FalloutReasonResult
                        {
                            FalloutReason = FalloutReason.Ineligible,
                            FalloutDescription = "Customer has a billing or rebilling exception"
                        };
                    }
                }
            }

            return new FalloutReasonResult { FalloutReason = FalloutReason.None };
        }
    }
}