using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System.Collections.Generic;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public interface IAccountEligibilityValidator
    {
        FalloutReasonResult Validate(Dictionary<string, object> variables);
        TariffChangeEligibilityCheckType falloutCheck { get; }
    }
}
