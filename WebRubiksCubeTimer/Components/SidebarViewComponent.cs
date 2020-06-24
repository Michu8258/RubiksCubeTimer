using Microsoft.AspNetCore.Mvc;
using WebRubiksCubeTimer.ViewModels.ViewComponents;

namespace WebRubiksCubeTimer.Components
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(SidebarViewModel model)
        {
            if (HttpContext.Request.RouteValues["controller"].ToString() == "User" && HttpContext.Request.RouteValues["action"].ToString() == "CreateSeries")
            {
                model.ControllerName = string.Empty;
                return View("Default", model);
            }
            else
            {
                model.ControllerName = HttpContext.Request.RouteValues["controller"].ToString();
            }

            return View("Default", model);
        }
    }
}
