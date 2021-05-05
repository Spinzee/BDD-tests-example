namespace Products.ServiceWrapper.QASService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Common;

    public interface IQASServiceWrapper
    {
        Task<QasAddress> GetAddressByMoniker(string moniker);

        Task<List<KeyValuePair<string, string>>> GetAddressByPostcode(string postcode);
    }
}