namespace Products.WebModel.Attributes
{
    using System;
    using System.Resources;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class HelpTextAttribute : Attribute, IMetadataAware
    {
        public Type ResourceType { get; set; }

        public string Name { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (ResourceType != null)
            {
                var resourceManager = new ResourceManager(ResourceType);
                string resourceValue = resourceManager.GetString(Name);
                if (!string.IsNullOrEmpty(resourceValue))
                {
                    metadata.AdditionalValues.Add("HelpText", resourceValue);
                }
            }
        }
    }
}
