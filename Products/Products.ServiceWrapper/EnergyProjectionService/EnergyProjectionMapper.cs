namespace Products.ServiceWrapper.EnergyProjectionService
{
    using System.Collections.Generic;
    using System.Linq;
    using Model.Energy;
    using Sse.Projection.Client.Models;

    public class EnergyProjectionMapper
    {
        public static List<EnergyMultiplier> ToEnergyMultiplierList(IList<Multiplier> multipliers)
        {
            return multipliers.Select((multiplier, index) => new EnergyMultiplier
            {
                MultiplierElec = multiplier?.MultiplierElec,
                MultiplierGas = multiplier?.MultiplierGas,
                MultiplierType = multiplier?.Multipliertype,
                Value = multiplier?.Value,
                Order = multiplier?.Order
            }).ToList();
        }

        public static CumulativeEnergyMultiplier ToCumulativeEnergyMultiplier(Multiplier response)
        {
            return new CumulativeEnergyMultiplier
            {
                MultiplierElec = response?.MultiplierElec,
                MultiplierGas = response?.MultiplierGas
            };
        }

        public static Projection ToProjection(Usage response)
        {
            return new Projection
            {
                EnergyAveEcon7ElecKwh = response?.EnergyAveEcon7ElecKwh,
                EnergyAveStandardElecKwh = response?.EnergyAveStandardElecKwh,
                EnergyAveStandardGasKwh = response?.EnergyAveGasKwh,
                LSOA = response?.Lsoa,
                MSOA = response?.Msoa
            };
        }

        public static IList<Multiplier> ToAPIMultiplierList(IEnumerable<EnergyMultiplier> multipliers)
        {
            return multipliers.Select(multiplier => new Multiplier
            {
                // ReSharper disable once PossibleInvalidOperationException
                MultiplierElec = multiplier.MultiplierElec.Value,
                // ReSharper disable once PossibleInvalidOperationException
                MultiplierGas = multiplier.MultiplierGas.Value,
                Multipliertype = multiplier.MultiplierType,
                Value = multiplier.Value,
                Order = multiplier.Order ?? 0
            }).ToList();
        }
    }
}