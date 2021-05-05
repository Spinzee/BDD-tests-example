using System.Collections.Generic;
using Products.WebModel.ViewModels.Common;

namespace Products.WebModel.ViewModels.Broadband
{
    public class AvailableTalkPackagesViewModel
    {
        public List<TalkProductViewModel> TalkProducts { get; set; }

        public bool IsTalkProductAvailable { get; set; }
    }
}
