namespace Products.Tests.TariffChange.Fakes.Services
{
    using System;
    using Model.TariffChange.Customers;
    using ServiceWrapper.PersonalProjectionService;

    public class FakePersonalProjectionServiceWrapper : IPersonalProjectionServiceWrapper
    {
        private readonly Exception _exception;

        public FakePersonalProjectionServiceWrapper()
        {
        }

        public FakePersonalProjectionServiceWrapper(Exception exception)
        {
            _exception = exception;
        }

        public PersonalProjectionDetails LastProjectionDetails { get; private set; }

        public SiteProjectionResponse SiteProjection(SiteProjectionRequest request)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            LastProjectionDetails = new PersonalProjectionDetails
            {
                SiteId = int.Parse(request.Site.SiteId),
                ElectricitySpend = double.Parse(request.Site.SiteConsumption.ElectricitySpend),
                ElectricityUsage = double.Parse(request.Site.SiteConsumption.ElectricityConsumption),
                GasSpend = double.Parse(request.Site.SiteConsumption.GasSpend),
                GasUsage = double.Parse(request.Site.SiteConsumption.GasConsumption)
            };

            return new SiteProjectionResponse();
        }
    }
}