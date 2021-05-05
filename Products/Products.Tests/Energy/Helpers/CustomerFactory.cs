namespace Products.Tests.Energy.Helpers
{
    using System;
    using System.Collections.Generic;
    using Broadband.Model;
    using Core;
    using Core.Enums;
    using HomeServices.Helpers;
    using Products.Model.Common;
    using Products.Model.Energy;
    using Products.Model.Enums;
    using Product = Products.Model.Energy.Product;

    public static class CustomerFactory
    {
        public static EnergyCustomer GetCustomerWithFullDetails(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "SSE 1 Year Fixed v15",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit)
        {
            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                SelectedTariff = new Tariff
                {
                    DisplayName = tariffName
                },
                SelectedAddress = GetFakeAddress(),
                Postcode = "RG1 1AA",
                UserId = Guid.NewGuid(),
                SelectedFuelType = fuelType,
                SelectedElectricityMeterType = meterType,
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2()
            };

            if (fuelType != FuelType.Gas)
            {
                energyCustomer.SelectedTariff.ElectricityProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 10.20
                };
            }

            if (fuelType != FuelType.Electricity)
            {
                energyCustomer.SelectedTariff.GasProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 20.20
                };
            }

            return energyCustomer;
        }


        public static EnergyCustomer GetCustomerWithFullBundleDetails(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "SSE 1 Year Fix And Fibre",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit)
        {
            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                SelectedTariff = new Tariff
                {
                    DisplayName = tariffName,
                    BundlePackage = new BundlePackage("100", 15, 22, BundlePackageType.FixAndFibre, new List<TariffTickUsp>()),
                    IsBundle = true
                },
                SelectedAddress = GetFakeAddress(),
                Postcode = "RG1 1AA",
                UserId = Guid.NewGuid(),
                SelectedFuelType = fuelType,
                SelectedElectricityMeterType = meterType,
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2(),
                CLIChoice = new CLIChoice
                {
                    UserProvidedCLI = "12345678900",
                    OpenReachProvidedCLI = "12345678900",
                    KeepExisting = true
                },
                IsBundlingJourney = true,
                SelectedBroadbandProduct = FakeBroadbandProductsData.GetPopulatedFibreBroadbandProduct(),
                IsSSECustomerCLI = true,
                SelectedBroadbandProductCode = "FF3_LR18",
                SelectedBTAddress = new BTAddress
                { ThoroughfareName = "RYE LANE", PostTown = "ALFORD", PremiseName = "ABY HOUSE FARM COTTAGE", Locality = "ABY" }
            };

            if (fuelType != FuelType.Gas)
            {
                energyCustomer.SelectedTariff.ElectricityProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 10.20
                };
            }

            if (fuelType != FuelType.Electricity)
            {
                energyCustomer.SelectedTariff.GasProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 20.20
                };
            }

            return energyCustomer;
        }

        public static EnergyCustomer CustomerDetailsWithFixNFibreBundleTariffChosen(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "Fix And Fibre",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
            bool applyInstallationFee = false)
        {
            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                SelectedTariff = new Tariff
                {
                    DisplayName = tariffName,
                    BundlePackage = new BundlePackage("BundleBBB1", 12, 24, BundlePackageType.FixAndFibre, null),
                    IsBundle = true,
                    IsUpgrade = false,
                    TariffId = "B009"
                },
                SelectedAddress = GetFakeAddress(),
                ApplyInstallationFee = applyInstallationFee,
                Postcode = "RG1 1AA",
                UserId = Guid.NewGuid(),
                SelectedFuelType = fuelType,
                SelectedElectricityMeterType = meterType,
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2()
            };

            if (fuelType != FuelType.Gas)
            {
                energyCustomer.SelectedTariff.ElectricityProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 10.20
                };
            }

            if (fuelType != FuelType.Electricity)
            {
                energyCustomer.SelectedTariff.GasProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 20.20
                };
            }

            return energyCustomer;
        }
        public static EnergyCustomer CustomerDetailsWithFixNFibrePlusBundleTariffChosen(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "Fix And Fibre Plus",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit,
            bool applyInstallationFee = false)
        {
            EnergyCustomer energyCustomer =
                CustomerDetailsWithFixNFibreBundleTariffChosen(fuelType, meterType, tariffName, selectedPaymentMethod, applyInstallationFee);
            energyCustomer.SelectedTariff.IsUpgrade = true;
            energyCustomer.SelectedBroadbandProduct = FakeBroadbandProductsData.GetBroadbandProductWithTalkProducts();
            energyCustomer.SelectedBroadbandProductCode = "FF3_LR18";
            return energyCustomer;
        }
        public static EnergyCustomer CustomerDetailsWithFixAndProtectTariffChosen(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "Fix And Protect",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit)
        {
            var fixNpBundlePackage = new BundlePackage("100", 0.00, 22, BundlePackageType.FixAndProtect, new List<TariffTickUsp>())
            {
                HesMoreInformation = FakeHomeServicesProductStub.GetFakeProducts("BOBC")
            };

            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                SelectedTariff = new Tariff
                {
                    DisplayName = tariffName,
                    TariffGroup = TariffGroup.FixAndProtect,
                    IsBundle = true,
                    TariffId = "HC002",
                    BundlePackage = fixNpBundlePackage
                },

                SelectedAddress = GetFakeAddress(),
                Postcode = "RG1 1AA",
                UserId = Guid.NewGuid(),
                SelectedFuelType = fuelType,
                SelectedElectricityMeterType = meterType,
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2()
            };

            if (fuelType != FuelType.Gas)
            {
                energyCustomer.SelectedTariff.ElectricityProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 10.20
                };
            }

            if (fuelType != FuelType.Electricity)
            {
                energyCustomer.SelectedTariff.GasProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 20.20
                };
            }

            return energyCustomer;
        }

        public static EnergyCustomer CustomerDetailsWithFixAndProtectWithExtraTariffChosen(
            FuelType fuelType,
            ElectricityMeterType meterType,
            string tariffName = "Fix And Protect",
            PaymentMethod selectedPaymentMethod = PaymentMethod.MonthlyDirectDebit)
        {
            var fixNpBundlePackage = new BundlePackage("100", 0.00, 22, BundlePackageType.FixAndProtect, new List<TariffTickUsp>())
            {
                HesMoreInformation = FakeHomeServicesProductStub.GetFakeProducts("ProductWithNoExcessNoExtrasAndNoOffers")
            };

            var energyCustomer = new EnergyCustomer
            {
                SelectedPaymentMethod = selectedPaymentMethod,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails(),
                DirectDebitDetails = GetFakeDirectDebitDetails(),
                SelectedTariff = new Tariff
                {
                    DisplayName = tariffName,
                    TariffGroup = TariffGroup.FixAndProtect,
                    IsBundle = true,
                    BundlePackage = fixNpBundlePackage
                },

                SelectedAddress = GetFakeAddress(),
                Postcode = "RG1 1AA",
                UserId = Guid.NewGuid(),
                SelectedFuelType = fuelType,
                SelectedElectricityMeterType = meterType,
                MeterDetail = FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2(),
                SelectedExtras = new HashSet<Extra>
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
                }
            };

            if (fuelType != FuelType.Gas)
            {
                energyCustomer.SelectedTariff.ElectricityProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 10.20
                };
            }

            if (fuelType != FuelType.Electricity)
            {
                energyCustomer.SelectedTariff.GasProduct = new Product
                {
                    ProjectedYearlyCost = 1200,
                    DirectDebitDiscount = 20.20
                };
            }

            return energyCustomer;
        }

        private static DirectDebitDetails GetFakeDirectDebitDetails()
        {
            return new DirectDebitDetails
            {
                AccountName = "Mr Test",
                AccountNumber = "12345678",
                SortCode = "102030",
                DirectDebitPaymentDate = 28,
                BankName = "SSE Bank"
            };
        }

        private static QasAddress GetFakeAddress()
        {
            return new QasAddress
            {
                HouseName = "123",
                AddressLine1 = "London Road",
                AddressLine2 = "Earley",
                Town = "Reading",
                County = "Berkshire"
            };
        }

        public static EnergyCustomer GetCustomerWithContactAndPersonalDetails()
        {
            return new EnergyCustomer
            {
                SelectedPaymentMethod = PaymentMethod.Quarterly,
                ContactDetails = GetFakeContactDetails(),
                PersonalDetails = GetFakePersonalDetails()
            };
        }

        private static PersonalDetails GetFakePersonalDetails()
        {
            return new PersonalDetails
            {
                Title = "Mr",
                FirstName = "Joe",
                LastName = "Bloggs",
                DateOfBirth = "01 January 1990"
            };
        }

        private static ContactDetails GetFakeContactDetails()
        {
            return new ContactDetails
            {
                ContactNumber = "01212121212",
                EmailAddress = "a@a.com",
                MarketingConsent = true
            };
        }
    }
}