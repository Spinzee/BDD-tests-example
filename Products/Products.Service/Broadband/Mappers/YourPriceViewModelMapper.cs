namespace Products.Service.Broadband.Mappers
{
    using Products.Model.Broadband;
    using Products.WebModel.Resources.Broadband;
    using Products.WebModel.ViewModels.Broadband;
    using Products.WebModel.ViewModels.Broadband.Extensions;

    public class YourPriceViewModelMapper
    {
        public static YourPriceViewModel MapCustomerToYourPriceViewModel(Customer customer, double surcharge, double connectionFee, double newLineInstallationFee)
        {
            BroadbandProductGroup broadbandProductGroup = customer.SelectedProduct.GetSelectedTalkProduct(customer.SelectedProductCode).BroadbandProductGroup;
            TalkProduct talkProduct = customer.SelectedProduct.GetSelectedTalkProduct(customer.SelectedProductCode);
            double telCost = talkProduct.GetPhoneCost();
            double bbCost = talkProduct.GetBroadbandCost();
            double monthlyCost = telCost + bbCost;
            double oneOffCost = GetOneOffCost(customer.IsSSECustomer, connectionFee, customer.ApplyInstallationFee, newLineInstallationFee);
            double firstBillTotal = monthlyCost;

            return new YourPriceViewModel
            {
                IsExistingCustomer = customer.IsSSECustomer,
                TelName = talkProduct.TalkCode.GetTelName(),
                BroadbandName = customer.SelectedProduct.BroadbandType.GetTitle(broadbandProductGroup),
                TelCost = telCost,
                BroadbandCost = bbCost,
                MonthlyCost = monthlyCost,
                Surcharge = surcharge,
                ConnectionCharge = connectionFee,
                ApplyInstallationFee = customer.ApplyInstallationFee,
                InstallationFee = newLineInstallationFee,
                OneOffCost = oneOffCost,
                FirstBillTotal = firstBillTotal,
                OneOffModalContent1 = string.Format(YourPrice_Resources.OneOffModalContent1, newLineInstallationFee),
                OneOffModalContent2 = string.Format(YourPrice_Resources.OneOffModalContent2, connectionFee),
                TelcoCostText = telCost.Equals(0) ? YourPrice_Resources.Included  : telCost.ToString("C"),
                BroadbandCostText = bbCost.ToString("C"),
                MonthlyCostText = monthlyCost.ToString("C"),
                ConnectionChargeText = connectionFee.ToString("C"),
                InstallationFeeText = newLineInstallationFee.ToString("C"),
                OneOffCostText = oneOffCost.ToString("C"),
                FirstBillTotalText = firstBillTotal.ToString("C")
            };
        }

        private static double GetOneOffCost(bool isSSeCustomer, double connectionFee, bool applyInstallationFee, double installationFee)
        {
            double totalCost = isSSeCustomer ? 0 : connectionFee;
            totalCost += applyInstallationFee ? installationFee : 0;
            return totalCost;
        }
    }
}
