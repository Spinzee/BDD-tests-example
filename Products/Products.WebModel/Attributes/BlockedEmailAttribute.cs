namespace Products.WebModel.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Core.Configuration.Settings;
    using Resources.Common;

    [AttributeUsage(AttributeTargets.Property)]
    public class BlockedEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = Convert.ToString(value).ToLower();
            return ConfigurationSettings.Instance.BlockedEmails.Contains(email) ? new ValidationResult(Form_Resources.EmailInvalidEmailErrorMessage) : ValidationResult.Success;
        }
    }
}