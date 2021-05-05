namespace Products.ServiceWrapper.CAndCService
{
    using System.Linq;
    using Core;
    using Model.Energy;
    using Model.Enums;
    using Sse.Retail.CAndC.Client.Models;

    public class CAndCMapper
    {
        public static Model.Energy.MeterDetail ToMeterDetails(MeterDetailsResponse response)
        {
            return new Model.Energy.MeterDetail
            {
                GeographicalArea = response?.Geographicalarea,
                MeterInformation = response?.Meters.Select((meterInfo, index) => new MeterInformation
                {
                    IsPrePay = meterInfo?.Paymenttype?.Equals("Prepay") ?? false,
                    IsInstallerSSE = meterInfo?.InstalledBySse ?? false,
                    ElectricityMeterType = GetMeterType(meterInfo?.Metertype),
                    MeterNumber = meterInfo?.Meternumber,
                    SmartType = GetSmartMeterType(meterInfo?.Smarttype),
                    FuelType = GetFuelType(meterInfo?.Fueltype),
                    MeterSerialNumber = meterInfo?.Meterserialnumber
                }).ToList()
            };
        }

        private static SmartMeterType GetSmartMeterType(string meterInfoSmartType)
        {
            switch (meterInfoSmartType)
            {
                case "Smets1":
                    return SmartMeterType.Smets1;
                case "Smets2":
                    return SmartMeterType.Smets2;
                default:
                    return SmartMeterType.None;
            }
        }

        private static ElectricityMeterType GetMeterType(string meterType)
        {
            switch (meterType)
            {
                case "Economy7":
                    return ElectricityMeterType.Economy7;
                case "Standard":
                    return ElectricityMeterType.Standard;
                case "Other":
                    return ElectricityMeterType.Other;
                default:
                    return ElectricityMeterType.None;
            }
        }

        private static FuelType GetFuelType(string fuelType)
        {
            switch (fuelType)
            {
                case "Electricity":
                    return FuelType.Electricity;
                case "Gas":
                    return FuelType.Gas;
                default:
                    return FuelType.None;
            }
        }
    }
}
