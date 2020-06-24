using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace WebRubiksCubeTimer.Models.ValidationAttributes
{
    public class PhoneNumberAttribute : Attribute, IModelValidator
    {
        public int MaxLength { get; set; } = 25;
        public int MinLength { get; set; } = 7;
        public string NotNumberError { get; set; } = ApplicationResources.UserInterface.ModelsValidationMessages.PhoneNumberError;

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            string value = context.Model as string;

            if (!string.IsNullOrEmpty(value))
            {
                if (!long.TryParse(value, out long number))
                {
                    yield return new ModelValidationResult("", NotNumberError);
                }
                else
                {
                    if (number < 0)
                    {
                        yield return new ModelValidationResult("", NotNumberError);
                    }
                    if (value.Length < MinLength)
                    {
                        yield return new ModelValidationResult("", string.Format(ApplicationResources.UserInterface.ModelsValidationMessages.PhoneNumberTooShort, MinLength));
                    }
                    if (value.Length > 25)
                    {
                        yield return new ModelValidationResult("", string.Format(ApplicationResources.UserInterface.ModelsValidationMessages.PhoneNumberTooLong, MaxLength));
                    }
                }
            }
        }

    }
}
