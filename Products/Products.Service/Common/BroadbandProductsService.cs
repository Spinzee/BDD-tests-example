namespace Products.Service.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Broadband.Managers;
    using Core;
    using Infrastructure;
    using Model.Broadband;
    using Model.Common;
    using ServiceWrapper.BroadbandProductsService;
    using LineSpeed = Model.Broadband.LineSpeed;
    using Tariff = Model.Broadband.Tariff;

    public class BroadbandProductsService : IBroadbandProductsService
    {
        private const string BrandName = "SSE";
        private readonly IBroadbandProductsServiceWrapper _broadbandProductsServiceWrapper;
        private readonly IBroadbandManager _manager;

        public BroadbandProductsService(IBroadbandProductsServiceWrapper broadbandProductsServiceWrapper, IBroadbandManager manager
        )
        {
            Guard.Against<ArgumentException>(broadbandProductsServiceWrapper == null, $"{nameof(broadbandProductsServiceWrapper)} is null");
            Guard.Against<ArgumentException>(manager == null, $"{nameof(manager)} is null");
            _broadbandProductsServiceWrapper = broadbandProductsServiceWrapper;
            _manager = manager;
        }

        public async Task<List<BroadbandProduct>> GetAvailableBroadbandProducts(BTAddress selectedAddress, string cliNumber)
        {
            var allProducts = new List<BroadbandProduct>();
            Task<List<Tariff>> allProductsTask = _broadbandProductsServiceWrapper.GetAllTariffs(BrandName);
            Task<BroadbandTariffsForAddress> availableProductsTask =
                _broadbandProductsServiceWrapper.GetAvailableTariffs(BrandName, selectedAddress, cliNumber);

            BroadbandTariffsForAddress availableProducts = await availableProductsTask;
            if (availableProducts.Tariffs.Any())
            {
                List<Tariff> allTariffs = await allProductsTask;
                IEnumerable<Tariff> unavailableTariffs = allTariffs.Where(x => availableProducts.Tariffs.All(y => y.ProductCode != x.ProductCode));
                allProducts = GetAvailableProducts(availableProducts);
                allProducts = RemoveInvalidProducts(allProducts);
                allProducts.AddRange(GetUnavailableProducts(unavailableTariffs));
            }

            return allProducts;
        }

        public async Task<List<BTAddress>> GetAddressesForPostcode(string postCode)
        {
            return await _broadbandProductsServiceWrapper.GetAddressesForPostcode(postCode);
        }

        private static List<BroadbandProduct> RemoveInvalidProducts(List<BroadbandProduct> availableProducts)
        {
            availableProducts.ForEach(x => x.TalkProducts.RemoveAll(p => p.BroadbandProductGroup == BroadbandProductGroup.NotAvailableOnline));
            availableProducts.Where(x => x.TalkProducts.Count == 0).ToList().ForEach(y => y.IsAvailable = false);
            return availableProducts;
        }

        private static LineSpeed GetLineSpeed(BroadbandType broadbandType, IEnumerable<LineSpeed> lineSpeeds)
        {
            switch (broadbandType)
            {
                case BroadbandType.ADSL:
                    LineSpeed adslLineSpeed = lineSpeeds.FirstOrDefault(x => x.Type == BroadbandType.ADSL);
                    return adslLineSpeed;
                case BroadbandType.Fibre:
                case BroadbandType.FibrePlus:
                    LineSpeed fiberLineSpeed = lineSpeeds.FirstOrDefault(x => x.Type == BroadbandType.Fibre);
                    return fiberLineSpeed;
                default:
                    return null;
            }
        }

        private List<BroadbandProduct> GetAvailableProducts(BroadbandTariffsForAddress tariffsForAddress)
        {
            var availableProducts = new List<BroadbandProduct>();
            foreach (Tariff tariff in tariffsForAddress.Tariffs)
            {
                BroadbandType broadbandType = _manager.GetBroadbandType(tariff.BroadbandCode);
                BroadbandProduct broadbandProduct = availableProducts.FirstOrDefault(p => p.BroadbandType == broadbandType);

                var talkProduct = new TalkProduct
                {
                    ProductCode = tariff.ProductCode,
                    ProductName = tariff.TariffName,
                    TalkCode = tariff.TalkCode,
                    Prices = tariff.Prices,
                    BroadbandProductGroup = _manager.BroadbandProductGroup(tariff.ProductCode)
                };

                if (broadbandProduct != null)
                {
                    broadbandProduct.TalkProducts.Add(talkProduct);
                    continue;
                }

                broadbandProduct = new BroadbandProduct
                {
                    TalkProducts = new List<TalkProduct> { talkProduct },
                    IsAvailable = true,
                    BroadbandType = broadbandType,
                    ProductOrder = (int) broadbandType,
                    LineSpeed = GetLineSpeed(broadbandType, tariffsForAddress.LineSpeeds)
                };

                availableProducts.Add(broadbandProduct);
            }

            return availableProducts;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private List<BroadbandProduct> GetUnavailableProducts(IEnumerable<Tariff> unavailableTariffs)
        {
            var unavailableProducts = new List<BroadbandProduct>();

            foreach (Tariff tariff in unavailableTariffs)
            {
                BroadbandType broadbandType = _manager.GetBroadbandType(tariff.BroadbandCode);
                BroadbandProduct broadbandProduct = unavailableProducts.FirstOrDefault(p => p.BroadbandType == broadbandType);
                if (broadbandProduct == null)
                {
                    unavailableProducts.Add(new BroadbandProduct
                    {
                        IsAvailable = false,
                        BroadbandType = broadbandType,
                        ProductOrder = (int) broadbandType
                    });
                }
            }

            return unavailableProducts;
        }
    }
}