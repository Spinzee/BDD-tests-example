namespace Products.Service.Energy
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Infrastructure;

    public class EnergyAlertService : IEnergyAlertService
    {
        private readonly ICustomerAlertService _customerAlertService;
        private readonly IConfigManager _configManager;

        private const string OurPriceCustomerAlertName = "OurPricesCustomerAlertName";
        private const string EnergyCustomerAlertName = "EnergyCustomerAlertName";

        public EnergyAlertService(ICustomerAlertService customerAlertService, IConfigManager configManager)
        {
            Guard.Against<ArgumentException>(customerAlertService == null, $"{nameof(customerAlertService)} is null");
            Guard.Against<ArgumentException>(configManager == null, $"{nameof(configManager)} is null");
            _customerAlertService = customerAlertService;
            _configManager = configManager;
        }

        public async Task<bool> IsOurPricesCustomerAlert()
        {
            return await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting(OurPriceCustomerAlertName));
        }

        public async Task<bool> IsEnergyCustomerAlert()
        {
            return await _customerAlertService.IsCustomerAlert(_configManager.GetAppSetting(EnergyCustomerAlertName));
        }
    }
}
