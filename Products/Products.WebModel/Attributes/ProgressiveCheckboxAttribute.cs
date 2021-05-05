namespace Products.WebModel.Attributes
{
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ProgressiveCheckboxAttribute : Attribute, IMetadataAware
    {
        public Type ResourceType { get; set; }

        public string Name { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("ProgressiveCheckbox", true);
        }
    }
}
