namespace Products.Tests.TariffChange.Helpers
{
    using System.Collections.Generic;
    using Core;
    using Model.TariffChange.Tariffs;

    public class FakeTariffStub
    {
        public static List<Tariff> GetFakeTariffList()
        {
            return new List<Tariff>
            {
                new Tariff { Name = "SingleFuelElectric", ElectricityDetails = new TariffForFuel { ServicePlanId = "ME001"} },
                new Tariff { Name = "SingleFuelGas", GasDetails = new TariffForFuel { ServicePlanId = "MG001"}  },
                new Tariff { Name = "DualFuel", ElectricityDetails = new TariffForFuel { ServicePlanId = "ME002"}, GasDetails = new TariffForFuel { ServicePlanId = "MG002"}  },
                new Tariff { Name = "Standard", ElectricityDetails = new TariffForFuel { ServicePlanId = "ME003"}, GasDetails = new TariffForFuel { ServicePlanId = "MG003"}  }
            };
        }

        public static List<Tariff> GetFakeFixAndDriveTariffList()
        {
            return new List<Tariff>
            {
                new Tariff { Name = "Fix And Drive", TariffGroup = TariffGroup.FixAndDrive }
            };
        }
    }
}