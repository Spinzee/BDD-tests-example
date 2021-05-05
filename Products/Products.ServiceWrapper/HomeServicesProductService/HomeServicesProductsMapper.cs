using Products.Model.HomeServices;
using Sse.Retail.Homeservices.Client.Models;
using System.Linq;
using HomeServicesModel = Products.Model.HomeServices;

namespace Products.ServiceWrapper.HomeServices
{
    public class HomeServicesProductsMapper
    {
        public static HomeServicesModel.ProductGroup ToHomeServiceProduct(ProductResult response)
        {
            if (response?.Productgroup == null) return null;

            return new HomeServicesModel.ProductGroup
            {
                Name = response?.Productgroup?.Name,
                WhatsExcluded = response?.Productgroup?.Whatsexcluded?.ToList(),
                WhatsIncluded = response?.Productgroup?.Whatsincluded?.ToList(),
                Extras = response?.Productgroup?.Addons?.Select(extra => new ProductExtra
                {
                    Cost = extra.Cost,
                    Id = extra.Id,
                    Name = extra.Name,
                    ProductCode = extra.Code,
                    ProductTagLine = extra.Tagline,
                    WhatsExcluded = extra.Whatsexcluded?.ToList(),
                    WhatsIncluded = extra.Whatsincluded?.ToList(),
                }).ToList(),
                Products = response?.Productgroup?.Products?.Select(product => new HomeServicesModel.Product
                {
                    MonthlyCost = product.Cost,
                    Description = product.Description,
                    Excess = product.Excess,
                    FullOfferText = product.Fulloffertext,
                    Id = product.Productid,
                    OfferSummary = new HomeServicesModel.OfferSummary
                    {
                        Amount = product.Offersummary?.Amount ?? 0,
                        Description = product.Offersummary?.Description
                    },
                    ProductCode = product.Productcode,
                    UpsellOfferText = product.Upselloffertext,
                    ContractLength = product.Contractlength,
                    OfferSummaryText = product.Offersummarytext,
                    CoverBulletText = product.Coverbullet
                }).ToList(),
            };
        }
    }
}
