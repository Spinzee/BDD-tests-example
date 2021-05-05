namespace Products.Service.Broadband.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Infrastructure.Extensions;
    using Model.Broadband;
    using Model.Energy;
    using WebModel.ViewModels.Broadband;

    public static class ApplicationAuditMapper
    {
        public static ApplicationAudit GetApplicationAuditData(int applicationId, Customer customer, List<BroadbandProduct> products,
            YourPriceViewModel yourPriceViewModel)
        {
            List<string> productsOffered = products.Where(p => p.IsAvailable).SelectMany(p => p.TalkProducts.Select(t => $"{t.ProductCode}: {t.ProductName}"))
                .ToList();
            string productsOfferedDescription = productsOffered.Any() ? string.Join(", ", productsOffered) : "";

            var data = new ApplicationAudit
            {
                ApplicationId = applicationId,
                CLIProvided = customer.IsSSECustomerCLI || !string.IsNullOrEmpty(customer.OriginalCliEntered),
                ProductCode = customer.SelectedProductCode,
                ProductsOfferedDescription = productsOfferedDescription,
                LineSpeedsQuoted = GetCustomerLineSpeed(customer),
                MonthlyDDPrice = yourPriceViewModel.MonthlyCost,
                ConnectionCharge = yourPriceViewModel.ConnectionCharge,
                InstallationCharge = customer.ApplyInstallationFee ? yourPriceViewModel.InstallationFee : 0,
                MonthlySurchargeAmount = customer.IsSSECustomer ? 0 : yourPriceViewModel.Surcharge,
                IsSSECustomer = customer.IsSSECustomer
            };

            return data;
        }

        public static OpenreachAuditData GetOpenReachAuditData(EnergyCustomer energyCustomer, OpenReachData openReachResponse)
        {
            var data = new OpenreachAuditData
            {
                ApplicationId = energyCustomer.BroadbandApplicationId,
                SaleDate = DateTime.Now,
                LineStatus = openReachResponse.LineStatus.GetDescription(),
                Postcode = energyCustomer.Postcode,
                CLI = !energyCustomer.CLIChoice.KeepExisting ? string.Empty : energyCustomer.CLIChoice.FinalCLI,
                AddressLineKey = openReachResponse.AddressLineKey,
                AddressLine1 = energyCustomer.SelectedBTAddress.FormattedAddressLine1
            };

            return data;
        }

        public static OpenreachAuditData GetOpenReachAuditData(int applicationId, Customer customer, OpenReachData openReachResponse)
        {
            var data = new OpenreachAuditData
            {
                ApplicationId = applicationId,
                SaleDate = DateTime.Now,
                LineStatus = openReachResponse.LineStatus.GetDescription(),
                Postcode = customer.SelectedAddress.Postcode,
                CLI = customer.CliNumber?.Trim() ?? "",
                AddressLineKey = openReachResponse.AddressLineKey,
                AddressLine1 = customer.SelectedAddress.FormattedAddressLine1
            };

            return data;
        }

        public static string GetCustomerLineSpeed(Customer customer)
        {
            var speeds = new StringBuilder();

            // TODO: logic copied from 2 separate mappers, remember DRY.
            if (customer.SelectedProduct.BroadbandType == BroadbandType.ADSL)
            {
                var adslLineSpeeds = (ADSLLineSpeeds) customer.SelectedProduct.LineSpeed;
                speeds.Append($"ADSL Max: {adslLineSpeeds.Max}");
                speeds.Append($", ADSL Min: {adslLineSpeeds.Min}");
            }
            else
            {
                var fibreLineSpeed = (FibreLineSpeeds) customer.SelectedProduct.LineSpeed;
                speeds.Append($"Fibre Max Download: {fibreLineSpeed.MaxDownload}");
                speeds.Append($", Fibre Min Download: {fibreLineSpeed.MinDownload}");
                speeds.Append($", Fibre Max Upload: {fibreLineSpeed.MaxUpload}");
                speeds.Append($", Fibre Min Upload: {fibreLineSpeed.MinUpload}");
            }

            return speeds.ToString();
        }
    }
}