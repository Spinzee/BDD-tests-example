namespace Products.Model.Broadband
{
    using System.Collections.Generic;
    using Core;

    public class BroadbandProduct
    {
        public string BroadbandCode { get; set; }

        public List<TalkProduct> TalkProducts { get; set; }

        public LineSpeed LineSpeed { get; set; }

        public BroadbandType BroadbandType { get; set; }

        public int ProductOrder { get; set; }

        public bool IsAvailable { get; set; }
    }
}
