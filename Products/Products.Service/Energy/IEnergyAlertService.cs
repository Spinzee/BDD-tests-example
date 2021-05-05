namespace Products.Service.Energy
{
    using System.Threading.Tasks;

    public interface IEnergyAlertService
    {
        Task<bool> IsOurPricesCustomerAlert();

        Task<bool> IsEnergyCustomerAlert();
    }
}
