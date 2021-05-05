using Products.Infrastructure;
using System;

namespace Products.ServiceWrapper.PersonalProjectionService
{
    public interface IPersonalProjectionServiceClientFactory
    {
        PersonalProjectionServiceClient Create();

        messageHeader CreateMessageHeader();
    }

    public class PersonalProjectionServiceClientFactory : IPersonalProjectionServiceClientFactory
    {
        private readonly IConfigManager _configManager;

        public PersonalProjectionServiceClientFactory(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public PersonalProjectionServiceClient Create()
        {
            return new PersonalProjectionServiceClient();
        }

        public messageHeader CreateMessageHeader()
        {
            var csEnvironment = _configManager.GetAppSetting("CSenvironment");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(csEnvironment), "CSEnviroment is missing from web.config");

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