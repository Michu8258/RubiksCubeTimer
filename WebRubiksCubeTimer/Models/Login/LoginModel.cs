using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Login
{
    public class LoginModel
    {
        public string UseName { get; set; }

        [Required]
        [EmailDot]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
