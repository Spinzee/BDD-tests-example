namespace Products.Model.HomeServices
{
    using System.Collections.Generic;

    public class ProductExtra
    {
        public double Cost { get; set; }
        public double Id { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string ProductTagLine { get; set; }
        public List<string> WhatsExcluded { get; set; }
        public List<string> WhatsIncluded { get; set; }
    }
}
