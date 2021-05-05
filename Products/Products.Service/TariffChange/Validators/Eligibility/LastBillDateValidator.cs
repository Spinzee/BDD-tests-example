namespace Products.Service.TariffChange.Validators.Eligibility
{
    using System.Collections.Generic;
    using Model.Enums;
    using Model.TariffChange.Customers;

    public class LastBillDateValidator : IAccountEligibilityValidator
    {
        public TariffChangeEligibilityCheckType falloutCheck => TariffChangeEligibilityCheckType.CheckAEREligibility;

        public FalloutReasonResult Validate(Dictionary<string, object> variables)
        {
            var falloutReason = new FalloutReasonResult();
            if (int.TryParse(variables["LastBillSendDays"].ToString(), out int daysSinceLastBill) && daysSinceLastBill <= 212)
            {
                falloutReason.FalloutReason = FalloutReason.None;
                return falloutReason;
            }

            falloutReason.FalloutReason = FalloutReason.Ineligible;
            falloutReason.FalloutDescription = "Customer doesn't have a recent bill.";
            return falloutReason;
        }
    }
}