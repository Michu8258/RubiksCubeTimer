using System.ComponentModel.DataAnnotations;
using WebRubiksCubeTimer.Models.ValidationAttributes;

namespace WebRubiksCubeTimer.Models.Login
{
    public class PasswordResetModel
    {
        [Required]
        [EmailDot]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        public int Attempts { get; set; }
        public int Hours { get; set; }
    }
}
