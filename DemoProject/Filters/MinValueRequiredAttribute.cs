using System;
using System.ComponentModel.DataAnnotations;

namespace DemoProject.Filters
{
    public class MinValueRequiredAttribute : ValidationAttribute
    {
        private readonly int _minValue;

        public MinValueRequiredAttribute(int minValue)
        {
            _minValue = minValue;
            ErrorMessage = $"The value must be at least {_minValue}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !(value is int))
            {
                return new ValidationResult("Invalid value. Must be an integer.");
            }

            int intValue = (int)value;
            if (intValue < _minValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
