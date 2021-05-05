namespace Products.Model.Broadband
{
    using System;
    using Core;

    [Serializable]
    public class OpenReachData
    {
        // ReSharper disable once IdentifierTypo
        public LineAvailability LineavailabilityFlags { get; set; }

        public string AddressLineKey { get; set; }

        public LineStatus LineStatus { get; set; }

        public string CLI { get; set; }
    }
}