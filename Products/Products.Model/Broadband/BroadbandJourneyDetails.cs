namespace Products.Model.Broadband
{
    using System;

    [Serializable]
    public class BroadbandJourneyDetails
    {        
        public Customer Customer { get; set; }

        public string Password { get; set; }
    }
}