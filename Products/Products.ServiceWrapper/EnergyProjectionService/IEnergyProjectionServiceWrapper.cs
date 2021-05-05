namespace Products.ServiceWrapper.EnergyProjectionService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Energy;

    public interface IEnergyProjectionServiceWrapper
    {
        Task<List<EnergyMultiplier>> GetMultipliers();

        Task<CumulativeEnergyMultiplier> GetCumulativeEnergyMultiplier(IEnumerable<EnergyMultiplier> selectedMultipliers);

        Task<Projection> GetProjection(CumulativeEnergyMultiplier cumulativeEnergyMultiplier, string postCode);
    }
}