namespace Products.Model.HomeServices
{
    using System.Collections.Generic;

    public class ProductGroup
    {
        public List<ProductExtra> Extras { get; set; }

        public string Name { get; set; }

        public List<Product> Products { get; set; }

        public List<string> WhatsExcluded { get; set; }

        public List<string> WhatsIncluded { get; set; }
    }
}
