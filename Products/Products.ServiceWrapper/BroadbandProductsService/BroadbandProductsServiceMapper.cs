namespace Products.ServiceWrapper.BroadbandProductsService
{
    using System.Collections.Generic;
    using System.Linq;
    using Products.Model.Broadband;
    using Products.Model.Common;

    public static class BroadbandProductsServiceMapper
    {
        public static List<BTAddress> ToAddresses(BroadbandProductAddress[] addresses)
        {
            return addresses.Select((address, index) => new BTAddress
            {
                Id = index,
                Postcode = address.Postcode,
                SubPremises = address.SubPremises,
                PremiseName = address.PremiseName,
                ThoroughfareNumber = address.ThoroughfareNumber,
                PostTown = address.PostTown,
                ThoroughfareName = address.ThoroughfareName,
                Locality = address.Locality,
                DistrictId = address.DistrictId,
                UPRN = address.UPRN
            }).ToList();
        }

        public static List<Model.Broadband.Tariff> ToTariffs(IEnumerable<Tariff> tariffs)
        {
            return tariffs?.Select(t => new Model.Broadband.Tariff
            {
                BroadbandCode = t.BroadbandCode,
                ProductCode = t.ProductCode,
                TalkCode = t.TalkCode,
                TariffName = t.TariffName,
                Prices = t.PriceLines.Select(p => new BroadbandPrice
                {
                    FeatureCode = p.FeatureCode,
                    Price = p.Rate
                }).ToList()
            }).ToList();
        }

        public static BroadbandTariffsForAddress ToAvailableTariffs(ProductsResponse response)
        {
            return new BroadbandTariffsForAddress
            {
                LineSpeeds = response.LineSpeeds?.Select<LineSpeed, Model.Broadband.LineSpeed>(lineSpeed =>
                {
                    switch (lineSpeed.Type)
                    {
                        case "adsl":
                            return new ADSLLineSpeeds
                            {
                                Max = lineSpeed.MaxSpeed,
                                Min = lineSpeed.MinSpeed,
                            };
                        case "fibre":
                            return new FibreLineSpeeds
                            {
                                MaxDownload = lineSpeed.MaxDownload,
                                MaxUpload = lineSpeed.MaxUpload,
                                MinUpload = lineSpeed.MinUpload,
                                MinDownload = lineSpeed.MinDownload
                            };
                        default:
                            return null;
                    }
                }) ?? new List<Model.Broadband.LineSpeed>(),
                Tariffs = ToTariffs(response.BroadbandProducts?.Broadband?.Brand?.Tariffs) ?? new List<Model.Broadband.Tariff>()
            };
        }
    }
}
