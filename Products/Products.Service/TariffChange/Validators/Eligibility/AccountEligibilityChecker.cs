using Products.Model.Enums;
using Products.Model.TariffChange.Customers;
using System.Collections.Generic;
using System.Linq;

namespace Products.Service.TariffChange.Validators.Eligibility
{
    public interface IAccountEligibilityChecker
    {
        List<FalloutReasonResult> IsEligible(Dictionary<string, object> variables, TariffChangeEligibilityCheckType falloutCheck);
    }

    public class AccountEligibilityChecker : IAccountEligibilityChecker
    {
        private readonly IEnumerable<IAccountEligibilityValidator> _accountEligibilityValidators;

        public AccountEligibilityChecker(IEnumerable<IAccountEligibilityValidator> accountEligibilityValidators)
        {
            _accountEligibilityValidators = accountEligibilityValidators;
        }

        public List<FalloutReasonResult> IsEligible(Dictionary<string, object> variables, TariffChangeEligibilityCheckType falloutCheck)
        {
            var falloutReasons = new List<FalloutReasonResult>();
            foreach (var validator in _accountEligibilityValidators.Where(p=>p.falloutCheck == falloutCheck))
            {
                var falloutReason = validator.Validate(variables);
                if (falloutReason.FalloutReason != FalloutReason.None)
                {
                    falloutReasons.Add(falloutReason);
                }
            }
            return falloutReasons;
        }
    }
}