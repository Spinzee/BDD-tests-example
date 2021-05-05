namespace Products.Tests.Energy.Helpers
{
    using System.Collections.Generic;
    using Core;
    using Products.Model.Energy;
    using Products.Model.Enums;

    public static class FakeCAndCData
    {
        public static MeterDetail GetMeterDetailsWithEmptyGeoCode()
        {
            return new MeterDetail
            {
                GeographicalArea = string.Empty,
                MeterInformation = new List<MeterInformation>()
            };
        }

        public static MeterDetail GetMeterDetailsWithElecPayGoNonSmart()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = true,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.None,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithGasPayGoNonSmart()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Gas,
                        IsPrePay = true,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.None,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithElecPayGoSmartSmets1()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = true,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithGasPayGoSmartSmets1()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Gas,
                        IsPrePay = true,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithElecPayGoSmartSmets2()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = true,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.Smets2,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithElecNonPayGoNonSmart()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = false,
                        SmartType = SmartMeterType.None,
                        MeterNumber = string.Empty
                    }
                }
            };
        }

        public static MeterDetail GetCAndCData()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>()
            };
        }

        public static MeterDetail GetMeterDetailsWithElecOtherMeterTypes()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Other,
                        IsInstallerSSE = true,
                        SmartType = SmartMeterType.None,
                        MeterNumber = "12345667"
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithDualNonPayGoSmartSSEInstalled()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = true,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty,
                        MeterSerialNumber = "111111E"
                    },
                    new MeterInformation
                    {
                        FuelType = FuelType.Gas,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = true,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty,
                        MeterSerialNumber = "111111G"
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithElecNonPayGoSmartSSEInstalled()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Electricity,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = true,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty,
                        MeterSerialNumber = "111111E"
                    }
                }
            };
        }

        public static MeterDetail GetMeterDetailsWithGasNonPayGoSmartSSEInstalled()
        {
            return new MeterDetail
            {
                GeographicalArea = "_H",
                MeterInformation = new List<MeterInformation>
                {
                    new MeterInformation
                    {
                        FuelType = FuelType.Gas,
                        IsPrePay = false,
                        ElectricityMeterType = ElectricityMeterType.Standard,
                        IsInstallerSSE = true,
                        SmartType = SmartMeterType.Smets1,
                        MeterNumber = string.Empty,
                        MeterSerialNumber = "111111G"
                    }
                }
            };
        }


        public static MeterDetail GetMeterDetailsByFuelType(FuelType fuelType)
        {
            if (fuelType == FuelType.Dual)
            {
                return GetMeterDetailsWithDualNonPayGoSmartSSEInstalled();
            }

            if (fuelType == FuelType.Electricity)
            {
                return GetMeterDetailsWithElecNonPayGoSmartSSEInstalled();
            }

            if (fuelType == FuelType.Gas)
            {
                return GetMeterDetailsWithGasNonPayGoSmartSSEInstalled();
            }

            return null;
        }
    }
}