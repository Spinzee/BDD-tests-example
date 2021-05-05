namespace Products.Infrastructure
{
    using System.Configuration;

    public interface IConfigManager
    {
        string GetConnectionString(string name);

        string GetAppSetting(string key);

        string GetValueForKeyFromSection(string sectionGroup, string sectionKey, string key);
        T GetConfigSectionGroup<T>(string sectionGroup) where T : ConfigurationSection;
    }
}