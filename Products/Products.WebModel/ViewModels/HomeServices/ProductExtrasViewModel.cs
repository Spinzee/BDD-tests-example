namespace Products.WebModel.ViewModels.HomeServices
{
    using System.Collections.Generic;

    public class ProductExtrasViewModel
    {
        public string ProductName { get; set; }
        public string ProductTagLine { get; set; }
        public string ProductCode { get; set; }
        public string MonthlyCost { get; set; }
        public bool IsSelected { get; set; }
        public List<string> WhatsIncluded { get; set; }
        public List<string> WhatsExcluded { get; set; }
    }
}