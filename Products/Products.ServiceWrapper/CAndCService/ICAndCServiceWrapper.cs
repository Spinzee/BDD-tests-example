
namespace Products.ServiceWrapper.CAndCService
{
    using Model.Common;
    using Model.Energy;
    using System.Threading.Tasks;

    public interface ICAndCServiceWrapper
    {
        Task<MeterDetail> GetMeterDetail(string postcode, QasAddress customerAddress);
    }
}