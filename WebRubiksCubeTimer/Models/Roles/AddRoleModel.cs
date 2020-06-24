using System.ComponentModel.DataAnnotations;

namespace WebRubiksCubeTimer.Models.Roles
{
    public class AddRoleModel
    {
        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Name { get; set; }
    }
}
