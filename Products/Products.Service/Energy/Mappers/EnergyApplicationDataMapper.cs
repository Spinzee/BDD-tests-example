namespace Products.Service.Energy.Mappers
{
    using System;
    using System.Linq;
    using Core;
    using Helpers;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Model.Constants;
    using Model.Enums;
    using Products.Model.Common;
    using Products.Model.Energy;
    using Security;

    public class EnergyApplicationDataMapper : IEnergyApplicationDataMapper
    {
        private readonly ICryptographyService _cryptographyService;

        public EnergyApplicationDataMapper(ICryptographyService cryptographyService)
        {
            Guard.Against<ArgumentException>(cryptographyService == null, $"{nameof(cryptographyService)} is null");
            _cryptographyService = cryptographyService;
        }

        public ApplicationData GetEnergyApplicationDataModel(EnergyCustomer energyCustomer, int subProductId)
        {
            string houseName = energyCustomer.SelectedAddress.GetHouseName();
            string houseNumber = energyCustomer.SelectedAddress.GetHouseNumber();
            string formattedPostCode = StringHelper.GetFormattedPostcode(energyCustomer.Postcode);

            return new ApplicationData
            {
                Postcode = formattedPostCode,
                AddressTypeId = (int)AddressTypes.Supply,
                HouseName = houseName,
                HouseNumber = houseNumber,
                AddressLine1 = energyCustomer.SelectedAddress.AddressLine1 ?? string.Empty,
                AddressLine2 = energyCustomer.SelectedAddress.AddressLine2 ?? string.Empty,
                AddressLine3 = energyCustomer.SelectedAddress.County,
                BaseProductId = null,
                PaymentMethodId = (int)energyCustomer.SelectedPaymentMethod,
                SubProductId = null,
                TariffTypeId = GetTariffTypeId(energyCustomer.SelectedFuelType, energyCustomer.SelectedElectricityMeterType),
                SiteDetailsId = SalesData.SiteId,
                AccountName = energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails?.AccountName) : string.Empty, // encrypt
                SortCode = energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails?.SortCode) : string.Empty, // encrypt
                AccountNumber = energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails?.AccountNumber) : string.Empty, // encrypt
                DayInMonthPaymentMade = !energyCustomer.IsDualFuel() && energyCustomer.IsDirectDebit() ? energyCustomer.DirectDebitDetails.DirectDebitPaymentDate : 0,
                MonthlyAmount = GetMonthlyAmount(energyCustomer),
                UserID = energyCustomer.UserId ?? Guid.Empty,
                Email = energyCustomer.ContactDetails.EmailAddress,
                ApplicationStatus = SalesData.ApplicationStatus,
                Title = energyCustomer.PersonalDetails.Title,
                Firstname = energyCustomer.PersonalDetails.FirstName,
                Surname = energyCustomer.PersonalDetails.LastName,
                DayPhone = energyCustomer.ContactDetails.ContactNumber,
                Mobile = string.Empty,
                MarketingConsent = energyCustomer.ContactDetails.MarketingConsent,
                ProductCode = string.Empty,
                Brand = SalesData.Brand,
                CampaignCode = energyCustomer.CampaignCode,
                LoyaltyProduct = string.Empty,
                ReferenceNumbers = string.Empty,
                BaseProductIDsLinkedToReferenceNumbers = string.Empty,
                BillingPostCode = formattedPostCode,
                BillingAddressTypeID = (int)AddressTypes.Billing,
                BillingHouseName = houseName,
                BillingHouseNumber = houseNumber,
                BillingAddressLine1 = energyCustomer.SelectedAddress.AddressLine1,
                BillingAddressLine2 = energyCustomer.SelectedAddress.AddressLine2,
                BillingAddressLine3 = energyCustomer.SelectedAddress.County,
                UseBillAddForMail = SalesData.UseBillAddForMail,
                Town = energyCustomer.SelectedAddress.Town,
                Country = string.Empty,
                BillingTown = energyCustomer.SelectedAddress.Town,
                BillingCountry = string.Empty,
                ElecSubProductID = energyCustomer.IsElectricityOnly() ? subProductId : 0,
                GasSubProductID = energyCustomer.IsGasOnly() ? subProductId : 0,
                TelSubProductID = 0,
                DualSubProductID = energyCustomer.IsDualFuel() ? subProductId : 0,
                ElecSubProductRef = energyCustomer.MeterDetail?.MeterInformation.FirstOrDefault(x => x.FuelType == FuelType.Electricity)?.MeterNumber,
                GasSubProductRef = energyCustomer.MeterDetail?.MeterInformation.FirstOrDefault(x => x.FuelType == FuelType.Gas)?.MeterNumber,
                TelSubProductRef = string.Empty,
                RugbyTeamName = string.Empty,
                ElecProductPresentButNoDual = energyCustomer.IsElectricityOnly(),
                GasProductPresentButNoDual = energyCustomer.IsGasOnly(),
                TelProductPresent = false,
                DualFuelProductPresent = energyCustomer.IsDualFuel(),
                WantToReceivePaperBills = !energyCustomer.IsPaperless(),
                SingleBankAccChosen = energyCustomer.IsPrePay() ? null : (bool?)true,
                GenericPaymentMethodID = energyCustomer.IsDualFuel() ? (int)energyCustomer.SelectedPaymentMethod : (int?)null,
                ElecPaymentMethodID = energyCustomer.HasElectricity() ? (int)energyCustomer.SelectedPaymentMethod : (int?)null,
                GasPaymentMethodID = energyCustomer.HasGas() ? (int)energyCustomer.SelectedPaymentMethod : (int?)null,
                TelPaymentMethodID = null,

                GenericAccountName = energyCustomer.IsDualFuel() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountName) : null,
                GenericSortCode = energyCustomer.IsDualFuel() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.SortCode) : null,
                GenericAccountNumber = energyCustomer.IsDualFuel() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountNumber) : null,

                ElecAccountName = energyCustomer.HasElectricity() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountName) : null,
                ElecSortCode = energyCustomer.HasElectricity() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.SortCode) : null,
                ElecAccountNumber = energyCustomer.HasElectricity() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountNumber) : null,
                ElecDayInMonthPaymentMade = energyCustomer.HasElectricity() && energyCustomer.IsDirectDebit() ? energyCustomer.DirectDebitDetails.DirectDebitPaymentDate : (int?)null,
                ElecMonthlyAmount = energyCustomer.HasElectricity() && energyCustomer.IsDirectDebit() ? Convert.ToInt32(energyCustomer.SelectedTariff?.GetProjectedMonthlyElectricityCost()?.RoundUpToNearestPound()) : (int?)null,

                GasAccountName = energyCustomer.HasGas() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountName) : null,
                GasSortCode = energyCustomer.HasGas() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.SortCode) : null,
                GasAccountNumber = energyCustomer.HasGas() && energyCustomer.IsDirectDebit() ? _cryptographyService.EncryptEnergyValue(energyCustomer.DirectDebitDetails.AccountNumber) : null,
                GasDayInMonthPaymentMade = energyCustomer.HasGas() && energyCustomer.IsDirectDebit() ? energyCustomer.DirectDebitDetails.DirectDebitPaymentDate : (int?)null,
                GasMonthlyAmount = energyCustomer.HasGas() && energyCustomer.IsDirectDebit() ? Convert.ToInt32(energyCustomer.SelectedTariff?.GetProjectedMonthlyGasCost()?.RoundUpToNearestPound()) : (int?)null,

                TelAccountName = null,
                TelSortCode = null,
                TelAccountNumber = null,
                TelDayInMonthPaymentMade = null,
                TelMonthlyAmount = null,
                ConsultantID = energyCustomer.MigrateMemberId ?? string.Empty,
                IsNsc = false,
                DateOfBirth = DateTime.Parse(energyCustomer.PersonalDetails.DateOfBirth),
                AntivirusRequested = null,
                LineSpeed = null,
                SelectedBenefits = false,
                PromotionCode = string.Empty,

                ElectricDayUsage = Convert.ToInt32(energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Economy7 ? energyCustomer.Projection?.EnergyEconomy7DayElecKwh : energyCustomer.Projection?.EnergyAveStandardElecKwh),
                ElectricNightUsage = energyCustomer.SelectedElectricityMeterType == ElectricityMeterType.Economy7 ? Convert.ToInt32(energyCustomer.Projection?.EnergyEconomy7NightElecKwh) : 0,
                GasUsage = energyCustomer.HasGas() ? Convert.ToInt32(energyCustomer.Projection?.EnergyAveStandardGasKwh) : 0,

                ElectricMonetaryAmount = energyCustomer.HasElectricity() ? energyCustomer.SelectedTariff.ElectricityProduct.ProjectedYearlyCost.ToDecimalPoints() : 0,
                GasMonetaryAmount = energyCustomer.HasGas() ? energyCustomer.SelectedTariff.GasProduct.ProjectedYearlyCost.ToDecimalPoints() : (decimal?)null,

                LifeStylePropertyType = 0,
                LifeStylePropertySize = 0,
                LifeStyleOccupancy = 0,

                ReadFrequency = energyCustomer.AskSmartMeterFrequency() ? GetReadFrequency(energyCustomer.SmartMeterFrequency) : string.Empty,
                SmartProducts = energyCustomer.AskSmartMeterFrequency() ? SalesData.SmartProducts : string.Empty,
                SmartServices = energyCustomer.AskSmartMeterFrequency() ? SalesData.SmartServices : string.Empty,

                SmartMeterType = energyCustomer.AskSmartMeterFrequency() ? GetSmartMeter(energyCustomer.GetElectricityMeterInformation()?.SmartType) : SalesData.OtherSmartMeter
            };
        }

        private static int GetTariffTypeId(FuelType selectedFuelType, ElectricityMeterType selectedElectricityMeterType)
        {
            if (selectedFuelType != FuelType.Gas && selectedElectricityMeterType == ElectricityMeterType.Economy7)
            {
                return SalesData.Economy7TariffTypeId;
            }

            return SalesData.StandardTariffTypeId;
        }

        private static int GetMonthlyAmount(EnergyCustomer energyCustomer)
        {
            if (energyCustomer.SelectedFuelType == FuelType.Dual || energyCustomer.SelectedPaymentMethod != PaymentMethod.MonthlyDirectDebit)
            {
                return 0;
            }

            return energyCustomer.SelectedFuelType == FuelType.Gas ?
                (int)(energyCustomer.SelectedTariff.GetProjectedMonthlyGasCost()?.RoundUpToNearestPound() ?? 0) :
                (int)(energyCustomer.SelectedTariff.GetProjectedMonthlyElectricityCost()?.RoundUpToNearestPound() ?? 0);
        }

        private static string GetReadFrequency(SmartMeterFrequency smartMeterFrequency)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (smartMeterFrequency)
            {
                case SmartMeterFrequency.HalfHourly:
                    return "H";
                case SmartMeterFrequency.Daily:
                    return "D";
                case SmartMeterFrequency.Monthly:
                    return "M";
                default:
                    return string.Empty;
            }
        }

        private static string GetSmartMeter(SmartMeterType? smartMeterType)
        {
            switch (smartMeterType)
            {
                case SmartMeterType.None:
                    return SalesData.OtherSmartMeter;
                case SmartMeterType.Smets1:
                    return SmartMeterType.Smets1.ToDescription();
                case SmartMeterType.Smets2:
                    return SmartMeterType.Smets2.ToDescription();
                default:
                    return SalesData.OtherSmartMeter;
            }
        }
    }
}
