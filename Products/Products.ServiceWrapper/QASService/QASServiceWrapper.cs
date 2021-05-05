namespace Products.ServiceWrapper.QASService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Model.Common;

    public class QASServiceWrapper : IQASServiceWrapper
    {
        public async Task<List<KeyValuePair<string, string>>> GetAddressByPostcode(string postcode)
        {
            var retVal = new List<KeyValuePair<string, string>>();

            using (var client = new QAPortTypeClient("QASService"))
            {
                DoSearchResponse response = await client.DoSearchAsync(QASMapper.CreateSearchRequest(postcode)).ConfigureAwait(false);

                PicklistEntryType[] picklist = response?.QASearchResult?.QAPicklist?.PicklistEntry;
                if (picklist != null && picklist.Any())
                {
                    retVal = QASMapper.ToQASAddressEntryList(picklist);
                }
            }

            return retVal;
        }

        public async Task<QasAddress> GetAddressByMoniker(string moniker)
        {
            QasAddress retVal = null;
            using (var client = new QAPortTypeClient("QASService"))
            {
                DoGetAddressResponse response = await client.DoGetAddressAsync(QASMapper.CreateAddressRequest(moniker)).ConfigureAwait(false);

                AddressLineType[] addressLines = response?.Address?.QAAddress?.AddressLine;
                if (addressLines != null && addressLines.Any())
                {
                    retVal = QASMapper.ToQASAddress(addressLines);
                }
            }

            return retVal;
        }
    }
}