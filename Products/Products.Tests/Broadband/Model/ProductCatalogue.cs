namespace Products.Tests.Broadband.Model
{
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;
    using ServiceWrapper.BroadbandProductsService;

    public class ProductCatalogue
    {
        public ProductsResponse Read()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // ReSharper disable once StringLiteralTypo
            const string resourceName = "Products.Tests.broadbandprices_26092017.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (var reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    var root = JsonConvert.DeserializeObject<ProductsResponse>(result);
                    return root;
                }
            }
        }
    }
}