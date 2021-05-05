namespace Products.WebModel.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Property)]
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ValidationType = "mustbetrue",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
        }

        public override bool IsValid(object value)
        {
            return value as bool? == true;
        }
    }
}
