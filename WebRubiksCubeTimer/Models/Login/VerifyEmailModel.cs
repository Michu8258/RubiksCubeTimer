using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Login
{
    public class VerifyEmailModel
    {
        [Display(Name = "User name")]
        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string UserName { get; set; }

        [Required]
        [EmailDot]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Verification key")]
        public string VerificationKey { get; set; }
    }
}
