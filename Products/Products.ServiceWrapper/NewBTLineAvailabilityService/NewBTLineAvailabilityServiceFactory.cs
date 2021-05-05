using Products.Infrastructure;
using System;

namespace Products.ServiceWrapper.NewBTLineAvailabilityService
{


    public interface INewBTLineAvailabilityServiceFactory
    {
        NewBTLineAvailabilityServiceInterfaceClient Create();
        messageHeader CreateMessageHeader();
    }

    public class NewBTLineAvailabilityServiceFactory : INewBTLineAvailabilityServiceFactory
    {
        private readonly IConfigManager _configManager;

        public NewBTLineAvailabilityServiceFactory(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public NewBTLineAvailabilityServiceInterfaceClient Create()
        {
            return new NewBTLineAvailabilityServiceInterfaceClient("NewBTLineAvailabilityService");
        }

        public messageHeader CreateMessageHeader()
        {
            var csEnvironment = _configManager.GetAppSetting("CSenvironment");
            var clientId = _configManager.GetAppSetting("NewBTLineAvailabilityServiceClientId");

            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(clientId), "clientId setting in app.config is missing.");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(csEnvironment), "CSenvironment setting in app.config is missing.");

            return new messageHeader()
            {
                creationTimeStamp = DateTime.UtcNow,
                correlationID = Guid.NewGuid().ToString(),
                channelID = messageHeaderChannelID.Web,
                clientID = clientId,
                environment = csEnvironment,
                messageID = Guid.NewGuid().ToString()
            };
        }
    }
}
