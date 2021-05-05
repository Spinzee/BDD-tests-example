using System.Collections.Generic;

namespace Products.Model.Energy
{
    public class MeterDetail
    {
        public string GeographicalArea { get; set; }
        public List<MeterInformation> MeterInformation { get; set; }
    }
}
