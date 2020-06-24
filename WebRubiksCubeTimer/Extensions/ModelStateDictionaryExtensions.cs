using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace WebRubiksCubeTimer.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static IEnumerable<string> ExtractMessages(this ModelStateDictionary modelState)
        {
            List<string> messages = new List<string>();
            foreach (var errors in modelState.Values)
            {
                foreach (var error in errors.Errors)
                {
                    messages.Add(error.ErrorMessage);
                }
            }

            return messages;
        }
    }
}
