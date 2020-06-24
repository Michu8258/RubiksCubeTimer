using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using WebRubiksCubeTimer.Models.Pagination;

namespace WebRubiksCubeTimer.Models.Users
{
    public class UsersListModel : ErrorModelBase
    {
        public UsersListModel(ModelStateDictionary modelState) : base(modelState)
        {
            Users = new List<UserModel>();
            Roles = new List<IdentityRole>();
        }

        public IEnumerable<UserModel> Users { get; set; }
        public string DefaultRoleName { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public string FilteredUserName { get; set; }
        public string FilteredMail { get; set; }
        public string FilteredPhoneNumber { get; set; }
        public string FilterRoles { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
