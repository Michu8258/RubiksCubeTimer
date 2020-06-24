using System.Threading.Tasks;

namespace WebRubiksCubeTimer.UserManager
{
    public interface IUserManagerExtending
    {
        Task<bool> UpdateUserLoginTimeSpan(string userId);
    }
}
