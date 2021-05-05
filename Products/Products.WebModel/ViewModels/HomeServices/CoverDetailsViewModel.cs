using Products.WebModel.ViewModels.Common;
using System.Collections.Generic;
using BaseViewModel = Products.WebModel.ViewModels.Common.BaseViewModel;

namespace Products.WebModel.ViewModels.HomeServices
{
    public class CoverDetailsViewModel : BaseViewModel
    {
        public RadioButtonList ExcessesRadioButtonList { get; set; } = new RadioButtonList();
        public string SingleExcessAmount { get; set; }
        public CoverDetailsHeaderViewModel CoverDetailsHeaderViewModel { get; set; }
        public List<string> WhatsIncluded{ get; set; }
        public List<string> WhatsExcluded { get; set; }
        public bool ProductExtrasAvailable { get; set; }
        public bool HasExcess { get; set; }
        public List<ProductExtrasViewModel> ProductExtras { get; set; }
        public AccordionViewModel AccordionViewModel { get; set;}
    }
}
