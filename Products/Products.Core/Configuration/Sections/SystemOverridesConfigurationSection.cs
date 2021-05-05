namespace Products.Core.Configuration.Sections
{
    using System.Configuration;

    public class SystemOverridesConfigurationSection : ConfigurationSection
    {
        public const string ConfigPath = "systemOverrides";

        public static SystemOverridesConfigurationSection Section { get; } = ConfigurationManager.GetSection(ConfigPath) as SystemOverridesConfigurationSection;

        [ConfigurationProperty("blockedEmails", IsRequired = false, IsKey = false)]
        public string BlockedEmails
        {
            get => (string)this["blockedEmails"];
            set => this["blockedEmails"] = value;
        }
    }
}
