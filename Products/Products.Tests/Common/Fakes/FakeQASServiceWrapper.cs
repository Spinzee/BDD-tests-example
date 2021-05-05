using Products.Model.Common;
using Products.ServiceWrapper.QASService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.Tests.Common.Fakes.Services
{
    public class FakeQASServiceWrapper : IQASServiceWrapper
    {
        public bool ThrowException { get; set; }

        public bool ReturnAddressList { get; set; } = true;

        public bool ReturnAddressByMoniker { get; set; } = true;

        public async Task<QasAddress> GetAddressByMoniker(string moniker)
        {
            if (ThrowException)
                throw new Exception("QAS Service Exception");

            if (!ReturnAddressByMoniker)
            {
                return null;
            }

            return await Task.FromResult(new QasAddress
            {
                HouseName = "1",
                AddressLine1 = "Waterloo Road",
                Town = "Havant"
            });
        }

        public async Task<List<KeyValuePair<string, string>>> GetAddressByPostcode(string postcode)
        {
            if (ThrowException)
            {
                throw new Exception("EnergyProjectionService Exception");
            }

            if (!ReturnAddressList)
            {
                return await Task.FromResult(new List<KeyValuePair<string, string>>());
            }

            var addresses = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ABC1", "1, London Road, Havant"),
                new KeyValuePair<string, string>("ABC2", "2, London Road, Havant"),
                new KeyValuePair<string, string>("ABC3", "3, London Road, Havant")
            };

            return await Task.FromResult(addresses);
        }
    }
}
