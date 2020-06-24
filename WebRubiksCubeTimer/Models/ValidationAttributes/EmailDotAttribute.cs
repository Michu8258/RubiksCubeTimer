using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebRubiksCubeTimer.Models.ValidationAttributes
{
    public class EmailDotAttribute : Attribute, IModelValidator
    {
        public string ErrorMessage { get; set; } = ApplicationResources.UserInterface.ModelsValidationMessages.EmailAddresError;

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context.Model is string value && value.Contains('@'))
            {
                if (!value.Substring(value.LastIndexOf('@')).Contains('.'))
                {
                    yield return new ModelValidationResult("", ErrorMessage);
                }
            }
            else
            {
                yield return new ModelValidationResult("", ErrorMessage);
            }
        }
    }
}
