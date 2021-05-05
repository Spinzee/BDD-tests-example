namespace Products.WebModel.ViewModels.Common
{
    using System.Collections.Generic;

    public class SelectTalkPackageViewModel
    {
        public List<TalkProductViewModel> TalkProducts { get; set; }
        public string SelectedTalkProductCode { get; set; }
    }
}
