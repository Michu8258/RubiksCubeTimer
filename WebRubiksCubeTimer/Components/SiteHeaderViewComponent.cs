using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Components
{
    public class SiteHeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
