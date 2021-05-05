namespace Products.Tests.Energy.Fakes.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using ServiceWrapper.BundleTariffService;

    public class FakeBundleTariffServiceWrapper : IBundleTariffServiceWrapper
    {
        public bool ReturnSingleBundle { get; set; }

        public bool ReturnNoBundles { get; set; }

        public bool ReturnFixNProtectBundle { get; set; }

        private static IEnumerable<Product> BundleOneElectricityProductTestData =>
            new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Fix and Fibre",
                    ServicePlanId = "ME724",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                }
            };

        private static IEnumerable<Product> BundleOneGasProductTestData =>
            new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Complete Home",
                    ServicePlanId = "MG123",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                }
            };

        private static IEnumerable<Product> BundleProductsTestData =>
            new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Fix and Fibre",
                    ServicePlanId = "ME724",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                },
                new Product
                {
                    ServicePlanInvoiceDescription = "Fix and Fibre",
                    ServicePlanId = "MG101",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                }
            };

        private static IEnumerable<Product> CompleteHomeProductsTestData =>
            new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Complete Home",
                    ServicePlanId = "ME123",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                },
                new Product
                {
                    ServicePlanInvoiceDescription = "Complete Home",
                    ServicePlanId = "MG123",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                }
            };

        private static IEnumerable<TariffTickUsp> TickUspsTestData =>
            new List<TariffTickUsp>
            {
                new TariffTickUsp("header 1", "desc 1", 1),
                new TariffTickUsp("header 2", "desc 2", 2)
            };

#pragma warning disable 1998
        public async Task<List<Bundle>> GetDualMultiRateElectricBundles(BundleRequest request)
        {
            return new List<Bundle>
            {
                new Bundle(
                    "test01",
                    "",
                    BundleProductsTestData,
                    TickUspsTestData,
                    false,
                    new BundlePackage("FF3_LR18", 23.00, 33.00, BundlePackageType.FixAndFibre,
                        new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                )
            };
        }

        public async Task<List<Bundle>> GetDualSingleRateElectricBundles(BundleRequest request)
        {
            if (ReturnSingleBundle)
            {
                return new List<Bundle>
                {
                    new Bundle(
                        "BO001"
                        , "Fix and Fibre Bundle"
                        , BundleProductsTestData
                        , TickUspsTestData
                        , false
                        , new BundlePackage("FF3_LR18", 23.00, 33.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                    )
                };
            }

            if (ReturnFixNProtectBundle)
            {
                var extras = new List<Extra>
                {
                    new Extra(
                        "SSE Electrical Wiring Cover",
                        11.5,
                        0.0,
                        "EC",
                        3,
                        12,
                        "Cover for your fixed electrical wiring system, giving you peace of mind",
                        new List<string>
                        {
                            "Repairs to your fixed wiring system",
                            "Parts, labour and unlimited call-outs",
                            "24/7 emergency helpline",
                            "24-hour call-outs for emergency repairs",
                            "An inspection every five years of continuous cover"
                        },
                        new List<string>
                        {
                            "Repairing the power supply to your property or the electricity meter",
                            "Any items that aren't part of the fixed electrical wiring system",
                            "Electrical heating equipment",
                            "Major rewiring works. This product only covers repairing faults"
                        },
                        ExtraType.ElectricalWiring
                    )
                };
                var usp = new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) };
                var bundle = new Bundle(
                    "BO002"
                    , "Fix and Protect Bundle"
                    , CompleteHomeProductsTestData
                    , TickUspsTestData
                    , false
                    , new BundlePackage("BOHC", 0.0, 9.50, BundlePackageType.FixAndProtect, usp)
                    , extras)
                ;

                return new List<Bundle> { bundle };
            }

            if (ReturnNoBundles)
            {
                return new List<Bundle>();
            }

            return new List<Bundle>
            {
                new Bundle(
                    "BO001"
                    , "Fix and Fibre Bundle"
                    , BundleProductsTestData
                    , TickUspsTestData
                    , false
                    , new BundlePackage("FF3_LR18", 23.00, 33.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                ),
                new Bundle(
                    "BO002"
                    , "Complete Bundle"
                    , CompleteHomeProductsTestData
                    , TickUspsTestData
                    , false
                    , new BundlePackage("FIBRE18_LR_CH", 23.00, 33.00, BundlePackageType.FixAndFibre, new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                )
            };
        }

        public async Task<List<Bundle>> GetSingleRateElectricBundles(BundleRequest request)
        {
            return new List<Bundle>
            {
                new Bundle(
                    "test03",
                    "",
                    BundleOneElectricityProductTestData,
                    TickUspsTestData,
                    false,
                    new BundlePackage("FF3_LR18", 23.00, 33.00, BundlePackageType.FixAndFibre,
                        new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                )
            };
        }

        public async Task<List<Bundle>> GetMultiRateElectricBundles(BundleRequest request)
        {
            return new List<Bundle>
            {
                new Bundle(
                    "test04",
                    "",
                    BundleOneElectricityProductTestData,
                    TickUspsTestData,
                    false,
                    new BundlePackage("FF3_LR18", 23.00, 33.00, BundlePackageType.FixAndFibre,
                        new List<TariffTickUsp> { new TariffTickUsp("TestHeader", "TestDescription", 1) })
                )
            };
        }

        public async Task<List<Bundle>> GetSingleRateGasBundles(BundleRequest request)
        {
            return new List<Bundle>
            {
                new Bundle(
                    "test04",
                    "",
                    BundleOneGasProductTestData,
                    TickUspsTestData,
                    false,
                    new BundlePackage("BOBC", 0.0, 9.50, BundlePackageType.FixAndProtect, new List<TariffTickUsp>
                    {
                        new TariffTickUsp("TestHeader", "TestDescription", 1)
                    })
                )
            };
        }
#pragma warning restore 1998
    }
}