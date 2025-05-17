using System;
using System.ComponentModel.DataAnnotations;

namespace Fitness_App.BL.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class OptionalUrlAttribute : ValidationAttribute
    {
        public OptionalUrlAttribute() : base("The {0} field is not a valid URL.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                // Empty or null values are considered valid
                return true;
            }

            // Use the standard URL validator for non-empty values
            var urlAttribute = new UrlAttribute();
            return urlAttribute.IsValid(value);
        }
    }
} 