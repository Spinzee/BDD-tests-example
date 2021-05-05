namespace Products.ServiceWrapper.BundleTariffService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.Enums;
    using Model.Energy;
    using Model.Enums;
    using Sse.Retail.Bundling.Client.Models;
    using Extra = Model.Energy.Extra;

    public static class BundleTariffMapper
    {
        public static List<Bundle> ToBundle(IEnumerable<BundleResponse> response)
        {
            IEnumerable<Bundle> bundles = response.Select(r =>
            {
                List<Product> products = r.EnergyTariff?.Products.Select(p => new Product
                {
                    ProjectedYearlyCost = p.ProjectedYearlyCost,
                    DirectDebitDiscount = p.Discount,
                    ServicePlanId = p.ServicePlanId,
                    ServicePlanInvoiceDescription = p.ServicePlanInvoiceDescription,
                    TariffType = TariffTypeExtensions.GetTariffType(p.TariffType),
                    EndOfTariffDate = p.EndOfTariffDate,
                    PriceGuaranteeDate = p.PriceGuaranteeDate,
                    ExitFee1 = p.ExitFee1,
                    RateCodeStandardDescription = p.RateCodeStandardDescription,
                    StandingCharge = p.StandingChargeInclVat,
                    UnitRate1InclVAT = p.UnitRate1InclVat,
                    UnitRate2InclVAT = p.UnitRate2InclVat,
                    LoyaltyBenefits = p.LoyaltyBenefits,
                    EndOfTariffDateDescription = p.TariffEndDateDescription,
                    PriceGuaranteeDateDescription = p.PriceGuaranteeDateDescription
                }).ToList();

                var bundlePackage = new BundlePackage
                (
                    r.BundlePackage.ProductCode,
                    r.BundlePackage.BundlePrice,
                    r.BundlePackage.OriginalPrice,
                    ToBundlePackageType(r.BundleType),
                    r.BundlePackage.TickUsp.Select(cb => new TariffTickUsp(cb.Title, cb.Paragraph, 0))
                );

                IEnumerable<TariffTickUsp> tickUsps = r.EnergyTariff?.TickUsp.Select(cb => new TariffTickUsp(cb.Title, cb.Paragraph, 0));
                IEnumerable<Extra> extras = r.Extras.Select(extra => new Extra(
                    extra.Name,
                    extra.BundlePrice,
                    extra.OriginalPrice,
                    extra.ProductCode,
                    (int)extra.Id,
                    (int)extra.ContractLength,
                    extra.TagLine,
                    extra.BulletList1.ToList(),
                    extra.BulletList2.ToList(),
                    ExtraType.ElectricalWiring));
                return new Bundle(r.BundleCode, r.BundleName, products, tickUsps,r.IsUpgrade ?? false , bundlePackage, extras.ToList());
            });

            return bundles.ToList();
        }

        private static BundlePackageType ToBundlePackageType(string bundlePackageType)
        {
            if (string.IsNullOrEmpty(bundlePackageType))
            {
                throw new ArgumentNullException($"bundleType is null or empty.");
            }

            if (bundlePackageType.Equals("FixAndFibre") || bundlePackageType.Equals("FixAndFibre Plus"))
            {
                return BundlePackageType.FixAndFibre;
            }
            else if (bundlePackageType.Equals("FixAndProtect") || bundlePackageType.Equals("FixAndProtect Plus"))
            {
                return BundlePackageType.FixAndProtect;
            }
            else
            {
                throw new InvalidDataException($"bundleType is invalid: {bundlePackageType}");
            }
        }
    }
}