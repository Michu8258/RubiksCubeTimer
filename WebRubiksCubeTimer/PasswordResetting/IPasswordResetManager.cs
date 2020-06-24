using System.Threading.Tasks;
using WebRubiksCubeTimer.PasswordResetting.Helpers;

namespace WebRubiksCubeTimer.PasswordResetting
{
    public interface IPasswordResetManager
    {
        Task<bool> AddResetRequestAsync(string userId);
        Task<PasswordReset> GetRequestAsync(string userId);
        Task<ResetResult> ValidateRequestAsync(string userId, string key);
    }
}