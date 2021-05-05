namespace Products.Core.Configuration.Settings
{
    using System.Collections.Generic;

    public interface IConfigurationSettings
    {
        HomeServicesSettings HomeServicesSettings { get; set; }

        Dictionary<string, APISettings> APISettings { get; set; }

        Dictionary<string, TariffGroup> TariffGroupSettings { get; set; }

        TariffManagementSettings TariffManagementSettings { get; set; }

        BroadbandManagementSettings BroadbandManagementSettings { get; set; }

        ConnectionStringSettings ConnectionStringSettings { get; set; }
		
		List<string> BlockedEmails { get; set; }
    }
}