using Products.Model.Common;
using System.Collections.Generic;
using System.Linq;

namespace Products.ServiceWrapper.QASService
{
    public static class QASMapper
    {
        public static QasAddress ToQASAddress(AddressLineType[] addressLines)
        {
            return new QasAddress
            {
                MPAN = addressLines.FirstOrDefault(a => a.Label == "Electricity Meter Number" && a.LineContent == LineContentType.DataPlus)?.Line,
                MPRN = addressLines.FirstOrDefault(a => a.Label == "Gas Meter Number" && a.LineContent == LineContentType.DataPlus)?.Line,
                HouseName = addressLines.FirstOrDefault(a => a.Label == "" && a.LineContent == LineContentType.Address)?.Line,
                AddressLine1 = addressLines.FirstOrDefault(a => a.Label == "Thoroughfare" && a.LineContent == LineContentType.Address)?.Line,
                Town = addressLines.FirstOrDefault(a => a.Label == "Town" && a.LineContent == LineContentType.Address)?.Line,
                County = addressLines.FirstOrDefault(a => a.Label == "County" && a.LineContent == LineContentType.Address)?.Line,
            };
        }

        public static List<KeyValuePair<string, string>> ToQASAddressEntryList(PicklistEntryType[] picklist)
        {
            return picklist
                    .Select(p => new KeyValuePair<string, string>(p.Moniker, p.Picklist))
                    .ToList();
        }

        public static DoGetAddressRequest CreateAddressRequest(string moniker)
        {
            return new DoGetAddressRequest
            {
                QAGetAddress = new QAGetAddress
                {
                    Moniker = moniker,
                    Layout = "DXP"
                }
            };
        }

        public static DoSearchRequest CreateSearchRequest(string postcode)
        {
            return new DoSearchRequest
            {
                QASearch = new QASearch
                {
                    Country = "GBR",
                    Engine = new EngineType
                    {
                        Flatten = true,
                        Value = EngineEnumType.Singleline,
                        FlattenSpecified = true
                    },
                    Search = postcode
                }
            };
        }
    }
}
