namespace Products.Tests.TariffChange.Fakes.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Model.Enums;
    using Model.TariffChange;
    using Models;
    using Repository.TariffChange;

    public class FakeTariffRepository : ITariffRepository
    {
        private readonly Exception _availableTariffException;
        private readonly Exception _currentTariffException;
        private readonly FakeTariffData[] _fakeTariffData;

        public FakeTariffRepository()
        {
            _fakeTariffData = new[]
            {
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Electricity,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC123"
                },
                new FakeTariffData
                {
                    Name = "Tariff 1",
                    FuelType = FuelType.Gas,
                    UnitRate1ExclVat = 15.00,
                    StandingChargeExclVat = 25.00,
                    ServicePlanId = "ABC123"
                }
            };
        }

        public FakeTariffRepository(Exception availableTariffException)
        {
            _availableTariffException = availableTariffException;
        }

        public FakeTariffRepository(Exception currentTariffException, FakeTariffData[] fakeTariffData)
        {
            _currentTariffException = currentTariffException;
            _fakeTariffData = fakeTariffData;
        }

        public FakeTariffRepository(FakeTariffData[] fakeTariffData)
        {
            _fakeTariffData = fakeTariffData;
        }

        public async Task<List<AvailableTariffResult>> GetAvailableSingleRateTariffsAsync(string postcode, string brand, FuelType fuelType, int rateCode)
        {
            if (_availableTariffException != null)
            {
                throw _availableTariffException;
            }

            return await GetAvailableTariffList(fuelType, rateCode);
        }

        public async Task<List<AvailableTariffResult>> GetAvailableMultiRateTariffsAsync(string postcode, string brand, string tariffType, int rateCode)
        {
            if (_availableTariffException != null)
            {
                throw _availableTariffException;
            }

            return await GetAvailableTariffList(FuelType.Electricity, rateCode);
        }

        public async Task<List<AvailableTariffResult>> GetPreservedSingleRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId,
            FuelType fuelType, int rateCode)
        {
            if (_currentTariffException != null)
            {
                throw _currentTariffException;
            }

            return await Task.FromResult(new List<AvailableTariffResult>());
        }

        public async Task<List<AvailableTariffResult>> GetPreservedMultiRateTariffsByServicePlanIdAsync(string postcode, string brand, string servicePlanId,
            string tariffType, int rateCode)
        {
            if (_currentTariffException != null)
            {
                throw _currentTariffException;
            }


            return await Task.FromResult(_fakeTariffData.Where(tariff => tariff.ServicePlanId == servicePlanId).Select(tariffData => new AvailableTariffResult
            {
                TariffType = tariffData.Type,
                MeterCategory = tariffData.MeterCategory,
                EndOfTariffDate = "",
                PriceGuaranteeDate = "",
                TariffEndDateDescription = tariffData.TariffEndDescription,
                PriceGuaranteeDateDescription = tariffData.PriceGuaranteeDescription,
                ExitFee1 = tariffData.ExitFee,
                RateCode = tariffData.RateCode,
                PaymentMethod = tariffData.PaymentMethod,
                RateCodeStandardDescription = tariffData.RateCodeStandardDescription,
                TCR = tariffData.TCR,
                StandingChargeExcVat = tariffData.StandingChargeExclVat,
                StandingChargeInclVAT = tariffData.StandingChargeInclVat,
                UnitRate1ExcVAT = tariffData.UnitRate1ExclVat,
                UnitRate1InclVAT = tariffData.UnitRate1InclVat,
                UnitRate1SPCOBillingDesc = tariffData.UnitRate1SPCOBillingDesc,
                UnitRate2ExcVAT = tariffData.UnitRate2ExclVat,
                UnitRate2InclVAT = tariffData.UnitRate2InclVat,
                UnitRate2SPCOBillingDesc = tariffData.UnitRate2SPCOBillingDesc,
                ServicePlanInvoiceDescription = tariffData.Name,
                LoyaltySchemeUnits = tariffData.LoyaltyItems,
                TariffUniqueId = "",
                ServicePlanID = tariffData.ServicePlanId
            }).ToList());

            //return await Task.FromResult(new List<AvailableTariffResult>());
        }

        private Task<List<AvailableTariffResult>> GetAvailableTariffList(FuelType fuelType, int rateCode)
        {
            return Task.FromResult(_fakeTariffData.Where(tariff => tariff.FuelType == fuelType && tariff.RateCode == rateCode).Select(tariffData =>
                new AvailableTariffResult
                {
                    TariffType = tariffData.Type,
                    MeterCategory = tariffData.MeterCategory,
                    EndOfTariffDate = "",
                    PriceGuaranteeDate = "",
                    TariffEndDateDescription = tariffData.TariffEndDescription,
                    PriceGuaranteeDateDescription = tariffData.PriceGuaranteeDescription,
                    ExitFee1 = tariffData.ExitFee,
                    RateCode = tariffData.RateCode,
                    PaymentMethod = tariffData.PaymentMethod,
                    RateCodeStandardDescription = tariffData.RateCodeStandardDescription,
                    TCR = tariffData.TCR,
                    StandingChargeExcVat = tariffData.StandingChargeExclVat,
                    StandingChargeInclVAT = tariffData.StandingChargeInclVat,
                    UnitRate1ExcVAT = tariffData.UnitRate1ExclVat,
                    UnitRate1InclVAT = tariffData.UnitRate1InclVat,
                    UnitRate1SPCOBillingDesc = tariffData.UnitRate1SPCOBillingDesc,
                    UnitRate2ExcVAT = tariffData.UnitRate2ExclVat,
                    UnitRate2InclVAT = tariffData.UnitRate2InclVat,
                    UnitRate2SPCOBillingDesc = tariffData.UnitRate2SPCOBillingDesc,
                    ServicePlanInvoiceDescription = tariffData.Name,
                    LoyaltySchemeUnits = tariffData.LoyaltyItems,
                    BusinessUseCode = fuelType == FuelType.Electricity ? "ESU" : "GSS",
                    TariffUniqueId = "",
                    ServicePlanID = tariffData.ServicePlanId
                }).ToList());
        }
    }
}