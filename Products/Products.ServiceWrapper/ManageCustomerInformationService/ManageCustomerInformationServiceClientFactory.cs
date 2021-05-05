using Products.Infrastructure;
using System;

namespace Products.ServiceWrapper.ManageCustomerInformationService
{
    public interface IManageCustomerInformationServiceClientFactory
    {
        ManageCustomerInformationServiceClient Create();

        MessageHeader CreateMessageHeader();
    }

    public class ManageCustomerInformationServiceClientFactory : IManageCustomerInformationServiceClientFactory
    {
        private readonly IConfigManager _configManager;

        public ManageCustomerInformationServiceClientFactory(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public ManageCustomerInformationServiceClient Create()
        {
            return new ManageCustomerInformationServiceClient("ManageCustomerInformationService");
        }

        public MessageHeader CreateMessageHeader()
        {
            var csEnvironment = _configManager.GetAppSetting("CSenvironment");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(csEnvironment), "CSEnvironment is missing from web.config");

            return new MessageHeader
            {
                CorrelationID = Guid.NewGuid().ToString(),
                MessageCreationTimeStamp = DateTime.UtcNow,
                MessageID = Guid.NewGuid().ToString(),
                SystemID = "MKTWEB",
                Environment = csEnvironment
            };
        }
    }
}