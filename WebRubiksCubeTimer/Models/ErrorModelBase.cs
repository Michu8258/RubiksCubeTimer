using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace WebRubiksCubeTimer.Models
{
    public abstract class ErrorModelBase
    {
        public IEnumerable<string> ErrorMessages { get; }

        public ErrorModelBase(ModelStateDictionary modelState)
        {
            List<string> messages = new List<string>();
            foreach (var error in modelState.Values)
            {
                foreach (var errorMessage in error.Errors)
                {
                    messages.Add(errorMessage.ErrorMessage);
                }
            }

            ErrorMessages = messages;
        }
    }
}
