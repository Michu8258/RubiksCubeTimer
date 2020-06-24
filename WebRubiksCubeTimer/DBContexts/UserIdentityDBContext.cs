using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.PasswordResetting;

namespace WebRubiksCubeTimer.DBContexts
{
    public class UserIdentityDBContext : IdentityDbContext<UserModel>
    {
        public UserIdentityDBContext(DbContextOptions<UserIdentityDBContext> options) : base(options)
        {

        }

        public DbSet<PasswordReset> PasswordResetRequests { get; set; }
    }
}
