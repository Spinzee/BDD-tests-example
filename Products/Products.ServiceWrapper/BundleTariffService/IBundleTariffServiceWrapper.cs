namespace Products.ServiceWrapper.BundleTariffService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Energy;

    public interface IBundleTariffServiceWrapper
    {
        Task<List<Bundle>> GetDualMultiRateElectricBundles(BundleRequest request);

        Task<List<Bundle>> GetDualSingleRateElectricBundles(BundleRequest request);

        Task<List<Bundle>> GetSingleRateElectricBundles(BundleRequest request);

        Task<List<Bundle>> GetMultiRateElectricBundles(BundleRequest request);

        Task<List<Bundle>> GetSingleRateGasBundles(BundleRequest request);
    }
}