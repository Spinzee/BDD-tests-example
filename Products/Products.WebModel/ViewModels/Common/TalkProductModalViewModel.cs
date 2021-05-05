namespace Products.WebModel.ViewModels.Common
{
    using System.Collections.Generic;
    
    public class TalkProductModalViewModel
    {
        public string ProductCode { get; set; }
        public string ModalTitle { get; set; }
        public string ModalParagraph { get; set; }
        public IList<string> ListItems { get; set; }
        public string LineFeatureLinkPrefix { get; set; }
        public string LineFeatureLinkUrl { get; set; }
        public string LineFeatureLinkAltText { get; set; }
        public string LineFeatureLinkText { get; set; }
        public string LineFeatureLinkSuffix { get; set; }
        public string MoreInformationPrefix { get; set; }
        public string MoreInformationLinkUrl { get; set; }
        public string MoreInformationLinkAltText { get; set; }
        public string MoreInformationLinkText { get; set; }
    }
}