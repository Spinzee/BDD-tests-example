namespace Products.WebModel.Attributes
{
    using System;
    using System.Resources;
    using Resources.TariffChange;

    [AttributeUsage(AttributeTargets.All)]
    public class LocalisedDescription : System.ComponentModel.DescriptionAttribute
    {
        public LocalisedDescription(string key)
            : base(GetDescription(key))
        {
        }

        private static string GetDescription(string key)
        {
            var resourceManager = new ResourceManager(typeof(AriaDescriptions));
            return resourceManager.GetString(key);
        }
    }
}
