using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Login
{
    public class PasswordResetCodeModel
    {
        [Required]
        [EmailDot]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password reset code")]
        [StringLength(50, MinimumLength = 8)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "ENew password")]
        [StringLength(50, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "New password confirm")]
        [StringLength(50, MinimumLength = 8)]
        public string NewPassword2 { get; set; }
    }
}
