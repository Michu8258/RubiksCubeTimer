using System.Collections.Generic;

namespace WebRubiksCubeTimer.Models.Roles
{
    public class UserRolesModel
    {
        public UserRolesModel()
        {
            Roles = new List<RoleBelongingModel>();
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public bool ValidUser { get; set; }
        public IList<RoleBelongingModel> Roles { get; set; }
    }
}
