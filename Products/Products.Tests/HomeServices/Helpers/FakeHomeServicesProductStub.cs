namespace Products.Tests.HomeServices.Helpers
{
    using System.Collections.Generic;
    using Products.Model.HomeServices;

    public class FakeHomeServicesProductStub
    {
        public static ProductGroup GetFakeProducts(string productCode)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (productCode == "BOBC")
            {
                return new ProductGroup
                {
                    Name = "Products With Extras And Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                            {
                                Cost = 10.0,
                                Id = 1,
                                Name = "Extra1",
                                ProductCode = "EC"
                            }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover",
                        Excess = 90,
                        FullOfferText = "with £0 excess SSE gas customers get £55 credit on their account",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 2,
                            Description = "Offer summary"
                        },
                        ProductCode = "BOBC",
                        UpsellOfferText = "Up sell offer text",
                        OfferSummaryText = "If this applies to you, your gas account will be credited in the next 46 calendar days of the cancellation period being complete.",
                        ContractLength = 12.0,
                        CoverBulletText = ""
                    }
                    }
                };
            }

            if (productCode == "BOHC")
            {
                return new ProductGroup
                {
                    Name = "Products With Extras And Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover",
                            Excess = 90,
                            FullOfferText = "with £0 excess SSE gas customers get £55 credit on their account",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 2,
                                Description = "Offer summary"
                            },
                            ProductCode = "BOHC",
                            UpsellOfferText = "Up sell offer text",
                            OfferSummaryText = "If this applies to you, your gas account will be credited in the next 46 calendar days of the cancellation period being complete.",
                            ContractLength = 12.0,
                            CoverBulletText = ""
                        }
                    }
                };
            }

            if (productCode == "BC")
            {
                return new ProductGroup
                {
                    Name = "Product With No Excess No Extras And  No Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover 1",
                        Excess = 0,
                        FullOfferText = "",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 0,
                            Description = ""
                        },
                        ProductCode = "BC",
                        UpsellOfferText = "",
                        ContractLength = 12,
                        OfferSummaryText =  "",
                        CoverBulletText = ""
                    },
                        new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover 2",
                            Excess = 0,
                            FullOfferText = "",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 0,
                                Description = ""
                            },
                            ProductCode = "BC50",
                            UpsellOfferText = "",
                            ContractLength = 12,
                            OfferSummaryText =  "",
                            CoverBulletText = ""
                        }
                    }
                };
            }

            if (productCode == "BC50")
            {
                return new ProductGroup
                {
                    Name = "Product With One Excess And Extras And Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover 2",
                        Excess = 90,
                        FullOfferText = "with £0 excess SSE gas customers get £55 credit on their account 90",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 90,
                            Description = "with £0 excess SSE gas customers get £55 credit on their account 90"
                        },
                        ProductCode = "BC50",
                        UpsellOfferText = "Up sell offer text 90",
                        ContractLength = 12,
                        CoverBulletText = ""

                    },
                        new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover 2",
                            Excess = 90,
                            FullOfferText = "with £0 excess SSE gas customers get £55 credit on their account 90",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 90,
                                Description = "with £0 excess SSE gas customers get £55 credit on their account 90"
                            },
                            ProductCode = "BC",
                            UpsellOfferText = "Up sell offer text 90",
                            ContractLength = 12,
                            CoverBulletText = ""
                        }
                    }
                };
            }

            if (productCode == "HC")
            {
                return new ProductGroup
                {
                    Name = "Product With No Excess Multiple Extras And  No Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover 1",
                        Excess = 0,
                        FullOfferText = "",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 0,
                            Description = ""
                        },
                        ProductCode = "HC",
                        UpsellOfferText = "",
                        ContractLength = 12,
                        OfferSummaryText =  "",
                        CoverBulletText = ""
                    },
                        new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover 1",
                            Excess = 0,
                            FullOfferText = "",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 0,
                                Description = ""
                            },
                            ProductCode = "HC50",
                            UpsellOfferText = "",
                            ContractLength = 12,
                            OfferSummaryText =  "",
                            CoverBulletText = ""
                        }
                    }
                };
            }


            if (productCode == "HC50")
            {
                return new ProductGroup
                {
                    Name = "Product With No Excess Multiple Extras And  No Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover 1",
                        Excess = 0,
                        FullOfferText = "",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 0,
                            Description = ""
                        },
                        ProductCode = "HC",
                        UpsellOfferText = "",
                        ContractLength = 12,
                        OfferSummaryText =  "",
                        CoverBulletText = ""
                    },
                        new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover 1",
                            Excess = 0,
                            FullOfferText = "",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 0,
                                Description = ""
                            },
                            ProductCode = "HC50",
                            UpsellOfferText = "",
                            ContractLength = 12,
                            OfferSummaryText =  "",
                            CoverBulletText = ""
                        }
                    }
                };
            }

            if (productCode == "LANDHC")
            {
                return new ProductGroup
                {
                    Name = "Product With No Excess Multiple Extras And  No Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Boiler cover 1",
                        Excess = 0,
                        FullOfferText = "",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 0,
                            Description = ""
                        },
                        ProductCode = "LANDHC",
                        UpsellOfferText = "",
                        ContractLength = 12,
                        OfferSummaryText =  "",
                        CoverBulletText = ""
                    }
                    }
                };
            }

            if (productCode == "LANDBC")
            {
                return new ProductGroup
                {
                    Name = "Product With No Excess Multiple Extras And  No Offers",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                        {
                            MonthlyCost = 20.0,
                            Description = "Boiler cover 1",
                            Excess = 0,
                            FullOfferText = "",
                            Id = 11,
                            OfferSummary = new OfferSummary
                            {
                                Amount = 0,
                                Description = ""
                            },
                            ProductCode = "LANDBC",
                            UpsellOfferText = "",
                            ContractLength = 12,
                            OfferSummaryText =  "",
                            CoverBulletText = ""
                        }
                    }
                };
            }

            if (productCode == "EC")
            {
                return new ProductGroup
                {
                    Name = "Electric wiring cover with cover bullet text ",
                    WhatsExcluded = new List<string> { "Exclude1", "Exclude2" },
                    WhatsIncluded = new List<string> { "Include1", "Include2" },
                    Extras = new List<ProductExtra>
                    {
                        new ProductExtra
                        {
                            Cost = 10.0,
                            Id = 1,
                            Name = "Extra1",
                            ProductCode = "EC"
                        }
                    },
                    Products = new List<Product>
                    { new Product
                    {
                        MonthlyCost = 20.0,
                        Description = "Electric Wiring cover",
                        Excess = 90,
                        FullOfferText = "with £0 excess SSE gas customers get £55 credit on their account 90",
                        Id = 11,
                        OfferSummary = new OfferSummary
                        {
                            Amount = 90,
                            Description = "with £0 excess SSE gas customers get £55 credit on their account 90"
                        },
                        ProductCode = "EC",
                        UpsellOfferText = "Up sell offer text 90",
                        ContractLength = 12,
                        CoverBulletText = "coverBulletText"
                    }}
                };
            }

            if (productCode == "InvalidProductCode")
            {
                return null;
            }

            return new ProductGroup();
        }
    }
}
