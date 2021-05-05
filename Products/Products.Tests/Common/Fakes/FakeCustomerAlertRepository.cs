using Products.Model;
using Products.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.Tests.Common.Fakes
{
    public class FakeCustomerAlertRepository : ICustomerAlertRepository
    {
        public int IsCustomerAlertMethodCount { get; set; }
        public string AlertName { get; set; } = string.Empty;

        private readonly Exception _exception;
        private readonly IEnumerable<CustomerAlertResult> _result;

        public FakeCustomerAlertRepository()
        {
            _result = null;
        }


        public FakeCustomerAlertRepository(IEnumerable<CustomerAlertResult> result)
        {
            _result = result;
        }

        public FakeCustomerAlertRepository(Exception exception)
        {
            _exception = exception;
        }

        public async Task<CustomerAlertResult> IsCustomerAlertActive(string customerAlertName)
        {
            AlertName = customerAlertName;

            IsCustomerAlertMethodCount++;

            if (_exception != null)
            {
                throw _exception;
            }

            return await Task.FromResult(_result?.FirstOrDefault());
        }

    }
}
