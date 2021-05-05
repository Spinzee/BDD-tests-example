using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Products.WebModel.Attributes
{
    public class RequiredIfAttribute : RequiredAttribute, IClientValidatable
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        public RequiredIfAttribute(string propertyName, object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;
            Type type = instance.GetType();
            object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (proprtyvalue.ToString() == DesiredValue.ToString())
            {
                ValidationResult result = base.IsValid(value, context);
                return result;
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "requiredif",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            // PS: This may not work for nested ViewModels, as per the conventions nested viewmodels get Parent_ChildPropertyName as the id.
            rule.ValidationParameters["dependentproperty"] = PropertyName;// (context as ViewContext).ViewData.TemplateInfo.GetFullHtmlFieldId(PropertyName); 
            rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;
            yield return rule;
        }
    }
}