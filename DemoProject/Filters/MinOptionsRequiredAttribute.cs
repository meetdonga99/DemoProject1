using DemoProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoProject.Filters
{
	public class MinOptionsRequiredAttribute : ValidationAttribute
    {
        private readonly int _minCount;

        public MinOptionsRequiredAttribute(int minCount)
        {
            _minCount = minCount;
            ErrorMessage = $"At least {_minCount} option(s) must be provided.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var options = value as List<OptionModel>;
            if (options == null || options.Count < _minCount)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}