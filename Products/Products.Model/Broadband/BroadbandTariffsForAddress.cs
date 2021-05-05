using System.Collections.Generic;

namespace Products.Model.Broadband
{
    public class BroadbandTariffsForAddress
    {
        public IEnumerable<LineSpeed> LineSpeeds { get; set; }

        public List<Tariff> Tariffs { get; set; }
    }
}
