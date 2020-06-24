using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Users
{
    public class AddUserModel
    {
        [Display(Name = "User name")]
        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string UserName { get; set; }

        [Required]
        [EmailDot]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [PhoneNumber]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [Display(Name = "Email confirmation")]
        [Required]
        public bool? ConfirmEmail { get; set; }

        [Display(Name = "Email verivication key")]
        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string EmailConfirmationString { get; set; }

        public string ReturnUrl { get; set; }
    }
}
