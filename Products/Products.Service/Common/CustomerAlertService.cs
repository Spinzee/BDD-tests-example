using Products.Infrastructure;
using Products.Repository.Common;
using System;
using System.Threading.Tasks;

namespace Products.Service.Common
{
    public class CustomerAlertService : ICustomerAlertService
    {
        private readonly ICustomerAlertRepository _customerAlertRepository;

        public CustomerAlertService(ICustomerAlertRepository customerAlertRepository)
        {
            Guard.Against<ArgumentNullException>(customerAlertRepository == null, "customerAlertRepository is null");
            _customerAlertRepository = customerAlertRepository;
        }

        public async Task<bool> IsCustomerAlert(string customerAlertName)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(customerAlertName), "CustomerAlertName is null");

            var alert = await _customerAlertRepository.IsCustomerAlertActive(customerAlertName);

            if (alert == null) return false;

            if (alert.StartTime == null && alert.EndTime == null)

                return true;

            return isInDateTimePeriod(alert.StartTime, alert.EndTime);
        }

        private bool isInDateTimePeriod(DateTime? startTime, DateTime? endTime)
        {
            DateTime now = DateTime.Now;
            return (now > startTime) && (now < endTime);
        }
    }
}
