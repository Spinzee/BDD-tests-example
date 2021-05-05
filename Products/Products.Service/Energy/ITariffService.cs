namespace Products.Service.Energy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Energy;

    public interface ITariffService
    {
        Task<List<Product>> GetEnergyProducts(EnergyCustomer customer);

        IEnumerable<Tariff> EnergyTariffs(EnergyCustomer customer, IEnumerable<Product> products, List<CMSEnergyContent> cmsEnergyContents);

        Task<IEnumerable<Tariff>> BundleTariffs(EnergyCustomer customer);
    }
}