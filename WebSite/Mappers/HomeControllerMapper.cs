using System.Collections.Generic;
using Model;
using WebSite.Models;

namespace WebSite.Mappers
{
    public static class HomeControllerMapper
    {
        public static ProductsViewModel GetProductsViewModel(IList<Service> services)
        {
            var productsViewModel = new ProductsViewModel();
            
            foreach (var service in services)
            {
                var product = new Service
                {
                    Id = service.Id, 
                    Name = service.Name, 
                    Products = service.Products
                };

                productsViewModel.Services.Add(product);
            }

            return productsViewModel;
        }
    }
}
