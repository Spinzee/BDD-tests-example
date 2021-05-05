namespace Products.Service.Energy.Mappers
{
    using Model.Common;
    using Model.Energy;

    public interface IEnergyApplicationDataMapper
    {
        ApplicationData GetEnergyApplicationDataModel(EnergyCustomer energyCustomer, int subProductId);
    }
}