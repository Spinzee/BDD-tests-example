namespace Products.WebModel.ViewModels.Common
{
    public class RadioButton
    {
        public string Value { get; set; }
        public string DisplayText { get; set; }
        public string DescriptiveText { get; set; }
        public bool Checked { get; set; }
        public string AriaDescription { get; set; }
        public string RightHandTextWithLink { get; set; }
        public string RightHandTextWithLinkUrl { get; set; }
        public string RightHandTextWithLinkAlt { get; set; }
        public string RightHandText { get; set; }
        public RightHandImage RightHandImage { get; set; }
        public string DataValRequiredIf { get; set; }
        public string DataValRequiredIfDependencyProperty { get; set; }
        public string DataValRequiredIfDesiredValue { get; set; }
        public Modal ModalInfo { get; set; }
    }
}