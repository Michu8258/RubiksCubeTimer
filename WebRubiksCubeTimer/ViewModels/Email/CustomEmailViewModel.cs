using System.ComponentModel.DataAnnotations;

namespace WebRubiksCubeTimer.ViewModels.Email
{
    public class CustomEmailViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
