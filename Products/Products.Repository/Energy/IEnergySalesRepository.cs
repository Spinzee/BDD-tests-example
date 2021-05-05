using System.Threading.Tasks;

namespace Products.Repository.Energy
{
    public interface IEnergySalesRepository
    {
        Task<int> GetSubProductIdForFuelType(string product, int baseProductId, bool isPrePay);
    }
}