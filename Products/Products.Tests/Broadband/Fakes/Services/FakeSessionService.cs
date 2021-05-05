namespace Products.Tests.Broadband.Fakes.Services
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Products.Infrastructure;
    using Products.Model.Broadband;

    public class FakeSessionManager : ISessionManager
    {
        public object SessionObject { get; set; }

        public object ListOfProductsSessionObject { get; set; }

        public object YourPriceSessionObject { get; set; }

        public object OpenReachSessionObject { get; set; }

        public bool ThrowException { get; set; }

        public FakeSessionManager()
        {
            SessionObject = new BroadbandJourneyDetails();
            ThrowException = false;
        }

        public void SetSessionDetails<T>(string key, T value)
        {
            if (ThrowException)
            {
                throw new Exception();
            }

            switch (key)
            {
                case "yourPriceDetails":
                    YourPriceSessionObject = value;
                    break;
                case "broadbandProducts":
                    ListOfProductsSessionObject = value;
                    break;
                case "openReachResponse":
                    OpenReachSessionObject = value;
                    break;
                default:
                    SessionObject = value;
                    break;
            }
        }

        public T GetSessionDetails<T>(string key)
        {
            if (ThrowException)
            {
                throw new Exception();
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (key)
            {
                case "yourPriceDetails":
                    return (T)YourPriceSessionObject;
                case "broadbandProducts":
                    return (T)ListOfProductsSessionObject;
                case "openReachResponse":
                    return (T)OpenReachSessionObject;
            }

            return (T)SessionObject;
        }

        public void RemoveSessionDetails(string key)
        {
            if (ThrowException)
            {
                throw new Exception();
            }
            switch (key)
            {
                case "yourPriceDetails":
                    YourPriceSessionObject = null;
                    break;
                case "broadbandProducts":
                    ListOfProductsSessionObject = null;
                    break;
                case "openReachResponse":
                    OpenReachSessionObject = null;
                    break;
                default:
                    SessionObject = null;
                    break;
            }
        }

        public void SetListOfProducts()
        {
            ListOfProductsSessionObject = new List<BroadbandProduct>
            {
                new BroadbandProduct
                {
                    BroadbandCode = "ABC",
                    TalkProducts = new List<TalkProduct>
                    {
                        new TalkProduct
                        {
                            ProductCode = "ABC",
                            Prices = new List<BroadbandPrice>
                            {
                                new BroadbandPrice { FeatureCode = FeatureCodes.MonthlyLineRental, Price = 17 }
                            }
                        }
                    },
                    BroadbandType = BroadbandType.ADSL
                }
            };
        }

        public void ClearSession()
        {
            YourPriceSessionObject = null;
            ListOfProductsSessionObject = null;
            SessionObject = null;
        }

        public T GetOrDefaultSessionDetails<T>(string key) where T : new()
        {
            throw new NotImplementedException();
        }
    }
}
