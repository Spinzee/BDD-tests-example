namespace Products.Tests.Broadband.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using ServiceWrapper.BroadbandProductsService;
    using LineSpeed = ServiceWrapper.BroadbandProductsService.LineSpeed;
    using Tariff = ServiceWrapper.BroadbandProductsService.Tariff;

    public enum AddressResult
    {
        AllAddresses,
        NoThoroughfareOnly,
        NoAddressFound,
        Exception
    }

    public enum BroadbandProductsResult
    {
        ShowProducts,
        LineUnsuitable,
        Exception
    }

    public static class FakeBroadbandProductsData
    {
        public static List<BroadbandProduct> GetListOfAllUnavailableProducts()
        {
            var adslProduct = new BroadbandProduct
            {
                BroadbandType = BroadbandType.ADSL,
                ProductOrder = 1,
                IsAvailable = false
            };

            var fibreProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.Fibre,
                ProductOrder = 2,
                IsAvailable = false
            };

            var fibreProduct2 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = false
            };

            var listOfAvailableProducts = new List<BroadbandProduct> { adslProduct, fibreProduct1, fibreProduct2 };

            return listOfAvailableProducts;

        }

        public static List<BroadbandProduct> GetListOfAvailableProductsFromSession()
        {
            var adslProduct = new BroadbandProduct
            {
                BroadbandType = BroadbandType.ADSL,
                ProductOrder = 1,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "AP19",
                        ProductCode = "BB_AP18",
                        ProductName = "Unlimited Bbad 18 + Anytime Plus 18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        }
                    },
                    new TalkProduct
                    {
                        TalkCode = "AL19",
                        ProductCode = "BB_ANY18",
                        ProductName = "Unlimited Bbad 18 + Anytime Landline 18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 7 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        }
                    },
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "BB18_LR",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        }
                    }
                },
                LineSpeed = new ADSLLineSpeeds
                {
                    Min = "8000",
                    Max = "12000"
                }
            };

            var fibreProduct1 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.Fibre,
                ProductOrder = 2,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FIBRE18_LR",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 9},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        }
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "18000",
                    MaxDownload = "22000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var fibreProduct2 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.Fibre,
                ProductOrder = 2,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "AL19",
                        ProductCode = "FIBRE_ANY18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 9},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 7}
                        }
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "18000",
                    MaxDownload = "22000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var fibreProduct3 = new BroadbandProduct
            {
                BroadbandType = BroadbandType.Fibre,
                ProductOrder = 2,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "AP19",
                        ProductCode = "FIBRE_AP18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 9},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12}
                        }
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "18000",
                    MaxDownload = "22000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var fibrePlusProduct = new BroadbandProduct
            {
                BroadbandType = BroadbandType.FibrePlus,
                ProductOrder = 3,
                IsAvailable = true,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        TalkCode = "LRO",
                        ProductCode = "FP18_LR",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 35},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 16},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                            new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0}
                        }
                    }
                },
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                }
            };

            var listOfAvailableProducts = new List<BroadbandProduct>
            {
                adslProduct,
                fibreProduct1,
                fibreProduct2,
                fibreProduct3,
                fibrePlusProduct
            };

            return listOfAvailableProducts;
        }

        public static List<Tariff> GetAllTariffs()
        {
            var productCatalogue = new ProductCatalogue();
            ProductsResponse response = productCatalogue.Read();
            return response.BroadbandProducts.Broadband.Brand.Tariffs.ToList();
        }

        public static ProductsResponse GetBroadbandProducts(BroadbandProductsResult broadbandProductsResult, FakeLineSpeed fakeLineSpeed)
        {
            var broadbandProductsResponse = new ProductsResponse();
            switch (broadbandProductsResult)
            {
                case BroadbandProductsResult.ShowProducts:
                    broadbandProductsResponse = ShowProducts(fakeLineSpeed);
                    break;
                case BroadbandProductsResult.LineUnsuitable:
                    broadbandProductsResponse.BroadbandProducts = new BroadbandProducts
                    {
                        Broadband = new Broadband
                        {
                            Brand = new Brand
                            {
                                Tariffs = new Tariff[] { }
                            }
                        }
                    };
                    break;
                case BroadbandProductsResult.Exception:
                    throw new Exception();
                default:
                    throw new ArgumentOutOfRangeException(nameof(broadbandProductsResult), broadbandProductsResult,
                        null);
            }

            return broadbandProductsResponse;
        }

        public static ProductsResponse ShowProducts(FakeLineSpeed fakeLineSpeed)
        {
            var productCatalogue = new ProductCatalogue();
            ProductsResponse response = productCatalogue.Read();

            if (fakeLineSpeed == null)
            {
                fakeLineSpeed = new FakeLineSpeed
                {
                    ADSLMaxSpeed = "6000",
                    ADSLMinSpeed = "3000",
                    FibreMinDownloadSpeed = "15200",
                    FibreMaxDownloadSpeed = "63600",
                    FibreMinUploadSpeed = "39800",
                    FibreMaxUploadSpeed = "20000"
                };
            }

            var broadbandProduct = new ProductsResponse
            {
                BroadbandProducts = new BroadbandProducts
                {

                    Broadband = new Broadband
                    {
                        Brand = new Brand
                        {
                            BrandId = "SSE",
                            Tariffs = response.BroadbandProducts.Broadband.Brand.Tariffs
                        }
                    }
                },

                LineSpeeds = fakeLineSpeed.GetLineSpeeds().ToArray()
            };

            return broadbandProduct;
        }

        public static List<BTAddress> GetAddresses(AddressResult addressResult)
        {
            switch (addressResult)
            {
                case AddressResult.AllAddresses:
                    return DefaultAddressList();
                case AddressResult.NoThoroughfareOnly:
                    var addresses = new List<BTAddress>
                    {
                        new BTAddress
                        {
                            SubPremises = "",
                            PremiseName = "",
                            ThoroughfareNumber = "",
                            ThoroughfareName = "Waterloo Road",
                            PostTown = "Havant",
                            Postcode = "PO9 1BH"
                        }
                    };
                    return addresses;                                    
                case AddressResult.NoAddressFound:
                    return new List<BTAddress>();
                case AddressResult.Exception:
                    throw new Exception();
                default:
                    return DefaultAddressList();
            }
        }

        private static List<BTAddress> DefaultAddressList()
        {
            var addresses = new List<BTAddress>
            {
                new BTAddress
                {
                    SubPremises = "",
                    PremiseName = "",
                    ThoroughfareNumber = "21",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Locality = "Hampshire",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "",
                    PremiseName = "Masonic Lodge",
                    ThoroughfareNumber = "",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "",
                    PremiseName = "Waterloo Hall",
                    ThoroughfareNumber = "",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "West Suite",
                    PremiseName = "Waterloo Hall",
                    ThoroughfareNumber = "",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "1",
                    PremiseName = "Steyning Terrace",
                    ThoroughfareNumber = "",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "3",
                    PremiseName = "Steyning Terrace",
                    ThoroughfareNumber = "",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "",
                    PremiseName = "Test House",
                    ThoroughfareNumber = "22",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                },
                new BTAddress
                {
                    SubPremises = "East",
                    PremiseName = "",
                    ThoroughfareNumber = "26",
                    ThoroughfareName = "Waterloo Road",
                    PostTown = "Havant",
                    Postcode = "PO9 1BH"
                }
            };

            return addresses;
        }

        public static BroadbandJourneyDetails GetCompleteBroadbandJourneyDetailsForBroadbandProductGroup(BroadbandProductGroup broadbandProductGroup)
        {
            BroadbandJourneyDetails journeyDetails = GetCompleteBroadbandJourneyDetails();
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (broadbandProductGroup)
            {
                case BroadbandProductGroup.FixAndFibreV3:
                    journeyDetails.Customer.SelectedProduct = new BroadbandProduct
                    {
                        BroadbandCode = "",
                        BroadbandType = BroadbandType.Fibre,
                        LineSpeed = new FibreLineSpeeds
                        {
                            MinDownload = "18000",
                            MaxDownload = "22000",
                            MinUpload = "8000",
                            MaxUpload = "12000"
                        },
                        ProductOrder = 2,
                        TalkProducts = new List<TalkProduct>
                        {
                            new TalkProduct
                            {
                                ProductCode = "FF3_ANY18",
                                ProductName = "Fix and Fibre V3 Broadband + Anytime (FF)",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12},
                                    new BroadbandPrice
                                        {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2}
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "FF3_AP18",
                                ProductName = "Fix and Fibre V3 Broadband + Anytime Plus (FF)",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 7},
                                    new BroadbandPrice
                                        {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2}
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "FF3_LR18",
                                ProductName = "Fix and Fibre V3 Broadband + Line Rental Only (FF)",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0},
                                    new BroadbandPrice
                                        {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2}
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "FF3_EAW18",
                                ProductName = "Fix and Fibre Broadband + Evening & Weekend (FF)",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0},
                                    new BroadbandPrice
                                        {FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19},
                                    new BroadbandPrice {FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2}
                                }
                            }
                        }
                    };
                    break;
            }

            return journeyDetails;
        }

        public static BroadbandJourneyDetails GetCompleteBroadbandJourneyDetails()
        {
            var sessionObject = new BroadbandJourneyDetails
            {
                Customer =
                new Customer {
                    UserId = new Guid("bf1d2e09-0d4a-4265-a6fa-d3d06446aba5"),
                    CliNumber = "02081231234",
                    OriginalCliEntered = "02081231234",
                    ContactDetails = new ContactDetails { ContactNumber = "079999999", EmailAddress = "Test@test.com", MarketingConsent = true },
                    DirectDebitDetails = new DirectDebitDetails {
                        AccountName = "TestAccountName",
                        AccountNumber = "12345678",
                        SortCode = "102030",
                        BankAddressLine1 = "BigBankAddressLine1",
                        BankAddressLine2 = "BigBankAddressLine2",
                        BankAddressLine3 = "BigBankAddressLine3",
                        BankAddressLine4 = "BigBankAddressLine4",
                        BankName = "The Big Bank",
                        Postcode = "W1 1PH"
                    },
                    IsSSECustomer = true,
                    PersonalDetails = new PersonalDetails { DateOfBirth = "01/02/1979", FirstName = "TestFirstName", LastName = "TestLastName", Title = "Ms"  },
                    PostcodeEntered = "PO9 1QH",
                    SelectedProductCode = "FIBRE18_LR",
                    SelectedAddress = new BTAddress
                    {
                        ThoroughfareNumber = "9",
                        ThoroughfareName = "Penner Road",
                        PostTown = "Havant",
                        Postcode = "Po9 1QH"
                    },
                    SelectedProduct = new BroadbandProduct
                    {
                        BroadbandCode = "",
                        BroadbandType = BroadbandType.Fibre,
                        LineSpeed = new FibreLineSpeeds
                        {
                            MinDownload = "18000",
                            MaxDownload = "22000",
                            MinUpload = "8000",
                            MaxUpload = "12000"
                        },
                        ProductOrder = 2,
                        TalkProducts = new List<TalkProduct>
                        {
                            new TalkProduct
                            {
                                ProductCode = "FIBRE_AP18",
                                ProductName = "Unlimited Bbad 18 + Anytime Plus 18",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "FIBRE_ANY18",
                                ProductName = "Unlimited Bbad 18 + Anytime Landline 18",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 7 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "FIBRE18_LR",
                                ProductName = "Unlimited Bbad 18 + Line Rental Only",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            }
                        }
                    }
                }
            };

            return sessionObject;
        }

        public static BroadbandJourneyDetails GetCompleteBroadbandJourneyDetailsWithNoCLI()
        {
            var sessionObject = new BroadbandJourneyDetails
            {
                Customer =
                new Customer {
                    UserId = new Guid("bf1d2e09-0d4a-4265-a6fa-d3d06446aba5"),
                    ContactDetails = new ContactDetails { ContactNumber = "079999999", EmailAddress = "Test@test.com", MarketingConsent = true },
                    DirectDebitDetails = new DirectDebitDetails {
                        AccountName = "TestAccountName",
                        AccountNumber = "12345678",
                        SortCode = "102030",
                        BankAddressLine1 = "BigBankAddressLine1",
                        BankAddressLine2 = "BigBankAddressLine2",
                        BankAddressLine3 = "BigBankAddressLine3",
                        BankAddressLine4 = "BigBankAddressLine4",
                        BankName = "The Big Bank",
                        Postcode = "W1 1PH"
                    },
                    IsSSECustomer = true,
                    PersonalDetails = new PersonalDetails { DateOfBirth = "01/02/1979", FirstName = "TestFirstName", LastName = "TestLastName", Title = "Ms"  },
                    PostcodeEntered = "PO9 1QH",
                    SelectedProductCode = "BB18_LR",
                    SelectedAddress = new BTAddress
                    {
                        ThoroughfareNumber = "9",
                        ThoroughfareName = "Penner Road",
                        PostTown = "Havant",
                        Postcode = "Po9 1QH"
                    },
                    SelectedProduct = new BroadbandProduct
                    {
                        BroadbandCode = "",
                        BroadbandType = BroadbandType.Fibre,
                        LineSpeed = new FibreLineSpeeds
                        {
                            MinDownload = "18000",
                            MaxDownload = "22000",
                            MinUpload = "8000",
                            MaxUpload = "12000"
                        },
                        ProductOrder = 2,
                        TalkProducts = new List<TalkProduct>
                        {
                            new TalkProduct
                            {
                                ProductCode = "BB_AP18",
                                ProductName = "Unlimited Bbad 18 + Anytime Plus 18",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "BB_ANY18",
                                ProductName = "Unlimited Bbad 18 + Anytime Landline 18",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 7 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            },
                            new TalkProduct
                            {
                                ProductCode = "BB18_LR",
                                ProductName = "Unlimited Bbad 18 + Line Rental Only",
                                Prices = new List<BroadbandPrice>
                                {
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 19 },
                                    new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                                }
                            }
                        }
                    }
                }
            };

            return sessionObject;
        }

        public static BroadbandProduct GetPopulatedFibreBroadbandProduct()
        {
            return new BroadbandProduct
            {
                BroadbandType = BroadbandType.Fibre,
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "28000",
                    MaxDownload = "72000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                },
                TalkProducts = new List<TalkProduct>()
            };
        }

        public static BroadbandProduct GetBroadbandProductWithTalkProducts()
        {
            return new BroadbandProduct
            {
                BroadbandCode = "",
                BroadbandType = BroadbandType.Fibre,
                LineSpeed = new FibreLineSpeeds
                {
                    MinDownload = "18000",
                    MaxDownload = "22000",
                    MinUpload = "8000",
                    MaxUpload = "12000"
                },
                ProductOrder = 2,
                TalkProducts = new List<TalkProduct>
                {
                    new TalkProduct
                    {
                        ProductCode = "FF3_EAW18",
                        ProductName = "Unlimited Bbad 18 + Anytime Plus 18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 4 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 0 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        },
                        TalkCode = "EAW19"
                    },
                    new TalkProduct
                    {
                        ProductCode = "FF3_LR18",
                        ProductName = "Fix and Fibre Broadband + Line Rental Only (FF)",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 0 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 0 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        },
                        TalkCode = "LRO"
                    },
                    new TalkProduct
                    {
                        ProductCode = "FF3_ANY18",
                        ProductName = "Unlimited Bbad 18 + Anytime Landline 18",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 8 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 28 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 0 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        },
                        TalkCode = "ANY18"
                    },
                    new TalkProduct
                    {
                        ProductCode = "FF3_AP18",
                        ProductName = "Unlimited Bbad 18 + Line Rental Only",
                        Prices = new List<BroadbandPrice>
                        {
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyTalkCharge, Price = 12 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.HeadlinePricePaperlessBilling, Price = 21 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 0 },
                            new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyBroadbandCharge, Price = 2 }
                        },
                        TalkCode = "AP19"
                    }
                }
            };
        }

        public class FakeLineSpeed
        {
            public string ADSLMinSpeed { get; set; }

            public string ADSLMaxSpeed { get; set; }

            public string FibreMinDownloadSpeed { get; set; }

            public string FibreMaxDownloadSpeed { get; set; }

            public string FibreMinUploadSpeed { get; set; }

            public string FibreMaxUploadSpeed { get; set; }

            public List<LineSpeed> GetLineSpeeds()
            {
                return new List<LineSpeed>
                {
                    new LineSpeed
                    {
                        MaxSpeed = ADSLMaxSpeed,
                        MinSpeed = ADSLMinSpeed,
                        Type = "adsl"
                    },
                    new LineSpeed
                    {
                        MaxDownload = FibreMaxDownloadSpeed,
                        MaxUpload = FibreMaxUploadSpeed,
                        MinUpload = FibreMinUploadSpeed,
                        MinDownload = FibreMinDownloadSpeed,
                        Type = "fibre"
                    }
                };
            }
        }
    }

    public static class FakeOpenReachData
    {
        public static OpenReachData GetOpenReachData()
        {
            return new OpenReachData
            {
                LineavailabilityFlags = new LineAvailability
                {
                    BackOfficeFile = true
                },
                AddressLineKey = "Ljk12345",
                LineStatus = LineStatus.NewConnection
            };
        }
    }
}
