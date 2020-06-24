using Microsoft.AspNetCore.Mvc;

namespace WebRubiksCubeTimer.Components
{
    public class AddResponseModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
