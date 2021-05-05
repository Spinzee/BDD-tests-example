using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.WebModel.ViewModels.HomeServices
{
    using Common;

    public class YourCoverBasketViewModel
    {
        public string ProductName { get; set; }
        public string ProductMonthlyCost { get; set; }
        public string TotalMonthlyCost { get; set; }
        public string YearlyCost { get; set; }
        public List<SelectedExtra> SelectedExtras { get; set; }
        public bool HasSelectedExtras{ get; set; }
        public string ContractDuration { get; set; } = "12";
        public string ExcessCost { get; set; }
        public string OffersText { get; set; }
        public string OffersAmount { get; set; }
        public bool OffersExist { get; set; }
        public bool HasExcess { get; set; }
    }
}
