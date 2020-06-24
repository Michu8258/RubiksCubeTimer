using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Login
{
    public class CreateAccountModel
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
        [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(50, MinimumLength = 8)]
        public string Password2 { get; set; }
    }
}
