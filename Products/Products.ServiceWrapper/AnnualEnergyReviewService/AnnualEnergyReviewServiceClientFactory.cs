using Products.Infrastructure;
using System;

namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    public class AnnualEnergyReviewServiceClientFactory : IAnnualEnergyReviewServiceClientFactory
    {
        private readonly IConfigManager _configManager;

        public AnnualEnergyReviewServiceClientFactory(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public AnnualEnergyReviewServiceClient Create()
        {
            return new AnnualEnergyReviewServiceClient();
        }

        public messageHeader CreateMessageHeader()
        {
            var csEnvironment = _configManager.GetAppSetting("CSenvironment");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(csEnvironment), "CSEnvironment is missing within web.config");

            return new messageHeader
            {
                channelID = messageHeaderChannelID.Web,
                clientID = "MKTWEB",
                correlationID = Guid.NewGuid().ToString(),
                creationTimeStamp = DateTime.UtcNow,
                environment = csEnvironment,
                messageID = Guid.NewGuid().ToString()
            };
        }
    }
}
