namespace Products.Infrastructure
{
    using System.Collections.Specialized;
    using System.Configuration;

    public class ConfigManager : IConfigManager
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public string GetValueForKeyFromSection(string sectionGroup, string sectionKey, string key)
        {
            var section = (NameValueCollection)ConfigurationManager.GetSection($"{sectionGroup}/{sectionKey}");
            return section?[key];
        }

        public T GetConfigSectionGroup<T>(string sectionGroup) where T : ConfigurationSection
        {
            var section = (T)ConfigurationManager.GetSection($"{sectionGroup}");
            return section;
        }
    }
}