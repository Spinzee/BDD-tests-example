namespace Products.Tests.Energy.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Products.Model.Energy;
    using Products.Model.Enums;

    public class FakeProductsStub
    {
        public static List<Product> GetSingleDualFuelFakeProduct()
        {
            return new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Standard",
                    ServicePlanId = "ME029",
                    ProjectedYearlyCost = 1200,
                    TariffType = TariffType.Evergreen,
                    ExitFee1 = 25.0,
                    RateCode = 1
                },
                new Product
                {
                    ServicePlanInvoiceDescription = "Standard",
                    ServicePlanId = "MG095",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Evergreen,
                    ExitFee1 = 25.0,
                    RateCode = 1
                }
            };
        }

        public static List<Product> GetFakeProducts(string fuelType, string meterType)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (fuelType == "Dual")
            {
                if (meterType == "Economy 7")
                {
                    return new List<Product>
                    {
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Standard Economy 7",
                            ServicePlanId = "ME029",
                            ProjectedYearlyCost = 1000,
                            TariffType = TariffType.Evergreen,
                            ExitFee1 = 25.0
                        },
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Smart Tariff",
                            ServicePlanId = "ME672",
                            ProjectedYearlyCost = 1000,
                            TariffType = TariffType.Fixed,
                            ExitFee1 = 25.0
                        },
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Smart Tariff",
                            ServicePlanId = "MG095",
                            ProjectedYearlyCost = 1000,
                            TariffType = TariffType.Fixed,
                            ExitFee1 = 25.0
                        },
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Standard",
                            ServicePlanId = "MG029",
                            ProjectedYearlyCost = 1200,
                            TariffType = TariffType.Evergreen,
                            ExitFee1 = 25.0
                        }
                    };
                }

                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "ME672",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "MG095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard",
                        ServicePlanId = "ME001",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0,
                        DirectDebitDiscount = 10.20
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard",
                        ServicePlanId = "MG001",
                        ProjectedYearlyCost = 1200,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0,
                        DirectDebitDiscount = 20.20
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v14",
                        ServicePlanId = "MG003",
                        ProjectedYearlyCost = 1500,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v14",
                        ServicePlanId = "ME003",
                        ProjectedYearlyCost = 1600,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v15",
                        ServicePlanId = "ME004",
                        ProjectedYearlyCost = 1234,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v15",
                        ServicePlanId = "MG004",
                        ProjectedYearlyCost = 1234.56,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fix and Protect",
                        ServicePlanId = "ME697",
                        ProjectedYearlyCost = 1234,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fix and Protect",
                        ServicePlanId = "MG098",
                        ProjectedYearlyCost = 1234.56,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0
                    }
                };
            }


            if (fuelType == "Electricity")
            {
                if (meterType == "Economy 7")
                {
                    return new List<Product>
                    {
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Smart Tariff",
                            ServicePlanId = "ME672",
                            ProjectedYearlyCost = 1000,
                            TariffType = TariffType.Fixed,
                            ExitFee1 = 25.0
                        },
                        new Product
                        {
                            ServicePlanInvoiceDescription = "Standard Economy 7",
                            ServicePlanId = "ME029",
                            ProjectedYearlyCost = 1000,
                            TariffType = TariffType.Evergreen,
                            ExitFee1 = 25.0
                        }
                    };
                }


                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "ME672",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v14",
                        ServicePlanId = "ME270",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 1234.234
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v16",
                        ServicePlanId = "ME290",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 1234.234,
                        DirectDebitDiscount = 10.20
                    }
                };
            }

            if (fuelType == "Gas")
            {
                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "MG095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE 1 Year Fixed v14",
                        ServicePlanId = "MG271",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 600.89
                    }
                };
            }

            return new List<Product>();
        }

        public static List<Product> GetFakeProductList()
        {
            return new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Standard",
                    ServicePlanId = "ME029",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Evergreen,
                    ExitFee1 = 25.0
                }
            };
        }

        public static List<Product> GetFakeFixAndDriveProductList()
        {
            return new List<Product>
            {
                new Product
                {
                    ServicePlanInvoiceDescription = "Fix And Drive",
                    ServicePlanId = "ME123",
                    ProjectedYearlyCost = 1000,
                    TariffType = TariffType.Fixed,
                    ExitFee1 = 25.0
                }
            };
        }

        public static List<Product> GetProductsForOurPrices(string tariffStatus, string fuelCategory)
        {
            if (fuelCategory == "Standard")
            {
                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "ME095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Smart Tariff",
                        ServicePlanId = "ME095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 2
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard",
                        ServicePlanId = "ME029",
                        ProjectedYearlyCost = 1200,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    }
                };
            }

            if (fuelCategory == "MultiRate")
            {
                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard Economy 7",
                        ServicePlanId = "ME095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard Economy 7",
                        ServicePlanId = "ME095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 2
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard Economy 10",
                        ServicePlanId = "ME029",
                        ProjectedYearlyCost = 1200,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    }
                };
            }

            if (fuelCategory == "Gas")
            {
                return new List<Product>
                {
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard",
                        ServicePlanId = "MG095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Evergreen,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "Standard",
                        ServicePlanId = "MG095",
                        ProjectedYearlyCost = 1000,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 2
                    },
                    new Product
                    {
                        ServicePlanInvoiceDescription = "SSE Smart 1 Year Fixed",
                        ServicePlanId = "MG029",
                        ProjectedYearlyCost = 1200,
                        TariffType = TariffType.Fixed,
                        ExitFee1 = 25.0,
                        RateCode = 1
                    }
                };
            }

            return new List<Product>();
        }

        public static List<Tariff> GetTariffs(string fuelType, string meterType) =>
            GetFakeProducts(fuelType, meterType).GroupBy(t => t.DisplayName).Select(group => new Tariff
            {
                DisplayName = group.OrderByDescending(t => t.ServicePlanInvoiceDescription).First().DisplayName,
                GasProduct = group.FirstOrDefault(g => g.ServicePlanId.StartsWith("MG")),
                ElectricityProduct = group.FirstOrDefault(g => g.ServicePlanId.StartsWith("ME")),
                TariffId = group.FirstOrDefault(g => !string.IsNullOrEmpty(g.ServicePlanId))?.ServicePlanId.Replace("MG", "").Replace("ME", "")
            }).OrderBy(p => p.GetProjectedCombinedYearlyCost()).ToList();

        public static Tariff GetSelectedProduct(string fuelType, string meterType, string tariffId) => GetTariffs(fuelType, meterType).FirstOrDefault(t => t.TariffId == tariffId);

        public static Tariff GetTariffByTariffType(string fuelType, string meterType, TariffType tariffType) => GetTariffs(fuelType, meterType).FirstOrDefault(t => t.ElectricityProduct?.TariffType == tariffType || t.GasProduct?.TariffType == tariffType);
    }
}