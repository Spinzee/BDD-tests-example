using Products.WebModel.Resources.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Products.WebModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidDateOfBirthAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string day = validationContext.ObjectType.GetProperty("DateOfBirthDay")?.GetValue(validationContext.ObjectInstance, null)?.ToString();
            string month = validationContext.ObjectType.GetProperty("DateOfBirthMonth")?.GetValue(validationContext.ObjectInstance, null)?.ToString();
            string year = validationContext.ObjectType.GetProperty("DateOfBirthYear")?.GetValue(validationContext.ObjectInstance, null)?.ToString();
            string formattedDateOfBirth = $"{day}/{month}/{year}";

            DateTime dateOfBirth;
            DateTime.TryParse(formattedDateOfBirth, out dateOfBirth);
            var age = CalculateAge(dateOfBirth);

            if (age >= 18) return ValidationResult.Success;

            bool isScottishPostcode;
            bool.TryParse(validationContext.ObjectType.GetProperty("IsScottishPostcode")?.GetValue(validationContext.ObjectInstance, null).ToString(), out isScottishPostcode);
            if (isScottishPostcode && age >= 16) return ValidationResult.Success;

            return new ValidationResult(Form_Resources.DateOfBirthRegExError, new List<string> { validationContext.MemberName });
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "dateofbirthcheck",
                ErrorMessage = Form_Resources.DateOfBirthRegExError,
            };
            rule.ValidationParameters.Add("otherproperties", "DateOfBirthDay,DateOfBirthMonth,DateOfBirthYear,IsScottishPostcode");
            yield return rule;
        }

        private int CalculateAge(DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;
            return age;
        }
    }
}