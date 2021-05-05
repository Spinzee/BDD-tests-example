namespace Products.WebModel.ViewModels.Common
{
    public class TalkProductViewModel
    {
        public string ProductCode { get; set; }
        public string TalkProductName { get; set; }
        public string TalkCode { get; set; }
        public double Price { get; set; }
        public string FormattedPrice => Price == 0 ? "Included" : Price.ToString("C0");
        public string TalkProductDescription { get; set; }
        public TalkProductModalViewModel ModalViewModel { get; set; }
        public bool Selected { get; set; }
    }
}