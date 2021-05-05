namespace Products.Service.Energy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Managers;
    using Core;
    using Core.Enums;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Mappers;
    using Model.Energy;
    using Model.Enums;
    using ServiceWrapper.BundleTariffService;
    using ServiceWrapper.EnergyProductService;
    using ServiceWrapper.HomeServicesProductService;

    public class TariffService : ITariffService
    {
        private readonly IBundleTariffServiceWrapper _bundleTariffServiceWrapper;
        private readonly IEnergyProductServiceWrapper _energyProductQuotationServiceWrapper;
        private readonly IHomeServicesProductServiceWrapper _homeServicesProductServiceWrapper;
        private readonly ITariffManager _tariffManager;

        public TariffService(IEnergyProductServiceWrapper energyProductServiceWrapper, ITariffManager tariffManager,
            IBundleTariffServiceWrapper bundleTariffServiceWrapper, IHomeServicesProductServiceWrapper homeServicesProductServiceWrapper)
        {
            Guard.Against<ArgumentException>(energyProductServiceWrapper == null, $"{nameof(energyProductServiceWrapper)} is null");
            Guard.Against<ArgumentException>(tariffManager == null, $"{nameof(tariffManager)} is null");
            Guard.Against<ArgumentException>(bundleTariffServiceWrapper == null, $"{nameof(bundleTariffServiceWrapper)} is null");
            Guard.Against<ArgumentException>(homeServicesProductServiceWrapper == null, $"{nameof(homeServicesProductServiceWrapper)} is null");

            _energyProductQuotationServiceWrapper = energyProductServiceWrapper;
            _tariffManager = tariffManager;
            _bundleTariffServiceWrapper = bundleTariffServiceWrapper;
            _homeServicesProductServiceWrapper = homeServicesProductServiceWrapper;
        }

        public async Task<List<Product>> GetEnergyProducts(EnergyCustomer customer)
        {
            var request = new ProductsRequest
            {
                PostCode = customer.Postcode,
                MeterType = customer.IsGasOnly()
                    ? ElectricityMeterType.Standard.ToDescription()
                    : customer.SelectedElectricityMeterType.ToDescription(),
                BillingPreference = customer.IsPrePay() ? "Paper" : "Paperless",
                AccountType = customer.IsPrePay() ? "Prepay" : "NonPrepay",
                PaymentType = customer.IsPrePay()
                    ? PaymentMethod.Quarterly.ToDescription()
                    : customer.SelectedPaymentMethod.ToDescription(),
                FuelType = customer.SelectedFuelType.ToString(),
                StandardGasKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyAveStandardGasKwh),
                StandardElectricityKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyAveStandardElecKwh),
                Economy7ElectricityDayKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyEconomy7DayElecKwh),
                Economy7ElectricityNightKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyEconomy7NightElecKwh)
            };

            return await _energyProductQuotationServiceWrapper.GetEnergyProducts(request);
        }

        public IEnumerable<Tariff> EnergyTariffs(EnergyCustomer energyCustomer, IEnumerable<Product> products, List<CMSEnergyContent> cmsEnergyContents)
        {
            List<Product> productsInCMS = ProductsMapper.MapProductsToCMSEnergyContentProducts(products.ToList(), cmsEnergyContents, _tariffManager);
            List<Product> filteredFixAndDriveProducts = ProductsMapper.MapProductsToFilteredFixAndDriveProducts(energyCustomer.SelectedFuelType, productsInCMS, _tariffManager);

            return filteredFixAndDriveProducts
                .GroupBy(t => t.DisplayName)
                .Select(group => new Tariff
                {
                    DisplayName = group.OrderByDescending(t => t.ServicePlanInvoiceDescription).First().DisplayName,
                    GasProduct = group.FirstOrDefault(g => g.ServicePlanId.StartsWith("MG")),
                    ElectricityProduct = group.FirstOrDefault(g => g.ServicePlanId.StartsWith("ME")),
                    TariffId = group.FirstOrDefault(g => !string.IsNullOrEmpty(g.ServicePlanId))?.ServicePlanId.Replace("MG", "").Replace("ME", ""),
                    EnergyTickUsps = _tariffManager.GetTariffGroup(group.Select(g => g.ServicePlanId).FirstOrDefault()) != TariffGroup.None ?
                        _tariffManager.GetTariffTickUsp(group.Select(g => g.ServicePlanId).FirstOrDefault()) :
                        cmsEnergyContents.FirstOrDefault(x => x.TariffNameWithoutTariffWording.Equals(group.Key, StringComparison.InvariantCultureIgnoreCase))?.TickUsps ?? new List<TariffTickUsp>(),
                    IsBundle = false,
                    TariffGroup = _tariffManager.GetTariffGroup(group.Select(g => g.ServicePlanId).FirstOrDefault()),
                    IsSmartTariff = _tariffManager.IsSmart(group.Select(g => g.ServicePlanId).FirstOrDefault())
                })
                .ToList();
        }

        public async Task<IEnumerable<Tariff>> BundleTariffs(EnergyCustomer customer)
        {
            var request = new BundleRequest
            {
                PostCode = customer.Postcode,
                StandardGasKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyAveStandardGasKwh) ?? 0,
                StandardElectricityKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyAveStandardElecKwh) ?? 0,
                Economy7ElectricityDayKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyEconomy7DayElecKwh) ?? 0,
                Economy7ElectricityNightKwh = GetConsumption(customer.Projection.Frequency, customer.Projection.EnergyEconomy7NightElecKwh) ?? 0
            };

            var bundles = new List<Bundle>();
            if (customer.IsDirectDebit())
            {
                if (customer.IsDualFuel() && customer.HasStandardMeter())
                {
                    bundles = await _bundleTariffServiceWrapper.GetDualSingleRateElectricBundles(request);
                }
                else if (customer.IsDualFuel() && customer.HasE7Meter())
                {
                    bundles = await _bundleTariffServiceWrapper.GetDualMultiRateElectricBundles(request);
                }
                else if (customer.IsElectricityOnly() && customer.HasStandardMeter())
                {
                    bundles = await _bundleTariffServiceWrapper.GetSingleRateElectricBundles(request);
                }
                else if (customer.IsElectricityOnly() && customer.HasE7Meter())
                {
                    bundles = await _bundleTariffServiceWrapper.GetMultiRateElectricBundles(request);
                }
                else if (customer.IsGasOnly())
                {
                    bundles = await _bundleTariffServiceWrapper.GetSingleRateGasBundles(request);
                }
            }

            List<Tariff> tariffs = bundles.Select(b => new Tariff
            {
                DisplayName = b.DisplayName,
                BackendTariffName = b.Products.GroupBy(t => t.ServicePlanInvoiceDescription).OrderByDescending(g => g.Key).First().Key.TrimEconomyWording(),
                ElectricityProduct = b.Products.FirstOrDefault(p => p.ServicePlanId.StartsWith("ME")),
                GasProduct = b.Products.FirstOrDefault(p => p.ServicePlanId.StartsWith("MG")),
                BundlePackage = b.BundlePackage,
                TariffId = b.BundleCode,
                EnergyTickUsps = b.TickUsps,
                IsBundle = true,
                Extras = b.Extras,
                TariffGroup = _tariffManager.GetTariffGroup(b.ServicePlanId ?? string.Empty),
                IsSmartTariff = _tariffManager.IsSmart(b.ServicePlanId ?? string.Empty),
                IsUpgrade = b.IsUpgrade
            }).ToList();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Tariff tariff in tariffs.Where(tariff => tariff.IsBundle && tariff.BundlePackage.BundlePackageType == BundlePackageType.FixAndProtect))
            {
                tariff.BundlePackage.HesMoreInformation = await _homeServicesProductServiceWrapper.GetHomeServiceResidentialProduct(tariff.BundlePackage.ProductCode, customer.Postcode);
            }

            return tariffs;
        }

        private static double? GetConsumption(UsageFrequency frequency, double? usage)
        {
            if (!usage.HasValue)
            {
                return null;
            }

            return frequency == UsageFrequency.Annual ? usage : usage * 12;
        }
    }
}