using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class UserSelfModifViewModel
    {
        public string UserIdentity { get; set; }

        [Display(Name = "User name")]
        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string UserName { get; set; }

        [StringLength(50)]
        [Display(Name = "Current password")]
        public string Password { get; set; }

        [StringLength(50, MinimumLength = 8)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [StringLength(50, MinimumLength = 8)]
        [Display(Name = "New password")]
        public string NewPassword2 { get; set; }

        [Display(Name = "Phone number")]
        [PhoneNumber]
        public string PhoneNumber { get; set; }
    }
}
