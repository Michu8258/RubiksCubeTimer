using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRubiksCubeTimer.ViewModels.Email
{
    public class GlobalEmailViewModel
    {
        public GlobalEmailViewModel()
        {
            ExcludedDomains = new Dictionary<string, bool>();
        }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public IDictionary<string, bool> ExcludedDomains { get; set; }
    }
}
