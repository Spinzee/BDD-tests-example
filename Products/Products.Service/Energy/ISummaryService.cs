namespace Products.Service.Energy
{
    using System.Threading.Tasks;
    using Model.Broadband;
    using Model.Energy;

    public interface ISummaryService
    {
        Task ConfirmSale(EnergyCustomer energyCustomer, OpenReachData openReachResponse);
    }
}