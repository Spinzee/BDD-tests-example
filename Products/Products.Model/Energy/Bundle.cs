namespace Products.Model.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Extensions;

    public class Bundle
    {
        public Bundle(
            string code
            , string name
            , IEnumerable<Product> products
            , IEnumerable<TariffTickUsp> tickUsps
            , bool isUpgrade
            , BundlePackage broadbandPackage = null
            , List<Extra> extras = null)
        {
            Guard.Against<ArgumentNullException>(code == null, $"{nameof(code)} is null");
            Guard.Against<ArgumentNullException>(name == null, $"{nameof(name)} is null");
            Guard.Against<ArgumentNullException>(products == null, $"{nameof(products)} is null");
            Guard.Against<ArgumentNullException>(tickUsps == null, $"{nameof(tickUsps)} is null");

            Product[] prods = products == null ? new Product[] { } : products.ToArray();

            Guard.Against<ArgumentException>(!prods.Any(), $"{nameof(products)} contains at 0 product. Bundle should contain at least 1 product.");
            Guard.Against<ArgumentException>(prods.Length > 2, $"{nameof(products)} contains more than 2 products. Bundle should have at most 2 products.");

            BundleCode = code;
            BundleName = name;
            Products = prods;
            TickUsps = tickUsps;
            BundlePackage = broadbandPackage;
            Extras = extras;
            IsUpgrade = isUpgrade;
        }
        public string BundleCode { get; }

        public string BundleName { get; }

        public string DisplayName => BundleName.TrimEconomyWording();

        public IEnumerable<Product> Products { get; }

        public IEnumerable<TariffTickUsp> TickUsps { get; }

        public BundlePackage BundlePackage { get; }

        public string ServicePlanId => Products.FirstOrDefault()?.ServicePlanId;

        public List<Extra> Extras { get; }

        public bool IsUpgrade { get; }
    }
}
