using System.Collections.Generic;
using Newtonsoft.Json;

namespace Model
{
    public class Service
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")] 
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Products")]
        public IList<Product> Products { get; set; }
    }
}
