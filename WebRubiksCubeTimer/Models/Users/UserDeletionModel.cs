using System.ComponentModel.DataAnnotations;

namespace WebRubiksCubeTimer.Models.Users
{
    public class UserDeletionModel
    {
        [Required]
        [Display(Name = "Identifier")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Delete all user data")]
        public bool DeleteUserDataStorage { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        public string ReturnUrl { get; set; }
    }
}
