using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebRubiksCubeTimer.DBContexts;

namespace WebRubiksCubeTimer.UserManager
{
    public class UserManagerExtensions : IUserManagerExtending
    {
        private readonly UserIdentityDBContext _dbContext;

        public UserManagerExtensions(UserIdentityDBContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> UpdateUserLoginTimeSpan(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            user.LastLoginTimeSpan = DateTime.Now;
            return (await _dbContext.SaveChangesAsync()) == 1;
        }
    }
}
