using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace dotnet_philly.Attributes
{
    public class IsValidUrlAttribute : ValidationAttribute
    {
        public IsValidUrlAttribute() : base("{0} must be a valid http or https URL. Example: https://localhost:5001/Samples") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var valid = Uri.TryCreate(value.ToString(), UriKind.Absolute, out Uri result) && (result.Scheme == "http" || result.Scheme == "https");
            if (!valid)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
