using System.Collections.Generic;
using Newtonsoft.Json;

namespace Model
{
    public class Product 
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Price")]
        public double Price { get; set; }
        
        [JsonProperty(PropertyName = "Stylists")]
        public IList<Stylist> Stylists { get; set; }
        
    }
}
