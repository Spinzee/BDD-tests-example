namespace Products.Tests.TariffChange.Fakes.Models
{
    using ServiceWrapper.ManageCustomerInformationService;

    public class FakeMCISData
    {
        public string CustomerAccountNumber { get; set; } = "1111111113";

        public string BrandCode { get; set; } = "Default MCIS Brand Code";

        public string Postcode { get; set; } = "SO14 1FJ";

        public ServiceTypeType Service { get; set; } = ServiceTypeType.electricity;

        public string ServicePlanDescription { get; set; } = "Default MCIS Tariff Name";

        public ServiceStatusType ServiceStatus { get; set; } = ServiceStatusType.Active;

        public CustomerAccountStatusType CustomerAccountStatus { get; set; } = CustomerAccountStatusType.Found;
    }
}