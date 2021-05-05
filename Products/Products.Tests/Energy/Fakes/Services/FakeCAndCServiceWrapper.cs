namespace Products.Tests.Energy.Fakes.Services
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Products.Model.Common;
    using Products.Model.Energy;
    using ServiceWrapper.CAndCService;

    public class FakeCAndCServiceWrapper : ICAndCServiceWrapper
    {
        public bool ThrowException { get; set; }

        public CAndCTestDataScenario TestDataScenario { get; set; }

        public Task<MeterDetail> GetMeterDetail(string postcode, QasAddress customerAddress)
        {
            if (ThrowException)
            {
                throw new Exception("C&C exception");
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (TestDataScenario)
            {
                case CAndCTestDataScenario.ElecPayGoNonSmart:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithElecPayGoNonSmart());
                case CAndCTestDataScenario.ElecPayGoSmartSmets1:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets1());
                case CAndCTestDataScenario.ElecPayGoSmartSmets2:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithElecPayGoSmartSmets2());
                case CAndCTestDataScenario.ElecNonPayGoNonSmart:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithElecNonPayGoNonSmart());
                case CAndCTestDataScenario.ElecNonPayGoSmartSmets1:
                    break;
                case CAndCTestDataScenario.ElecNonPayGoSmartSmets2:
                    break;
                case CAndCTestDataScenario.GasPayGoNonSmart:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithGasPayGoNonSmart());
                case CAndCTestDataScenario.GasPayGoSmartSmets1:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithGasPayGoSmartSmets1());
                case CAndCTestDataScenario.GasPayGoSmartSmets2:
                    break;
                case CAndCTestDataScenario.GasNonPayGoNonSmart:
                    break;
                case CAndCTestDataScenario.GasNonPayGoSmartSmets1:
                    break;
                case CAndCTestDataScenario.GasNonPayGoSmartSmets2:
                    break;
                case CAndCTestDataScenario.DualPayGoNonSmart:
                    break;
                case CAndCTestDataScenario.DualPayGoSmartSmets1:
                    break;
                case CAndCTestDataScenario.DualPayGoSmartSmets2:
                    break;
                case CAndCTestDataScenario.DualNonPayGoNonSmart:
                    break;
                case CAndCTestDataScenario.DualNonPayGoSmartSmets1:
                    break;
                case CAndCTestDataScenario.DualNonPayGoSmartSmets2:
                    break;
                case CAndCTestDataScenario.EmptyGeoCode:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithEmptyGeoCode());
                case CAndCTestDataScenario.OtherMeterTypes:
                    return Task.FromResult(FakeCAndCData.GetMeterDetailsWithElecOtherMeterTypes());
                default:
                    return Task.FromResult(FakeCAndCData.GetCAndCData());
            }
            return Task.FromResult(FakeCAndCData.GetCAndCData());
        }

        public enum CAndCTestDataScenario
        {
            DefaultData,
            EmptyGeoCode,
            OtherMeterTypes,

            ElecPayGoNonSmart,
            ElecPayGoSmartSmets1,
            ElecPayGoSmartSmets2,
            ElecNonPayGoNonSmart,
            ElecNonPayGoSmartSmets1,
            ElecNonPayGoSmartSmets2,

            GasPayGoNonSmart,
            GasPayGoSmartSmets1,
            GasPayGoSmartSmets2,
            GasNonPayGoNonSmart,
            GasNonPayGoSmartSmets1,
            GasNonPayGoSmartSmets2,

            DualPayGoNonSmart,
            DualPayGoSmartSmets1,
            DualPayGoSmartSmets2,
            DualNonPayGoNonSmart,
            DualNonPayGoSmartSmets1,
            DualNonPayGoSmartSmets2
        }
    }
}
