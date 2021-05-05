namespace Products.Tests.Energy.Fakes.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.Energy;
    using ServiceWrapper.EnergyProjectionService;

    public class FakeEnergyProjectionServiceWrapper : IEnergyProjectionServiceWrapper
    {
        public bool ThrowException { get; set; }

        public async Task<List<EnergyMultiplier>> GetMultipliers()
        {
            await Task.Delay(1);
            if (ThrowException)
            {
                throw new Exception("EnergyProjectionService Exception");
            }

            return new List<EnergyMultiplier>
            {
                new EnergyMultiplier
                {
                    Value = "Mid Terrace",
                    MultiplierGas = 0.8865248,
                    MultiplierElec = 0.9112366,
                    MultiplierType = "PropertyType",
                    Order = 4
                },
                new EnergyMultiplier
                {
                    Value = "1",
                    MultiplierGas = 0.8081875,
                    MultiplierElec = 0.8439716,
                    MultiplierType = "Occupancy",
                    Order = 1
                },
                new EnergyMultiplier
                {
                    Value = "2 Bedrooms",
                    MultiplierGas = 0.8926172,
                    MultiplierElec = 0.8014184,
                    MultiplierType = "PropertySize",
                    Order = 3
                }
            };
        }

        public async Task<CumulativeEnergyMultiplier> GetCumulativeEnergyMultiplier(IEnumerable<EnergyMultiplier> selectedMultipliers)
        {
            await Task.Delay(1);
            if (ThrowException)
            {
                throw new Exception("EnergyProjectionService Exception");
            }

            return new CumulativeEnergyMultiplier
            {
                MultiplierGas = 3.2836879000000003,
                MultiplierElec = 3.1422073999999998
            };
        }

        public async Task<Projection> GetProjection(CumulativeEnergyMultiplier cumulativeMultiplier, string postCode)
        {
            await Task.Delay(1);
            if (ThrowException)
            {
                throw new Exception("EnergyProjectionService Exception");
            }

            return new Projection
            {
                EnergyAveStandardElecKwh = 11614.0,
                EnergyAveStandardGasKwh = 46737.0,
                EnergyAveEcon7ElecKwh = 16792.0,
                MSOA = "E02004775",
                LSOA = "E01022953"
            };
        }
    }
}