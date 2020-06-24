using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TimerRequestsDataBase.Abstractions;
using WebRubiksCubeTimer.Models.Users;
using WebRubiksCubeTimer.ViewModels.Requests;

namespace WebRubiksCubeTimer.Components
{
    public class RequestsViewComponent : ViewComponent
    {
        private readonly UserManager<UserModel> _usermanager;
        private readonly IRequestService _requestService;

        public RequestsViewComponent(UserManager<UserModel> userManager,
            IRequestService requestService)
        {
            _usermanager = userManager;
            _requestService = requestService;
        }

        public IViewComponentResult Invoke(string controllerName)
        {
            var model = new RequestViewComponentViewModel()
            {
                Display = false,
                AdminOrMod = false,
                ControllerName = controllerName,
            };
            var user = _usermanager.FindByNameAsync(User?.Identity?.Name).Result;
            if (user != null)
            {
                model.Display = true;
                model.AdminOrMod = User.IsInRole("Administrator") || User.IsInRole("Moderator");
                model.AmountOfNewStatesUser = _requestService.GetAMountOfModifiedRequestUserAsync(user.Id).Result;
            }

            if (model.AdminOrMod)
            {
                model.AmountOfNewStatesAdmin = _requestService.GetAmountOfModifiedRequestsAsync().Result;
            }
            else
            {
                model.AmountOfNewStatesAdmin = 0;
            }

            return View(model);
        }
    }
}
