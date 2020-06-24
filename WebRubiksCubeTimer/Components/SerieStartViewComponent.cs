using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TimerDataBase.Abstractions;
using WebRubiksCubeTimer.Models.Users;

namespace WebRubiksCubeTimer.Components
{
    public class SerieStartViewComponent : ViewComponent
    {
        private readonly ICubeCollectionService _cubeCollectionService;
        private readonly UserManager<UserModel> _usermanager;

        public SerieStartViewComponent(ICubeCollectionService cubeCollectionService,
            UserManager<UserModel> userManager)
        {
            _cubeCollectionService = cubeCollectionService;
            _usermanager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var user = _usermanager.FindByNameAsync(User?.Identity?.Name);
            if (user.Result != null)
            {
                var cubesAmount = _cubeCollectionService.GetAllCubesOfUserAsync(user.Result.Id);
                if (cubesAmount.Result.Any())
                {
                    return View(true);
                }
            }

            return View(false);
        }
    }
}
