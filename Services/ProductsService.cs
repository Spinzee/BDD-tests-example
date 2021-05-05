using System.Collections.Generic;
using System.IO;
using Model;
using Newtonsoft.Json;

namespace Services
{
    public class ProductsService : IProductService
    {
        public IList<Service> GetListOfServices()
        {
            string json = File.ReadAllText(@"TestData\\Products.json");

            var services = JsonConvert.DeserializeObject<List<Service>>(json);
            
            return services;
        }
    }
}
