using Products.Model.TariffChange.Customers;
using System.Collections.Generic;

namespace Products.WebModel.ViewModels.TariffChange
{
    public class CustomerEligibilityViewModel
    {
        public List<FalloutReasonResult> FalloutReasons { get; set; }

        public FalloutReasonResult FalloutReasonResult { get; set; }
        public string AccountNumber { get; set; }

        public bool IsPostLogin { get; set; }
    }
}