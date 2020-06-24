using Microsoft.AspNetCore.Identity;
using System;

namespace WebRubiksCubeTimer.Models.Users
{
    public class UserModel : IdentityUser
    {
        public string EmailVerificationKey { get; set; }
        public DateTime LastLoginTimeSpan { get; set; }
    }
}
