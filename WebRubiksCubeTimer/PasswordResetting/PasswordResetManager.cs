using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebRubiksCubeTimer.DBContexts;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.PasswordResetting.Helpers;

namespace WebRubiksCubeTimer.PasswordResetting
{
    public class PasswordResetManager : IPasswordResetManager
    {
        private readonly UserIdentityDBContext _dbContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;

        public PasswordResetManager(UserIdentityDBContext context, UserManager<UserModel> userManager,
            IConfiguration configuration)
        {
            _dbContext = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> AddResetRequestAsync(string userId)
        {
            UserModel user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var expirePeriod = Convert.ToInt32(_configuration["PasswordReset:KeyExpireHours"]);
                var time = DateTime.Now;

                _dbContext.PasswordResetRequests.Add(new PasswordReset()
                {
                    UserIdentity = user.Id,
                    EmailAdddress = user.Email,
                    ResetKey = RandomKeyGenerator.RandomString(25),
                    RequestTime = time,
                    KeyExpires = time.AddHours(expirePeriod),
                    Attempts = 0,
                    WasVerified = user.EmailConfirmed,
                });

                await _dbContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<PasswordReset> GetRequestAsync(string userId)
        {
            return await ReadResetDataAsync(userId);
        }

        public async Task<ResetResult> ValidateRequestAsync(string userId, string key)
        {
            var request = await ReadResetDataAsync(userId);

            if (request == null)
            {
                return new ResetResult()
                {
                    Success = false,
                    NoRequest = true,
                };
            }

            var maxAttempts = Convert.ToInt32(_configuration["PasswordReset:MaxAttempts"]);
            var time = DateTime.Now;
            var output = new ResetResult();

            await ValidateAttempt(request, output, maxAttempts, time, key);
            await _dbContext.SaveChangesAsync();

            return output;
        }

        private async Task<PasswordReset> ReadResetDataAsync(string userId)
        {
            var request = _dbContext.PasswordResetRequests
                .Where(r => r.UserIdentity == userId && !r.ResetVerified)
                .OrderByDescending(r => r.RequestTime)
                .FirstOrDefault();

            return await Task.FromResult(request);
        }

        private async Task ValidateAttempt(PasswordReset request, ResetResult result, int maxAttempts, DateTime time, string key)
        {
            if (request.Attempts >= maxAttempts)
            {
                result.Success = false;
                result.Blocked = true;
                result.Attempts = request.Attempts;
            }

            if (request.KeyExpires < time)
            {
                result.Success = false;
                result.Expired = true;
                result.Attempts = request.Attempts;
            }

            if (Equals(request.ResetKey, key))
            {
                request.ResetVerified = true;
                result.Success = true;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                result.Success = false;
                result.Attempts = request.Attempts;
                request.Attempts++;
            }
        }
    }
}
