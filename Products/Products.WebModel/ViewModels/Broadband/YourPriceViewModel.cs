namespace Products.WebModel.ViewModels.Broadband
{
    public class YourPriceViewModel
    {
        public bool IsExistingCustomer { get; set; }

        public string TelName { get; set; }

        public double TelCost { get; set; }

        public string BroadbandName { get; set; }

        public double BroadbandCost { get; set; }

        public double MonthlyCost { get; set; }

        public double OneOffCost { get; set; }

        public double FirstBillTotal { get; set; }

        public double ConnectionCharge { get; set; }

        public double Surcharge { get; set; }

        public double InstallationFee { get; set; }

        public string OneOffModalContent1 { get; set; }

        public string OneOffModalContent2 { get; set; }

        public bool ApplyInstallationFee { get; set; }

        public string TelcoCostText { get; set; }

        public string BroadbandCostText { get; set; }

        public string MonthlyCostText { get; set; }

        public string InstallationFeeText { get; set; }

        public string ConnectionChargeText { get; set; }

        public string OneOffCostText { get; set; }
        
        public string FirstBillTotalText { get; set; }
    }
}
