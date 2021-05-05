using System.Collections.Generic;
using Model;

namespace WebSite.Models
{
    public class ProductsViewModel
    {
        public IList<Service> Services { get; set; } = new List<Service>();
    }
}
