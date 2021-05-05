namespace Products.Tests.Common.Fakes
{
    using System.Collections.Generic;
    using System.Configuration;
    using Products.Infrastructure;

    public class FakeConfigManager : IConfigManager
    {
        private readonly Dictionary<string, string> _configurations;

        public FakeConfigManager()
        {
            _configurations = new Dictionary<string, string>();
        }

        public string GetAppSetting(string key)
        {
            if (key != null && _configurations.ContainsKey(key))
            {
                return _configurations[key];
            }

            return null;
        }

        public string GetConnectionString(string key)
        {
            if (key != null && _configurations.ContainsKey(key))
            {
                return _configurations[key];
            }

            return null;
        }

        public string GetValueForKeyFromSection(string sectionGroup, string sectionKey, string key)
        {
            if (key != null && _configurations.ContainsKey(key))
            {
                return _configurations[key];
            }

            if (sectionGroup != null && sectionKey != null && key != null && _configurations.ContainsKey($"{sectionGroup}|{sectionKey}|{key}"))
            {
                return _configurations[$"{sectionGroup}|{sectionKey}|{key}"];
            }

            return null;
        }

        public T GetConfigSectionGroup<T>(string sectionGroup) where T : ConfigurationSection
        {
            var section = (T) ConfigurationManager.GetSection($"{sectionGroup}");
            return section;
        }

        public void AddConfiguration(string key, string value)
        {
            _configurations.Add(key, value);
        }

        public void AddConfiguration(string sectionGroup, string sectionKey, string key, string value)
        {
            AddConfiguration($"{sectionGroup}|{sectionKey}|{key}", value);
        }
    }
}