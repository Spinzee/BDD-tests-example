namespace Products.Service.Energy.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Managers;
    using Core;
    using Model.Energy;
    using Model.Enums;

    public class ProductsMapper
    {
        public static List<Product> MapProductsToCMSEnergyContentProducts(List<Product> products, List<CMSEnergyContent> cmsEnergyContents, ITariffManager tariffManager)
        {
            var productsInCMS = new List<Product>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Product product in products.ToList())
            {
                if (cmsEnergyContents.Any(cmsEnergyContent => product.DisplayName == cmsEnergyContent.TariffNameWithoutTariffWording))
                {
                    productsInCMS.Add(product);
                }
                else
                {
                    if (tariffManager.GetTariffGroup(product.ServicePlanId) != TariffGroup.None)
                    {
                        productsInCMS.Add(product);
                    }
                }
            }

            return productsInCMS;
        }

        public static List<Product> MapProductsToFilteredFixAndDriveProducts(FuelType selectedFuelType, List<Product> products, ITariffManager tariffManager)
        {
            var filteredProducts = new List<Product>();

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (Product product in products.ToList())
            {
                if (tariffManager.GetTariffGroup(product.ServicePlanId) == TariffGroup.FixAndDrive)
                {
                    if (selectedFuelType == FuelType.Dual)
                    {
                        filteredProducts.Add(product);
                    }
                }
                else
                {
                    filteredProducts.Add(product);
                }
            }

            return filteredProducts;
        }
    }
}
