using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class CubesCollectionViewModel
    {
        public IEnumerable<UserCubeViewModel> UserCubes { get; set; }

        public IEnumerable<Category> AvailableCategories { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }
    }
}
