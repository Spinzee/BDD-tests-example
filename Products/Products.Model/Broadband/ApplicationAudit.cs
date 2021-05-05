namespace Products.Model.Broadband
{
    public class ApplicationAudit
    {
        public int ApplicationId { get; set; }
        public string ProductCode { get; set; }
        public double MonthlyDDPrice { get; set; }
        public double MonthlySurchargeAmount { get; set; }
        public double ConnectionCharge { get; set; }
        public double InstallationCharge { get; set; }
        public string ProductsOfferedDescription { get; set; }
        public string LineSpeedsQuoted { get; set; }
        public bool CLIProvided { get; set; }
        public bool IsSSECustomer { get; set; }
    }
}
